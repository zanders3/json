﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace TinyJson
{
    //Really simple JSON writer
    //- Outputs JSON structures from an object
    //- Really simple API (new List<int> { 1, 2, 3 }).ToJson() == "[1,2,3]"
    //- Will only output public fields and property getters on objects
    public static class JSONWriter
    {
        public static string ToJson(this object item)
        {
            StringBuilder stringBuilder = new StringBuilder();
            AppendValue(stringBuilder, item);
            return stringBuilder.ToString();
        }

        static void AppendValue(StringBuilder stringBuilder, object item)
        {
            if (item == null)
            {
                stringBuilder.Append(item.GetType().Name + ": null");
                return;
            }

            Type type = item.GetType();
            if (type == typeof(string))
            {
                stringBuilder.Append('"');
                string str = (string)item;
                for (int i = 0; i < str.Length; ++i)
                    if (str[i] < ' ' || str[i] == '"' || str[i] == '\\')
                    {
                        stringBuilder.Append('\\');
                        int j = "\"\\\n\r\t\b\f".IndexOf(str[i]);
                        if (j >= 0)
                            stringBuilder.Append("\"\\nrtbf"[j]);
                        else
                            stringBuilder.AppendFormat("u{0:X4}", (UInt32)str[i]);
                    }
                    else
                        stringBuilder.Append(str[i]);
                stringBuilder.Append('"');
            }
            else if (type == typeof(byte) || type == typeof(int))
            {
                stringBuilder.Append(item.ToString());
            }
            else if (type == typeof(float))
            {
                stringBuilder.Append(((float)item).ToString(System.Globalization.CultureInfo.InvariantCulture));
            }
            else if (type == typeof(double))
            {
                stringBuilder.Append(((double)item).ToString(System.Globalization.CultureInfo.InvariantCulture));
            }
            else if (type == typeof(bool))
            {
                stringBuilder.Append(((bool)item) ? "true" : "false");
            }
            else if (item is IList)
            {
                stringBuilder.Append('[');
                bool isFirst = true;
                IList list = item as IList;
                for (int i = 0; i < list.Count; i++)
                {
                    if (isFirst)
                        isFirst = false;
                    else
                        stringBuilder.Append(',');
                    AppendValue(stringBuilder, list[i]);
                }
                stringBuilder.Append(']');
            }
            else if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Dictionary<,>))
            {
                Type keyType = type.GetGenericArguments()[0];

                //Refuse to output dictionary keys that aren't of type string
                if (keyType != typeof(string))
                {
                    stringBuilder.Append("{}");
                    return;
                }

                stringBuilder.Append('{');
                IDictionary dict = item as IDictionary;
                bool isFirst = true;
                foreach (object key in dict.Keys)
                {
                    if (isFirst)
                        isFirst = false;
                    else
                        stringBuilder.Append(',');
                    stringBuilder.Append('\"');
                    stringBuilder.Append((string)key);
                    stringBuilder.Append("\":");
                    AppendValue(stringBuilder, dict[key]);
                }
                stringBuilder.Append('}');
            }
            else
            {
                stringBuilder.Append('{');

                bool isFirst = true;
                FieldInfo[] fieldInfos = type.GetFields();
                for (int i = 0; i < fieldInfos.Length; i++)
                {
                    JsonIgnoreAttribute ignoreAttr = (JsonIgnoreAttribute)fieldInfos[i].GetCustomAttribute(typeof(JsonIgnoreAttribute), false);
                    if (ignoreAttr != null)
                        continue;

                    if (fieldInfos[i].IsPublic && !fieldInfos[i].IsStatic)
                    {
                        object value = fieldInfos[i].GetValue(item);
                        if (value != null)
                        {
                            if (isFirst)
                                isFirst = false;
                            else
                                stringBuilder.Append(',');
                            stringBuilder.Append('\"');
                            stringBuilder.Append(fieldInfos[i].Name);
                            stringBuilder.Append("\":");
                            AppendValue(stringBuilder, value);
                        }
                        else
                        {
                            if (isFirst)
                                isFirst = false;
                            else
                                stringBuilder.Append(',');
                            stringBuilder.Append('\"');
                            stringBuilder.Append(fieldInfos[i].Name);
                            stringBuilder.Append("\":");
                            stringBuilder.Append("null");
                        }
                    }
                }
                PropertyInfo[] propertyInfo = type.GetProperties();
                for (int i = 0; i < propertyInfo.Length; i++)
                {
                    if (propertyInfo[i].CanRead)
                    {
                        string propertyName = propertyInfo[i].Name;

                        JsonIgnoreAttribute ignoreAttr = (JsonIgnoreAttribute)propertyInfo[i].GetCustomAttribute(typeof(JsonIgnoreAttribute), false);
                        if (ignoreAttr != null)
                            continue;

                        JsonPropertyAttribute nameAttribute = (JsonPropertyAttribute)propertyInfo[i].GetCustomAttribute(typeof(JsonPropertyAttribute), false);
                        if (nameAttribute != null)
                            propertyName = nameAttribute.Name;

                        object value = propertyInfo[i].GetValue(item, null);
                        if (value != null)
                        {
                            if (isFirst)
                                isFirst = false;
                            else
                                stringBuilder.Append(',');
                            stringBuilder.Append('\"');
                            stringBuilder.Append(propertyName);
                            stringBuilder.Append("\":");
                            AppendValue(stringBuilder, value);
                        }
                        else
                        {
                            if (isFirst)
                                isFirst = false;
                            else
                                stringBuilder.Append(',');
                            stringBuilder.Append('\"');
                            stringBuilder.Append(propertyName);
                            stringBuilder.Append("\":");
                            stringBuilder.Append("null");
                        }
                    }
                }

                stringBuilder.Append('}');
            }
        }
    }
}
