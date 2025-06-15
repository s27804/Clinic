namespace Clinic.DTOs;
using System.Collections.Generic;

public class PrescriptionRequestDto
{
    public DateTime Date { get; set; }
    public DateTime DueDate { get; set; }
    public DoctorDto Doctor { get; set; }
    public PatientDto Patient { get; set; }
    public List<PrescriptionMedicamentDto> Medicaments { get; set; }
}

public class DoctorDto
{
    public int DoctorId { get; set; }
}

public class PatientDto
{
    public int PatientId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public DateTime Birthdate { get; set; }
}

public class PrescriptionMedicamentDto
{
    public int MedicamentId { get; set; }
    public int Dose { get; set; }
    public string Details { get; set; }
}