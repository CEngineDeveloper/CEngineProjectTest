//------------------------------------------------------------------------------
// ExtensionType.cs
// Copyright 2020 2020/7/15 
// Created by CYM on 2020/7/15
// Owner: CYM
// 填写类的描述...
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;

namespace CYM
{
    public static class ExtensionType
    {
        public static bool IsDerivedFromOpenGenericType(
                this Type type, Type openGenericType
            )
        {
            Contract.Requires(type != null);
            Contract.Requires(openGenericType != null);
            Contract.Requires(openGenericType.IsGenericTypeDefinition);
            return type.GetTypeHierarchy()
                       .Where(t => t.IsGenericType)
                       .Select(t => t.GetGenericTypeDefinition())
                       .Any(t => openGenericType.Equals(t));
        }

        public static IEnumerable<Type> GetTypeHierarchy(this Type type)
        {
            Contract.Requires(type != null);
            Type currentType = type;
            while (currentType != null)
            {
                yield return currentType;
                currentType = currentType.BaseType;
            }
        }

        public static IEnumerable<string> Keys(this Type type, BindingFlags? propBindingAttr = null, BindingFlags? fieldBindingAttr = null)
        {
            List<string> result = new List<string>();
            result.AddRange(PropertyKeys(type, propBindingAttr));
            result.AddRange(FieldKeys(type, fieldBindingAttr));
            return result;
        }

        public static IEnumerable<string> PropertyKeys(this Type type, BindingFlags? bindingAttr = null)
        {
            PropertyInfo[] props = bindingAttr.HasValue ? type.GetProperties(bindingAttr.Value) : type.GetProperties();
            return props.Select(x => x.Name);
        }

        public static IEnumerable<string> FieldKeys(this Type type, BindingFlags? bindingAttr = null)
        {
            FieldInfo[] fields = bindingAttr.HasValue ? type.GetFields(bindingAttr.Value) : type.GetFields();
            return fields.Select(x => x.Name);
        }

        public static IDictionary<string, object> KeyValueList(this Type type, object obj, BindingFlags? propBindingAttr = null, BindingFlags? fieldBindingAttr = null)
        {
            Dictionary<string, object> result = new Dictionary<string, object>();
            PropertyInfo[] props = propBindingAttr.HasValue ? type.GetProperties(propBindingAttr.Value) : type.GetProperties();
            Array.ForEach(props, x => result.Add(x.Name, x.GetValue(obj)));
            FieldInfo[] fields = fieldBindingAttr.HasValue ? type.GetFields(fieldBindingAttr.Value) : type.GetFields();
            Array.ForEach(fields, x => result.Add(x.Name, x.GetValue(obj)));
            return result;
        }

        public static IDictionary<string, object> PropertyKeyValueList(this Type type, object obj, BindingFlags? bindingAttr = null)
        {
            Dictionary<string, object> result = new Dictionary<string, object>();
            PropertyInfo[] props = bindingAttr.HasValue ? type.GetProperties(bindingAttr.Value) : type.GetProperties();
            Array.ForEach(props, x => result.Add(x.Name, x.GetValue(obj)));
            return result;
        }

        public static IDictionary<string, object> FieldKeyValueList(this Type type, object obj, BindingFlags? bindingAttr = null)
        {
            Dictionary<string, object> result = new Dictionary<string, object>();
            FieldInfo[] fields = bindingAttr.HasValue ? type.GetFields(bindingAttr.Value) : type.GetFields();
            Array.ForEach(fields, x => result.Add(x.Name, x.GetValue(obj)));
            return result;
        }

        public static bool HasKey(this Type type, string key, BindingFlags? propBindingAttr = null, BindingFlags? fieldBindingAttr = null)
        {
            return type.Keys(propBindingAttr, fieldBindingAttr).Contains(key);
        }

        public static object GetValue(this Type type, string key, object obj, BindingFlags? propBindingAttr = null, BindingFlags? fieldBindingAttr = null)
        {
            IDictionary<string, object> propertyKeyValueList = PropertyKeyValueList(type, obj, propBindingAttr);
            if (propertyKeyValueList.ContainsKey(key))
            {
                return propertyKeyValueList[key];
            }
            IDictionary<string, object> fieldKeyValueList = FieldKeyValueList(type, obj, fieldBindingAttr);
            if (fieldKeyValueList.ContainsKey(key))
            {
                return fieldKeyValueList[key];
            }
            return null;
        }

        public static List<FieldInfo> GetFieldsUpUntilBaseClass<BaseClass>(
            this Type type, bool includeBaseClass = true)
        {
            List<FieldInfo> fields = new List<FieldInfo>();
            while (typeof(BaseClass).IsAssignableFrom(type))
            {
                if (type == typeof(BaseClass) && !includeBaseClass)
                    break;

                fields.AddRange(type.GetFields(
                    BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly));

                type = type.BaseType;

                if (type == typeof(BaseClass) && includeBaseClass)
                    break;
            }
            return fields;
        }

        public static List<FieldInfo> GetFieldsUpUntilBaseClass<BaseClass, FieldType>(
            this Type type, bool includeBaseClass = true)
        {
            List<FieldInfo> fields = GetFieldsUpUntilBaseClass<BaseClass>(type, includeBaseClass);

            for (int i = fields.Count - 1; i >= 0; i--)
            {
                if (!typeof(FieldType).IsAssignableFrom(fields[i].FieldType))
                    fields.RemoveAt(i);
            }
            return fields;
        }

        private static FieldInfo GetDeclaringFieldUpUntilBaseClass<BaseClass, FieldType>(
            this Type type, object instance, FieldType value, bool includeBaseClass = true)
        {
            List<FieldInfo> fields = GetFieldsUpUntilBaseClass<BaseClass, FieldType>(
                type, includeBaseClass);

            FieldType fieldValue;
            for (int i = 0; i < fields.Count; i++)
            {
                fieldValue = (FieldType)fields[i].GetValue(instance);
                if (Equals(fieldValue, value))
                    return fields[i];
            }

            return null;
        }

        public static string GetNameOfDeclaringField<BaseClass, FieldType>(
            this Type type, object instance, FieldType value, bool capitalize = false)
        {
            FieldInfo declaringField = type
                .GetDeclaringFieldUpUntilBaseClass<BaseClass, FieldType>(instance, value);

            if (declaringField == null)
                return null;

            return GetFieldName(type, declaringField, capitalize);
        }

        private static string GetFieldName(this Type type, FieldInfo fieldInfo, bool capitalize = false)
        {
            string name = fieldInfo.Name;

            if (!capitalize)
                return name;

            if (name.Length <= 1)
                return name.ToUpper();

            return char.ToUpper(name[0]) + name.Substring(1);
        }

        public static Type[] GetAllAssignableClasses(
            this Type type, bool includeAbstract = true, bool includeItself = false)
        {
            return AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .Where(t => type.IsAssignableFrom(t) && (t != type || includeItself) && (includeAbstract || !t.IsAbstract))
                .ToArray();
        }
        public static T GetAttribute<T>(this MemberInfo memberInfo, bool inherit = true)
            where T : Attribute
        {
            object[] attributes = memberInfo.GetCustomAttributes(inherit);
            return attributes.OfType<T>().FirstOrDefault();
        }

        public static bool HasAttribute<T>(this MemberInfo memberInfo, bool inherit = true)
            where T : Attribute
        {
            object[] attributes = memberInfo.GetCustomAttributes(inherit);
            return attributes.OfType<T>().Any();
        }
    }
}