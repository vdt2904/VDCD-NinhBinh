using Microsoft.AspNetCore.Mvc;
using VDCD.Business.Service;
using VDCD.Entities.DTO;

namespace VDCD.Controllers
{
    public class ContactController : Controller
    {   
        private readonly ContactMessageService _contactMessageService;
        public ContactController(ContactMessageService contactMessageService) { 
            _contactMessageService = contactMessageService;
        }
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ContactCreateDto dto)
        {
            await _contactMessageService.Create(dto);
            return Ok();
        }
    }
}
