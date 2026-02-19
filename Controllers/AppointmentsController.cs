using MediClinic.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;

namespace MediClinic.Controllers
{
    public class AppointmentsController : PatientBaseController
    {
        private readonly MediClinicDbContext _context;

        public AppointmentsController(MediClinicDbContext context)
        {
            _context = context;
        }

        // ============================================
        // VIEW ALL APPOINTMENTS (Patient Specific)
        // ============================================
        public IActionResult Index(string status)
        {
            var check = RequireLogin();
            if (check != null) return check;

            int patientId = PatientId.Value;

            var appointments = _context.Appointments
                .Where(a => a.PatientId == patientId);

            if (!string.IsNullOrEmpty(status))
            {
                appointments = appointments
                    .Where(a => a.ScheduleStatus == status);
            }

            var list = appointments
                .OrderByDescending(a => a.AppointmentDate)
                .ToList();

            return View(list);
        }

        // ============================================
        // CREATE APPOINTMENT (GET)
        // ============================================
        public IActionResult Create()
        {
            var check = RequireLogin();
            if (check != null) return check;

            return View();
        }

        // ============================================
        // CREATE APPOINTMENT (POST)
        // ============================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Appointment appointment)
        {
            var check = RequireLogin();
            if (check != null) return check;

            if (!ModelState.IsValid)
                return View(appointment);

            appointment.PatientId = PatientId;
            appointment.ScheduleStatus = "Pending";

            _context.Appointments.Add(appointment);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }



        // ============================================
        // CANCEL APPOINTMENT
        // ============================================
        public IActionResult Cancel(int id)
        {
            var check = RequireLogin();
            if (check != null) return check;

            int patientId = PatientId.Value;

            var appointment = _context.Appointments
                .FirstOrDefault(a =>
                    a.AppointmentId == id &&
                    a.PatientId == patientId);

            if (appointment == null)
                return NotFound();

            if ((appointment.ScheduleStatus == "Pending" ||
                 appointment.ScheduleStatus == "Scheduled") &&
                 appointment.AppointmentDate.HasValue &&
                 appointment.AppointmentDate > DateTime.Now)
            {
                appointment.ScheduleStatus = "Cancelled";
                _context.SaveChanges();
            }

            return RedirectToAction("Index");
        }

        // ============================================
        // VIEW APPOINTMENT DETAILS
        // ============================================
        public IActionResult Details(int id)
        {
            var check = RequireLogin();
            if (check != null) return check;

            int patientId = PatientId.Value;

            var appointment = _context.Appointments
                .FirstOrDefault(a =>
                    a.AppointmentId == id &&
                    a.PatientId == patientId);

            if (appointment == null)
                return NotFound();

            return View(appointment);
        }
    }
}

