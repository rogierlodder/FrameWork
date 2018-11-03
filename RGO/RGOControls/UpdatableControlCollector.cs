using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RGF
{
    public enum ElementTypeEnum { Read, Write, ReadWrite }

    public static class UpdatableControlCollector
    {
        //static collection of all controls used in the scope
        public static List<IUpdatableControl> AllControls { get; set; } = new List<IUpdatableControl>();
    }
}
