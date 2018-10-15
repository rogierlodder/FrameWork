using System;
using System.Collections.Generic;

namespace RGF
{
    public enum ServerPorts
    {
        ServerCommService = 45010,
        FrameWorkObjectService = 45011,
        RWService = 45012,
        DebugMessageService = 45013,
        DescriptionService = 45014,
        NotificationService = 45015
    }

    public abstract class GTLClientBase 
    {
        public static uint ClientID { get; protected set; }

        public string Name { get; protected set; }

        public static List<GTLClientBase> AllClients { get; protected set; } = new List<GTLClientBase>();

        public abstract void Run();
    }
}
