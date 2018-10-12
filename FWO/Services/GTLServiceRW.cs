
namespace FWO
{
    public class GTLServiceRW : GTLService<RequestRW, ReplyRW>
    {
        public GTLServiceRW(string name, int portNr, int bufferSize) : base(name, portNr, bufferSize)
        {
            Request = new RequestRW();
            Reply = new ReplyRW();
        }


        public override bool ProcessData()
        {
            foreach (var F in Request.WriteList) FWOBase.AllFWO[F.ID].CopyValues(F);

            Reply.FWOList.Clear();
            foreach (var I in Request.ReqList)
            {
                if (FWOBase.AllFWO[I].MustSerialize)
                {
                    FWOBase.AllFWO[I].MustSerialize = false;
                    Reply.FWOList.Add(FWOBase.AllFWO[I]);
                }
            }

            return true;
        }
    }
}
