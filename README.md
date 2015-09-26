# json
Really simple JSON parser in ~250 lines
- Attempts to parse JSON files with minimal GC allocation
- Nice and simple "[1,2,3]".FromJson<List<int>>() API
- Classes and structs can be parsed too!
  class Foo { public int Value; }
  "{\"Value\":10}".FromJson<Foo>()
- No JIT Emit support to support AOT compilation on iOS
- Attempts are made to NOT throw an exception if the JSON is corrupted or invalid: returns null instead.
- Only public fields and property setters on classes/structs will be written to

Limitations:
- No JIT Emit support to parse structures quickly
- Limited to parsing <2GB JSON files (due to int.MaxValue)
- Parsing of abstract classes or interfaces is NOT supported and will throw an exception.
