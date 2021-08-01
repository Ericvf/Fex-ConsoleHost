using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BossConsoleHost
{
    public class ProcessStartService
    {
        public delegate void ConsoleOutput(string output, bool isError);

        private System.Diagnostics.Process process;
        public bool isrunning = false;

        internal Task RunProcessAsync(string fileName, IEnumerable<string> arguments, ConsoleOutput consoleOutput)
        {
            var tcs = new TaskCompletionSource<bool>();

            string processArguments = string.Join(" ", arguments.ToArray());

            process = new System.Diagnostics.Process
            {
                EnableRaisingEvents = true,
                StartInfo =
                {
                    FileName = fileName,
                    Arguments = processArguments,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardInput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true
                }
            };

            process.Exited += (sender, args) =>
            {
                process.Dispose();
                tcs.TrySetResult(true);
            };

            process.OutputDataReceived += (s, e) =>
            {
                if (consoleOutput != null)
                    consoleOutput(e.Data, false);
            };

            process.ErrorDataReceived += (s, e) =>
            {
                if (consoleOutput != null)
                    consoleOutput(e.Data, true);
            };

            this.isrunning = true;
            process.Start();

            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

            return tcs.Task;
        }

        internal void Kill()
        {
            process.Kill();
            this.isrunning = true;
        }

        internal void Send(string input)
        {
            process.StandardInput.WriteLine(input);
        }
    }
}
