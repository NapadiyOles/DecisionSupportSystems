
const int payout = 25_000;

//Forecast probabilities
var inspection = new List<Forecast>
{
    new("Win") { Coef = 0.75, Win = 0.85, Loss = 0.15 },
    new("Loss") { Coef = 0.25, Win = 0.25, Loss = 0.75 },
};

var strategies = new List<StrategyData>
{
    new("Big Factory") { WinValue = 200_000, LossValue = -180_000 },
    new("Small Business") { WinValue = 100_000, LossValue = -20_000 },
    new("Patent Sale") { WinValue = 10_000, LossValue = 10_000 },
};

//Strategies
List<Strategy> GetDefault(double win = 0.5, double loss = 0.5) => strategies
    .Select(s => new Strategy(s.Name, new List<Decision>
    {
        new("Win", coef: win, value: s.WinValue),
        new("Loss", coef: loss, value: s.LossValue),
    })).ToList();


//Build tree
var root = new Decision(strategies:new List<Strategy>
{
    new("NotInspect", GetDefault()),
    new("Inspect", payout:payout, nodes:inspection!
        .Select(e => new Decision(name:e.Status, coef:e.Coef, strategies:GetDefault(e.Win, e.Loss))))
});

Console.WriteLine($"Result is {root.Value}");
Console.WriteLine();

root.Display();


internal sealed class StrategyData
{
    public StrategyData(string name)
    {
        Name = name;
    }

    public string Name { get; set; }
    public int WinValue { get; set; }
    public int LossValue { get; set; }
}

internal sealed class Forecast
{
    public Forecast(string status)
    {
        Status = status;
    }

    public string Status { get; set; }
    public double Coef { get; set; }
    public double Win { get; set; }
    public double Loss { get; set; }
}

abstract class Node
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
        Console.WriteLine($"{Name}: {Value}");

        if(Children is null) return;
        
        foreach (var child in Children)
        {
            child.Display(level + 1);
        }
    }
}

internal sealed class Strategy : Node
{
    public Strategy(string name, IEnumerable<Node> nodes, double payout = 0) : base(name)
    {
        Payout = payout;
        Children = nodes;
    }

    public double Payout { get; }
    public override double Value => CalcValue();

    private double CalcValue()
    {
        if (Children!.First() is Strategy)
            return Children!.Max(e => e.Value);

        if (Children!.First() is Decision)
            return Children!.Sum(e => (e as Decision)!.Coef * e.Value)
                   - (Name == "Inspect" ? Payout : 0);

        throw new InvalidCastException();
    }
}

internal sealed class Decision : Node
{
    private double _value;
    public Decision(string name = "Root", double coef = 0.5, double value = 0, IEnumerable<Strategy> strategies = null!) : base(name)
    {
        _value = value;
        Coef = coef;
        Children = strategies;
    }

    public double Coef { get; set; }

    public override double Value => CalcValue();

    private double CalcValue()
    {
        if (Children is null)
            return _value;
        
        return Children!.Max(e => e.Value);
    }
}

