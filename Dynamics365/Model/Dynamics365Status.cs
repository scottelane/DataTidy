namespace ScottLane.DataTidy.Dynamics365
{
    /// <summary>
    /// Models a Dynamics 365 entity status.
    /// </summary>
    public class Dynamics365Status
    {
        /// <summary>
        /// Gets or sets the status name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the status code.
        /// </summary>
        public int Code { get; set; }

        /// <summary>
        /// Gets or sets the logical name of the status field.
        /// </summary>
        public string LogicalName { get; set; }

        /// <summary>
        /// Overrides the ToString method.
        /// </summary>
        /// <returns>The name.</returns>
        public override string ToString()
        {
            return Name.ToString();
        }
    }
}
