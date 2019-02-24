using System;

namespace ScottLane.DataTidy.Core
{
    public interface IFieldValueCreator
    {
        FieldValue CreateFieldValue(Type type);
    }
}
