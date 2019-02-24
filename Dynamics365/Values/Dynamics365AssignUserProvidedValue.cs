using System.Collections.Generic;
using System.ComponentModel;
using ScottLane.DataTidy.Core;

namespace ScottLane.DataTidy.Dynamics365
{
    public class Dynamics365AssignUserProvidedValue : Dynamics365UserProvidedValue, IDynamics365AssignDataDestinationFieldsProvider
    {
        [TypeConverter(typeof(Dynamics365AssignFieldConverter))]
        public override Field DestinationField
        {
            get { return base.DestinationField; }
            set { base.DestinationField = value; }
        }

        /// <summary>
        /// Initialises a new instance of the Dynamics365AssignUserProvidedValue class with the specified parent operation.
        /// </summary>
        /// <param name="parentOperation">The parent operation.</param>
        public Dynamics365AssignUserProvidedValue(IOperation parentOperation) : base(parentOperation)
        { }

        public List<Field> GetDynamics365AssignDataDestinationFields()
        {
            return ((IDynamics365AssignDataDestinationFieldsProvider)Parent).GetDynamics365AssignDataDestinationFields();
        }
    }
}
