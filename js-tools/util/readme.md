# Converter

Simple converter to create classes from a json file

## Usage

    node converter.js -i:input.json -o:output.cs -n:rootObject -ns:datamodels -t:cs

## Options

|argument|shortcut|description|required|
|--------|--------|-----------|--------|
|input|i|The json file you want to read|[X]|
|output|o|The file it should create, when not specified the output goes to the console|[O]|
|namespace|ns|The namespace argument for the file to be created, when not specified only the classes are printed|[O]|
|name|n|The name used for the rootObject, when not specified `RootObject` will be used|[O]|
|target|t|The target for the output system (either cs or ts), defaults to CS|[O]|

## Generators available

Typescript generator & C# generator

## Quirks

Any object that has options that aren't translatable to C# will get a property called `Properties` that has the `[JsonExtensionData]` attribute. All properties that can be set will be available on the object.

All properties that have a `null` value will be treated as type `string`.

All items in an Array will be merged together in one subclass, where `decimal` will win from `int` (if multiple types were specified) and if a property has both the `decimal` and a `string` property, it will assume the property was `null` at one time, and treat it as a `decimal?` instead.

All empty arrays will be created as `JArray`.

When no `input` file was specified, the program will throw a `ParseException`.

All input files and output files are read using `utf-8` encoding.

The serializer knows following types: `int`, `string`, `decimal`, `decimal?`, `JArray`, `IDictionary<string, JObject>`, `bool`. 

## Known issues

- If multiple same named array properties exist, the first encountered will be used as the class to output, even if their properties may differ.
- The program doesn't provide a helpfile, therefor it assumes one has access to this document.
- The program will not try to match classes that have exactly the same properties.
- Array of primitive types are not supported