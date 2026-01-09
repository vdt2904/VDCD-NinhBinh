using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VDCD.Business.Infrastructure;
using VDCD.DataAccess;
using VDCD.Entities.Cache;
using VDCD.Entities.Custom;
using VDCD.Entities.DTO;

namespace VDCD.Business.Service
{
    public class FileManagerService
    {
        private readonly IRepository<Files> _filesRepository;
        private readonly ICacheService _cache;
        private readonly string _uploadFolder;
        public FileManagerService(IRepository<Files> repo, ICacheService cache, IConfiguration config)
        {
            _filesRepository = repo;
            _cache = cache;
            _uploadFolder = config["FileManager:UploadFolder"] ?? "wwwroot/uploads";
            if (!Directory.Exists(_uploadFolder))
                Directory.CreateDirectory(_uploadFolder);
        }
        public void CreateFolder(string name, long? parentId = null)
        {
            _filesRepository.Create(new Files
            {
                Name = name,
                IsFolder = true,
                parent_id = parentId,
                path = "", // Thư mục không có đường dẫn vật lý [cite: 2]
                created_at = DateTime.Now
            });
            _cache.Remove(CacheParam.FileAllKey); // Refresh cache [cite: 2]
}
        public IEnumerable<FilePickerItemDto> GetForPicker(long? parentId = null, bool onlyImage = false)
        {
            // cache folder
            if (!_cache.TryGet(CacheParam.FileAllKey, out List<Files> folders))
            {
                folders = _filesRepository.GetsReadOnly().ToList();
                _cache.Set(CacheParam.FileAllKey, folders, TimeSpan.FromMinutes(60));
            }

            var items = folders
        .Where(x => x.parent_id == parentId)
        .Where(x => !onlyImage || (x.content_type != null && x.content_type.StartsWith("image/")))
        .Select(x => new FilePickerItemDto
        {
            Id = x.Id,
            Name = x.Name,
            Url = x.IsFolder ? null : "/" + x.path.Replace("wwwroot/", "").Replace("\\", "/"),
            IsFolder = x.IsFolder,
            ContentType = x.content_type,
            ParentId = x.parent_id
        })
        .ToList();

            return items;
        }
        public async Task UploadAsync(Stream fileStream, string fileName, string contentType, long? parentId = null)
        {
            if (fileStream == null || string.IsNullOrEmpty(fileName))
                throw new ArgumentException("File không hợp lệ");

            // Tạo tên file duy nhất
            var uniqueFileName = Guid.NewGuid() + "_" + Path.GetFileName(fileName);
            var filePath = Path.Combine(_uploadFolder, uniqueFileName);

            // Lưu file lên disk
            using (var fs = new FileStream(filePath, FileMode.Create, FileAccess.Write))
            {
                await fileStream.CopyToAsync(fs);
            }

            // Lưu metadata vào DB
            _filesRepository.Create(new Files
            {
                Name = fileName,
                path = Path.Combine(_uploadFolder, uniqueFileName),
                IsFolder = false,
                parent_id = parentId,
                content_type = contentType,
                size = fileStream.Length,
                created_at = DateTime.Now
            });

            _cache.Remove(CacheParam.FileAllKey); // refresh cache
        }
        public void Move(long id, long? newParentId)
        {
            var node = _filesRepository.Get(id);
            if (node == null)
                throw new Exception("File/Folder không tồn tại");

            node.parent_id = newParentId;
            _filesRepository.Update(node);
            
            _cache.Remove(CacheParam.FileAllKey); // refresh cache
        }
        public void Delete(long id)
        {
            var node = _filesRepository.Get(id);
            if (node == null)
                throw new Exception("File/Folder không tồn tại");

            if (!node.IsFolder && File.Exists(node.path))
                File.Delete(node.path);

            _filesRepository.Delete(node);

            _cache.Remove(CacheParam.FileAllKey); // refresh cache
        }
        public Files GetById(long id)
        {
            if (_cache.TryGet(CacheParam.FileAllKey, out List<Files> folders))
            {
                var item = folders.FirstOrDefault(x => x.Id == id);
                if (item != null) return item;
            }

            return _filesRepository.Get(id);
        }
    }
}
