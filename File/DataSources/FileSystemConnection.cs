using System.IO;
using ScottLane.DataTidy.Core;

namespace ScottLane.DataTidy.File
{
    /// <summary>
    /// Provides connectivity to a file.
    /// </summary>
    [Connection(typeof(FileSystemConnection), "ScottLane.DataTidy.File.Resources.FileSystemConnection.png")]
    public class FileSystemConnection : Connection
    {
        /// <summary>
        /// Initializes a new instance of the FileConnection class.
        /// </summary>
        public FileSystemConnection(Project parent) : base(parent)
        { }

        public virtual FileStream GetFileStream(string path)
        {
            return new FileStream(path, FileMode.Open);
        }
    }
}
