using Microsoft.AspNetCore.Mvc;
using VDCD.Business.Service;

namespace VDCD.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class FileManagerController : Controller
    {
        private readonly FileManagerService _fileManager;
        public FileManagerController(FileManagerService fileManager)
        {
            _fileManager = fileManager;
        }

        /// <summary>
        /// Trang chính FilePicker/Explorer
        /// </summary>
        public IActionResult Index(long? parentId = null)
        {
            var items = _fileManager.GetForPicker(parentId);
            return View(items);
        }
        [HttpPost]
        public IActionResult CreateFolder(string name, long? parentId)
        {
            if (string.IsNullOrEmpty(name)) return BadRequest("Tên không hợp lệ");
            _fileManager.CreateFolder(name, parentId);
            return Ok();
        }
        [HttpPost]
        public async Task<IActionResult> UploadMultiple(List<IFormFile> files, long? parentId)
        {
            if (files == null || files.Count == 0) return BadRequest("Không có file");
            foreach (var file in files)
            {
                await _fileManager.UploadAsync(file.OpenReadStream(), file.FileName, file.ContentType, parentId);
            }
            return Ok();
        }

        [HttpPost]
        public IActionResult DeleteItem(long id)
        {
            _fileManager.Delete(id);// Sử dụng hàm Delete có sẵn [cite: 1]
            return Ok();
        }
        public IActionResult GetFoldersTree()
        {
            var folders = _fileManager.GetForPicker(null)
                .Where(x => x.IsFolder)
                .Select(x => (dynamic)new
                {
                    id = x.Id,
                    name = x.Name,
                    parentId = x.ParentId
                }).ToList();

            // Build tree
            var tree = BuildTree(folders, null);
            return new JsonResult(tree);
        }

        private List<object> BuildTree(List<dynamic> items, long? parentId)
        {
            return items
                .Where(x => x.parentId == parentId)
                .Select(x => new {
                    id = x.id,
                    name = x.name,
                    children = BuildTree(items, x.id)
                })
                .ToList<object>();
        }

        /// <summary>
        /// Lấy file/folder theo folder

        public IActionResult GetFilesByFolder(long? parentId)
        {
            var files = _fileManager.GetForPicker(parentId)
                .Select(x => new
                {
                    id = x.Id,
                    name = x.Name,
                    url = x.Url,
                    isFolder = x.IsFolder,
                    contentType = x.ContentType,
                    parentId = x.ParentId
                }).ToList();

            return new JsonResult(files);
        }

        /// <summary>
        /// Upload file
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Upload(IFormFile file, long? parentId)
        {
            if (file == null || file.Length == 0)
                return BadRequest("File không hợp lệ");

            await _fileManager.UploadAsync(file.OpenReadStream(), file.FileName, file.ContentType, parentId);

            return RedirectToAction(nameof(Index), new { parentId });
        }

        /// <summary>
        /// Xóa file/folder
        /// </summary>
        [HttpPost]
        public IActionResult Delete(long id)
        {
            _fileManager.Delete(id);
            return Ok();
        }

        /// <summary>
        /// Move file/folder
        /// </summary>
        [HttpPost]
        public IActionResult Move(long id, long? newParentId)
        {
            _fileManager.Move(id, newParentId);
            return Ok();
        }

    }
}
