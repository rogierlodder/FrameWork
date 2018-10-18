
namespace RGF
{
    public class RGOServiceRW : RGOService<RequestRW, ReplyRW>
    {
        public RGOServiceRW(string name, int portNr, int bufferSize) : base(name, portNr, bufferSize)
        {
            Request = new RequestRW();
            Reply = new ReplyRW();
        }


        public override bool ProcessData()
        {
            foreach (var F in Request.WriteList) RGOBase.AllRGO[F.ID].CopyValues(F);

            Reply.FWOList.Clear();
            foreach (var I in Request.ReqList)
            {
                if (RGOBase.AllRGO.ContainsKey(I)) 
                {
                    if (RGOBase.AllRGO[I].MustSerialize)
                    {
                        RGOBase.AllRGO[I].MustSerialize = false;
                        Reply.FWOList.Add(RGOBase.AllRGO[I]);
                    }
                }
            }

            return true;
        }
    }
}
