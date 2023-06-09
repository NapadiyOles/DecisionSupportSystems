

const int cost = 120;       //K
const int number = 20000;   //N

var suppliers = new List<SupplierData>
{
    new("A") { Retreat = 0, FailureProbabilities = new(5) { 0.4, 0.2, 0.1, 0.1, 0.1 } },
    new("B") { Retreat = 1200, FailureProbabilities = new(5) { 0.4, 0.35, 0.2, 0.1, 0.05 } },
};

var root = new Root
{
    Children = suppliers.Select(s => new Supplier(s.Name)
    { Retreat = s.Retreat,
        Children = s.FailureProbabilities.Select((f, i) => new Product(s.Name + (i + 1))
            { FailureProbability = f, Number = number, Cost = cost })
    })
};

root.Display();
Console.WriteLine();

Console.WriteLine($"The best supplier is {new string(root.Best.Take(1).ToArray())}");

internal class SupplierData
{
    public SupplierData(string name)
    {
        Name = name;
    }

    public string Name { get; }
    public int Retreat { get; init; }
    public List<double> FailureProbabilities { get; init; }
}

internal class Node
{
    protected Node(string name)
    {
        Name = name;
    }

    public string Name { get; set; }
    public virtual double Value { get; set; }
    public IEnumerable<Node>? Children { get; set; }
    
    public void Display(int level = 0)
    {
        Console.Write(new string(' ', level * 10));
        var val = Value;
        Console.WriteLine($"{Name}: {val}");

        if(Children is null) return;
        
        foreach (var child in Children)
        {
            child.Display(level + 1);
        }
    }
}

internal sealed class Root : Node
{
    public Root() : base("Root")
    {
    }
    
    public string Best { get; private set; }
    public override double Value => CalcValue();

    double CalcValue()
    {
        var supplier = Children?.MinBy(s => s.Value);

        Best = supplier?.Name ?? "NONE";

        return supplier?.Value ?? 0;
    }
}

internal sealed class Product : Node
{
    public Product(string name) : base(name)
    {
    }

    public double FailureProbability { get; init; }
    public int Number { get; init; }
    public int Cost { get; init; }

    public override double Value => CalcValue();

    double CalcValue()
    {
        Name += $" ({Number} * {FailureProbability} + {Cost})";
        
        return Number * FailureProbability * Cost;
    }
}

internal sealed class Supplier : Node
{
    public Supplier(string name) : base(name)
    {
    }

    public int Retreat { get; set; }

    public override double Value => CalcValue();

    double CalcValue()
    {
        var sum = Children?.Sum(p => p.Value) ?? 0;

        Name += $" ({sum} - {Retreat})";

        return sum - Retreat;
    }
}