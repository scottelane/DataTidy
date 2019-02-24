namespace ScottLane.DataTidy.Core
{
    /// <summary>
    /// Captures a validation error.
    /// </summary>
    public class ValidationError
    {
        /// <summary>
        /// Gets or sets the property that has the validation error.
        /// </summary>
        public string PropertyName { get; set; }

        /// <summary>
        /// Gets or sets the validation error message.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Initialises a new instance of the ValidationError class.
        /// </summary>
        public ValidationError()
        { }

        /// <summary>
        /// Initialises a new instance of the ValidationError class with the specified error message.
        /// </summary>
        /// <param name="message">The error message.</param>
        public ValidationError(string message) : this()
        {
            Message = message;
        }

        /// <summary>
        /// Initialises a new instance of the ValidationError class with the specified property name and error message.
        /// </summary>
        /// <param name="propertyName">The property name.</param>
        /// <param name="message">The error message.</param>
        public ValidationError(string message, string propertyName) : this(message)
        {
            PropertyName = propertyName;
        }
    }
}
