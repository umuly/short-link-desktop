using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Models
{
    public class MResponseBase<T>
    {
        public string type { get; set; }
        public string title { get; set; }
        public int status { get; set; }
        public string statusText { get; set; }
        public string traceId { get; set; }
        public T item { get; set; }
        public int itemCount { get; set; }
        public int skipCount { get; set; }
        public DateTime requestDate { get; set; }
        public DateTime responseDate { get; set; }
        public Dictionary<string,string[]> errors { get; set; }
    }
}
