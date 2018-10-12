using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FWO
{
    public class FWOServiceStarter : FWOStarterBase
    {
        //Services
        private static GTLServiceServerConn ServerConn;
        private static GTLServiceFWO FWOConn;
        private static GTLServiceRW RWService;
        private static GTLServiceDescriptions DescriptionService;
        private static GTLServiceNotification Notificationservice;

        public delegate void ConnStateChangedDelegate(bool connected, uint ID);
        public static ConnStateChangedDelegate ConnStateChanged;

        public static int ServerConnBufSize { get; set; } = 500000;
        public static int FWOServiceBufSize { get; set; } = 500000;
        public static int RWServiceBufSize { get; set; } = 500000;
        public static int DebugServiceBufSize { get; set; } = 500000;
        public static int DescriptionServiceBufSize { get; set; } = 500000;
        public static int NotifServiceBufSize { get; set; } = 500000;

        public static int MeasuredCycleTime { get; set; }

        public static void Run()
        {
            GTLServiceBase.RunAllServices();
        }

        public static void StartServices(int basePortNr)
        {
            FWOBase.RunsOnServer = true;

            if (basePortNr == 0) basePortNr = DefBasePort;
            SetPorts(basePortNr);

            //Start the Services
            ServerConn = new GTLServiceServerConn("ServerCommService", ServerCommServicePort);
            FWOConn = new GTLServiceFWO("FrameWorkObjectService", FrameWorkObjectServiceport, FWOServiceBufSize);
            RWService = new GTLServiceRW("RWService", RWServicePort, RWServiceBufSize);
            DescriptionService = new GTLServiceDescriptions("DescriptionService", DescriptionServicePort, DescriptionServiceBufSize);
            Notificationservice = new GTLServiceNotification("NotificationService", NotificationServicePort, NotifServiceBufSize);
        }

        
    }
}
