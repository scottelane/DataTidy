using System;

namespace ScottLane.DataTidy.Core
{
    /// <summary>
    /// A Field.
    /// </summary>
    public class Field : IComparable<Field>
    {
        /// <summary>
        /// Gets or sets the Field display name.
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// Compares two Field objects based on the display name.
        /// </summary>
        /// <param name="other">The other object.</param>
        /// <returns>The comparison result.</returns>
        public int CompareTo(Field other)
        {
            if (other == null)
            {
                return 1;
            }
            else
            {
                return DisplayName.CompareTo(other.DisplayName);
            }
        }
    }
}
