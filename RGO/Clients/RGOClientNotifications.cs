
using System;
using System.Threading.Tasks;

namespace RGF
{
    public class RGOClientNotifications : RGOClientRW
    {
        bool NotifReceived = false;

        public RGOClientNotifications(string IPAddress, string serviceName) : base(IPAddress, serviceName)
        {
            if (ClientID == 0)
            {
                throw new Exception("The client ID is undefined");
            }
            Request.ClientID = ClientID;
        }

        public Task AllNotifReceived()
        {
            return Task.Run(() => 
            {
                while (NotifReceived == false) Task.Delay(100);
            });
        }

        protected override void CreateRequest()
        {
            
        }
    }
}
