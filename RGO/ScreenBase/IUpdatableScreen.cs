using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RGF
{
    interface IUpdatableScreen
    {
        void StartUpdating();
        void StopUpdating();
        int ModNr { get; set; }
        string Key { get; set; }
        bool IsPermanent { get; set; }
        void Setup();
    }
}
