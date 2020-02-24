using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Model
{
    public class Object
    {
        public string objectsUid { get; set; }

        public string modelId { get; set; }

        public string link { get; set; }

        public State state { get; set; }
    }
}
