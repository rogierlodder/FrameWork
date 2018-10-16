using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RGF
{
    public class RGOStarterBase
    {
        public const int DefBasePort = 45010;

        public static int ServerCommServicePort { get; protected set; } = 0;
        public static int FrameWorkObjectServiceport { get; protected set; } = 1;
        public static int RWServicePort { get; protected set; } = 2;
        public static int DescriptionServicePort { get; protected set; } = 4;
        public static int NotificationServicePort { get; protected set; } = 5;

        protected static void SetPorts(int basePortNr)
        {
            ServerCommServicePort += basePortNr;
            FrameWorkObjectServiceport += basePortNr;
            RWServicePort += basePortNr;
            DescriptionServicePort += basePortNr;
            NotificationServicePort += basePortNr;
        }
    }
}
