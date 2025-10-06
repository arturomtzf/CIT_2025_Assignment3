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

        public bool ParseUrl(string url, string method)
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
            if (split[1] == "api" && split[2] == "xxx" && (method == "create" || method == "update"))
            {
                return true;
            }
            return false;
        }
    }
}
