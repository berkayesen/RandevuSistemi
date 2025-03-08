using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using RandevuSistemi.Data;
using RandevuSistemi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace RandevuSistemi.Controllers
{
    public class AppointmentController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AppointmentController(AppDbContext context, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        //GET: Appointments/Create
        public IActionResult Create()
        {
            ViewBag.Services = new SelectList(_context.Services, "Id", "Name");
            return View();
        }

        //POST: Appointments/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("AppointmentDate,ServiceId")] Appointment appointment)
        {
                // Giriş yapan kullanıcıyı al
                var user = await _userManager.GetUserAsync(User);

                // Randevuyu kullanıcıyla ilişkilendir
                appointment.UserId = user.Id;

                // Appointment'ı veritabanına kaydet
                _context.Add(appointment);
                await _context.SaveChangesAsync();

                // Başarıyla kaydedildiyse, başka bir sayfaya yönlendir
                return RedirectToAction("Index", "Appointment"); // Randevuların listelendiği sayfaya yönlendir
        }


        //Randevuları listeleme işlemi
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);

            var appointments = await _context.Appointments
                .Where(a => a.UserId == user.Id)
                .Include(a => a.Service)
                .ToListAsync();

            return View(appointments);
        }

        //Çıkış Yapma
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync(); 
            return RedirectToAction("Index", "Home");
        }

        //Düzenleme
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment == null)
            {
                return NotFound();
            }
            return Json(appointment);
        }
        [HttpPost]
        public async Task<IActionResult> Edit([FromBody] Appointment appointment)
        {
            if (ModelState.IsValid)
            {
                var existingAppointment = await _context.Appointments.FindAsync(appointment.Id);
                if (existingAppointment == null)
                {
                    return Json(new { success = false, message = "Randevu bulunamadı!" });
                }

                if (string.IsNullOrEmpty(appointment.UserId))
                {
                    appointment.UserId = existingAppointment.UserId;
                }

                existingAppointment.AppointmentDate = appointment.AppointmentDate;
                existingAppointment.UserId = appointment.UserId;

                _context.Update(existingAppointment);
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Randevu başarıyla güncellendi!" });
            }
            else
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors)
                                              .Select(e => e.ErrorMessage)
                                              .ToList();

                return Json(new { success = false, message = "Geçersiz veri!", errors = errors });
            }
        }

        //Silme
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment == null)
            {
                return Json(new { success = false, message = "Randevu bulunamadı" });
            }

            _context.Appointments.Remove(appointment);
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Randevu başarıyla silindi!" });
        }

        [Authorize(Roles = "Admin")] //Admin 
        public async Task<IActionResult> AllAppointments()
        {
            var appointments = await _context.Appointments
                .Include(a => a.Service)
                .Include(a => a.User)
                .ToListAsync();

            return View(appointments);
        }


    }
}
