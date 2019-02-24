using System.Collections.Generic;
using System.ComponentModel;
using ScottLane.DataTidy.Core;

namespace ScottLane.DataTidy.Dynamics365
{
    public class Dynamics365SetStateUserProvidedValue : Dynamics365UserProvidedValue, IDynamics365SetStateDataDestinationFieldsProvider
    {
        [TypeConverter(typeof(Dynamics365SetStateFieldConverter))]
        public override Field DestinationField
        {
            get { return base.DestinationField; }
            set { base.DestinationField = value; }
        }

        /// <summary>
        /// Initialises a new instance of the Dynamics365SetStateUserProvidedValue class with the specified parent operation.
        /// </summary>
        /// <param name="parentOperation">The parent operation.</param>
        public Dynamics365SetStateUserProvidedValue(IOperation parentOperation) : base(parentOperation)
        { }

        public List<Field> GetDynamics365SetStateDataDestinationFields()
        {
            return ((IDynamics365SetStateDataDestinationFieldsProvider)Parent).GetDynamics365SetStateDataDestinationFields();
        }
    }
}
