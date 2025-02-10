using System.Dynamic;
using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using GetCountries.Extensions;
using Spectre.Console;

namespace GetCountries;

public class Worker
{
    private GetCountriesOptions Options { get; }
    private string FullPath { get; }
    private string OutputFilePath { get; }
    private List<dynamic> ParseResult { get; set; } = new();
    private List<string?> Postcodes { get; set; } = new();

    private readonly bool _isReady;

    public Worker(GetCountriesOptions options)
    {
        Options = options;
        FullPath = options.GetFullInputPath();
        OutputFilePath = options.GetFullOutputPath();
        _isReady = true;
    }

    public async Task<int> Run()
    {
        AnsiConsole.MarkupLine("[bold]Starting postcode lookup...[/]");
        if (!_isReady || !ParseCsv())
        {
            return 1;
        }

        MapToPostcodes();
        await FetchPostcodesAndMerge();
        WriteCsv();
        AnsiConsole.MarkupLine("Output file written to:");
        var path = new TextPath(OutputFilePath)
            .RootColor(Color.Yellow)
            .SeparatorColor(Color.Yellow)
            .StemColor(Color.Yellow)
            .LeafColor(Color.Yellow);
        AnsiConsole.Write(path);
        AnsiConsole.WriteLine();
        return 0;
    }

    private bool ParseCsv()
    {
        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HeaderValidated = null,
            MissingFieldFound = null,
        };
        using var reader = new StreamReader(FullPath);
        using var csv = new CsvReader(reader, config);
        ParseResult = csv.GetRecords<dynamic>().ToList();
        var header = Options.HeaderName;
        var headerRecord = csv.HeaderRecord;
        if (headerRecord != null && headerRecord.Contains(header)) return true;

        AnsiConsole.MarkupLine($"[bold red]Error:[/] Column header [bold]{header}[/] not found in input file.");
        return false;
    }

    private void MapToPostcodes()
    {
        Postcodes = ParseResult.Select(vp => (string?)((IDictionary<string, object>)vp)[Options.HeaderName]).ToList();
    }

    private async Task FetchPostcodesAndMerge()
    {
        var service = new PostcodeService();
        var postcodeResults = await service.GetBulkPostcodes(Postcodes);
        MergeData(postcodeResults);
    }

    private void MergeData(List<PostcodeResult> postcodeResults)
    {
        for (var i = 0; i < ParseResult.Count; i++)
        {
            var parseResult = ParseResult[i];
            var found = postcodeResults.Find(pi =>
                pi.Query == (string?)((IDictionary<string, object>)parseResult)[Options.HeaderName]);
            var updated = new ExpandoObject();
            var updatedDict = (IDictionary<string, object>)updated;

            foreach (var property in (IDictionary<string, object>)parseResult)
            {
                updatedDict[property.Key] = property.Value;
            }

            updatedDict[Options.CountryHeaderName] = found?.Result?.country ?? string.Empty;
            ParseResult[i] = updated;
        }
    }

    private void WriteCsv()
    {
        using var writer = new StreamWriter(OutputFilePath);
        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            Delimiter = Options.GetOutputExtension().Item1.ToDelimiter(),
        };
        using var csv = new CsvWriter(writer, config);
        csv.WriteRecords(ParseResult);
    }
}