using System.Data;

namespace BinaryRelations;

public static class ArrayExtensions
{
    public static int[,] Mirroring(this int[,] arr)
    {
        var rows = arr.GetLength(0);
        var cols = arr.GetLength(1);
        
        for (int i = 1; i < rows; i++)
        {
            for (int j = 0; j < i; j++)
            {
                arr[i, j] = -arr[j, i];
            }
        }

        return arr;
    }

    public static int[,] Display(this int[,] arr, string name)
    {
        var rows = arr.GetLength(0);
        var cols = arr.GetLength(1);

        Console.WriteLine(name);
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                var val = arr[i, j];
                Console.Write(((val < 0 ? "" : " ") + val).PadRight(5));
            }
            Console.WriteLine();
        }
        return arr;
    }

    public static int[] Sum(this int[,] arr)
    {
        var rows = arr.GetLength(0);
        var cols = arr.GetLength(1);
        var sums = new int[rows];

        for (int i = 0; i < rows; i++)
        {
            var sum = 0;
            for (int j = 0; j < cols; j++)
            {
                sum += arr[i, j];
            }

            sums[i] = sum;
        }

        return sums;
    }

    public static IEnumerable<IEnumerable<(string Sport, int Sum)>> GetCombinations(
        this IEnumerable<(string Sport, int Sum)> source)
    {
        var list = source.ToList();

        for (int i = 0; i < list.Count; i++)
        {
            for (int j = i; j < list.Count; j++)
            {
                if (i != j)
                {
                    for (int k = j; k < list.Count; k++)
                    {
                        if (i != k && j != k)
                        {
                            yield return new List<(string Sport, int Sum)> { list[i], list[j], list[k] };
                        }
                    }
                }
            }
        }
    }

    public static bool CheckTransitivity(this IEnumerable<int> source)
    {
        var list = source.ToList();
        if (list[0] > list[1] && list[1] > list[2])
            return true;

        return false;
    }

    public static bool CheckNonTransitivity(this IEnumerable<int> source)
    {
        var list = source.ToList();
        if (list[0] < list[1] && list[1] < list[2])
            return true;

        return false;
    }

    public static string SetTransitivityCharacteristics(this IEnumerable<int> source)
    {
        var list = source.ToList();
        if (CheckTransitivity(list))
            return "Transitive";

        if (CheckNonTransitivity(list))
            return "NonTransitive";

        return "Can't be checked";
    }
}