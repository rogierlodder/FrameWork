using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FWO
{
    public static class FWOSettings
    {
        public static int LocalBatchsize { get; set; } = 500;
        public static int RemoteBatchsize { get; set; } = 10;

        public static int DisconnectCount { get; set; } = 20;

        public static int maxNrClients { get; set; } = 4;

    }
}
