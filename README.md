# JaysonParser

## Overview
JaysonParser is a JSON parser implemented in C# that provides a simple and efficient way to parse JSON strings into .NET objects.

## Table of Contents

- [Installation](#installation)
- [Features](#features)
- [Examples](#examples)
- [Contributing](#contributing)

## Installation
To get started with JaysonParser, follow these steps:

```bash
# Clone the repository
git clone https://github.com/OkaforGerald/JaysonParser.git

# Navigate to the project directory
cd JaysonParser

# Install dependencies
dotnet restore
```
After installing the dependencies, you can build and run the project:

```bash
# Build the project
dotnet build

# Run the project
dotnet run
```

## Features
1. Parses JSON strings into nested structures of C# objects
2. Supports all standard JSON data types: objects, arrays, strings, numbers, booleans, and null
3. Handles nested structures to arbitrary depth
4. Provides detailed error messages for invalid JSON
5. Implements proper whitespace handling as per JSON specification

## Examples

Parsing an object with nested structures
```csharp
using JaysonParser;

class Program
{
    static void Main(string[] args)
    {
        string json = @"{
          ""Image"": {
          ""Width"": 800,
          ""Height"": 600,
          ""Title"": ""View from 15th Floor"",
          ""Thumbnail"": {
            ""Url"": ""http://www.example.com/image/481989943"",
            ""Height"": 125,
            ""Width"": 100
                        },
          ""Animated"": false,
          ""IDs"": [116, 943, 234, 38793],
          ""Omo"": [9, 7, null, true]
                      }
              }";

        try
        {
            var result = JaysonParser.Parse(json);
            if (result != null)
            {
                Console.WriteLine("JSON parsed successfully!");
                // You can now work with the parsed object
                // For example, if you know it's a Dictionary<string, object>:
                var jsonObject = result as Dictionary<string, object>;
                if (jsonObject != null)
                {
                    // Access nested elements
                    var image = jsonObject["Image"] as Dictionary<string, object>;
                    if (image != null)
                    {
                        Console.WriteLine($"Image Width: {image["Width"]}");
                        Console.WriteLine($"Image Height: {image["Height"]}");
                        Console.WriteLine($"Image Title: {image["Title"]}");
                        
                        var thumbnail = image["Thumbnail"] as Dictionary<string, object>;
                        if (thumbnail != null)
                        {
                            Console.WriteLine($"Thumbnail URL: {thumbnail["Url"]}");
                        }
                        
                        var ids = image["IDs"] as object[];
                        if (ids != null)
                        {
                            Console.WriteLine($"IDs: {string.Join(", ", ids)}");
                        }
                        
                        var omo = image["Omo"] as object[];
                        if (omo != null)
                        {
                            Console.WriteLine($"Omo: {string.Join(", ", omo)}");
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error parsing JSON: {ex.Message}");
        }
    }
}
```
This outputs:
```
JSON parsed successfully!
Image Width: 800
Image Height: 600
Image Title: View from 15th Floor
Thumbnail URL: http://www.example.com/image/481989943
IDs: 116, 943, 234, 38793
Omo: 9, 7, , True
```
### Another example

Here's another example of parsing a JSON array containing multiple objects:

```csharp
using JaysonParser;
using System;
using System.Collections.Generic;

class Program
{
    static void Main(string[] args)
    {
        var parser = new JaysonParser();
        string json = @"[
        {
           ""precision"": ""zip"",
           ""Latitude"":  37.7668,
           ""Longitude"": -122.3959,
           ""Address"":   """",
           ""City"":      ""SAN FRANCISCO"",
           ""State"":     ""CA"",
           ""Zip"":       ""94107"",
           ""Country"":   ""US""
        },
        {
           ""precision"": ""zip"",
           ""Latitude"":  37.371991,
           ""Longitude"": -122.026020,
           ""Address"":   """",
           ""City"":      ""SUNNYVALE"",
           ""State"":     ""CA"",
           ""Zip"":       ""94085"",
           ""Country"":   ""US""
        }
      ]";

        try
        {
            var result = parser.Parse(json);
            if (result != null && result is object[] array)
            {
                Console.WriteLine("JSON parsed successfully!");
                Console.WriteLine($"Number of objects in array: {array.Length}");

                foreach (var item in array)
                {
                    if (item is Dictionary<string, object> location)
                    {
                        Console.WriteLine("\nLocation Details:");
                        Console.WriteLine($"City: {location["City"]}");
                        Console.WriteLine($"State: {location["State"]}");
                        Console.WriteLine($"Zip: {location["Zip"]}");
                        Console.WriteLine($"Latitude: {location["Latitude"]}");
                        Console.WriteLine($"Longitude: {location["Longitude"]}");
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error parsing JSON: {ex.Message}");
        }
    }
}
```
This outputs: 
```
JSON parsed successfully!
Number of objects in array: 2

Location Details:
City: SAN FRANCISCO
State: CA
Zip: 94107
Latitude: 37.7668
Longitude: -122.3959

Location Details:
City: SUNNYVALE
State: CA
Zip: 94085
Latitude: 37.371991
Longitude: -122.02602
```

## Contributing

Contributions are welcome! If you have any suggestions, bug reports, or feature requests, please open an issue or submit a pull request.
