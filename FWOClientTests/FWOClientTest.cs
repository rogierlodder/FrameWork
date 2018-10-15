using RGO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using log4net;

namespace FWOClientTests
{
    public class FWOClientTest
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        GTLClientRW FWORWClient;

        void CreateWriteList()
        {
            foreach (var F in FWORWClient.Request.WriteList) F.MustSerialize = true;
        }

        async void StartApplication()
        {
            FWOClientManager.StartClients(45010, "127.0.0.1");

            await FWOClientManager.WaitForClientConnectTask();
            log.Info("Client connected to server");

            FWORWClient = new GTLClientRW("127.0.0.1", "GTLRWService");
            for (int i = 10000; i < 10000 + 100; i++) FWORWClient.Request.ReqList.Add(i * 100);

            FWORWClient.Start();
            FWORWClient.CreateWriteList = CreateWriteList;

            await FWOClientManager.WaitForClientDisconnectTask();
            log.Info("Connection to server lost");
        }

        static void Main(string[] args)
        {
            log.Info("Application started");
            FWOClientTest p = new FWOClientTest();

            p.StartApplication();
            Console.ReadLine();
        }
    }
}
