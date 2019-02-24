using System;

namespace ScottLane.DataTidy.Client.Model
{
    /// <summary>
    /// Encapsulates command line arguments that can be passed to the application.
    /// </summary>
    public class CommandLineArguments
    {
        public bool OpenProject { get; private set; } = false;
        public string ProjectPath { get; private set; } = string.Empty;
        public bool Execute { get; private set; } = false;
        public Guid ExecuteItemID { get; private set; }
        public bool QuitAfterExecution { get; private set; } = false;

        private CommandLineArguments()
        { }

        public static CommandLineArguments Parse(string[] args)
        {
            CommandLineArguments arguments = new CommandLineArguments();

            for (int argumentIndex = 0; argumentIndex < args.Length; argumentIndex++)
            {
                string argument = args[argumentIndex];

                if (argument.ToLower() == "-p" || argument.ToLower() == "-project")
                {
                    arguments.OpenProject = true;

                    if (argumentIndex + 1 < args.Length)
                    {
                        arguments.ProjectPath = args[++argumentIndex];
                    }
                }
                else if (argument.ToLower() == "-e" || argument.ToLower() == "-execute")
                {
                    arguments.Execute = true;
                }
                else if (argument.ToLower() == "-q" || argument.ToLower() == "-quit")
                {
                    arguments.QuitAfterExecution = true;
                }
            }

            return arguments;
        }     
    }
}
