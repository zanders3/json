using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;

namespace TinyJson
{
    //Really simple JSON writer
    //- Outputs JSON structures from an object
    //- Really simple API (new List<int> { 1, 2, 3 }).ToJson() == "[1,2,3]"
    //- Will only output public fields and property getters on objects
    public static class JSONWriter
    {
        public static string ToJson(this object item, bool makeBeautiful = false)
        {
            StringBuilder stringBuilder = new StringBuilder();
            AppendValue(stringBuilder, item);
            return makeBeautiful ?
                MakeBeautiful(stringBuilder.ToString().ToArray()).ToString() :
                stringBuilder.ToString();
        }

        static void AppendValue(StringBuilder stringBuilder, object item)
        {
            if (item == null)
            {
                stringBuilder.Append("null");
                return;
            }

            Type type = item.GetType();
            if (type == typeof(string) || type == typeof(char))
            {
                stringBuilder.Append('"');
                string str = item.ToString();
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
            else if (type == typeof(byte) || type == typeof(sbyte))
            {
                stringBuilder.Append(item.ToString());
            }
            else if (type == typeof(short) || type == typeof(ushort))
            {
                stringBuilder.Append(item.ToString());
            }
            else if (type == typeof(int) || type == typeof(uint))
            {
                stringBuilder.Append(item.ToString());
            }
            else if (type == typeof(long) || type == typeof(ulong))
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
            else if (type == typeof(decimal))
            {
                stringBuilder.Append(((decimal)item).ToString(System.Globalization.CultureInfo.InvariantCulture));
            }
            else if (type == typeof(bool))
            {
                stringBuilder.Append(((bool)item) ? "true" : "false");
            }
            else if (type == typeof(DateTime))
            {
                stringBuilder.Append('"');
                stringBuilder.Append(((DateTime)item).ToString(System.Globalization.CultureInfo.InvariantCulture));
                stringBuilder.Append('"');
            }
            else if (type.IsEnum)
            {
                stringBuilder.Append('"');
                stringBuilder.Append(item.ToString());
                stringBuilder.Append('"');
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
                FieldInfo[] fieldInfos = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy);
                for (int i = 0; i < fieldInfos.Length; i++)
                {
                    if (fieldInfos[i].IsDefined(typeof(IgnoreDataMemberAttribute), true))
                        continue;

                    object value = fieldInfos[i].GetValue(item);
                    if (value != null)
                    {
                        if (isFirst)
                            isFirst = false;
                        else
                            stringBuilder.Append(',');
                        stringBuilder.Append('\"');
                        stringBuilder.Append(GetMemberName(fieldInfos[i]));
                        stringBuilder.Append("\":");
                        AppendValue(stringBuilder, value);
                    }
                }
                PropertyInfo[] propertyInfo = type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy);
                for (int i = 0; i < propertyInfo.Length; i++)
                {
                    if (!propertyInfo[i].CanRead || propertyInfo[i].IsDefined(typeof(IgnoreDataMemberAttribute), true))
                        continue;

                    object value = propertyInfo[i].GetValue(item, null);
                    if (value != null)
                    {
                        if (isFirst)
                            isFirst = false;
                        else
                            stringBuilder.Append(',');
                        stringBuilder.Append('\"');
                        stringBuilder.Append(GetMemberName(propertyInfo[i]));
                        stringBuilder.Append("\":");
                        AppendValue(stringBuilder, value);
                    }
                }

                stringBuilder.Append('}');
            }
        }

        static string GetMemberName(MemberInfo member)
        {
            if (member.IsDefined(typeof(DataMemberAttribute), true))
            {
                DataMemberAttribute dataMemberAttribute = (DataMemberAttribute)Attribute.GetCustomAttribute(member, typeof(DataMemberAttribute), true);
                if (!string.IsNullOrEmpty(dataMemberAttribute.Name))
                    return dataMemberAttribute.Name;
            }

            return member.Name;
        }

        private static string GetTabs(int level)
        {
            if (level == 0)
                return null;


            var chars = new char[level];
            for (int i = 0; i < chars.Length; i++)
            {
                chars[i] = '\t';
            }

            return new string(chars);
        }

        private static StringBuilder MakeBeautiful(char[] chars)
        {
            var newStringBuilder = new StringBuilder();
            var tabs = 0;

            //char? chrRight;
            char? chrLeft = null;
            for (var i = 0; i < chars.Length; i++)
            {
                var chr = chars[i];
                switch (chr)
                {
                    case '[':
                        if (chrLeft.HasValue && chrLeft == ',')
                        {
                            newStringBuilder.Append('\n');
                            newStringBuilder.Append($"{GetTabs(tabs)}{chr}");
                        }
                        else
                        {
                            newStringBuilder.Append(chr);
                        }

                        tabs++;
                        break;
                    case ']':
                        tabs--;
                        newStringBuilder.Append($"\n{GetTabs(tabs)}{chr}");
                        break;
                    case '{':
                        if (chrLeft.HasValue && (chrLeft == ',' || chrLeft == '['))
                        {
                            newStringBuilder.Append('\n');
                            newStringBuilder.Append($"{GetTabs(tabs)}{chr}");
                        }
                        else
                        {
                            newStringBuilder.Append(chr);
                        }

                        tabs++;
                        break;
                    case '}':
                        tabs--;
                        newStringBuilder.Append($"\n{GetTabs(tabs)}{chr}");
                        break;
                    case '"':
                        if (chrLeft.HasValue && (chrLeft == '{' || chrLeft == '[' || chrLeft == ',' || chrLeft == ':'))
                        {
                            if (chrLeft != ':')
                            {
                                newStringBuilder.Append('\n');
                                newStringBuilder.Append($"{GetTabs(tabs)}{chr}");
                            }
                            else
                            {
                                newStringBuilder.Append(chr);
                            }


                            var nextIndex = GetNextChar(chars, i + 1);
                            newStringBuilder.Append(new string(chars, i + 1, nextIndex - i - 1));
                            i = nextIndex - 1;
                            chr = chars[i];
                        }
                        else
                        {
                            newStringBuilder.Append(chr);
                        }

                        break;
                    default:
                        newStringBuilder.Append(chr);
                        break;
                }


                chrLeft = chr;
            }
            return newStringBuilder;
        }

        private static int GetNextChar(char[] chars, in int start)
        {
            for (var i = start; i < chars.Length; i++)
            {
                var chr = chars[i];
                if (chr == '"' && chars[i - 2] != '\\')
                    return i;
            }

            return -1;
        }
    }
}
