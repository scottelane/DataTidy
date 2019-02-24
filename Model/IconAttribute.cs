using System;

namespace ScottLane.SurgeonV2.Model
{
    public class IconAttribute : Attribute
    {
        public IconType IconType { get; }

        public IconAttribute(IconType iconType)
        {
            IconType = iconType;
        }
    }
}
