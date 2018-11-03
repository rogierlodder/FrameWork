using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RGF
{
    public class ObjectID
    {
        public string ID { get; }
        public ElementTypeEnum Type { get; }

        public ObjectID(string id, ElementTypeEnum type)
        {
            ID = id;
            Type = type;
        }
    }
}
