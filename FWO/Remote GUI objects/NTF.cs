using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RGO
{
    public enum ErrLevel { Notification, Warning, Error }

    [Serializable]
    public class NTF : RGOBase
    {
        private string _Issuer;
        public string Issuer { get { return _Issuer; } }

        private string _ErrorText;
        public string ErrorText { get { return _ErrorText; } }

        private string _addedtext;
        public string AddedText { get { return _addedtext; } set { } }

        private DateTime _TimeStamp;
        public DateTime TimeStamp { get { return _TimeStamp; } }

        private bool _isRaised;
        public bool IsRaised {
            get { return _isRaised; }
            set { if (value != _isRaised) _isRaised = value; MustSerialize = true; }
        }

        private bool _Acknowledged;
        public bool Acknowledged { get { return _Acknowledged; } set { } }

        private ErrLevel _ErrorLevel;
        public ErrLevel ErrorLevel { get { return _ErrorLevel; } }

        private string _Options;
        public string Options { get { return _Options; } }

        private int _userReply;
        public int UserReply { get { return _userReply; } set { } }

        public override void CopyValues(RGOBase newVal)
        {
            NTF ntfTemp = newVal as dynamic;
            if (RunsOnServer)
            {
                _userReply = ntfTemp._userReply;
                _Acknowledged = ntfTemp._Acknowledged;
            }
            else //runs on GUI
            {
                _TimeStamp = ntfTemp._TimeStamp;
                _isRaised = ntfTemp._isRaised;
                _addedtext = ntfTemp.AddedText;
            }
        }

        public NTF(int modnr, int id, string issuer, ErrLevel level, string errortext, string options, string desc) : base(modnr, id, desc)
        {
            ModNr = modnr;
            _Issuer = issuer;
            _ErrorLevel = level;
            _ErrorText = errortext;
            _addedtext = " "; //should not be null

            _Options = options;
            _userReply = 0;
            _Acknowledged = false;
            _isRaised = false;
        }

        public void Raise()
        {
            if (_isRaised == true) return; //an error can not be raised/submitted to the GUI more than once
            _isRaised = true;
            _Acknowledged = false;
            _userReply = 0;
            _TimeStamp = DateTime.Now;
            MustSerialize = true;
        }

        public void Raise(string addedtext)
        {
            _addedtext = addedtext;
            Raise();
        }

        public void Lower()
        {
            if (_isRaised == false) return; //unraised errors can not be lowered / submitted to the GUI
            _isRaised = false;
            _userReply = 0;
            _Acknowledged = false;
            MustSerialize = true;
        }

    }
}
