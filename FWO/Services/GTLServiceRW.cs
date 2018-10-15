
namespace RGF
{
    public class GTLServiceRW : RGOService<RequestRW, ReplyRW>
    {
        public GTLServiceRW(string name, int portNr, int bufferSize) : base(name, portNr, bufferSize)
        {
            Request = new RequestRW();
            Reply = new ReplyRW();
        }


        public override bool ProcessData()
        {
            foreach (var F in Request.WriteList) RGOBase.AllFWO[F.ID].CopyValues(F);

            Reply.FWOList.Clear();
            foreach (var I in Request.ReqList)
            {
                if (RGOBase.AllFWO[I].MustSerialize)
                {
                    RGOBase.AllFWO[I].MustSerialize = false;
                    Reply.FWOList.Add(RGOBase.AllFWO[I]);
                }
            }

            return true;
        }
    }
}
