using Spectre.Console.Cli;

namespace GetCountries
{
    public static class Program
    {
        public static int Main(string[] args)
        {
            var app = new CommandApp<GetCountriesCommand>()
                .WithDescription("Adds a new column to csv/tsv files with a country found from a postcode column.");
            app.Configure(config =>
            {
                config.SetApplicationName("GetCountries");
#if DEBUG
                config.PropagateExceptions();
#endif
            });

            return app.Run(args);
        }
    }
}