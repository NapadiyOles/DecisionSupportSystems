using System.Globalization;
using System.Text.Json;
using AdmissionSupportSystem;

var cfg = @"D:\Documents\Repos\DecisionSupportSystems\AdmissionSupportSystem\config.json";
var db = @"D:\Documents\Repos\DecisionSupportSystems\AdmissionSupportSystem\applicants.json";
var res = @"D:\Documents\Repos\DecisionSupportSystems\AdmissionSupportSystem\result.xlsx";

using var system = new AdmissionSystem(cfg, db, res);

var repeat = true;
var keys = new List<ConsoleKey> { ConsoleKey.D1, ConsoleKey.D2, ConsoleKey.Q };

do
{
    ConsoleKey key;
    do
    {
        Console.Clear();
        Console.Write(
            "Press the correct key to perform action:\n" +
            "1: Display applicants\n" +
            "2: Add applicant\n" +
            "3: Remove applicant\n" +
            "4: Perform selection\n" +
            "Q: Quit\n" +
            "Waiting for key press..."
        );

        key = Console.ReadKey(true).Key;
        switch (key)
        {
            case ConsoleKey.D1:
                Display(system.GetAllApplicants());
                break;
            case ConsoleKey.D2:
                PerformAddition();
                break;
            case ConsoleKey.D3:
                PerformRemoval();
                break;
            case ConsoleKey.D4:
                Display(system.GetAccepted());
                break;
            case ConsoleKey.Q:
                repeat = false;
                break;
        }
    } while (!keys.Contains(key));

} while (repeat);

void PerformAddition()
{
    Console.Clear();
    Console.WriteLine("Enter the data regarding applicant");
    
    string name;
    do
    {
        Console.Write("Enter name: ");
        name = Console.ReadLine() ?? "";    
    } while (name == "");
    
    Console.Write("Has privileges (y/n): ");
    ConsoleKey answer;
    do
    {
        answer = Console.ReadKey(true).Key;
    } while (answer != ConsoleKey.Y && answer != ConsoleKey.N);

    var privileged = answer == ConsoleKey.Y;
    
    int math;
    do
    {
        Console.Write("Enter math score (from 100 to 200): ");
    } while (!int.TryParse(Console.ReadLine(), out math) || math is < 100 or > 200);
    
    int eng;
    do
    {
        Console.Write("Enter english score (from 100 to 200): ");
    } while (!int.TryParse(Console.ReadLine(), out eng) || eng is < 100 or > 200);
    
    int ukr;
    do
    {
        Console.Write("Enter ukrainian score (from 100 to 200): ");
    } while (!int.TryParse(Console.ReadLine(), out ukr) || ukr is < 100 or > 200);

    
    system.AddApplicant(name, math, eng, ukr, privileged);

    Console.Write("New applicant has been added...");
    Console.ReadKey(true);
}

void PerformRemoval()
{
    Console.Clear();
    string name;
    do
    {
        Console.Write("Enter name: ");
        name = Console.ReadLine() ?? "";    
    } while (name == "");

    Console.Write(system.RemoveApplicant(name) ? "Applicant successfully removed..." : "Applicant not found...");

    Console.ReadKey(true);
}

void Display(IEnumerable<Applicant> list)
{
    int i = 0, chunk = 10;
    var count = list.Count();
    foreach (var applicants in list.Chunk(chunk))
    {
        ConsoleKey key;

        Console.Clear();
        Console.WriteLine($"Number of records: {count}");
        Console.WriteLine(
            "".PadRight(5) +
            "Name".PadRight(30) +
            "Rating".PadRight(10) +
            "Privileged".PadRight(15) +
            "Math score".PadRight(15) +
            "English score".PadRight(15) +
            "Ukrainian score"
        );
        
        foreach (var (a, y) in applicants.Select((a, y) => (applicant: a, index: y)))
        {
            Console.WriteLine(
                $"{i + y + 1}.".PadRight(5) +
                a.Name.PadRight(30) +
                (a.Rating == 0 ? "-" : a.Rating.ToString(CultureInfo.CurrentCulture)).PadRight(10) +
                (a.HasPrivileges ? "Yes" : "No").PadRight(15) +
                a.MathScore.ToString().PadRight(15) +
                a.EnglishScore.ToString().PadRight(15) +
                a.UkrainianScore.ToString().PadRight(0)
            );
        }

        Console.Write("Press space to continue or q to quit...");
        do
        {
            key = Console.ReadKey(true).Key;
        } while (key != ConsoleKey.Q && key != ConsoleKey.Spacebar);

        i += chunk;

        if (key == ConsoleKey.Q) break;
    }
}

