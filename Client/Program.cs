using System;
using System.IO;
using System.Windows.Forms;
using ScottLane.DataTidy.Client.Forms;
using ScottLane.DataTidy.Client.Model;

namespace ScottLane.DataTidy.Client
{
    static class Program
    {
        static MainForm mainForm;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static int Main(string[] args)
        {
            try
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);

                if (!Directory.Exists(ClientUtility.LogFolder))
                {
                    Directory.CreateDirectory(ClientUtility.LogFolder);
                }

                if (Properties.Settings.Default.UpgradeSettings)
                {
                    // prevent settings from being reset when upgrading the application
                    Properties.Settings.Default.Upgrade();
                    Properties.Settings.Default.UpgradeSettings = false;
                    Properties.Settings.Default.Save();
                }

                CommandLineArguments arguments = CommandLineArguments.Parse(args);

                if (Properties.Settings.Default.IsFirstRun)
                {
                    Properties.Settings.Default.IsFirstRun = false;
                    Properties.Settings.Default.Save();
                    mainForm = new MainForm(true);
                }
                else if (arguments.OpenProject)
                {
                    mainForm = new MainForm(arguments.ProjectPath);
                }
                else
                {
                    mainForm = new MainForm();
                }

                if (arguments.Execute)
                {
                    if (arguments.QuitAfterExecution)
                    {
                        ApplicationState.Default.AsyncProcessStopped += ApplicationState_AsyncProcessStopped;
                    }

                    if (arguments.ExecuteItemID != default(Guid))
                    {
                        ApplicationState.Default.RequestExecution(ApplicationState.Default.ActiveProject.FindExecutableItem(arguments.ExecuteItemID));
                    }
                    else
                    {
                        ApplicationState.Default.RequestExecution(ApplicationState.Default.ActiveProject);
                    }
                }

                ApplicationState.Default.NotificationRaised += Default_NotificationRaised;
                Application.Run(mainForm);
            }
            catch (Exception ex)
            {
                LogError(DateTime.Now, ex.Message, ex.StackTrace);
            }

            return Environment.ExitCode;
        }

        private static void Default_NotificationRaised(object sender, NotificationEventArgs e)
        {
            try
            {
                using (StreamWriter writer = System.IO.File.AppendText(ClientUtility.NotificationLogPath))
                {
                    writer.WriteLine("{0} {1} {2}", e.TimeStamp, e.NotificationType.ToString(), e.Message ?? string.Empty);
                }
            }
            catch { }

            if (e.NotificationType == Core.NotificationType.Error)
            {
                LogError(e.TimeStamp, e.Message, e?.Exception?.StackTrace);
            }
        }

        private static void LogError(DateTime timeStamp, string message, string stackTrace)
        {
            try
            {
                using (StreamWriter writer = System.IO.File.AppendText(ClientUtility.ErrorLogPath))
                {
                    writer.WriteLine("{0} {1} {2}", timeStamp, message ?? string.Empty, stackTrace ?? string.Empty);
                }
            }
            catch { }
        }

        private static void ApplicationState_AsyncProcessStopped(object sender, AsyncStoppedEventArgs e)
        {
            if (e.CompletedSuccessfully)
            {
                mainForm.Close();
            }
            else
            {
                ApplicationState.Default.AsyncProcessStopped -= ApplicationState_AsyncProcessStopped;
            }
        }
    }
}
