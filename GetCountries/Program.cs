using Microsoft.VisualBasic.FileIO;

namespace GetCountries
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            if (args.Length != 1)
            {
                throw new ArgumentException("Enter a filepath to a csv file!");
            }

            var worker = new Worker(args[0]);
            await worker.Run();
        }
    }
}