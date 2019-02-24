using System.Collections.Generic;

namespace ScottLane.DataTidy.Core
{
    /// <summary>
    /// Base class for any destination table or repository that can be updated by an operation and exposes the fields that can be updated in that destination.
    /// </summary>
    public abstract class DataDestination
    {
        /// <summary>
        /// Gets or sets the data destination name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Initialises a new instance of the DataDestination class.
        /// </summary>
        public DataDestination()
        { }

        /// <summary>
        /// Gets a list of fields that can be updated in the destination.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <returns>The field list.</returns>
        public abstract List<Field> GetFields(IConnection connection);
    }
}
