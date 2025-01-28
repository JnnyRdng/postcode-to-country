using CsvHelper.Configuration.Attributes;

namespace GetCountries;

public class VetPractice
{
    [Name("postcode")]
    public string Postcode { get; set; } = string.Empty;
    
    [Name("country")]
    public string Country { get; set; } = string.Empty;
}