using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RGF
{
    [Serializable]
    public class ARRAY<T> : RGOBase where T: IComparable<T>
    {
        private T[] _Value;
        public T this[int index] {
            get { return _Value[index]; }
            set { if (value.CompareTo(_Value[index]) != 0) { _Value[index] = value; MustSerialize = true; } }
        }

        public ARRAY(int modnr, string id, int size, string desc) : base(modnr, id, desc)
        {
            _Value = new T[size];
        }

        public override void CopyValues(RGOBase newVal)
        {
            if (newVal == null || _Value == null) return;
            for (int i = 0; i < _Value.Length; i++) _Value[i] = (newVal as dynamic)[i];
        }
    }
}
