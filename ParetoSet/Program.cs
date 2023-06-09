using ParetoSet;



var houses = new List<House>
{
    new("A1") { Cost = Cost.Cheap, Size = Size.Big, Status = Status.MajorRepairsRequired, Distance = Distance.Near },
    new("A2") { Cost = Cost.Expensive, Size = Size.Medium, Status = Status.EuroRenovationMade, Distance = Distance.Far },
    new("A3") { Cost = Cost.Cheap, Size = Size.Small, Status = Status.CosmeticRepairsRequired, Distance = Distance.Far },
    new("A4") { Cost = Cost.Cheap, Size = Size.Big, Status = Status.CosmeticRepairsRequired, Distance = Distance.Out },
    new("A5") { Cost = Cost.Cheap, Size = Size.Medium, Status = Status.EuroRenovationMade, Distance = Distance.Out },
    new("A6") { Cost = Cost.Expensive, Size = Size.Big, Status = Status.EuroRenovationMade, Distance = Distance.Far },
    new("A7") { Cost = Cost.Cheap, Size = Size.Medium, Status = Status.CosmeticRepairsRequired, Distance = Distance.Far },
    new("A8") { Cost = Cost.Expensive, Size = Size.Small, Status = Status.EuroRenovationMade, Distance = Distance.Out },
    new("A9") { Cost = Cost.Cheap, Size = Size.Small, Status = Status.CosmeticRepairsRequired, Distance = Distance.Near },
    new("A10") { Cost = Cost.Expensive, Size = Size.Big, Status = Status.MajorRepairsRequired, Distance = Distance.In },
};

var mask = Enumerable.Range(0, houses.Count).Select(e => true).ToList();

foreach (var (outer, i) in houses.Select((v, i) => (val: v, idx: i)))
{
    foreach (var (inner, j) in houses.Select((v, y) => (val: v, idx: y)))
    {
        if (i != j && inner.Cost <= outer.Cost && inner.Size <= outer.Size && inner.Status <= outer.Status && inner.Distance <= outer.Distance)
        {
            mask[j] = false;
        }
    }
}

var effective = houses.Zip(mask, (a, b) => (house: a, effective: b))
    .Where(e => e.effective).Select(e => e.house);

Console.WriteLine("Name".PadRight(5) + "Cost".PadRight(15) + "Size".PadRight(10) + "Status".PadRight(30) + "Distance");
Console.WriteLine(new string(Enumerable.Repeat('-', 70).ToArray()));
foreach (var house in effective)
{
    Console.WriteLine($"{house.Name}".PadRight(5) +
                      $"{house.Cost}".PadRight(15) +
                      $"{house.Size}".PadRight(10) +
                      $"{house.Status}".PadRight(30) +
                      $"{house.Distance}");
}

Console.WriteLine();

var normalized = effective.Select(e => new HouseNormalized(e.Name)
{
    Cost = (int)e.Cost / (double)effective.Max(h => h.Cost),
    Size = (int)e.Size / (double)effective.Max(h => h.Size),
    Status = (int)e.Status / (double)effective.Max(h => h.Status),
    Distance = (int)e.Distance / (double)effective.Max(h => h.Distance),
});


var final = normalized
    .Select(e => (Name: e.Name, Sum: e.Cost + e.Status + e.Size + e.Distance))
    .OrderByDescending(e => e.Sum);

foreach (var house in final)
{
    Console.WriteLine($"House: {house.Name}\tSum: {Math.Round(house.Sum, 3)}");
}

Console.WriteLine();