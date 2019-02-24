using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace ScottLane.DataTidy.Core
{
    [Serializable]
    public class MissingLookupValueException : ApplicationException
    {
        public LookupValue LookupValue { get; set; }

        public MissingLookupValueException(string message, LookupValue lookupValue) : base(message)
        {
            LookupValue = lookupValue;
        }

        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("LookupValue", LookupValue);
        }
    }
}
