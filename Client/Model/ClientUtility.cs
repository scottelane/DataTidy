using ScottLane.DataTidy.Client.Controls;
using System;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace ScottLane.DataTidy.Client.Model
{
    /// <summary>
    /// Provides client helper methods.
    /// </summary>
    public static class ClientUtility
    {
        public static BindableTreeNode GetContextMenuTreeviewNode(BindableTreeView treeView, object sender)
        {
            BindableTreeNode node = default(BindableTreeNode);

            if (sender is ContextMenuStrip)
            {
                ContextMenuStrip contextMenuStrip = (ContextMenuStrip)sender;
                TreeViewHitTestInfo testInfo = treeView.HitTest(treeView.PointToClient(new Point(contextMenuStrip.Left, contextMenuStrip.Top)));

                if (testInfo.Node != default(TreeNode))
                {
                    node = (BindableTreeNode)testInfo.Node;
                }
            }
            else if (sender is ToolStripMenuItem)
            {
                ToolStripMenuItem menuItem = (ToolStripMenuItem)sender;
                node = GetContextMenuTreeviewNode(treeView, menuItem.Owner);
            }
            else if (sender is ToolStripDropDownMenu)
            {
                ToolStripDropDownMenu menu = (ToolStripDropDownMenu)sender;
                node = GetContextMenuTreeviewNode(treeView, menu.OwnerItem);
            }

            return node;
        }

        /// <summary>
        /// Gets a string representation of a timespan.
        /// </summary>
        /// <param name="duration">The timespan.</param>
        /// <returns>The string representation.</returns>
        public static string GetDurationString(TimeSpan duration)
        {
            string durationString = string.Empty;

            if (duration.TotalHours >= 1)
            {
                int hours = Convert.ToInt32(Math.Round(duration.TotalHours, 0));
                string hoursPlural = hours == 1 ? string.Empty : "s";
                int minutes = Convert.ToInt32(Math.Round(duration.TotalMinutes % 60, 0));
                string minutesPlural = minutes == 1 ? string.Empty : "s";

                durationString = string.Format("{0} hour{1}, {2} minute{3}", hours, hoursPlural, minutes, minutesPlural);
            }
            else if (duration.TotalMinutes >= 1)
            {
                int minutes = Convert.ToInt32(Math.Round(duration.TotalMinutes, 0));
                string minutesPlural = minutes == 1 ? string.Empty : "s";
                int seconds = Convert.ToInt32(Math.Round(duration.TotalSeconds % 60, 0));
                string secondsPlural = seconds == 1 ? string.Empty : "s";

                durationString = string.Format("{0} minute{1}, {2} second{3}", minutes, minutesPlural, seconds, secondsPlural);
            }
            else
            {
                int seconds = Convert.ToInt32(Math.Round(duration.TotalSeconds, 0));
                string secondsPlural = seconds == 1 ? string.Empty : "s";

                durationString = string.Format("{0} second{1}", seconds, secondsPlural);
            }

            return durationString;
        }

        /// <summary>
        /// Shortens a file path to the specified length
        /// </summary>
        /// <param name="path">The file path to shorten</param>
        /// <param name="maxLength">The max length of the output path (including the ellipsis if inserted)</param>
        /// <returns>The path with some of the middle directory paths replaced with an ellipsis (or the entire path if it is already shorter than maxLength)</returns>
        /// <remarks>Credit: https://stackoverflow.com/questions/1764204/how-to-display-abbreviated-path-names-in-net</remarks>
        public static string ShortenPath(string path, int maxLength)
        {
            string ellipsisChars = "...";
            char dirSeperatorChar = Path.DirectorySeparatorChar;
            string directorySeperator = dirSeperatorChar.ToString();

            //simple guards
            if (path.Length <= maxLength)
            {
                return path;
            }
            int ellipsisLength = ellipsisChars.Length;
            if (maxLength <= ellipsisLength)
            {
                return ellipsisChars;
            }

            //alternate between taking a section from the start (firstPart) or the path and the end (lastPart)
            bool isFirstPartsTurn = true; //drive letter has first priority, so start with that and see what else there is room for

            //vars for accumulating the first and last parts of the final shortened path
            string firstPart = "";
            string lastPart = "";
            //keeping track of how many first/last parts have already been added to the shortened path
            int firstPartsUsed = 0;
            int lastPartsUsed = 0;

            string[] pathParts = path.Split(dirSeperatorChar);
            for (int i = 0; i < pathParts.Length; i++)
            {
                if (isFirstPartsTurn)
                {
                    string partToAdd = pathParts[firstPartsUsed] + directorySeperator;
                    if ((firstPart.Length + lastPart.Length + partToAdd.Length + ellipsisLength) > maxLength)
                    {
                        break;
                    }
                    firstPart = firstPart + partToAdd;
                    if (partToAdd == directorySeperator)
                    {
                        //this is most likely the first part of and UNC or relative path 
                        //do not switch to lastpart, as these are not "true" directory seperators
                        //otherwise "\\myserver\theshare\outproject\www_project\file.txt" becomes "\\...\www_project\file.txt" instead of the intended "\\myserver\...\file.txt")
                    }
                    else
                    {
                        isFirstPartsTurn = false;
                    }
                    firstPartsUsed++;
                }
                else
                {
                    int index = pathParts.Length - lastPartsUsed - 1; //-1 because of length vs. zero-based indexing
                    string partToAdd = directorySeperator + pathParts[index];
                    if ((firstPart.Length + lastPart.Length + partToAdd.Length + ellipsisLength) > maxLength)
                    {
                        break;
                    }
                    lastPart = partToAdd + lastPart;
                    if (partToAdd == directorySeperator)
                    {
                        //this is most likely the last part of a relative path (e.g. "\websites\myproject\www_myproj\App_Data\")
                        //do not proceed to processing firstPart yet
                    }
                    else
                    {
                        isFirstPartsTurn = true;
                    }
                    lastPartsUsed++;
                }
            }

            if (lastPart == "")
            {
                //the filename (and root path) in itself was longer than maxLength, shorten it
                lastPart = pathParts[pathParts.Length - 1];//"pathParts[pathParts.Length -1]" is the equivalent of "Path.GetFileName(pathToShorten)"
                lastPart = lastPart.Substring(lastPart.Length + ellipsisLength + firstPart.Length - maxLength, maxLength - ellipsisLength - firstPart.Length);
            }

            return firstPart + ellipsisChars + lastPart;
        }

        public static string LogFolder
        {
            get { return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Scott_Lane", Application.ProductName); }
        }

        public static string ErrorLogPath
        {
            get { return Path.Combine(LogFolder, "Errors.txt"); }
        }

        public static string NotificationLogPath
        {
            get { return Path.Combine(LogFolder, "Notifications.txt"); }
        }

        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, Int32 wMsg, bool wParam, Int32 lParam);

        private const int WM_SETREDRAW = 11;

        /// <summary>
        /// Some controls, such as the DataGridView, do not allow setting the DoubleBuffered property.
        /// It is set as a protected property. This method is a work-around to allow setting it.
        /// Call this in the constructor just after InitializeComponent().
        /// </summary>
        /// <param name="control">The Control on which to set DoubleBuffered to true.</param>
        public static void SetDoubleBuffered(Control control)
        {
            // if not remote desktop session then enable double-buffering optimization
            if (!System.Windows.Forms.SystemInformation.TerminalServerSession)
            {

                // set instance non-public property with name "DoubleBuffered" to true
                typeof(Control).InvokeMember("DoubleBuffered",
                                             System.Reflection.BindingFlags.SetProperty |
                                                System.Reflection.BindingFlags.Instance |
                                                System.Reflection.BindingFlags.NonPublic,
                                             null,
                                             control,
                                             new object[] { true });
            }
        }

        /// <summary>
        /// Suspend drawing updates for the specified control. After the control has been updated
        /// call DrawingControl.ResumeDrawing(Control control).
        /// </summary>
        /// <param name="control">The control to suspend draw updates on.</param>
        public static void SuspendDrawing(Control control)
        {
            SendMessage(control.Handle, WM_SETREDRAW, false, 0);
        }

        /// <summary>
        /// Resume drawing updates for the specified control.
        /// </summary>
        /// <param name="control">The control to resume draw updates on.</param>
        public static void ResumeDrawing(Control control)
        {
            SendMessage(control.Handle, WM_SETREDRAW, true, 0);
            control.Refresh();
        }

    }
}