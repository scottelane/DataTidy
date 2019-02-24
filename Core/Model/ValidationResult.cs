using System.Collections.Generic;

namespace ScottLane.DataTidy.Core
{
    /// <summary>
    /// Provides overall validation results.
    /// </summary>
    public class ValidationResult
    {
        /// <summary>
        /// Indicates whether the validation was successful.
        /// </summary>
        public bool IsValid
        {
            get { return Errors.Count == 0; }
        }

        /// <summary>
        /// Gets or sets a list of associated ValidationError objects.
        /// </summary>
        public List<ValidationError> Errors { get; set; }

        /// <summary>
        /// Initialises a new instance of the ValidationResult class.
        /// </summary>
        public ValidationResult()
        {
            Errors = new List<ValidationError>();
        }

        /// <summary>
        /// Adds an error to the error list if the specified condition is met.
        /// </summary>
        /// <param name="condition">The condition.</param>
        /// <param name="errorMessage">The error message.</param>
        public void AddErrorIf(bool condition, string errorMessage)
        {
            if (condition)
            {
                Errors.Add(new ValidationError(errorMessage));
            }
        }

        /// <summary>
        /// Adds an error to the error list if the specified condition is met.
        /// </summary>
        /// <param name="condition">The condition.</param>
        /// <param name="errorMessage">The error message.</param>
        /// <param name="propertyName">The property name.</param>
        public void AddErrorIf(bool condition, string errorMessage, string propertyName)
        {
            if (condition)
            {
                Errors.Add(new ValidationError(errorMessage, propertyName));
            }
        }
    }
}
