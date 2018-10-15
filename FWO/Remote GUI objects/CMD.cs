using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RGO
{
    [Serializable]
    public class CMD : RGOBase
    {
        public delegate void CMDDelegate(string args);
        [NonSerialized]
        public CMDDelegate Command;

        public string _Args;
        public string Args { get { return _Args; } set { _Args = value; MustSerialize = true; } }

        private bool _CanExecute = true;
        public bool CanExecute {
            get { return _CanExecute; }
            set { if (value != _CanExecute) { _CanExecute = value; MustSerialize = true; } }
        }

        public CMD(int modnr, int id, CMDDelegate cmd, string desc) : base(modnr, id, desc)
        {
            _Args = "";
            Command = cmd;
        }

        public override void CopyValues(RGOBase newVal)
        {
            if (RunsOnServer)
            if ((newVal as CMD).Args != "") Command?.Invoke((newVal as CMD).Args);

            _CanExecute = ((CMD)newVal)._CanExecute;
        }
    }
}
