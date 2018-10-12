using System.Linq;
using System.Collections.Generic;

namespace FWO
{
    public class GTLServiceNotification : GTLServiceRW
    {
        public List<FWOBase> AllNTF = new List<FWOBase>();

        public GTLServiceNotification(string name, int portNr, int bufferSize) : base(name, portNr, bufferSize)
        {
            AllNTF = FWOBase.AllFWO.Values.Where(p => p is NTF).ToList();
        }


        public override bool ProcessData()
        {
            foreach (var F in Request.WriteList) FWOBase.AllFWO[F.ID].CopyValues(F);

            Reply.FWOList.Clear();
            foreach (var N in AllNTF)
            {
                if (N.MustSerialize == true)
                {
                    N.MustSerialize = false;
                    Reply.FWOList.Add(N);
                }
            }

            return true;
        }
    }
}
