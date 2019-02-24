namespace ScottLane.DataTidy.Core
{
    /// <summary>
    /// A container for utility data sources.
    /// </summary>
    [Connection(typeof(UtilityConnection), "ScottLane.DataTidy.Core.Resources.UtilityConnection.png")]
    public class UtilityConnection : Connection
    {
        /// <summary>
        /// Initialises a new instance of the UtilityConnection class with the specified parent Project.
        /// </summary>
        /// <param name="parent">The parent Project.</param>
        public UtilityConnection(Project parent) : base(parent)
        { }
    }
}
