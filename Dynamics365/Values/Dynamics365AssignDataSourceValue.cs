using System.Collections.Generic;
using System.ComponentModel;
using ScottLane.DataTidy.Core;

namespace ScottLane.DataTidy.Dynamics365
{
    /// <summary>
    /// Gets a value from a Dynamics 365 data source.
    /// </summary>
    public class Dynamics365AssignDataSourceValue : Dynamics365DataSourceValue, IDynamics365AssignDataDestinationFieldsProvider
    {
        [TypeConverter(typeof(Dynamics365AssignFieldConverter))]
        public override Field DestinationField
        {
            get { return base.DestinationField; }
            set { base.DestinationField = value; }
        }

        /// <summary>
        /// Initialises a new instance of the Dynamics365AssignDataSourceValue class with the specified parent operation.
        /// </summary>
        /// <param name="parentOperation">The parent operation.</param>
        public Dynamics365AssignDataSourceValue(IOperation parentOperation) : base(parentOperation)
        { }

        public List<Field> GetDynamics365AssignDataDestinationFields()
        {
            return ((IDynamics365AssignDataDestinationFieldsProvider)Parent).GetDynamics365AssignDataDestinationFields();
        }
    }
}
