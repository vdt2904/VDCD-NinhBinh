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
    public class ContactMessageService
    {
        private readonly IRepository<ContactMessages> _contactMessagesRepo;
        private readonly ICacheService _cache;
        protected readonly AppDbContext _context;
        public ContactMessageService(IRepository<ContactMessages> customRepo,
                              ICacheService cache,
                              AppDbContext context
                              )
        {
            _contactMessagesRepo = customRepo;
            _cache = cache;
            _context = context;
        }
        public IEnumerable<ContactMessages> GetContactMessages()
        {
            return _contactMessagesRepo.Gets(true).OrderByDescending(x=>x.Id);
        }

        public void DeleteContact(int id)
        {
            var contact = _contactMessagesRepo.Get(false,x=>x.Id == id);
            if(contact != null) { 
                _contactMessagesRepo.Delete(contact);
                _context.SaveChanges();
            }
        }

        public void IsRead(int id)
        {
            var contact = _contactMessagesRepo.Get(false, x => x.Id == id);
            if(contact != null)
            {
                contact.IsRead = true;
                contact.UpdatedAt = DateTime.Now;
                _contactMessagesRepo.Update(contact);
                _context.SaveChanges();
            }
        }

        public ContactMessages GetById(int id)
        {
            return  _contactMessagesRepo.Get(false , x=>x.Id == id);
        }

        public void Update(ContactMessages model)
        {
            var contact = _contactMessagesRepo.Get(false, x => x.Id == model.Id);
            if (contact != null)
            {
                contact.IsRead = true;
                contact.UpdatedAt = DateTime.Now;
                contact.ReplyContent = model.ReplyContent;
                contact.RepliedAt = DateTime.Now;
                _contactMessagesRepo.Update(contact);
                _context.SaveChanges();
            }
        }
    }
}
