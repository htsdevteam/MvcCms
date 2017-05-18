using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvcCms.Data
{
    public interface ITagRepository
    {
        IEnumerable<string> GetAll();
        void Edit(string existingTag, string newTag);
        void Delete(string tag);
        string Get(string tag);
    }
}
