using System;
using System.ComponentModel;

namespace ScottLane.DataTidy.Core
{
    public class FancyTypeConverter : ExpandableObjectConverter
    {
        public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
        {
            PropertyDescriptorCollection properties = base.GetProperties(context, value, attributes);
            //PropertyDescriptorCollection fancyProperties = new PropertyDescriptorCollection(null);

            //foreach (PropertyDescriptor property in properties)
            //{
            //    fancyProperties.Add(property);//new FancyPropertyDescriptor(property));
            //}

            //return fancyProperties;
            return properties;
            //return base.GetProperties(context, value, attributes);
        }
    }

    public class FancyPropertyDescriptor : PropertyDescriptor
    {
        private PropertyDescriptor basePropertyDescriptor;

        /// <summary>
        /// Constructor.
        /// </summary>
        public FancyPropertyDescriptor(PropertyDescriptor basePropertyDescriptor) : base(basePropertyDescriptor)
        {
            this.basePropertyDescriptor = basePropertyDescriptor;
        }

        public override bool CanResetValue(object component)
        {
            return basePropertyDescriptor.CanResetValue(component);
        }

        public override Type ComponentType
        {
            get { return basePropertyDescriptor.ComponentType; }
        }

        public override string DisplayName
        {
            get { return "yo display name"; }
        }

        public override string Description
        {
            get { return "yo description"; }
        }

        public override string Category
        {
            get { return "yo category"; }
        }

        public override object GetValue(object component)
        {
            return basePropertyDescriptor.GetValue(component);
        }

        public override bool IsReadOnly
        {
            get { return basePropertyDescriptor.IsReadOnly; }
        }

        public override bool IsBrowsable
        {
            get { return basePropertyDescriptor.IsBrowsable; }
        }

        public override string Name
        {
            get { return basePropertyDescriptor.Name; }
        }

        public override Type PropertyType
        {
            get { return basePropertyDescriptor.PropertyType; }
        }

        public override void ResetValue(object component)
        {
            basePropertyDescriptor.ResetValue(component);
        }

        public override bool ShouldSerializeValue(object component)
        {
            return basePropertyDescriptor.ShouldSerializeValue(component);
        }

        public override void SetValue(object component, object value)
        {
            basePropertyDescriptor.SetValue(component, value);
        }

    }
}
