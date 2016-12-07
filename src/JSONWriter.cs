using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
                stringBuilder.Append("null");
                return;
            }

            Type type = item.GetType();
            if (type == typeof(string))
            {
                stringBuilder.Append('\"');
                stringBuilder.Append(((string)item).Replace("\\", "\\\\"));
                stringBuilder.Append('\"');
            }
            else if (type == typeof(int) || type == typeof(float) || type == typeof(double))
            {
                stringBuilder.Append(item.ToString());
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
                    if (fieldInfos[i].IsPublic)
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
                    }
                }
                PropertyInfo[] propertyInfo = type.GetProperties();
                for (int i = 0; i<propertyInfo.Length; i++)
                {
                    if (propertyInfo[i].CanRead)
                    {
                        object value = propertyInfo[i].GetValue(item, null);
                        if (value != null)
                        {
                            if (isFirst)
                                isFirst = false;
                            else
                                stringBuilder.Append(',');
                            stringBuilder.Append('\"');
                            stringBuilder.Append(propertyInfo[i].Name);
                            stringBuilder.Append("\":");
                            AppendValue(stringBuilder, value);
                        }
                    }
                }

                stringBuilder.Append('}');
            }
        }
    }
}
