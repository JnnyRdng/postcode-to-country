using System.Diagnostics.CodeAnalysis;
using Spectre.Console.Cli;

namespace GetCountries;

public class GetCountriesCommand : AsyncCommand<GetCountriesOptions>
{
    public override async Task<int> ExecuteAsync(CommandContext context, GetCountriesOptions settings)
    {
        var worker = new Worker(settings);
        return await worker.Run();
    }
}