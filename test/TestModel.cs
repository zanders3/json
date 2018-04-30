using System;
using System.Collections.Generic;
using System.Text;

namespace jsontest
{
    public class TestModel<T>
    {
        public T Data { get; set; }
        public string Name { get; set; }
    }

    public class DataModel
    {
        [TinyJson.JsonIgnore]
        public string Name { get; set; }
        [TinyJson.JsonProperty("CustomeName")]
        public int? Id { get; set; }
    }
}
