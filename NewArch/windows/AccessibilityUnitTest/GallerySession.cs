using System;
using Axe.Windows.Automation;
using Axe.Windows.Automation.Data;
using Newtonsoft.Json;

namespace AccessibilityUnitTest
{
    public class GallerySession
    {
        private const string ApplicationProcessName = "rngallery";

        public static int GetProcessId()
        {
            //Code to get the Process ID from the process name
            var processes = System.Diagnostics.Process.GetProcessesByName(ApplicationProcessName);
            return processes[0].Id;
        }

        public static IScanner CreateScanner(int processId)
        {
            Console.WriteLine("Creating scanner...");
            var configBuilder = Config.Builder.ForProcessId(processId)
                .WithOutputFileFormat(OutputFileFormat.None);
            var config = configBuilder.Build();
            var scanner = ScannerFactory.CreateScanner(config);
            return scanner;
        }

        public static ScanOutput GetScanResults(IScanner scanner)
        {
            Console.WriteLine("Scanning...");
            var scanResults = scanner.Scan(null);
            return scanResults;
        }

        public static void PrintScanResults(ScanOutput scanResults, bool fullContent)
        {
            Console.WriteLine("Scan results:");
            Console.WriteLine();
            Console.WriteLine("===========================================");
            if (fullContent)
            {
                Console.WriteLine(JsonConvert.SerializeObject(scanResults, Formatting.Indented));
            }
            else
            {
                foreach (var scanResult in scanResults.WindowScanOutputs)
                {
                    foreach (var error in scanResult.Errors)
                    {
                        Console.WriteLine(error.Rule.Description);
                    }
                }
            }
            Console.WriteLine("===========================================");
            Console.WriteLine();
        }
        public static bool IsErrorPresent(ScanOutput scanResults)
        {
            bool isError = false;
            foreach (var scanResult in scanResults.WindowScanOutputs)
            {
                foreach (var error in scanResult.Errors)
                {
                    Console.WriteLine($"- {error.Rule.Description}");
                    isError = true;
                }
            }
            return isError;
        }
    }
}
