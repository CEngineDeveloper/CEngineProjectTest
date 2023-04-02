#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEditor;

namespace CYM
{
    /// <summary>
    /// Utilities for things like quickly getting a custom value of a serialized property.
    /// </summary>
    public static class ExtensionSerializedProperty
    {
        /// <summary>
        /// Courtesy of douduck08. Cheers.
        /// https://gist.github.com/douduck08/6d3e323b538a741466de00c30aa4b61f
        /// </summary>
        public static T GetValue<T>(this SerializedProperty property) where T : class
        {
            object obj = property.serializedObject.targetObject;
            string path = property.propertyPath.Replace(".Array.data", "");
            string[] fieldStructure = path.Split('.');
            Regex rgx = new Regex(@"\[\d+\]");
            for (int i = 0; i < fieldStructure.Length; i++)
            {
                if (fieldStructure[i].Contains("["))
                {
                    int index = System.Convert.ToInt32(new string(fieldStructure[i].Where(char.IsDigit).ToArray()));
                    obj = GetFieldValueWithIndex(rgx.Replace(fieldStructure[i], ""), obj, index);
                }
                else
                {
                    obj = GetFieldValue(fieldStructure[i], obj);
                }
            }

            return (T)obj;
        }

        public static bool SetValue<T>(this SerializedProperty property, T value) where T : class
        {
            object obj = property.serializedObject.targetObject;
            string path = property.propertyPath.Replace(".Array.data", "");
            string[] fieldStructure = path.Split('.');
            Regex rgx = new Regex(@"\[\d+\]");
            for (int i = 0; i < fieldStructure.Length - 1; i++)
            {
                if (fieldStructure[i].Contains("["))
                {
                    int index = System.Convert.ToInt32(new string(fieldStructure[i].Where(char.IsDigit).ToArray()));
                    obj = GetFieldValueWithIndex(rgx.Replace(fieldStructure[i], ""), obj, index);
                }
                else
                {
                    obj = GetFieldValue(fieldStructure[i], obj);
                }
            }

            string fieldName = fieldStructure.Last();
            if (fieldName.Contains("["))
            {
                int index = System.Convert.ToInt32(new string(fieldName.Where(char.IsDigit).ToArray()));
                return SetFieldValueWithIndex(rgx.Replace(fieldName, ""), obj, index, value);
            }

            return SetFieldValue(fieldName, obj, value);
        }

        private static object GetFieldValue(
            string fieldName, object obj,
            BindingFlags bindings = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public |
                                    BindingFlags.NonPublic)
        {
            FieldInfo field = obj.GetType().GetField(fieldName, bindings);
            if (field != null)
                return field.GetValue(obj);

            return default(object);
        }

        private static object GetFieldValueWithIndex(
            string fieldName, object obj, int index,
            BindingFlags bindings = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public |
                                    BindingFlags.NonPublic)
        {
            if (index < 0)
                return null;
            
            FieldInfo field = obj.GetType().GetField(fieldName, bindings);
            if (field != null)
            {
                object list = field.GetValue(obj);
                if (list.GetType().IsArray)
                {
                    object[] array = (object[])list;
                    if (index >= array.Length)
                        return null;
                    return array[index];
                }
                if (list is IEnumerable)
                {
                    IList iList = (IList)list;
                    if (index >= iList.Count)
                        return null;
                    return iList[index];
                }
            }

            return null;
        }

        public static bool SetFieldValue(
            string fieldName, object obj, object value, bool includeAllBases = false,
            BindingFlags bindings = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public |
                                    BindingFlags.NonPublic)
        {
            FieldInfo field = obj.GetType().GetField(fieldName, bindings);
            if (field != null)
            {
                field.SetValue(obj, value);
                return true;
            }

            return false;
        }

        public static bool SetFieldValueWithIndex(
            string fieldName, object obj, int index, object value, bool includeAllBases = false,
            BindingFlags bindings = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public |
                                    BindingFlags.NonPublic)
        {
            FieldInfo field = obj.GetType().GetField(fieldName, bindings);
            if (field != null)
            {
                object list = field.GetValue(obj);
                if (list.GetType().IsArray)
                {
                    ((object[])list)[index] = value;
                    return true;
                }

                if (value is IEnumerable)
                {
                    ((IList)list)[index] = value;
                    return true;
                }
            }

            return false;
        }
        
        /// <summary>
        /// From: https://gist.github.com/monry/9de7009689cbc5050c652bcaaaa11daa
        /// </summary>
        public static SerializedProperty GetParent(this SerializedProperty serializedProperty)
        {
            string[] propertyPaths = serializedProperty.propertyPath.Split('.');
            if (propertyPaths.Length <= 1)
            {
                return default(SerializedProperty);
            }

            SerializedProperty parentSerializedProperty =
                serializedProperty.serializedObject.FindProperty(propertyPaths.First());
            for (int index = 1; index < propertyPaths.Length - 1; index++)
            {
                if (propertyPaths[index] == "Array")
                {
                    if (index + 1 == propertyPaths.Length - 1)
                    {
                        // reached the end
                        break;
                    }
                    if (propertyPaths.Length > index + 1 && Regex.IsMatch(propertyPaths[index + 1], "^data\\[\\d+\\]$"))
                    {
                        Match match = Regex.Match(propertyPaths[index + 1], "^data\\[(\\d+)\\]$");
                        int arrayIndex = int.Parse(match.Groups[1].Value);
                        parentSerializedProperty = parentSerializedProperty.GetArrayElementAtIndex(arrayIndex);
                        index++;
                    }
                }
                else
                {
                    parentSerializedProperty = parentSerializedProperty.FindPropertyRelative(propertyPaths[index]);
                }
            }

            return parentSerializedProperty;
        }
        
        public static SerializedProperty AddArrayElement(this SerializedProperty serializedProperty)
        {
            serializedProperty.InsertArrayElementAtIndex(serializedProperty.arraySize);
            return serializedProperty.GetArrayElementAtIndex(serializedProperty.arraySize - 1);
        }

        private static Dictionary<string, float> propertyPathToHeight = new Dictionary<string, float>();
        private static Dictionary<string, Type> managedReferenceFullTypeNameToTypeCache = new Dictionary<string, Type>();

        private static Dictionary<string, object> propertyPathToObjectCache = new Dictionary<string, object>();

        public static Type GetTypeFromManagedFullTypeName(this SerializedProperty serializedProperty)
        {
            if (string.IsNullOrEmpty(serializedProperty.managedReferenceFullTypename)) return null;
            //throw new Exception($"Serialized Property doesnt have managedReferenceFullTypename");

            if (managedReferenceFullTypeNameToTypeCache.TryGetValue(serializedProperty.managedReferenceFullTypename, out Type type))
                return type;

            string[] typeInfo = serializedProperty.managedReferenceFullTypename.Split(' ');
            type = Type.GetType($"{typeInfo[1]}, {typeInfo[0]}");
            managedReferenceFullTypeNameToTypeCache.Add(serializedProperty.managedReferenceFullTypename, type);

            return type;
        }

        public static float GetPropertyDrawerHeight(this SerializedProperty property, float defaultHeight = 18)
        {
            return GetPropertyDrawerHeight(property.propertyPath, defaultHeight);
        }

        public static float GetPropertyDrawerHeight(string propertyPath, float defaultHeight = 18)
        {
            if (propertyPathToHeight.TryGetValue(propertyPath, out float result))
                return result;

            result = defaultHeight;
            return result;
        }

        public static void SetPropertyDrawerHeight(this SerializedProperty property, float height)
        {
            SetPropertyDrawerHeight(property.propertyPath, height);
        }

        public static void SetPropertyDrawerHeight(string propertyPath, float height)
        {
            propertyPathToHeight[propertyPath] = height;
        }

        public static void ClearPropertyCache(string pathOrPartOfPath = "")
        {
            if (string.IsNullOrEmpty(pathOrPartOfPath))
            {
                propertyPathToObjectCache.Clear();
                return;
            }

            List<string> propertiesTobeRemoved = new List<string>();
            foreach (KeyValuePair<string, object> keyValuePair in propertyPathToObjectCache)
            {
                string key = keyValuePair.Key;
                if (key.IndexOf(pathOrPartOfPath, StringComparison.Ordinal) == -1)
                    continue;

                propertiesTobeRemoved.Add(key);
            }

            for (int i = 0; i < propertiesTobeRemoved.Count; i++)
                propertyPathToObjectCache.Remove(propertiesTobeRemoved[i]);
        }

        public static bool TryGetTargetObjectOfProperty<T>(this SerializedProperty prop, out T resultObject) where T : class
        {
            resultObject = null;
            if (prop == null)
                return false;

            // if (propertyPathToObjectCache.TryGetValue(prop.propertyPath, out object result))
            // {
            //     if (result != null)
            //     {
            //         resultObject = result as T;
            //         return true;
            //     }
            //
            //     propertyPathToObjectCache.Remove(prop.propertyPath);
            // }

            string path = prop.propertyPath.Replace(".Array.data[", "[");
            object obj = prop.serializedObject.targetObject;
            string[] elements = path.Split('.');
            for (int i = 0; i < elements.Length; i++)
            {
                string element = elements[i];
                if (element.Contains("["))
                {
                    string elementName = element.Substring(0, element.IndexOf("[", StringComparison.Ordinal));
                    int index = Convert.ToInt32(element.Substring(element.IndexOf("[", StringComparison.Ordinal)).Replace("[", "").Replace("]", ""));
                    obj = GetValue_Imp(obj, elementName, index);
                }
                else
                {
                    obj = GetValue_Imp(obj, element);
                }
            }

            if (obj is T t)
            {
                resultObject = t;
                // propertyPathToObjectCache.Add(prop.propertyPath, resultObject);
                return true;
            }

            return false;
        }

        private static object GetValue_Imp(object source, string name)
        {
            if (source == null)
                return null;
            Type type = source.GetType();

            while (type != null)
            {
                FieldInfo f = type.GetField(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
                if (f != null)
                    return f.GetValue(source);

                PropertyInfo p = type.GetProperty(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                if (p != null)
                    return p.GetValue(source, null);

                type = type.BaseType;
            }
            return null;
        }

        private static object GetValue_Imp(object source, string name, int index)
        {
            IEnumerable enumerable = GetValue_Imp(source, name) as IEnumerable;
            if (enumerable == null)
                return null;
            IEnumerator enm = enumerable.GetEnumerator();

            for (int i = 0; i <= index; i++)
            {
                if (!enm.MoveNext())
                    return null;
            }
            return enm.Current;
        }


        public static IEnumerable<SerializedProperty> GetChildren(this SerializedProperty serializedProperty)
        {
            SerializedProperty currentProperty = serializedProperty.Copy();
            SerializedProperty nextSiblingProperty = serializedProperty.Copy();
            {
                nextSiblingProperty.Next(false);
            }

            if (currentProperty.Next(true))
            {
                do
                {
                    if (SerializedProperty.EqualContents(currentProperty, nextSiblingProperty))
                        break;

                    yield return currentProperty;
                }
                while (currentProperty.Next(false));
            }
        }
        public static IEnumerable<SerializedProperty> GetVisibleChildren(this SerializedProperty serializedProperty)
        {
            SerializedProperty currentProperty = serializedProperty.Copy();
            SerializedProperty nextSiblingProperty = serializedProperty.Copy();
            {
                nextSiblingProperty.NextVisible(false);
            }

            if (currentProperty.NextVisible(true))
            {
                do
                {
                    if (SerializedProperty.EqualContents(currentProperty, nextSiblingProperty))
                        break;

                    yield return currentProperty;
                }
                while (currentProperty.NextVisible(false));
            }
        }
    }
}
#endif