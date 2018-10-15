using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RGO
{
    /* The Setpoint class
     * The purpose of this class is to send values and setpoint from the GUI to the Server
     */
    [Serializable]
    public class STP<T> : RGOBase where T : struct, IComparable<T>
    {
        private T _Value;
        public T Value {
            get { return _Value; }
            set { if (value.CompareTo(_Value) != 0) { _Value = value; MustSerialize = true; } }
        }

        private bool _CanSetValue = true;
        public bool CanSetValue {
            get { return _CanSetValue; }
            set { if (_CanSetValue != value) { _CanSetValue = value; MustSerialize = true; } }
        }

        public STP(int modnr, int id, string desc) : base(modnr, id, desc) { }

        public override void CopyValues(RGOBase newVal)
        {
            if (newVal == null || this == null) return;
            (this as dynamic)._Value = (newVal as dynamic)._Value;
            _CanSetValue = (newVal as dynamic)._CanSetValue;
        }
    }

    [Serializable]
    public class EnumSTP : STP<byte>
    {
        public string OptionsString {get; set;}

        public EnumSTP(int modnr, int id, string desc, string options) : base(modnr, id, desc)
        {
            if (options != "") OptionsString = options.Replace(" ", "");
        }

        public override void CopyValues(RGOBase newVal)
        {
            base.CopyValues(newVal);
        }
    }
}
