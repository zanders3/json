using System;

namespace TinyJson
{
    public class JsonPropertyAttribute : Attribute
    {
        public JsonPropertyAttribute(string name)
        {
            Name = name;
        }

        public string Name { get; set; }
    }
}
