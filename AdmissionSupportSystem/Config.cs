namespace AdmissionSupportSystem;

public class Config
{
    public decimal MathCoefficient { get; init; }
    public decimal EnglishCoefficient { get; init; }
    public decimal UkrainianCoefficient { get; init; }
    public int MaxAccepted { get; init; }
    public int MinRating { get; init; }
    public int MinMathScore { get; init; }
    public decimal PrivilegedPercentage { get; set; }
    public int MinPrivilegedRating { get; init; }
    public int MinPrivilegedScore { get; init; }
}