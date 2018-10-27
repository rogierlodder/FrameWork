using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RGF;
using System.Windows.Threading;
using System.Net;
using System.Diagnostics;

namespace FWOTestingApplication
{
    class RGOServiceTest
    {
        log4net.ILog log = log4net.LogManager.GetLogger("FWOServiceTest");

        const int ServiceCycleInterval = 1;
        private int ServiceCycleCounter = 0;

        public void CreateRGO(int nr)
        {
            int id = 10000;
            for (int i=0; i < nr; i ++)
            {
                var sv1 = new SV<double>(0, id++.ToString(), "The quick brown fox jumped over the lazy dog");
            }

            for (int i = 0; i < nr; i++)
            {
                var stp1 = new STP<double>(0, id++.ToString(), "This is an unused description");
            }

            for (int i = 0; i < nr; i++)
            {
                var ai = new AI(5, id++.ToString(), IOCategory.Real, "Test description that has no meaning");
                var ao = new AO(5, id++.ToString(), IOCategory.Real, "Test description that has no opinion");
                var di = new DI(5, id++.ToString(), IOCategory.Real, "Test description that has no purpose");
                var d0 = new DO(5, id++.ToString(), IOCategory.Real, "Test description that has no direction");
            }

            for (int i = 0; i < nr; i++)
            {
                var EQP1 = new EQP<double>(6, id++.ToString(), "VacuumSystem", "PumpTime", 5.5, 0, 10.3, Units.s, "This is a default description");
                var EQP2 = new EQP<int>(6, id++.ToString(), "Chuckdrive", "Travel", 5, 0, 10, Units.s, "This is a default description");
                var EQP3 = new EQP<bool>(6, id++.ToString(), "Source", "Rotationspeed", true, false, true, Units.s, "This is a default description");
            }
        }

        private void ConnStateChanged(bool state, uint ID)
        {
            if (state) log.Debug($"Client {ID} connected");
            else log.Debug($"Client {ID} disconnected");
        }

        public async void RunServices()
        {
            log.Debug($"Started servers: {RGOServiceBase.AllServers.Count.ToString()}");

            double counter = 0;
            while (true)
            {
                if (ServiceCycleCounter >= ServiceCycleInterval)
                {
                    ServiceCycleCounter = 0;
                    RGOServiceStarter.Run();

                    for (int i = 10000; i < 10000 + 100; i++) (RGOBase.AllRGO[$"{i}0"] as SV<double>).Value = counter;
                    counter += 0.1;
                }
                else ServiceCycleCounter++;

                await Task.Delay(1);
            }
        }

        public void StartServices()
        {
            //create RGO objects for testing
            CreateRGO(200);

            //Start the Services
            RGOServiceStarter.ConnStateChanged = ConnStateChanged;
            RGOServiceStarter.StartServices(basePortNr: 45010);
        }



        static void Main(string[] args)
        {
            RGOServiceTest p = new RGOServiceTest();

            p.StartServices();
            p.RunServices();

            Console.ReadLine();
        }
    }
}
