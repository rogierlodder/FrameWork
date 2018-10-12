using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FWO
{
    [Serializable]
    public class GuiCMD : FWOBase
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

        public GuiCMD(int modnr, int id, CMDDelegate cmd, string desc) : base(modnr, id, desc)
        {
            _Args = "";
            Command = cmd;
        }

        public override void CopyValues(FWOBase newVal)
        {
            if (RunsOnServer)
            if ((newVal as GuiCMD).Args != "") Command?.Invoke((newVal as GuiCMD).Args);

            _CanExecute = ((GuiCMD)newVal)._CanExecute;
        }
    }
}
