using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VDCD.Business.Infrastructure;
using VDCD.DataAccess;
using VDCD.Entities.Custom;
using VDCD.Entities.DTO;

namespace VDCD.Business.Service
{
    public class ContactMessageService
    {
        private readonly IRepository<ContactMessages> _contactMessagesRepo;
        private readonly ICacheService _cache;
        protected readonly AppDbContext _context;
        private readonly IRealtimeNotifier _notifier;
        public ContactMessageService(IRepository<ContactMessages> customRepo,IRealtimeNotifier notifier,
                              ICacheService cache,
                              AppDbContext context
                              )
        {
            _contactMessagesRepo = customRepo;
            _cache = cache;
            _context = context;
            _notifier = notifier;
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
        public async Task Create(ContactCreateDto contact)
        {
            var ct = new ContactMessages
            {
                Email = contact.Email,
                Name = contact.Name,
                Content = contact.Content,
                Subject = contact.Title,
                Phone = contact.Phone,
                CreatedAt = DateTime.Now,
                IsRead = false
            };
            _contactMessagesRepo.Create(ct);
            await _context.SaveChangesAsync();
            // xử lý nghiệp vụ + DB
            await _notifier.Notify("NewContact", new
            {
/*                contact.Name,
                contact.Phone,
                contact.Email,
                contact.Title,
                Time = DateTime.Now.ToString("HH:mm:ss")*/
                name = contact.Name,
                time = DateTime.Now.ToString("HH:mm dd/MM")
            });
        }
    }
}
