namespace GetCountries;

using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

public class PostcodeInfo
{
    public string postcode { get; set; }
    public string country { get; set; }
}

public class PostcodeRequest
{
    public List<string> postcodes { get; set; }
}

public class PostcodeResponse
{
    public List<PostcodeResult> Result { get; set; }
}

public class PostcodeResult
{
    public string Query { get; set; }
    public PostcodeInfo Result { get; set; }
}

public class PostcodeService
{
    private readonly HttpClient _httpClient;
    private const string ApiUrl = "https://api.postcodes.io/postcodes";

    public PostcodeService()
    {
        _httpClient = new HttpClient();
    }

    public async Task<List<PostcodeInfo>> GetBulkPostcodes(List<string> postcodes)
    {
        var request = new PostcodeRequest { postcodes = postcodes };
        var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync(ApiUrl, content);
        response.EnsureSuccessStatusCode();

        var responseString = await response.Content.ReadAsStringAsync();
        var postcodeResponse = JsonConvert.DeserializeObject<PostcodeResponse>(responseString);

        var result = new List<PostcodeInfo>();
        foreach (var item in postcodeResponse.Result)
        {
            if (item.Result != null)
            {
                result.Add(item.Result);
            }
        }

        return result;
    }
}