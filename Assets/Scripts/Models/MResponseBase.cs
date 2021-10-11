using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Models
{
    public class MResponseBase<T>
    {
        public int status { get; set; }
        public string statusText { get; set; }
        public T item { get; set; }
        public int itemCount { get; set; }
        public int skipCount { get; set; }
        public DateTime requestDate { get; set; }
        public DateTime responseDate { get; set; }
        public Errors errors { get; set; }
    }

    public class Errors
    {
        public List<string> Name { get; set; }
        public List<string> Email { get; set; }
        public List<string> Password { get; set; }
    }
}
