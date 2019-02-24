using System;
using System.Collections.Generic;
using System.Data;

namespace ScottLane.DataTidy.Core
{
    /// <summary>
    /// A field that is derived from a DataTable
    /// </summary>
    public class DataTableField : Field, IComparable<DataTableField>, IEquatable<DataTableField>
    {
        /// <summary>
        /// Gets or sets the column name.
        /// </summary>
        public string ColumnName { get; set; }

        /// <summary>
        /// Gets or sets the data type.
        /// </summary>
        public Type DataType { get; set; }

        /// <summary>
        /// Gets a string representation of the instance by combining the display name and logical name.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("{0} ({1})", DisplayName, ColumnName);
        }

        public bool Equals(DataTableField other)
        {
            if (other == default(DataTableField))
            {
                return false;
            }

            return ColumnName == other.ColumnName;
        }

        /// <summary>
        /// Compares two DataSourceField objects based on the ColumnName.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public int CompareTo(DataTableField other)
        {
            if (other == default(DataTableField))
            {
                return 1;
            }

            return ColumnName.CompareTo(other.ColumnName);
        }

        public static List<DataTableField> GetDataTableFields(DataColumnCollection dataColumns)
        {
            List<DataTableField> fields = new List<DataTableField>();

            if (dataColumns != default(DataColumnCollection))
            {
                foreach (DataColumn column in dataColumns)
                {
                    fields.Add(new DataTableField()
                    {
                        ColumnName = column.ColumnName,
                        DataType = column.DataType,
                        DisplayName = column.Caption
                    });
                }
            }

            return fields;
        }

        public DataTableField Clone()
        {
            return (DataTableField)MemberwiseClone();
        }
    }
}
