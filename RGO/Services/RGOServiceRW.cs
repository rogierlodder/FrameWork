
namespace RGF
{
    public class RGOServiceRW : RGOService<RequestRW, ReplyRW>
    {
        public RGOServiceRW(string name, int portNr, int bufferSize) : base(name, portNr, bufferSize)
        {

        }


        public override ReplyRW ProcessData(RequestRW Request)
        {
            var Reply = new ReplyRW();
            foreach (var F in Request.WriteList) RGOBase.AllRGO[F.ID].CopyValues(F);

            Reply.RGOList.Clear();
            foreach (var I in Request.ReqList)
            {
                if (RGOBase.AllRGO.ContainsKey(I)) 
                {
                    if (RGOBase.AllRGO[I].MustSerialize)
                    {
                        RGOBase.AllRGO[I].MustSerialize = false;
                        Reply.RGOList.Add(RGOBase.AllRGO[I]);
                    }
                }
            }

            return Reply;
        }
    }
}
