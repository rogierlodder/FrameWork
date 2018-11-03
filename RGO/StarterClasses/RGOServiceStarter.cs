using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RGF
{
    public class RGOServiceStarter : RGOStarterBase
    {
        //Services
        private static RGOServiceServerConn ServerConn;
        private static RGOServiceTransfer RGOConn;
        private static RGOServiceRW RWService;
        private static RGOServiceDescriptions DescriptionService;
        private static RGOServiceNotification Notificationservice;

        public static Action<bool, uint> ConnStateChanged;

        public static int ServerConnBufSize { get; set; } = 500000;
        public static int RGOServiceBufSize { get; set; } = 500000;
        public static int RWServiceBufSize { get; set; } = 500000;
        public static int DescriptionServiceBufSize { get; set; } = 500000;
        public static int NotifServiceBufSize { get; set; } = 500000;

        public static int MeasuredCycleTime { get; set; }

        public static void Run()
        {
            RGOServiceBase.RunAllServices();
        }

        public static void StartServices(int basePortNr)
        {
            RGOBase.RunsOnServer = true;

            if (basePortNr == 0) basePortNr = DefBasePort;
            SetPorts(basePortNr);

            //Start the Services
            ServerConn          = new RGOServiceServerConn  ("ServerCommService", ServerCommServicePort);
            RGOConn             = new RGOServiceTransfer    ("FrameWorkObjectService", FrameWorkObjectServiceport, RGOServiceBufSize);
            RWService           = new RGOServiceRW          ("RWService", RWServicePort, RWServiceBufSize);
            DescriptionService  = new RGOServiceDescriptions("DescriptionService", DescriptionServicePort, DescriptionServiceBufSize);
            Notificationservice = new RGOServiceNotification("NotificationService", NotificationServicePort, NotifServiceBufSize);

            ServerConn.ConnStateChanged = ConnStateChanged;
        }
    }
}
