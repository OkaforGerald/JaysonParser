using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace JaysonParser
{
    public class Test
    {
        public static void Main(string[] args)
        {
            // [61,\"Test\", null, [65, 78, [3], [false]]]
            //string json = "{\r\n        \"Image\": {\r\n            \"Width\":  800,\r\n            \"Height\": 600,\r\n            \"Title\":  \"View from 15th Floor\",\r\n            \"Thumbnail\": {\r\n                \"Url\":    \"http://www.example.com/image/481989943\",\r\n                \"Height\": 125,\r\n                \"Width\":  100\r\n            },\r\n            \"Animated\" : false,\r\n            \"IDs\": [116, 943, 234, 38793]\r\n, \"Omo\": [9,7, null, true]         }\r\n      }";
            //var c = JaysonParser.Parse(json);

            var e = JaysonParser.Parse("{\"omo\": 8}");
            var d = JsonNode.Parse("[\r\n        {\r\n           \"precision\": \"zip\",\r\n           \"Latitude\":  37.7668,\r\n           \"Longitude\": -122.3959,\r\n           \"Address\":   \"\",\r\n           \"City\":      \"SAN FRANCISCO\",\r\n           \"State\":     \"CA\",\r\n           \"Zip\":       \"94107\",\r\n           \"Country\":   \"US\"\r\n        },\r\n        {\r\n           \"precision\": \"zip\",\r\n           \"Latitude\":  37.371991,\r\n           \"Longitude\": -122.026020,\r\n           \"Address\":   \"\",\r\n           \"City\":      \"SUNNYVALE\",\r\n           \"State\":     \"CA\",\r\n           \"Zip\":       \"94085\",\r\n           \"Country\":   \"US\"\r\n        }\r\n      ]");
            //Console.WriteLine(c);
            Console.WriteLine(d);
        }
    }
}
