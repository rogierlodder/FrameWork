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

    public abstract class RGOClientBase 
    {
        public static uint ClientID { get; protected set; }

        public string Name { get; protected set; }

        public static List<RGOClientBase> AllClients { get; protected set; } = new List<RGOClientBase>();

        public abstract bool Run();

        public abstract void Disconnect();
    }
}
