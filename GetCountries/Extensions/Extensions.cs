namespace GetCountries.Extensions;

public static class Extensions
{
    public static IEnumerable<string> NotEmpty(this IEnumerable<string> source)
    {
        return source.Where(x => !string.IsNullOrWhiteSpace(x));
    }
    
    public static string ToDelimiter(this FileType fileType)
    {
        return fileType switch
        {
            FileType.Csv => ",",
            FileType.Tsv => "\t",
            _ => throw new ArgumentOutOfRangeException(nameof(fileType), fileType, null),
        };
    }
}