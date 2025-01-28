using Microsoft.VisualBasic.FileIO;

namespace GetCountries
{
    internal class Program
    {
        static async Task<int> Main(string[] args)
        {
            if (args.Length != 1)
            {
                Console.WriteLine("Enter a filepath to a csv!");
                return 1;
            }

            var worker = new Worker(args[0]);
            var res = await worker.Run();
            return res;
        }
    }
}