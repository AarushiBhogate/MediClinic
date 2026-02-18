using MediClinic.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace MediClinic.Controllers
{
    [Authorize(Roles = "Admin")]   // 🔐 Only Admin can access
    public class AdminController : Controller
    {
        private readonly MediClinicDbContext _context;

        public AdminController(MediClinicDbContext context)
        {
            _context = context;
        }

        // ================= ADMIN DASHBOARD =================
        public IActionResult Index()
        {
            ViewBag.TotalPhysicians = _context.Physicians.Count();
            ViewBag.TotalPatients = _context.Patients.Count();
            ViewBag.TotalSuppliers = _context.Suppliers.Count();
            ViewBag.ChemistCount = _context.Chemists.Count();
            ViewBag.ScheduleCount = _context.Schedules.Count();
            ViewBag.TotalAppointments = _context.Appointments.Count();
            ViewBag.TotalUsers = _context.Users.Count();
            ViewBag.PendingPatientCount = _context.Patients.Count(p => p.PatientStatus == "Pending");
            ViewBag.UserName = User.Identity.Name;
            return View();
        }

        //Get:Patient
        public async Task<IActionResult> GetPatients()
        {
            var patients = await _context.Patients
                    .Where(p => p.PatientStatus != "Pending")
                    .ToListAsync();

            return View(patients);
        }



        // GET: Patients/Details/5
        public async Task<IActionResult> GetPatientById(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var patient = await _context.Patients
                .FirstOrDefaultAsync(m => m.PatientId == id);
            if (patient == null)
            {
                return NotFound();
            }

            return View(patient);
        }                                                   


        // GET: Patients/Create
        public IActionResult CreatPatients()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePatients([Bind("PatientId,PatientName,Dob,Gender,Address,Phone,Email,Summary,PatientStatus")] Patient patient)
        {
            if (ModelState.IsValid)
            {
                _context.Add(patient);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(patient);
        }


        // GET: Patients/Edit/5
        public async Task<IActionResult> EditPatients(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var patient = await _context.Patients.FindAsync(id);
            if (patient == null)
            {
                return NotFound();
            }
            return View(patient);
        }

        // POST: Patients/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPatients(int id, [Bind("PatientId,PatientName,Dob,Gender,Address,Phone,Email,Summary,PatientStatus")] Patient patient)
        {
            if (id != patient.PatientId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(patient);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PatientExists(patient.PatientId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(patient);
        }

        private bool PatientExists(int patientId)
        {
            throw new NotImplementedException();
        }


        // GET: Patients/Delete/5
        public async Task<IActionResult> DeletePatients(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var patient = await _context.Patients
                .FirstOrDefaultAsync(m => m.PatientId == id);
            if (patient == null)
            {
                return NotFound();
            }

            return View(patient);
        }

        // POST: Patients/Delete/5
        [HttpPost, ActionName("DeletePatients")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletepatientsConfirmed(int id)
        {
            var patient = await _context.Patients.FindAsync(id);
            if (patient != null)
            {

                patient.PatientStatus = "Inactive";
                _context.Patients.Update(patient);
                await _context.SaveChangesAsync();

                var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == patient.Email);
                if (user != null)
                {
                    user.Status = "Inactive";
                }
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));

        }

        private bool PatientExistss(int id)
        {
            return _context.Patients.Any(e => e.PatientId == id);
        }







        // ================= PENDING PATIENTS =================
        public IActionResult PendingPatients()
        {
            var pendingPatients = _context.Patients
                .Where(p => p.PatientStatus == "Pending")
                .ToList();
            ViewBag.PendingPatientCount = pendingPatients.Count;
            return View(pendingPatients);
        }

        public IActionResult ApprovePatient(int id)
        {
            var patient = _context.Patients.Find(id);
            if (patient != null)
            {
                patient.PatientStatus = "Active";
                string username = patient.Email;
                string password = "Patient@123";

                var existingUser = _context.Users.FirstOrDefault(u => u.UserName == username);
                if (existingUser == null)
                {
                    _context.Users.Add(new User
                    {
                        UserName = username,
                        Password = password,
                        Role = "Patient",
                        Status = "Active"
                    });
                }

                _context.SaveChanges();
            }
            return RedirectToAction(nameof(PendingPatients));
        }

        public IActionResult DenyPatient(int id)
        {
            var patient = _context.Patients.Find(id);
            if (patient != null)
            {
                patient.PatientStatus = "Inactive";
                var user = _context.Users.FirstOrDefault(u => u.UserName == patient.Email);
                if (user != null) user.Status = "Inactive";
                _context.SaveChanges();
            }
            return RedirectToAction(nameof(PendingPatients));
        }

        // ================= LOGOUT =================
        public IActionResult Logout()
        {
            return RedirectToAction("Logout", "User");
        }

        // ============================================================
        // ================= SUPPLIERS MANAGEMENT =====================
        // ============================================================
        //public async Task<IActionResult> GetSuppliers()
        //{
        //    return View(await _context.Suppliers.ToListAsync());
        //}

        //public IActionResult Suppliers(int id)
        //{
        //    var supplier = _context.Suppliers.Find(id);
        //    if (supplier == null) return NotFound();
        //    return View("Suppliers/Details", supplier);
        //}

        //public IActionResult CreateSupplier()
        //{
        //    return View("Suppliers/Create");
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public IActionResult CreateSupplier(Supplier supplier)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _context.Suppliers.Add(supplier);
        //        _context.SaveChanges();
        //        return RedirectToAction(nameof(GetSuppliers));
        //    }
        //    return View("Suppliers/Create", supplier);
        //}

        //public IActionResult EditSupplier(int id)
        //{
        //    var supplier = _context.Suppliers.Find(id);
        //    if (supplier == null) return NotFound();
        //    return View("Suppliers/Edit", supplier);
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public IActionResult EditSupplier(Supplier supplier)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _context.Suppliers.Update(supplier);
        //        _context.SaveChanges();
        //        return RedirectToAction(nameof(GetSuppliers));
        //    }
        //    return View("Suppliers/Edit", supplier);
        //}

        //public IActionResult DeleteSupplier(int id)
        //{
        //    var supplier = _context.Suppliers.Find(id);
        //    if (supplier == null) return NotFound();
        //    return View("Suppliers/Delete", supplier);
        //}

        //[HttpPost, ActionName("DeleteSupplier")]
        //[ValidateAntiForgeryToken]
        //public IActionResult DeleteConfirmedSupplier(int id)
        //{
        //    var supplier = _context.Suppliers.Find(id);
        //    if (supplier != null)
        //    {
        //        _context.Suppliers.Remove(supplier);
        //        _context.SaveChanges();
        //    }
        //    return RedirectToAction(nameof(GetSuppliers));
        //}







        // GET: Suppliers
        public async Task<IActionResult> GetSuppliers()
        {
            return View(await _context.Suppliers.ToListAsync());
        }

        // GET: Suppliers/Details/5
        public async Task<IActionResult> SupplierDetails(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var supplier = await _context.Suppliers
                .FirstOrDefaultAsync(m => m.SupplierId == id);
            if (supplier == null)
            {
                return NotFound();
            }

            return View(supplier);
        }

        // GET: Suppliers/Create
        public IActionResult CreateSupplier()
        {
            return View();
        }

        // POST: Suppliers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateSupplier([Bind("SupplierId,SupplierName,Address,Phone,Email,SupplierStatus")] Supplier supplier)
        {
            if (ModelState.IsValid)
            {
                _context.Add(supplier);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(supplier);
        }

        // GET: Suppliers/Edit/5
        public async Task<IActionResult> EditSupplier(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var supplier = await _context.Suppliers.FindAsync(id);
            if (supplier == null)
            {
                return NotFound();
            }
            return View(supplier);
        }

        // POST: Suppliers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditSupplier(int id, [Bind("SupplierId,SupplierName,Address,Phone,Email,SupplierStatus")] Supplier supplier)
        {
            if (id != supplier.SupplierId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(supplier);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SupplierExists(supplier.SupplierId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(supplier);
        }

        // GET: Suppliers/Delete/5
        public async Task<IActionResult> DeleteSupplier(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var supplier = await _context.Suppliers
                .FirstOrDefaultAsync(m => m.SupplierId == id);
            if (supplier == null)
            {
                return NotFound();
            }

            return View(supplier);
        }

        // POST: Suppliers/Delete/5
        [HttpPost, ActionName("DeleteSupplier")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteSupplierConfirmed(int id)
        {
            var supplier = await _context.Suppliers.FindAsync(id);
            if (supplier != null)
            {
                supplier.SupplierStatus = "Inactive";
                _context.Suppliers.Update(supplier);
                await _context.SaveChangesAsync();

                var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == supplier.Email);
                if (user != null)
                {
                    user.Status = "Inactive";
                }
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
            
        }

        private bool SupplierExists(int id)
        {
            return _context.Suppliers.Any(e => e.SupplierId == id);
        }







        // GET: Physicians
        public async Task<IActionResult> GetPhysicians()
        {
            return View(await _context.Physicians.ToListAsync());
        }

        public async Task<IActionResult> GetPhysiciansByID(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var physician = await _context.Physicians
                .FirstOrDefaultAsync(m => m.PhysicianId == id);
            if (physician == null)
            {
                return NotFound();
            }

            return View(physician);
        }

        public IActionResult CreatePhysician()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePhysician([Bind("PhysicianId,PhysicianName,Specialization,Address,Phone,Email,Summary,PhysicianStatus")] Physician physician)
        {
            if (ModelState.IsValid)
            {
                _context.Add(physician);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(physician);
        }


        // GET: Physicians/Edit/5
        public async Task<IActionResult> EditPhysicians(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var physician = await _context.Physicians.FindAsync(id);
            if (physician == null)
            {
                return NotFound();
            }
            return View(physician);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPhysicians(int id, [Bind("PhysicianId,PhysicianName,Specialization,Address,Phone,Email,Summary,PhysicianStatus")] Physician physician)
        {
            if (id != physician.PhysicianId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(physician);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PhysicianExists(physician.PhysicianId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(physician);
        }

        private bool PhysicianExists(int physicianId)
        {
            throw new NotImplementedException();
        }



        // GET: Physicians/Delete/5
        public async Task<IActionResult> DeletePhysicians(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var physician = await _context.Physicians
                .FirstOrDefaultAsync(m => m.PhysicianId == id);
            if (physician == null)
            {
                return NotFound();
            }

            return View(physician);
        }

        // POST: Physicians/Delete/5
        [HttpPost, ActionName("DeletePhysicians")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletePhysiciansConfirmed(int id)
        {
            var physician = await _context.Physicians.FindAsync(id);
            if (physician != null)
            {
                physician.PhysicianStatus = "Inactive";
                _context.Physicians.Update(physician);
                await _context.SaveChangesAsync();

                var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == physician.Email);
                if (user != null)
                {
                    user.Status = "Inactive";
                }
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));

        }

        private bool SupplierExistss(int id)
        {
            return _context.Suppliers.Any(e => e.SupplierId == id);
        }



        // GET: Chemists
        public async Task<IActionResult> GetChemist()
        {
            return View(await _context.Chemists.ToListAsync());
        }

        public async Task<IActionResult> GetChemistDetails(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var chemist = await _context.Chemists
                .FirstOrDefaultAsync(m => m.ChemistId == id);
            if (chemist == null)
            {
                return NotFound();
            }

            return View(chemist);
        }

        // GET: Chemists/Create
        public IActionResult CreateChemist()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateChemist([Bind("ChemistId,ChemistName,Address,Phone,Email,Summary,ChemistStatus")] Chemist chemist)
        {
            if (ModelState.IsValid)
            {
                _context.Add(chemist);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(chemist);
        }


        // GET: Chemists/Edit/5
        public async Task<IActionResult> EditChemist(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var chemist = await _context.Chemists.FindAsync(id);
            if (chemist == null)
            {
                return NotFound();
            }
            return View(chemist);
        }

        // POST: Chemists/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditChemist(int id, [Bind("ChemistId,ChemistName,Address,Phone,Email,Summary,ChemistStatus")] Chemist chemist)
        {
            if (id != chemist.ChemistId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(chemist);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ChemistExists(chemist.ChemistId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(chemist);
        }

        private bool ChemistExists(int chemistId)
        {
            throw new NotImplementedException();
        }


        // GET: Chemists/Delete/5
        public async Task<IActionResult> DeleteChemist(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var chemist = await _context.Chemists
                .FirstOrDefaultAsync(m => m.ChemistId == id);
            if (chemist == null)
            {
                return NotFound();
            }

            return View(chemist);
        }

        // POST: Chemists/Delete/5
        [HttpPost, ActionName("DeleteChemist")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteChemistConfirmed(int id)
        {
            var chemist = await _context.Chemists.FindAsync(id);
            if (chemist != null)
            {
                chemist.ChemistStatus = "Inactive";
                _context.Chemists.Update(chemist);
                await _context.SaveChangesAsync();

                var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == chemist.Email);
                if (user != null)
                {
                    user.Status = "Inactive";
                }
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));

        }

        private bool ChemistExistss(int id)
        {
            return _context.Chemists.Any(e => e.ChemistId == id);
        }



        // GET: Schedules
        // GET: Schedules
        public async Task<IActionResult> GetSchedule()
        {
            var mediClinicDbContext = _context.Schedules
                .Include(s => s.Appointment)
                    .ThenInclude(a => a.Patient)   // 🔥 This was missing
                .Include(s => s.Physician);

            return View(await mediClinicDbContext.ToListAsync());
        }


        // GET: Schedules/Details/5
        public async Task<IActionResult> ScheduleDetails(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var schedule = await _context.Schedules
    .Include(s => s.Appointment)
        .ThenInclude(a => a.Patient)
    .Include(s => s.Physician)
    .FirstOrDefaultAsync(m => m.ScheduleId == id);

            if (schedule == null)
            {
                return NotFound();
            }

            return View(schedule);
        }


        // GET: Schedules/Create
        public IActionResult CreateSchedule()
        {
            ViewData["AppointmentId"] = new SelectList(_context.Appointments, "AppointmentId", "AppointmentId");
            ViewData["PhysicianId"] = new SelectList(_context.Physicians, "PhysicianId", "PhysicianId");
            return View();
        }

        // POST: Schedules/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateSchedule([Bind("ScheduleId,PhysicianId,AppointmentId,ScheduleDate,ScheduleTime,ScheduleStatus")] Schedule schedule)
        {
            if (ModelState.IsValid)
            {
                _context.Add(schedule);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["AppointmentId"] = new SelectList(_context.Appointments, "AppointmentId", "AppointmentId", schedule.AppointmentId);
            ViewData["PhysicianId"] = new SelectList(_context.Physicians, "PhysicianId", "PhysicianId", schedule.PhysicianId);
            return View(schedule);
        }


        // GET: Schedules/Edit/5
        public async Task<IActionResult> EditSchedule(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var schedule = await _context.Schedules.FindAsync(id);
            if (schedule == null)
            {
                return NotFound();
            }
            ViewData["AppointmentId"] = new SelectList(_context.Appointments, "AppointmentId", "AppointmentId", schedule.AppointmentId);
            ViewData["PhysicianId"] = new SelectList(_context.Physicians, "PhysicianId", "PhysicianId", schedule.PhysicianId);
            return View(schedule);
        }

        // POST: Schedules/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditSchedule(int id, [Bind("ScheduleId,PhysicianId,AppointmentId,ScheduleDate,ScheduleTime,ScheduleStatus")] Schedule schedule)
        {
            if (id != schedule.ScheduleId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(schedule);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ScheduleExists(schedule.ScheduleId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["AppointmentId"] = new SelectList(_context.Appointments, "AppointmentId", "AppointmentId", schedule.AppointmentId);
            ViewData["PhysicianId"] = new SelectList(_context.Physicians, "PhysicianId", "PhysicianId", schedule.PhysicianId);
            return View(schedule);
        }

        private bool ScheduleExists(int scheduleId)
        {
            throw new NotImplementedException();
        }


        // GET: Schedules/Delete/5
        public async Task<IActionResult> DeleteSchedule(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var schedule = await _context.Schedules
    .Include(s => s.Appointment)
        .ThenInclude(a => a.Patient)
    .Include(s => s.Physician)
    .FirstOrDefaultAsync(m => m.ScheduleId == id);

            if (schedule == null)
            {
                return NotFound();
            }

            return View(schedule);
        }

        // POST: Schedules/Delete/5
        [HttpPost, ActionName("DeleteSchedule")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteScheduleConfirmed(int id)
        {
            var schedule = await _context.Schedules.FindAsync(id);

            if (schedule == null)
            {
                return NotFound();
            }

            // Instead of deleting → make inactive
            schedule.ScheduleStatus = "Inactive";

            _context.Update(schedule);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }


        // GET: Appointments
        public async Task<IActionResult> GetAppointment()
        {
            var mediClinicDbContext = _context.Appointments.Include(a => a.Patient);
            return View(await mediClinicDbContext.ToListAsync());
        }

        // GET: Appointments/Details/5
        public async Task<IActionResult> AppointmentDetails(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var appointment = await _context.Appointments
                .Include(a => a.Patient)
                .FirstOrDefaultAsync(m => m.AppointmentId == id);
            if (appointment == null)
            {
                return NotFound();
            }

            return View(appointment);
        }

        // GET: Appointments/Create
        public IActionResult CreateAppointment()
        {
            ViewData["PatientId"] = new SelectList(_context.Patients, "PatientId", "PatientId");
            return View();
        }

        // POST: Appointments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateAppointment([Bind("AppointmentId,PatientId,AppointmentDate,Criticality,Reason,Note,ScheduleStatus")] Appointment appointment)
        {
            if (ModelState.IsValid)
            {
                _context.Add(appointment);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["PatientId"] = new SelectList(_context.Patients, "PatientId", "PatientId", appointment.PatientId);
            return View(appointment);
        }

        // GET: Appointments/Edit/5
        public async Task<IActionResult> EditAppointment(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment == null)
            {
                return NotFound();
            }
            ViewData["PatientId"] = new SelectList(_context.Patients, "PatientId", "PatientId", appointment.PatientId);
            return View(appointment);
        }

        // POST: Appointments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditAppointment(int id, [Bind("AppointmentId,PatientId,AppointmentDate,Criticality,Reason,Note,ScheduleStatus")] Appointment appointment)
        {
            if (id != appointment.AppointmentId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(appointment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AppointmentExists(appointment.AppointmentId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["PatientId"] = new SelectList(_context.Patients, "PatientId", "PatientId", appointment.PatientId);
            return View(appointment);
        }

        private bool AppointmentExists(int appointmentId)
        {
            throw new NotImplementedException();
        }

        // GET: Appointments/Delete/5
        public async Task<IActionResult> DeleteAppointment(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var appointment = await _context.Appointments
                .Include(a => a.Patient)
                .FirstOrDefaultAsync(m => m.AppointmentId == id);
            if (appointment == null)
            {
                return NotFound();
            }

            return View(appointment);
        }

        [HttpPost, ActionName("DeleteAppointment")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteAppointmentConfirmed(int id)
        {
            var appointment = await _context.Appointments.FindAsync(id);

            if (appointment != null)
            {
                // Soft delete
                appointment.ScheduleStatus = "Inactive";

                _context.Appointments.Update(appointment);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }



        // GET: Pending Appointments
        public async Task<IActionResult> PendingAppointments()
        {
            var pendingAppointments = await _context.Appointments
                .Include(a => a.Patient)
                .Where(a => a.ScheduleStatus == "Pending")
                .ToListAsync();

            return View(pendingAppointments);
        }


        public async Task<IActionResult> AssignDoctor(int id)
        {
            var appointment = await _context.Appointments
                .Include(a => a.Patient)
                .FirstOrDefaultAsync(a => a.AppointmentId == id);

            if (appointment == null)
                return NotFound();

            ViewBag.Appointment = appointment;

            ViewData["PhysicianId"] = new SelectList(
                _context.Physicians.Where(p => p.PhysicianStatus == "Active"),
                "PhysicianId",
                "PhysicianName"
            );

            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignDoctor(Schedule schedule)
        {
            // 🔥 Check Doctor Availability
            var doctorBusy = await _context.Schedules
                .AnyAsync(s =>
                    s.PhysicianId == schedule.PhysicianId &&
                    s.ScheduleDate == schedule.ScheduleDate &&
                    s.ScheduleTime == schedule.ScheduleTime &&
                    s.ScheduleStatus == "Scheduled");

            if (doctorBusy)
            {
                TempData["Error"] = "Doctor is busy at this time. Assign another doctor.";
                return RedirectToAction("AssignDoctor", new { id = schedule.AppointmentId });
            }

            // Save Schedule
            schedule.ScheduleStatus = "Scheduled";
            _context.Schedules.Add(schedule);

            // Update Appointment Status
            var appointment = await _context.Appointments
                .FirstOrDefaultAsync(a => a.AppointmentId == schedule.AppointmentId);

            if (appointment != null)
            {
                appointment.ScheduleStatus = "Scheduled";
                _context.Update(appointment);
            }

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(GetSchedule));
        }










        // ============================================================
        // ================= REST OF CONTROLLERS ======================
        // Patients, Physicians, Chemists, Schedules
        // ... [Keep all your previous code as-is]
    }
}
