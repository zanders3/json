# Tiny Json

[![Build Status](https://travis-ci.org/zanders3/json.png?branch=master)](https://travis-ci.org/zanders3/json)
[![License](https://img.shields.io/badge/license-MIT-lightgrey.svg)](https://raw.githubusercontent.com/zanders3/json/master/LICENSE)
[![Nuget Version](https://img.shields.io/nuget/v/TinyJson.svg)](https://www.nuget.org/packages/TinyJson)
[![Nuget Downloads](https://img.shields.io/nuget/dt/TinyJson.svg)](https://www.nuget.org/packages/TinyJson)

A really simple C# JSON parser in ~250 lines
- Attempts to parse JSON files with minimal GC allocation
- Nice and simple `"[1,2,3]".FromJson<List<int>>()` API
- Classes and structs can be parsed too!
```csharp
class Foo 
{ 
  public int Value;
}
"{\"Value\":10}".FromJson<Foo>()
```
- Anonymous JSON is parsed into `Dictionary<string,object>` and `List<object>`
```csharp
var test = "{\"Value\:10}".FromJson<object>();
int number = ((Dictionary<string,object>)test)["Value"];
```
- No JIT Emit support to support AOT compilation on iOS
- Attempts are made to NOT throw an exception if the JSON is corrupted or invalid: returns null instead.
- Only public fields and property setters on classes/structs will be written to

Limitations:
- No JIT Emit support to parse structures quickly
- Limited to parsing <2GB JSON files (due to int.MaxValue)
- Parsing of abstract classes or interfaces is NOT supported and will throw an exception.
