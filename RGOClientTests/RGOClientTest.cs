using RGF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using log4net;

namespace RGOClientTests
{
    public class RGOClientTest
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        RGOClientManager RGOMan = new RGOClientManager();

        RGOClientRW RGORWClient;

        void CreateWriteList()
        {
            foreach (var F in RGORWClient.Request.WriteList) F.MustSerialize = true;
        }

        async void StartApplication()
        {
            RGOClientManager.StartClients(45010, "127.0.0.1", 200, true);

            await RGOMan.WaitForClientConnectTask();
            log.Info("Client connected to server");

            RGORWClient = new RGOClientRW("127.0.0.1", "RGORWService");
            for (int i = 10000; i < 10000 + 100; i++) RGORWClient.Request.ReqList.Add($"{i}0");

            RGORWClient.Connect();
            RGORWClient.CreateWriteList = CreateWriteList;

            await RGOMan.WaitForClientDisconnectTask();
            log.Info("Connection to server lost");
        }

        static void Main(string[] args)
        {
            log.Info("Application started");
            RGOClientTest p = new RGOClientTest();

            p.StartApplication();
            Console.ReadLine();
        }
    }
}
