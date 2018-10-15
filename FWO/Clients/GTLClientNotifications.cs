
using System;
using System.Threading.Tasks;

namespace RGF
{
    public class GTLClientNotifications : GTLClientRW
    {
        bool NotifReceived = false;

        public GTLClientNotifications(string IPAddress, string serviceName) : base(IPAddress, serviceName)
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
