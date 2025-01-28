using System.Dynamic;
using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;

namespace GetCountries;

public class Worker
{
    private const string Baseurl = "https://api.postodes.io/postcodes/";
    private string FullPath { get; }
    private string OutputFilePath { get; }
    private List<dynamic> ParseResult { get; set; } = new();
    private List<string> Postcodes { get; set; } = new();

    public Worker(string? arg)
    {
        if (arg == null)
        {
            throw new ArgumentException("CLI argument missing!");
        }

        FullPath = Path.GetFullPath(arg);
        if (!File.Exists(FullPath))
        {
            throw new FileNotFoundException($"File does not exist! '{FullPath}'");
        }

        var outputFile = $"countries-{Path.GetFileName(FullPath)}";
        var outputDir = Path.GetDirectoryName(FullPath);
        if (outputDir == null)
        {
            throw new DirectoryNotFoundException($"Directory does not exist!");
        }
        OutputFilePath = Path.Combine(outputDir, outputFile);
    }

    public async Task Run()
    {
        ParseCsv();
        MapToPostcodes();
        await FetchPostcodesAndMerge();
        WriteCsv();
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
        var postcodeInfo = await service.GetBulkPostcodes(Postcodes);
        MergeData(postcodeInfo);
    }

    private void MergeData(List<PostcodeInfo> postcodeInfo)
    {
        for (int i = 0; i < ParseResult.Count; i++)
        {
            var vp = ParseResult[i];
            var found = postcodeInfo.Find(p => p.postcode == (string)vp.postcode);
            if (found == null) continue;

            var updatedVp = new ExpandoObject();
            var updatedDict = (IDictionary<string, object>)updatedVp;

            foreach (var property in (IDictionary<string, object>)vp)
            {
                updatedDict[property.Key] = property.Value;
            }

            updatedDict["country"] = found.country;
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