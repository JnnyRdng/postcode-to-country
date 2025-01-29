using System.Dynamic;
using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;

namespace GetCountries;

public class Worker
{
    private const string Baseurl = "https://api.postodes.io/postcodes/";
    private string FullPath { get; }
    private string OutputFilePath { get; } = string.Empty;
    private List<dynamic> ParseResult { get; set; } = new();
    private List<string> Postcodes { get; set; } = new();

    private readonly bool _isReady;

    public Worker(string arg)
    {
        FullPath = Path.GetFullPath(arg);
        if (!File.Exists(FullPath))
        {
            Console.WriteLine($"File does not exist! '{FullPath}'");
            return;
        }

        var inputFileName = Path.GetFileName(FullPath);
        var ext = Path.GetExtension(inputFileName);
        if (ext != ".csv")
        {
            Console.WriteLine($"Invalid file format! {ext}");
            return;
        }

        var outputFile = $"countries-{inputFileName}";
        var outputDir = Path.GetDirectoryName(FullPath);

        OutputFilePath = Path.Combine(outputDir!, outputFile);
        _isReady = true;
    }

    public async Task<int> Run()
    {
        if (!_isReady) return 1;
        ParseCsv();
        MapToPostcodes();
        await FetchPostcodesAndMerge();
        WriteCsv();
        Console.WriteLine($"Updated CSV written to {OutputFilePath}");
        return 0;
    }

    private void ParseCsv()
    {
        var config = new CsvConfiguration((CultureInfo.InvariantCulture))
        {
            HeaderValidated = null,
            MissingFieldFound = null,
        };
        using var reader = new StreamReader(FullPath);
        using var csv = new CsvReader(reader, config);
        ParseResult = csv.GetRecords<dynamic>().ToList();
    }

    private void MapToPostcodes()
    {
        Postcodes = ParseResult.Select(vp => (string)vp.postcode).ToList();
    }

    private async Task FetchPostcodesAndMerge()
    {
        var service = new PostcodeService();
        var postcodeResults = await service.GetBulkPostcodes(Postcodes);
        MergeData(postcodeResults);
    }

    private void MergeData(List<PostcodeResult> postcodeInfo)
    {
        for (var i = 0; i < ParseResult.Count; i++)
        {
            var vp = ParseResult[i];
            var found = postcodeInfo.Find(p => p.Query == (string)vp.postcode);
            if (found == null) continue;

            var updatedVp = new ExpandoObject();
            var updatedDict = (IDictionary<string, object>)updatedVp;

            foreach (var property in (IDictionary<string, object>)vp)
            {
                updatedDict[property.Key] = property.Value;
            }

            updatedDict["country"] = found.Result.country;
            ParseResult[i] = updatedVp;
        }
    }

    private void WriteCsv()
    {
        using var writer = new StreamWriter(OutputFilePath);
        using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);
        csv.WriteRecords(ParseResult);
    }
}