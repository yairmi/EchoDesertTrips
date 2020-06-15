using Core.Common.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Core.Common.Utils
{
    public static class PropertySupport
    {
        public static string ExtractPropertyName<T>(Expression<Func<T>> propertyExpression)
        {
            if (propertyExpression == null)
                throw new ArgumentNullException("propertyExpression");

            var memberExpression = propertyExpression.Body as MemberExpression;
            if (memberExpression == null)
                throw new ArgumentException("memberExpression");

            var property = memberExpression.Member as PropertyInfo;
            if (property == null)
                throw new ArgumentException("property");

            var getMethod = property.GetGetMethod(true);
            if (getMethod.IsStatic)
                throw new ArgumentException("static method");

            return memberExpression.Member.Name;
        }

        public static void ResetProperties<T>(T source) where T : class
        {
            List<PropertyInfo> sourceProperties = source.GetType().GetProperties().ToList<PropertyInfo>();
            foreach (PropertyInfo sourceProperty in sourceProperties)
            {
                var type = sourceProperty.PropertyType;
                var attrs = sourceProperty.GetCustomAttributes(true);
                if (attrs.Length > 0)
                {
                    foreach (var attr in attrs)
                    {
                        var defaultAttr = attr as DefaultValueAttribute;
                        if (defaultAttr != null)
                        {
                            if (type.IsValueType == true
                                || defaultAttr.Value is string)
                            {
                                if (defaultAttr != null)
                                {
                                    sourceProperty.SetValue(source, defaultAttr.Value);
                                }
                            }
                            else
                            {
                                //                            if (defaultAttr.Value.GetType().IsByRef)
                                //                            {
                                var refType = sourceProperty.PropertyType;
                                var obj = Activator.CreateInstance(refType, false);
                                sourceProperty.SetValue(source, obj);
                                //                           }
                            }
                        }
                    }
                }
                //    else if (type.BaseType == typeof(ObjectBase) ||
                //             type.BaseType.IsSubclassOf(typeof(ICollection<>)))
                //    {
                //        var refType = sourceProperty.PropertyType;
                //        var obj = Activator.CreateInstance(refType, false);
                //        sourceProperty.SetValue(source, obj);
                //    }
                //}
            }
        }
    }
}
