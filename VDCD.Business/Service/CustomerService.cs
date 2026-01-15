using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VDCD.Business.Infrastructure;
using VDCD.DataAccess;
using VDCD.Entities.Custom;

namespace VDCD.Business.Service
{
    public class CustomerService
    {
        private readonly IRepository<Customer> _customerRepo;
        private readonly ICacheService _cache;
        protected readonly AppDbContext _context;
        public CustomerService(IRepository<Customer> customRepo,
                              ICacheService cache,
                              AppDbContext context
                              )
        {
            _customerRepo = customRepo;
            _cache = cache;
            _context = context;
          
        }
        public IReadOnlyList<Customer> GetAll()
        {
            return _customerRepo.GetsReadOnly().OrderByDescending(x => x.Id).ToList();
        }

        public void Save(Customer model)
        {
            if (string.IsNullOrWhiteSpace(model.CustomerName))
                throw new Exception("Tên khách hàng không được để trống");

            if (model.Id == 0)
            {
                _customerRepo.Create(model);
            }
            else
            {
                var entity = _customerRepo.Get(model.Id);
                if (entity == null) throw new Exception("Khách hàng không tồn tại");

                entity.CustomerName = model.CustomerName;
                entity.Image = model.Image;
                entity.Email = model.Email;
                entity.Phone = model.Phone;
                entity.Address = model.Address;
                entity.IsShow = model.IsShow;

                _customerRepo.Update(entity);
            }
            _context.SaveChanges();
        }

        public void Delete(int id)
        {
            var entity = _customerRepo.Get(id);
            if (entity != null)
            {
                _customerRepo.Delete(entity);
                _context.SaveChanges();
            }
        }
    }
}
