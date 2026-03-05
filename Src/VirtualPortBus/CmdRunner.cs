using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using VirtualPortBus.Interface;

namespace VirtualPortBus
{
    public class CmdRunner : ICmdRunner
    {
        /// <summary>
        /// Run a command on the cmd line and get the standard out with no shell execute and no window
        /// </summary>
        /// <param name="workingDir">The working directory to run the command in</param>
        /// <param name="command">The command to run</param>
        /// <param name="args">The args to supply to the command</param>
        /// <returns>Lines of the Standard Out</returns>
        public string[] RunCommandGetStdOut(string workingDir, string command, string args)
        {
            var proc = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    WorkingDirectory = workingDir,
                    FileName = command,
                    Arguments = args,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                }
            };

            proc.Start();

            while (!proc.HasExited)
            {
                Thread.Sleep(100);
            }

            if (proc.ExitCode != 0)
                throw new ApplicationException($"Exit code of {proc.ExitCode} received when running '{command} {args}'");

            var ret = new List<string>();
            while (!proc.StandardOutput.EndOfStream)
            {
                ret.Add(proc.StandardOutput.ReadLine());
            }

            return ret.ToArray();
        }
    }
}
