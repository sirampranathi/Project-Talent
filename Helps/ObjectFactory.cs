using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using MVPStudio.Framework.Helps.Excel;

namespace MVPStudio.Framework.Helps
{
    public class ObjectFactory
    {
        private ObjectFactory() { }

        /// <summary>
        ///  This method is a basic generic method which will create TYPED instance
        ///  with passed in ExcelData.<br/>
        ///  It is important that the excel spreadsheet column names match
        ///  the properties of the object that is being instantiated.<br/>
        ///  It will skip any properties which do not match the column name.<br/>
        ///  Currently, this is a simple implementation and will not work
        ///  if property is either a collection or tuple type.<br/>
        ///  It will only be able to handle value types or types that implement <i>IConvertible</i>
        /// </summary>
        /// <returns>T</returns>
        public static T CreateInstance<T>(ExcelData data) where T : class, new()
        {
            return (T)CreateInstance(typeof(T), data);
        }

        public static object CreateInstance(Type type, ExcelData data)
        {
            var instance = Activator.CreateInstance(type);

            foreach (var propertyInfo in GetProperties())
            {
                Type propertyType = Nullable.GetUnderlyingType(propertyInfo.PropertyType) ?? propertyInfo.PropertyType;
                var convertedValue = ConvertToPropertyValue(propertyType, GetColumnValue(propertyInfo.Name)) ?? default;
                propertyInfo.SetValue(instance, convertedValue, null);
            }
            return instance;

            IEnumerable<PropertyInfo> GetProperties()
            {
                foreach (var property in type.GetProperties().Where(p => p.CanWrite))
                {
                    yield return property;
                }
            }

            string GetColumnValue(string columnName)
            {
                return data.GetValue(columnName);
            }

            object ConvertToPropertyValue(Type propertyType, string value)
            {
                if (value == null)
                {
                    return default;
                }

                if (propertyType.IsEnum)
                {
                    return GetValueFromDescription<IConvertible>(propertyType, value);
                }
                else if (!propertyType.IsValueType && propertyType != typeof(string))
                {
                    // perform recursion
                    return CreateInstance(propertyType, data);
                } 
                else if (propertyType.IsGenericType
                         && (typeof(IEnumerable).IsAssignableFrom(propertyType) || typeof(ITuple).IsAssignableFrom(propertyType)))
                {
                    // it cannot handle collection types or tuples
                    throw new ArgumentException($"{nameof(ObjectFactory)} cannot handle property type of {propertyType}");
                }

                return Convert.ChangeType(value, propertyType);
            }
        }

        private static T GetValueFromDescription<T>(Type enumType, string description)
        {
            if (!enumType.IsEnum)
            {
                throw new InvalidOperationException();
            }
            foreach (var field in enumType.GetFields())
            {
                if (Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) is DescriptionAttribute attribute)
                {
                    if (attribute.Description == description)
                    {
                        return (T)field.GetValue(null);
                    }
                }
                else
                {
                    if (field.Name == description)
                    {
                        return (T)field.GetValue(null);
                    }
                }
            }
            throw new ArgumentException($"Unable to derive Enum using {nameof(description)} '{description}' for type '{enumType.Name}'");
        }
    }
}
