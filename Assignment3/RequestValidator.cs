using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Assignment3
{
    public class RequestValidator
    {
        public static UrlParser urlParser = new UrlParser();
        public CategoryService categoryService;
        public void setCategoryService(CategoryService categoryService)
        {
            this.categoryService = categoryService;
        }
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
            
            string parseUrlResponse = urlParser.ParseUrl(request.Path, request.Method);
            if (parseUrlResponse != "true") 
            {
                response.Status = parseUrlResponse;
                return response;
            }

            if(!ValidateUnixTimestamp(request.Date)) return null;

            response.Status = "1 Ok";
            if(categoryService != null)
                response = connectToCategory(request, response);

            return response;
        }
        public Response connectToCategory(Request request, Response response)
        {
            if (request.Path.StartsWith("/api"))
            {
                if(request.Path == "/api/categories")
                {
                    if(request.Method == "read")
                    {
                        List<Category> categories = categoryService.GetCategories();
                        response.Body = parseToJSON(categories);
                        return response;
                    }
                    if(request.Method == "create")
                    {
                        Category messageCategory = parseFromJSON(request.Body);

                        int newID = categoryService.NextSequenceID();
                        bool isCreated = categoryService.CreateCategory(newID, messageCategory.Name);

                        Category createdCategory = categoryService.GetCategory(newID);
                        if (isCreated)
                        {
                            response.Status = "2 Created";
                            response.Body = parseToJSON(createdCategory);
                            return response;
                        }
                        else
                        {
                            response.Status = "6 Internal Error";
                            return response;
                        }
                    }
                }
                if(request.Path.StartsWith("/api/categories/"))
                {
                    if(request.Method == "read")
                    {
                        Category category = categoryService.GetCategory(urlParser.Id);
                        if (category == null)
                        {
                            response.Status = "5 not found";
                            return response;
                        }
                        response.Body = parseToJSON(category);
                        return response;
                    }
                    if(request.Method == "update")
                    {
                        Category messageCategory = parseFromJSON(request.Body);

                        bool isUpdated = categoryService.UpdateCategory(urlParser.Id, messageCategory.Name);

                        if (isUpdated)
                        {
                            response.Status = "3 updated";
                            return response;
                        }
                        else
                        {
                            response.Status = "5 not found";
                            return response;
                        }
                    }
                    if(request.Method == "delete")
                    {
                        bool isDeleted = categoryService.DeleteCategory(urlParser.Id);

                        if (isDeleted)
                        {
                            response.Status = "1 ok";
                            return response;
                        }
                        else
                        {
                            response.Status = "5 not found";
                            return response;
                        }
                    }
                }
            }
            return response;
        }
        public static string parseToJSON(List<Category> categories)
        {
            var categoryAsJson = JsonSerializer.Serialize<List<Category>>(categories
                        , new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
            return categoryAsJson;
        }
        public static string parseToJSON(Category category)
        {
            var categoryAsJson = JsonSerializer.Serialize<Category>(category
                        , new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
            return categoryAsJson;
        }
        public static Category parseFromJSON(string json)
        {
            Category category = JsonSerializer.Deserialize<Category>(json
                , new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
            return category;
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
