using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace ScottLane.DataTidy.Core
{
    /// <summary>
    /// Exception that is thrown when duplicate lookup values are found.
    /// </summary>
    [Serializable]
    public class DuplicateLookupValueException : ApplicationException
    {
        /// <summary>
        /// Gets or sets the lookup that found duplicate values.
        /// </summary>
        public LookupValue Lookup { get; set; }

        /// <summary>
        /// Initialises a new instance of the DuplicateLookupValueException class with the specified message and lookup.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="lookup">The LookupValue.</param>
        public DuplicateLookupValueException(string message, LookupValue lookup) : base(message)
        {
            Lookup = lookup;
        }

        /// <summary>
        /// Gets object data associated with the exception.
        /// </summary>
        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue(nameof(Lookup), Lookup);
        }
    }
}
