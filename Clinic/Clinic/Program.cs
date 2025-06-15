using Clinic.DTOs;
using Clinic.Models;
using Clinic.DbContexts;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<ClinicDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapPost("/api/prescriptions", (PrescriptionRequestDto dto, ClinicDbContext db) =>
{
    if (dto.Medicaments.Count > 10)
        return Results.BadRequest("Recepta może zawierać maksymalnie 10 leków.");

    if (dto.DueDate < dto.Date)
        return Results.BadRequest("DueDate nie może być wcześniejszy niż Date.");

    var patient = db.Patients.FirstOrDefault(p => p.PatientId == dto.Patient.PatientId);
    if (patient == null)
    {
        patient = new Patient
        {
            FirstName = dto.Patient.FirstName,
            LastName = dto.Patient.LastName,
            Birthdate = dto.Patient.Birthdate
        };
        db.Patients.Add(patient);
        db.SaveChanges();
    }

    foreach (var med in dto.Medicaments)
    {
        if (!db.Medicaments.Any(m => m.MedicamentId == med.MedicamentId))
            return Results.BadRequest($"Lek o ID {med.MedicamentId} nie istnieje.");
    }

    var doctor = db.Doctors.FirstOrDefault(d => d.DoctorId == dto.Doctor.DoctorId);
    if (doctor == null)
        return Results.BadRequest("Lekarz nie istnieje.");

    var prescription = new Prescription
    {
        Date = dto.Date,
        DueDate = dto.DueDate,
        Doctor = doctor,
        Patient = patient
    };

    db.Prescriptions.Add(prescription);
    db.SaveChanges();

    foreach (var med in dto.Medicaments)
    {
        db.PrescriptionMedicaments.Add(new PrescriptionMedicament
        {
            MedicamentId = med.MedicamentId,
            PrescriptionId = prescription.PrescriptionId,
            Dose = med.Dose,
            Details = med.Details
        });
    }

    db.SaveChanges();
    return Results.Ok("Recepta dodana.");
});

app.MapGet("/api/patients/{id}", (int id, ClinicDbContext db) =>
{
    var patient = db.Patients
        .Include(p => p.Prescriptions)
            .ThenInclude(pr => pr.Doctor)
        .Include(p => p.Prescriptions)
            .ThenInclude(pr => pr.PrescriptionMedicaments)
                .ThenInclude(pm => pm.Medicament)
        .FirstOrDefault(p => p.PatientId == id);

    if (patient == null)
        return Results.NotFound("Pacjent nie istnieje.");

    var result = new
    {
        patient.PatientId,
        patient.FirstName,
        patient.LastName,
        patient.Birthdate,
        Prescriptions = patient.Prescriptions
            .OrderBy(p => p.DueDate)
            .Select(p => new
            {
                p.PrescriptionId,
                p.Date,
                p.DueDate,
                Doctor = new
                {
                    p.Doctor.DoctorId,
                    p.Doctor.FirstName,
                    p.Doctor.LastName,
                    p.Doctor.Email
                },
                Medicaments = p.PrescriptionMedicaments.Select(pm => new
                {
                    pm.Medicament.MedicamentId,
                    pm.Medicament.Name,
                    pm.Dose,
                    pm.Details
                })
            })
    };

    return Results.Ok(result);
});

app.Run();