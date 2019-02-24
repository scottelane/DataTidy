using System;

namespace ScottLane.SurgeonV2.Model
{
    [Serializable]
    public class Field : IComparable<Field>
    {
        public string DisplayName { get; set; }

        public Type DataType { get; set; }

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
