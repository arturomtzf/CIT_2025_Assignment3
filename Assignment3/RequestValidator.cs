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
            if (request.Date == null)
            {
                response.Status = "missing date";
                return response;
            }
            if(!ValidateUnixTimestamp(request.Date))
            {
                response.Status = "illegal date";
                return response;

            }
            if (request.Body == null)
            {
                response.Status = "missing body";
                return response;
            }
            if(isValidJson(request.Body))
            {
                response.Status = "1 Ok";
                return response;
            }

                UrlParser urlParser = new UrlParser();
            if (!urlParser.ParseUrl(request.Path)) return null;

            if(!ValidateUnixTimestamp(request.Date)) return null;

            return response;
        }
        public static bool isValidJson (string input)
        {
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
