namespace Clinic.DTOs;
using System.Collections.Generic;

public class PatientDetailsDto
{
    public int PatientId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public DateTime Birthdate { get; set; }
    public List<PrescriptionDetailsDto> Prescriptions { get; set; }
}

public class PrescriptionDetailsDto
{
    public int PrescriptionId { get; set; }
    public DateTime Date { get; set; }
    public DateTime DueDate { get; set; }
    public List<MedicamentDto> Medicaments { get; set; }
    public DoctorDto Doctor { get; set; }
}

public class MedicamentDto
{
    public int MedicamentId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public int Dose { get; set; }
}