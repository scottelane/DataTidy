using System;

namespace ScottLane.DataTidy.Core
{
    public interface ILookupCriteriaCreator
    {
        LookupCriteria CreateLookupCriteria(Type type);
    }
}
