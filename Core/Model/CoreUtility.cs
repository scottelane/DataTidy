using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;

namespace ScottLane.DataTidy.Core
{
    public class CoreUtility
    {
        public static List<Type> GetInterfaceImplementorsWithAttribute(Type interfaceType, Type attributeType)
        {
            List<Type> interfaceImplementors = new List<Type>();

            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                try
                {
                    foreach (Type type in assembly.GetTypes())
                    {
                        if (type.IsClass && !type.IsAbstract && interfaceType.IsAssignableFrom(type) && Attribute.GetCustomAttribute(type, attributeType) != default(Attribute))
                        {
                            interfaceImplementors.Add(type);
                        }
                    }
                }
                catch { }
            }

            interfaceImplementors.Sort(delegate (Type type1, Type type2) { return type1.Name.CompareTo(type2.Name); });

            return interfaceImplementors;
        }

        public static void SetBrowsable(object item, string propertyName, bool visible)
        {
            PropertyDescriptor descriptor = TypeDescriptor.GetProperties(item.GetType())[propertyName];
            BrowsableAttribute attribute = (BrowsableAttribute)descriptor.Attributes[typeof(BrowsableAttribute)];
            FieldInfo browsable = attribute.GetType().GetField("browsable", BindingFlags.NonPublic | BindingFlags.Instance);
            browsable.SetValue(attribute, visible);
        }

        public static readonly string FieldMatchPattern = @"\(((?:[^()]|(?<open>\()|(?<-open>\)))+)\)$";
    }
}
