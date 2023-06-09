namespace ParetoSet;

internal sealed class House
{
    public House(string name)
    {
        Name = name;
    }

    public string Name { get; set; }
    public Cost Cost { get; set; }
    public Size Size { get; set; }
    public Status Status { get; set; }
    public Distance Distance { get; set; }
}

internal sealed class HouseNormalized
{
    public HouseNormalized(string name)
    {
        Name = name;
    }

    public string Name { get; set; }
    public double Cost { get; set; }
    public double Size { get; set; }
    public double Status { get; set; }
    public double Distance { get; set; }
}


enum Cost
{
    Expensive,
    Cheap
}

enum Size
{
    Small,
    Medium,
    Big
}

enum Status
{ 
    MajorRepairsRequired,
    CosmeticRepairsRequired,
    EuroRenovationMade
}

enum Distance
{
    Out,
    Far,
    Near,
    In,
}