
namespace VirtualPortBus.Interface
{
    public interface ICmdRunner
    {
        /// <summary>
        /// Run a command on the cmd line and get the standard out
        /// </summary>
        /// <param name="workingDir">The working directory to run the command in</param>
        /// <param name="command">The command to run</param>
        /// <param name="args">The args to supply to the command</param>
        /// <returns>Lines of the Standard Out</returns>
        string[] RunCommandGetStdOut(string workingDir, string command, string args);
    }
}
