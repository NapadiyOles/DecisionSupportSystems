using System.Text;
using System.Text.Json;


var path = @"D:\Documents\Repos\DecisionSupportSystem\DecisionTree.Task1\strategies.json";

var strategies = JsonSerializer.Deserialize<List<Strategy>>(File.ReadAllText(path));

var root = new Root(strategies!);
var result = root.Calculate();

Console.WriteLine(root);
Console.WriteLine($"The best strategy is {result.strategy} with gain value of {result.value}");

internal class Root
{
    public Root(List<Strategy> strategies)
    {
        Strategies = strategies;
    }

    public double Value { get; set; }
    public List<Strategy> Strategies { get; set; }

    public (string strategy, double value) Calculate()
    {
        var best = Strategies.MaxBy(strategy => strategy.Value) ?? 
                   throw new NullReferenceException();

        Value = best.Value;
        return (best.Name, best.Value);
    }

    public override string ToString()
    {
        var tree = new StringBuilder($"Root: {Value}\n");
        foreach (var strategy in Strategies)
        {
            strategy.Display(tree, 1);
        }

        return tree.ToString();
    }
}

internal class Strategy
{
    public Strategy(string name, Leaf win, Leaf loss)
    {
        Name = name;
        Win = win;
        Loss = loss;
    }

    public string Name { get; }
    public Leaf Win { get; }
    public Leaf Loss { get; }
    public double Value => Win.Value / 2.0 + Loss.Value / 2.0;
    
    public void Display(StringBuilder tree, int level)
    {
        tree.Append(new string(' ', level * 6));
        tree.AppendLine($"{Name}: {Value}");

        Win.Display(tree, "Win", level + 1);
        Loss.Display(tree, "Loss", level + 1);
    }
}

internal class Leaf
{
    public Leaf(int value)
    {
        Value = value;
    }

    public int Value { get; }
    
    public void Display(StringBuilder tree, string type, int level)
    {
        tree.Append(new string(' ', level * 6));
        tree.AppendLine($"{type}: {Value}");
    }
}