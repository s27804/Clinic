namespace Clinic.Models;
using System.Collections.Generic;

public class Doctor
{
    public int DoctorId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }

    public ICollection<Prescription> Prescriptions { get; set; }
}