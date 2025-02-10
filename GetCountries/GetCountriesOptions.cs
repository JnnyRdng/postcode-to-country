using System.ComponentModel;
using Spectre.Console;
using Spectre.Console.Cli;

namespace GetCountries;

public sealed class GetCountriesOptions : CommandSettings
{
    [Description("Required. Absolute or relative path to [bold underline]csv[/] or [bold underline]tsv[/] file. ")]
    [CommandArgument(0, "<INPUTFILE>")]
    public string InputFile { get; }

    [Description("[italic]Optional.[/] Output filename with extension or without extension.\nCan be used to convert between valid file formats. ")]
    [CommandOption("-o|--output <FILENAME>")]
    public string? OutputFile { get; }

    [Description("[italic]Optional.[/] Postcode column column name. Case-sensitive. ")]
    [CommandOption("-c|--column <COLUMN>")]
    [DefaultValue("postcode")]
    public string HeaderName { get; }

    [Description("[italic]Optional.[/] Name of appended country column. ")]
    [CommandOption("-n|--new-column <NEWCOLUMN>")]
    [DefaultValue("country")]
    public string CountryHeaderName { get; private set; }

    public GetCountriesOptions(string inputFile, string? outputFile, string? headerName = null,
        string? countryHeaderName = null)
    {
        InputFile = inputFile;
        OutputFile = string.IsNullOrWhiteSpace(outputFile) ? null : outputFile;
        HeaderName = headerName ?? "postcode";
        CountryHeaderName = countryHeaderName ?? "country";
    }

    public override ValidationResult Validate()
    {
        if (string.IsNullOrWhiteSpace(InputFile) || !File.Exists(InputFile))
        {
            return ValidationResult.Error($"Please specify a valid input file. \"{InputFile}\"");
        }

        var (filetype, ext) = GetInputExtension();
        if (filetype == FileType.Invalid)
        {
            return ValidationResult.Error($"Input file must be .csv or .tsv. Found {ext}.");
        }

        if (OutputFile != null)
        {
            if (IsInvalidFilename(OutputFile))
            {
                return ValidationResult.Error("Output filename is invalid.");
            }

            if (Path.HasExtension(OutputFile))
            {
                var (fileType, outputExt) = GetExtension(OutputFile);
                if (fileType == FileType.Invalid)
                {
                    return ValidationResult.Error($"Output filetype must be .csv or .tsv. Found {outputExt}.");
                }
            }
        }

        return ValidationResult.Success();
    }

    public string GetFullInputPath()
    {
        return Path.IsPathRooted(InputFile) ? InputFile : Path.GetFullPath(InputFile);
    }

    public (FileType, string) GetInputExtension()
    {
        return GetExtension(InputFile);
    }

    public (FileType, string) GetOutputExtension()
    {
        var outputExt = Path.GetExtension(OutputFile);
        return string.IsNullOrWhiteSpace(outputExt) ? GetInputExtension() : GetExtension(OutputFile!);
    }

    private static (FileType, string) GetExtension(string filename)
    {
        var ext = Path.GetExtension(filename).ToLower();
        var filetype = ext switch
        {
            ".csv" => FileType.Csv,
            ".tsv" => FileType.Tsv,
            _ => FileType.Invalid,
        };
        return (filetype, ext);
    }

    public string GetFullOutputPath()
    {
        var outputFile = GetOutputFileName();
        var outputDir = Path.GetDirectoryName(InputFile);

        return Path.Combine(outputDir!, outputFile);
    }

    private string GetOutputFileName()
    {
        if (OutputFile != null)
        {
            var hasExtension = Path.HasExtension(OutputFile);
            return hasExtension ? OutputFile : $"{OutputFile}{GetInputExtension().Item2}";
        }

        var inputFileName = Path.GetFileName(InputFile);
        var inputExtension = Path.GetExtension(inputFileName);
        var inputFile = Path.GetFileNameWithoutExtension(inputFileName);
        return $"{inputFile}-with-countries{inputExtension}";
    }

    private static bool IsInvalidFilename(string filename)
    {
        var invalidChars = Path.GetInvalidFileNameChars();
        return filename.Any(c => invalidChars.Contains(c));
    }
}