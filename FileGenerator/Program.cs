using System.Text;
using System.Text.RegularExpressions;

namespace FileGenerator;

/// <summary>
/// Class responsible for generating files with random "Number. String" lines
/// </summary>
public class RandomDataFileGenerator(string outputFile, double targetSizeInGB)
{
    private readonly string _outputFile = outputFile;
    private readonly long _targetSizeInBytes = (long)(targetSizeInGB * 1024 * 1024 * 1024);
    private readonly string[] _sampleStrings = ["Apple", "Banana is yellow", "Cherry is the best", "Something something something"];
    private readonly Random _random = new();

    /// <summary>
    /// Generate output file
    /// </summary>
    public void GenerateFile()
    {
        long currentSize = 0;

        using var writer = new StreamWriter(_outputFile, false, Encoding.UTF8);
        while (currentSize < _targetSizeInBytes)
        {
            string line = GenerateRandomData();
            writer.WriteLine(line);

            currentSize += Encoding.UTF8.GetByteCount(line + Environment.NewLine);
        }
    }

    /// <summary>
    /// Generate data from sample
    /// </summary>
    /// <returns></returns>
    private string GenerateRandomData()
    {
        int number = _random.Next(1, 100000);
        string text = _sampleStrings[_random.Next(_sampleStrings.Length)];
        return $"{number}. {text}";
    }
}

class Program
{
    static void Main()
    {
        string outputFile = GetValidatedInput<string>(
            "Enter the output file name (e.g., output.txt): ",
            "Invalid file name.",
            input => !string.IsNullOrWhiteSpace(input) && IsValidFileName(input));

        double targetSizeInGB = GetValidatedInput(
            "Enter the size of the file in GB: ",
            "Invalid size.",
            input => double.TryParse(input, out double size) && size > 0,
            double.Parse);

        var generator = new RandomDataFileGenerator(outputFile, targetSizeInGB);
        generator.GenerateFile();

        Console.WriteLine($"File generated: {outputFile} ({targetSizeInGB} GB)");
    }

    /// <summary>
    /// A universal method for receiving and validating user input.
    /// </summary>
    /// <typeparam name="T"> The data type returned </typeparam>
    /// <param name="inputMessage"></param>
    /// <param name="errorMessage"></param>
    /// <param name="validationLogic"></param>
    /// <param name="parseInputs"></param>
    /// <returns></returns>
    private static T GetValidatedInput<T>(string inputMessage, string errorMessage, Func<string, bool> validationLogic, Func<string, T>? parseInputs = null)
    {
        while (true)
        {
            Console.Write(inputMessage);
            string? input = Console.ReadLine();

            if (!string.IsNullOrWhiteSpace(input) && validationLogic(input))
            {
                return parseInputs != null ? parseInputs(input) : (T)(object)input!;
            }

            Console.WriteLine(errorMessage);
        }
    }

    /// <summary>
    /// Check for valid file name
    /// </summary>
    /// <param name="fileName"></param>
    /// <returns></returns>
    private static bool IsValidFileName(string fileName)
    {
        if (string.IsNullOrWhiteSpace(fileName))
        {
            return false;
        }

        string pattern = @"^[\w\-. ]+\.[a-zA-Z0-9]+$";
        return Regex.IsMatch(fileName, pattern);
    }
}