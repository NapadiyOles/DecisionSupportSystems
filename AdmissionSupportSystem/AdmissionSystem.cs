using System.Net.Http.Headers;
using System.Text.Json;
using Bogus;
using OfficeOpenXml;

namespace AdmissionSupportSystem;

public sealed class AdmissionSystem : IDisposable
{
    private Config _cfg;
    private string _pathDb, _pathRes;
    private List<Applicant> _applicants;

    public AdmissionSystem(string pathCfg, string pathDb, string pathRes)
    {
        _pathDb = pathDb;
        _pathRes = pathRes;

        ReadConfig(pathCfg);
        ReadJson();
    }

    private void ReadJson()
    {
        var json = File.ReadAllText(_pathDb);
        _applicants = JsonSerializer.Deserialize<List<Applicant>>(json)?.ToList() ??
                      throw new NullReferenceException();
        
        if(_applicants.Count == 0)
        {
            _applicants = GenerateApplicants(1500).ToList();
        }
    }

    private void ReadConfig(string pathCfg)
    {
        var json = File.ReadAllText(pathCfg);
        _cfg = JsonSerializer.Deserialize<Config>(json) ??
               throw new NullReferenceException("Config is invalid or missing.");
    }

    public IEnumerable<Applicant> GetAllApplicants()
    {
        return CalculateRating();
    }

    public void AddApplicant(string name, int math, int eng, int ukr, bool privileged)
    {
        _applicants.Add(new Applicant(name)
        {
            MathScore = math,
            EnglishScore = eng,
            UkrainianScore = ukr,
            HasPrivileges = privileged,
        });
    }

    public bool RemoveApplicant(string name)
    {
        var index = _applicants.FindIndex(0, a => a.Name == name);

        if (index < 0)
            return false;
        
        _applicants.RemoveAt(index);

        return true;
    }

    IEnumerable<Applicant> GenerateApplicants(int count)
    {
        
        var faker = new Faker();
        return Enumerable.Range(1, count).Select(e =>
            new Applicant(faker.Name.FullName())
            {
                MathScore = faker.Random.Number(100, 200),
                EnglishScore = faker.Random.Number(100, 200),
                UkrainianScore = faker.Random.Number(100, 200),
                HasPrivileges = faker.Random.Bool(0.1f),
            }).ToList();
    }

    public IEnumerable<Applicant> GetAccepted()
    {
        var slotsPrivileged = (int)Math.Round(_cfg.MaxAccepted * _cfg.PrivilegedPercentage);

        var applicants = CalculateRating();

        var nonPrivileged = applicants
            .Where(a => !a.HasPrivileges && a.Rating >= _cfg.MinRating && a.MathScore >= _cfg.MinMathScore)
            .OrderByDescending(a => a.Rating).Take(_cfg.MaxAccepted - slotsPrivileged).ToList();
        
        var privileged = applicants
            .Where(a => a.HasPrivileges && a.Rating >= _cfg.MinPrivilegedRating 
                                        && a.MathScore >= _cfg.MinPrivilegedScore
                                        && a.EnglishScore >= _cfg.MinPrivilegedScore
                                        && a.UkrainianScore >= _cfg.MinPrivilegedScore)
            .OrderByDescending(a => a.Rating).Take(slotsPrivileged).ToList();

        
        var res = nonPrivileged.Concat(privileged).OrderByDescending(a => a.Rating).ToList();
        
        WriteToExcel(res);

        return res;
    }

    private List<Applicant> CalculateRating()
    {
        return _applicants.Select(a => a with
        {
            Rating = _cfg.MathCoefficient * a.MathScore +
                     _cfg.EnglishCoefficient * a.EnglishScore +
                     _cfg.UkrainianCoefficient * a.UkrainianScore
        }).ToList();
    }

    void WriteToExcel(List<Applicant> list)
    {
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        using var package = new ExcelPackage();
        ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Accepted applicants");
        
        // Write header row
        worksheet.Cells[1, 1].Value = "Name";
        worksheet.Cells[1, 2].Value = "Privileged";
        worksheet.Cells[1, 3].Value = "Rating";
        worksheet.Cells[1, 4].Value = "Math score";
        worksheet.Cells[1, 5].Value = "English score";
        worksheet.Cells[1, 6].Value = "Ukrainian score";

        // Write data rows
        for (int i = 0; i < list.Count; i++)
        {
            Applicant applicant = list[i];
            int row = i + 2;

            worksheet.Cells[row, 1].Value = applicant.Name;
            worksheet.Cells[row, 2].Value = applicant.HasPrivileges ? "Yes" : "No";
            worksheet.Cells[row, 3].Value = applicant.Rating;
            worksheet.Cells[row, 4].Value = applicant.MathScore;
            worksheet.Cells[row, 5].Value = applicant.EnglishScore;
            worksheet.Cells[row, 6].Value = applicant.UkrainianScore;

        }
        
        // Auto-fit columns
        worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();
        
        // Save the Excel file
        FileInfo file = new FileInfo(_pathRes);
        package.SaveAs(file);
    }

    public void Dispose()
    {
        var json = JsonSerializer.Serialize(_applicants);
        File.WriteAllText(_pathDb, json);
    }
}