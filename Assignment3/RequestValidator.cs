using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Assignment3
{
    public class RequestValidator
    {
        private bool ValidateMethod(string method)
        {
            List<string> possibleMethods = new List<string> { "create", "read", "update", "delete", "echo" };

            // Check if the list contains the method
            if (possibleMethods.Contains(method?.ToLower()))
                return true;

            return false;
        }
        private bool ValidateUnixTimestamp(string input)
        {
            if (!long.TryParse(input, out long timestamp))
                return false;

            return true;
        }
        public Response ValidateRequest(Request request)
        {
            Response response = new Response();
            if(request == null)
            {
                response.Status = "empty request";
                return response;
            }
            if(request.Method == "echo" && !string.IsNullOrEmpty(request.Body))
            {
                response.Status = "1 Ok";
                response.Body = request.Body;
                return response;
            }
            if (request.Date == null)
            {
                response.Status = "missing date";
                return response;
            }
            if (request.Method == null)
            {
                response.Status = "missing method";
                return response;
            }
            if(!ValidateMethod(request.Method))
            {
                response.Status = "illegal method";
                return response;
            }
            if(string.IsNullOrEmpty(request.Path))
            {
                response.Status = "missing path";
                return response;
            }
            if(!ValidateUnixTimestamp(request.Date))
            {
                response.Status = "illegal date";
                return response;

            }
            
            if ((request.Method == "create" || request.Method == "update" || request.Method == "echo") &&  request.Body == null)
            {
                response.Status = "missing body";
                return response;
            }
            
            if(request.Body != null && !isValidJson(request.Body, request.Method))
            {
                response.Status = "illegal body";
                return response;
            }
            
            UrlParser urlParser = new UrlParser();
            string parseUrlResponse = urlParser.ParseUrl(request.Path, request.Method);
            if (parseUrlResponse != "true") 
            {
                response.Status = parseUrlResponse;
                return response;
            }

            if(!ValidateUnixTimestamp(request.Date)) return null;

            response.Status = "1 Ok";
            return response;
        }
        public static bool isValidJson (string input, string method)
        {
            if (method == "echo")
            {
                return true;
            }

            try
            {
                JsonDocument.Parse(input);
                return true;
            }
            catch (JsonException)
            {
                return false;
            }

        }
    }
}
