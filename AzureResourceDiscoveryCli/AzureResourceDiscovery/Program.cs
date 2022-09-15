using AzureResourceDiscovery.Core;
using CommandLine;

namespace AzureResourceDiscovery
{
    internal class Program
    {
        public class Options
        {
            [Option('f', "filepath", Required = true, HelpText = "File path to JSON")]
            public string? FilePath { get; set; }

            [Option('d', "destination", Required = true, HelpText = "Destination directory")]
            public string? DestinationDirectory { get; set; }
        }

        private static int _counter = 0;
        private static string? _directoryPath;

        private static void ProcessAzurePolicy(AzurePolicy azurePolicy)
        {
            string fileName = $"{_directoryPath}\\{_counter}.json";
            var content = azurePolicy.ToString();
            File.WriteAllText(fileName, content);

            _counter += 1;

            Console.WriteLine($"Created {fileName}");
        }

        static int Main(string[] args)
        {
            bool hasErrors = false;
            Parser.Default.ParseArguments<Options>(args).WithParsed(o =>
            {
                if (string.IsNullOrEmpty(o.DestinationDirectory) || !Directory.Exists(o.DestinationDirectory))
                {
                    hasErrors = true;
                    using TextWriter errorWriter = Console.Error;
                    errorWriter.WriteLine("Invalid destination directory!");
                    return;
                }

                if (!o.DestinationDirectory.EndsWith("\\"))
                {
                    o.DestinationDirectory += "\\";
                }

                var directoryPath = $"{o.DestinationDirectory}{DateTime.Now.ToString("MMddHHmmss")}";

                if (!string.IsNullOrEmpty(o.FilePath) && File.Exists(o.FilePath))
                {
                    var gen = new AzurePolicyGenerator();

                    if (!Directory.Exists(directoryPath))
                    {
                        Directory.CreateDirectory(directoryPath);
                    }

                    _directoryPath = directoryPath;

                    try
                    {
                        if (!gen.GenerateFiles(File.ReadAllText(o.FilePath), ProcessAzurePolicy))
                        {
                            hasErrors = true;
                            using TextWriter errorWriter = Console.Error;
                            errorWriter.WriteLine("Unable to generate Azure Policy(ies)!");
                        }
                    }
                    catch (Exception e)
                    {
                        hasErrors = true;
                        using TextWriter errorWriter = Console.Error;
                        errorWriter.WriteLine(e.Message);
                    }
                }
                else
                {
                    hasErrors = true;
                    using TextWriter errorWriter = Console.Error;
                    errorWriter.WriteLine("Invalid file path!");
                }
            });

            return hasErrors ? -1 : 0;
        }
    }
}