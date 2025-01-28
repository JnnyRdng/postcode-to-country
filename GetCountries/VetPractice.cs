using CsvHelper.Configuration.Attributes;

namespace GetCountries;

public class VetPractice
{
    [Name("postcode")]
    public string Postcode { get; set; } = string.Empty;
    
    [Name("country")]
    public string Country { get; set; } = string.Empty;
    
    // [Name("org_id")]
    // public long OrgId { get; set; }
    //
    // [Name("contact_email")]
    // public string ContactEmail { get; set; } = string.Empty;
    //
    // [Name("contact_name")]
    // public string ContactName { get; set; } = string.Empty;
    //
    // [Name("name")]
    // public string Name { get; set; } = string.Empty;
    //
    // [Name("county")]
    // public string County { get; set; } = string.Empty;
    //
    // [Name("area")]
    // public string Area { get; set; } = string.Empty;
    //
    // [Name("town")]
    // public string Town { get; set; } = string.Empty;
    //
    
}