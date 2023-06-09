

using BinaryRelations;

var sports = new List<string>
{
    "Football", "Basketball", "Cricket", "Tennis", "Athletics", "Rugby"
};

var comparisons = new[,]
{   //Football, Basketball, Cricket,    Tennis,     Athletics,  Rugby
    { 0,        1,          1,         -1,          1,          1 },   //Football
    { 0,        0,          1,          1,          1,          1 },   //Basketball
    { 0,        0,          0,          1,         -1,         -1 },   //Cricket
    { 0,        0,          0,          0,          1,          1 },   //Tennis
    { 0,        0,          0,          0,          0,         -1 },   //Athletics
    { 0,        0,          0,          0,          0,          0 },   //Rugby
};

var mirrored = comparisons.Mirroring().Display("Comparisons");
Console.WriteLine();

// var mirrored = new[,]
// {
//     {  0, -1, -1,  1,  1,  0 },
//     {  1,  0,  1,  0, -1,  1 },
//     {  1, -1,  0,  1,  1, -1 },
//     { -1,  0, -1,  0,  1, -1 },
//     { -1,  1, -1, -1,  0,  1 },
//     {  0, -1,  1,  1, -1,  0 },
// };

var sums = sports.Zip(mirrored.Sum(), (a, b) => (Sport: a, Sum: b)).ToList();
var ordered = sums.OrderByDescending(e => e.Sum).ToList();

Console.WriteLine("Sport".PadRight(15) + "Sum");
foreach (var (sport, sum) in ordered)
{
    Console.WriteLine(sport.PadRight(15) + (sum < 0 ? "" : " ") + sum);
}

var combinations = sums.GetCombinations().ToList();//.Select(e => e.Select(p => p.Sum).SetTransitivityCharacteristics()).ToList();


Console.WriteLine();
Console.WriteLine("Combinations");
foreach (var (combination, characteristic) in combinations.Zip(combinations
             .Select(e => e.Select(p => p.Sum).SetTransitivityCharacteristics())))
{
    var list = combination.ToList();
    Console.WriteLine(list[0].ToString().PadRight(20) +
                      list[1].ToString().PadRight(20) +
                      list[2].ToString().PadRight(20) +
                      characteristic
                      );
}