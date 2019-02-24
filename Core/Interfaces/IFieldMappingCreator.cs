using System;

namespace ScottLane.DataTidy.Core
{
    public interface IFieldMappingCreator
    {
        FieldMapping CreateFieldMapping(Type type);
    }
}
