using System.Diagnostics;
using log4net;

namespace DocumentRender.DocumentConverter
{
    class ToolRunner
    {
        public void Run(ToolProperties tool, string filename, string outputDir)
        {
            RunTool(tool, filename, outputDir, null);
        }

        public void Run(ToolProperties tool, string filename, string outputDir, int page)
        {
            RunTool(tool, filename, outputDir, page);
        }

        private static void RunTool(ToolProperties tool, string filename, string outputDir, int? page)
        {
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = tool.ToolName,
                    Arguments = tool.GetArguments(filename, outputDir, page),
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                }
            };
            process.Start();
            process.StandardOutput.ReadToEnd();
            process.WaitForExit();
        }
    }
}