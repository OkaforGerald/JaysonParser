using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace JaysonParser
{
    public class JaysonParser
    {
        public static object? Parse([StringSyntax("Json")] string json)
        {
            if (string.IsNullOrEmpty(json)) throw new ArgumentNullException(json);
            object? result = null;
            bool resultAssignedNull = false;
            int position = 0;

            while(position < json.Length)
            {
                try
                {
                    var (newPosition, assigned) = ParseValue(json, position, resultAssignedNull, out result);
                    position = newPosition + 1;
                    resultAssignedNull = assigned;

                    // Check for any non-whitespace characters after the parsed value
                    while (position < json.Length)
                    {
                        if (!char.IsWhiteSpace(json[position]))
                        {
                            throw new Exception($"Unexpected character '{json[position]}' after JSON value");
                        }
                        position++;
                    }
                }
                catch(Exception e)
                {
                    Console.WriteLine(e.Message);
                    return null;
                }
                
            }

            if(!resultAssignedNull && result is null)
            {
                throw new Exception("Failure to parse JSON!");
            }
            else
            {
                return result;
            }
        }

        public static (int position, bool assigned) ParseValue([StringSyntax("Json")]string json, int position, bool resultAssignedNull, out object? Value)
        {
            object? result = null;
            int i = position;
            if (json is null) throw new ArgumentNullException("Input string for parsing!");

            for(i = position; i < json.Length; i++)
            {
                char character = json[i];

                if (character == 'n' && json.Substring(i).Length >= 4) //Assume it's null
                {
                    i += 4;
                    result = null;
                    resultAssignedNull = true;
                    break;
                }

                if (character == 't' && json.Substring(i).Length >= 4) //Assume it's true
                {
                    i += 4;
                    result = true;
                    break;
                }

                if (character == 'f' && json.Substring(i).Length >= 5) //Assume it's false
                {
                    i += 5;
                    result = false;
                    break;
                }

                if(character == '\"')
                {
                    try
                    {
                        i = ParseString(json, i, out string? ParsedString);
                        result = ParsedString;
                        break;
                    }catch(Exception e)
                    {
                        throw;
                    }
                    
                }

                if(character == '-' || Char.IsDigit(character))
                {
                    try
                    {
                        i = ParseNumber(json, i, out decimal Number);
                        result = Number;
                        break;
                    }
                    catch(Exception e)
                    {
                        throw;
                    }
                }

                if(character == '[')
                {
                    i = ParseArray(json, i, out object[]? Array);
                    result = Array;
                    break;
                }

                if(character == '{')
                {
                    i = ParseObject(json, i, out Dictionary<string, object>? Object);
                    result = Object;
                    break;
                }
            }

            if (!resultAssignedNull && result is null)
            {
                throw new Exception("Invalid token!");
            }

            Value = result;

            return (i, resultAssignedNull);
        }

        private static int ParseNumber(string json, int position, out decimal Number)
        {
            Number = 0;
            char c = json[position];
            string NumberAsString = "";
            while(position < json.Length && (Char.IsDigit(json[position]) || json[position] == '.' || json[position] == 'e' || json[position] == 'E' || json[position] == '-'))
            {
                NumberAsString += json[position];
                position++;
            }
            try
            {
                Number = Decimal.Parse(NumberAsString, System.Globalization.NumberStyles.Any);

            }catch(Exception ex)
            {
                Console.WriteLine(ex.Message);   
            }
            
            return position;
        }

        private static int ParseString(string json, int position, out string? ParsedString)
        {
            position++;
            bool ClosedString = false;
            StringBuilder builder = new StringBuilder();
            while (position < json.Length)
            {
                if (json[position] == '\\')
                {
                    builder.Append('\\');
                    position++;
                    builder.Append(json[position]);
                }
                else if (json[position] != '\"')
                {
                    builder.Append(json[position]);
                }
                else
                {
                    ClosedString = true;
                    position++;
                    break;
                }
                
                position++;
            }

            if (position == json.Length && !ClosedString) throw new Exception("Expected end of string, but instead reached end of data.");

            ParsedString = Regex.Unescape(builder.ToString());
            return position;
        }

        private static int ParseArray(string json, int position, out object[]? ParsedArray)
        {
            bool AwaitingValue = true;
            List<object> result = new();
            position++;

            while (position < json.Length && json[position] != ']')
            {
                if (char.IsWhiteSpace(json[position]))
                {
                    position++;
                }
                else if(json[position] == ',')
                {
                    AwaitingValue = true;
                    position++;
                }
                else
                {
                    if (!AwaitingValue) throw new Exception("Comma expected before another value!");
                    var value = ParseValue(json, position, false, out var Value);
                    if (!value.assigned)
                    {
                        if (Value?.GetType() == typeof(object[])) { position = value.position + 1; } else { position = value.position; };
                    }
                    else
                    {
                        position = value.position;
                    }
                    result.Add(Value!);
                    AwaitingValue = false;
                }
            }
            if (AwaitingValue && result.Any())
            {
                throw new Exception("Trailing comma in array");
            }

            if (position >= json.Length || json[position] != ']')
            {
                throw new Exception("Array is not properly closed");
            }

            ParsedArray = result.ToArray();
            return position;
        }

        private static int ParseObject(string json, int position, out Dictionary<string, object>? ParsedObject)
        {
            bool AwaitingKey = true;
            string? Key = null;
            Dictionary<string, object> result = new();
            position++;

            while (position < json.Length && json[position] != '}')
            {
                if (json[position] == ':')
                {
                    AwaitingKey = false;
                    position++;
                }
                else if (char.IsWhiteSpace(json[position]))
                {
                    position++;
                }
                else if (json[position] == ',')
                {
                    AwaitingKey = true;
                    position++;
                }
                else
                {
                    if (AwaitingKey)
                    {
                        if (json[position] == '\"')
                        {
                            position = ParseString(json, position, out Key);
                        }
                        else
                        {
                            throw new Exception("Invalid start for property name!");
                        }
                    }
                    else
                    {
                        var value = ParseValue(json, position, false, out object? Value);

                        if (!value.assigned)
                        {
                            if (Value?.GetType() == typeof(object[]) || Value?.GetType() == typeof(Dictionary<string, object>)) { position = value.position + 1; } else { position = value.position; };
                        }
                        else
                        {
                            position = value.position;
                        }

                        if (result.ContainsKey(Key)) throw new Exception("Key already Exists!");

                        result[Key] = Value;
                    }
                }
            }

            if (AwaitingKey && Key is not null)
            {
                throw new Exception($"No value for Key {Key}");
            }

            if (position >= json.Length || json[position] != '}')
            {
                throw new Exception("Object is not properly closed");
            }

            // position++;
            ParsedObject = result;
            return position;
        }
    }
}
