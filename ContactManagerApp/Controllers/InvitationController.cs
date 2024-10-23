using ContactManagerApp.Data;
using ContactManagerApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ContactManagerApp.Controllers
{
    [Authorize]
    public class InvitationController : Controller
    {
        private readonly ApplicationDbContext _context;

        public InvitationController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Invitation/AcceptInvitation
        public async Task<IActionResult> AcceptInvitation(int contactId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Find the contact based on the contactId and userId
            var contact = await _context.Contacts
                .FirstOrDefaultAsync(c => c.Id == contactId && c.ApplicationUserId == userId);

            if (contact == null)
            {
                return NotFound();
            }

            ViewBag.Contact = contact;
            ViewBag.InvitedBy = User.Identity.Name;

            return View();
        }
    }
}
