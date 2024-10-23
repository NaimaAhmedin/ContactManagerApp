using ContactManagerApp.Data;
using ContactManagerApp.Models;
using ContactManagerApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Claims;


namespace ContactManagerApp.Controllers
{
    [Authorize]
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailSender _emailSender;

        public UserController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IEmailSender emailSender)
        {
            _context = context;
            _userManager = userManager;
            _emailSender = emailSender;
        }

        // GET: User/Index
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(claimType: ClaimTypes.NameIdentifier);
            var Contacts = await _context.Contacts.Where(c => c.ApplicationUserId == userId).ToListAsync();
            return View(Contacts);
        }

        // GET: User/Create
        public IActionResult AddContact()
        {
            return View();
        }

        // POST: User/AddContact
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddContact(Contact model)
        {
            var contacts = new Contact()
            {
                Id = model.Id,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber,
                ApplicationUserId = model.ApplicationUserId
            };

            await _context.Contacts.AddAsync(contacts);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");


        }


            // GET: ContactLists/Details/5
            public async Task<IActionResult> Details(int? id)
            {
                if (id == null)
                {
                    return NotFound();
                }

                var contact = await _context.Contacts
                    .FirstOrDefaultAsync(m => m.Id == id);
                if (contact == null)
                {
                    return NotFound();
                }

                return View(contact);
            }
        // GET: User/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var contact = await _context.Contacts.FindAsync(id);
            if (contact == null)
            {
                return NotFound();
            }
            return View(contact);
        }

        // POST: User/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,FirstName,LastName,Email,PhoneNumber")] Contact contact)
        {
            if (id != contact.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                { 
                    var existingContact = await _context.Contacts.FindAsync(id);
                    if (existingContact == null)
                    {
                        return NotFound();
                    }
                    existingContact.FirstName = contact.FirstName;
                    existingContact.LastName = contact.LastName;
                    existingContact.Email = contact.Email;
                    existingContact.PhoneNumber = contact.PhoneNumber;
                   
                    _context.Update(existingContact);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ContactListExists(contact.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            return View(contact);
        }

        
        public async Task<IActionResult> Delete(int? id)
            {
                if (id == null)
                {
                    return NotFound();
                }

                var contact = await _context.Contacts
                    .FirstOrDefaultAsync(m => m.Id == id);
                if (contact == null)
                {
                    return NotFound();
                }

                return View(contact);
            }

            // POST: Contact/Delete/5
            [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var contact = await _context.Contacts.FindAsync(id);
            if (contact != null)
            {
                _context.Contacts.Remove(contact);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ContactListExists(int id)
        {
            return _context.Contacts.Any(e => e.Id == id);
        }

// POST: SendInvitations
[HttpPost]
        public async Task<IActionResult> SendInvitations(int[] selectedContacts)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userName = User.Identity.Name; 

            var contacts = await _context.Contacts
                .Where(c => selectedContacts.Contains(c.Id) && c.ApplicationUserId == userId)
                .ToListAsync();

            foreach (var contact in contacts)
            {
                var invitationLink = Url.Action("AcceptInvitation", "Invitation", new { contactId = contact.Id }, Request.Scheme);
                var message = $"Hello {contact.FirstName},<br/><br/>You are invited  Click <a href='{invitationLink}'>here</a> to accept.";
                var email=contact.Email;
                try
                {
                    await _emailSender.SendEmailAsync(email, "Invitation!", message);
                    TempData["Message"] = $"Invitation sent to {contact.Email}.";
                }
                catch (Exception ex)
                {
                    TempData["Message"] = $"Failed to send invitation to {contact.Email}. Error: {ex.Message}";
                }
            }

            return RedirectToAction(nameof(Index));
        }
    }
}