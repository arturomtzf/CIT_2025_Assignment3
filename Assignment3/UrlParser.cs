using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignment3
{
    public class UrlParser
    {
        public bool HasId { get; set; }
        public int Id { get; set; }
        public string Path { get; set; }
        public bool ParseUrl(string url)
        {
            var split = url.Split('/');
            if (split[1] == "api" && split[2] == "categories")
            {
                if (split.Length > 3)
                {
                    HasId = true;
                    Id = int.Parse(split[3]);
                    Path = split[0] + "/" + split[1] + "/" + split[2];
                }
                else
                {
                    HasId = false;
                    Path = url;
                }
                return true;
            }
            return false;
        }

        public string ParseUrl(string url, string method)
        {
            string notFound = "5 not found";
            string success = "true";
            string badRequest = "4 bad request";
            var methodsThatRequireID = new List<string> { "update", "delete" };
            if (string.IsNullOrEmpty(url)) return notFound;

            var split = url.Split('/');

            // if (methodsThatRequireID.Contains(method) && split.Length < 4) return "4 bad request";// it doesn't contain id

            if (split[1] == "api" && split[2] == "xxx" && (method == "create" || method == "update"))
            {
                return success;
            }
            if (split[1] == "api" && split[2] == "categories")
            {
                if (methodsThatRequireID.Contains(method) && split.Length == 3) return badRequest;

                if (split.Length > 3) // It contains an id
                {
                    if (method == "create") return badRequest;

                    HasId = true;
                    int tempId;
                    if (int.TryParse(split[3], out tempId))
                    {
                        Id = tempId;
                    }
                    else
                    {
                        return badRequest;
                    }
                    Path = split[0] + "/" + split[1] + "/" + split[2];
                }
                else
                {
                    HasId = false;
                    Path = url;
                }
                return success;
            }
            return notFound;
        }
    }
}
