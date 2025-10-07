using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignment3
{
    public class CategoryService
    {
        public List<Category> categories = new List<Category> { new Category { Cid=1, Name="Beverages" }, new Category { Cid = 2, Name = "Condiments" }, new Category { Cid = 3, Name = "Confections" } };

        public List<Category> GetCategories()
        {
            return categories;
        }

        public Category? GetCategory(int cid)
        {
            foreach (Category c in categories)
            {
                if (cid == c.Cid) return c;
            }
            return null;
        }

        public bool UpdateCategory(int cid, string newName) // UpdateCategory(1, 'tomatoes')
        {
            foreach (Category c in categories)
            {
                if (cid  == c.Cid)
                {
                    c.Name = newName;
                    return true;
                }
            }
            return false;
        }

        public bool DeleteCategory(int id)
        {
            foreach (Category c in categories)
            {
                if (id == c.Cid)
                {
                    categories.Remove(c);
                    return true;
                }
            }
            return false;
        }

        public bool CreateCategory(int id, string name)
        {
            foreach (Category c in categories)
            {
                if (id == c.Cid) return false;
            }
            categories.Add(new Category { Cid = id, Name = name });
            return true;
        }

        public int NextSequenceID()
        {
            int maxID = -1;
            foreach(Category c in categories)
            {
                if(c.Cid > maxID) maxID = c.Cid;
            }
            return maxID + 1;
        }
    }
}
