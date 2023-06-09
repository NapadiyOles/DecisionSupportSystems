using System.Globalization;

namespace AdmissionSupportSystem;

public record Applicant
{
    public Applicant(string name)
    {
        Name = name;
    }

    public string Name { get; }
    public int MathScore { get; set; }
    public int EnglishScore { get; set; }
    public int UkrainianScore { get; set; }
    public bool HasPrivileges { get; set; }
    public decimal Rating { get; set; }
    
}