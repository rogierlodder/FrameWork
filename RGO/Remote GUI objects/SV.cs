﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RGF
{
    /* The Status variable class
     * Objects of this class are typically only given values by the server and read by the GUI 
     */

    [Serializable]
    public class SV<T> : RGOBase where T : IComparable<T>
    {
        private T _Value = default(T);
        public T Value {
            get {return _Value; }
            set { if (value.CompareTo(_Value) != 0) { _Value = value; MustSerialize = true; } }
        }

        public SV(int modnr, string id, string desc) : base(modnr, id, desc)
        {
            if (typeof(T) == typeof(string)) (this as SV<string>)._Value = "";
        }

        public override void CopyValues(RGOBase newVal)
        {
            if (newVal == null || this == null) return;
            (this as dynamic)._Value = ((dynamic)newVal)._Value;
        }
    }
}
