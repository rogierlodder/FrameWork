using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace RGF
{

    /* The purpose of the IO category is to distinguish between values that are measured by the IO system and those that sre measured by external devices like pumps or power supplies */
    public enum IOCategory { Real, Virtual }

    /* An IO is any measured, physical value. They are therefore only floating point of boolean.*/
    [Serializable]
    public abstract class IO<T> : RGOBase where T : struct, IComparable<T>
    {
        //conversion function to convert from Volts to real-world values or back
        public delegate double ExternalConversion(double input);
        //[NonSerialized]
        public ExternalConversion Convert = p => p;

        protected T _Value;
        public abstract T Value { get; set; }

        protected T _RawValue;
        public abstract T RawValue { get; set; }

        private T _OverrideValue;
        public T OverrideValue {
            get { return _OverrideValue; }
            set { if (value.CompareTo(_OverrideValue) != 0) { _OverrideValue = value; MustSerialize = true; } }
        }

        private bool _IsValid = true;
        public bool IsValid {
            get { return _IsValid; }
            set { if (value != _IsValid) { _IsValid = value; MustSerialize = true; } }
        }

        private bool _OverRide = false;
        public bool OverRide {
            get { return _OverRide; }
            set { if (value != _OverRide) { _OverRide = _IsOverRidden = value; MustSerialize = true; } }
        }

        private bool _UseConvertFunction = true;
        public bool UseConvertFunction {
            get { return _UseConvertFunction; }
            set { if (value != _UseConvertFunction) { _UseConvertFunction = value; MustSerialize = true; } }
        }

        private bool _IsOverRidden = false;
        public bool IsOverRidden
        {
            get { return _IsOverRidden; }
            set { if (value != _IsOverRidden) { _IsOverRidden = value;  MustSerialize = true; } }
        }


        public IOCategory Category { get; private set; } = IOCategory.Real;

        public IO(int modnr, int id, IOCategory cat, string desc) : base(modnr, id, desc)
        {
            MustSerialize = true;
            Category = cat;
        }

        public override void CopyValues(RGOBase newVal)
        {
            if (newVal == null || this == null) return;
            if (RunsOnServer)
            {
                _OverrideValue = (newVal as dynamic)._OverrideValue;
                _OverRide = _IsOverRidden = ((dynamic)newVal)._OverRide;
                _UseConvertFunction = ((dynamic)newVal)._UseConvertFunction;

                if (this is AI) RawValue = _RawValue;
                if (this is AO) Value = _Value;
                if (this is DI) RawValue = _RawValue;
                if (this is DO) Value = _Value;

                MustSerialize = true;
            }
            else 
            {
                (this as dynamic)._Value = (newVal as dynamic)._Value;
                (this as dynamic)._RawValue = (newVal as dynamic)._RawValue;
                (this as dynamic)._IsValid = ((dynamic)newVal)._IsValid;
                (this as dynamic)._IsOverRidden = ((dynamic)newVal)._IsOverRidden;
            }
        }
    }

    [Serializable]
    public class AI : IO<double>
    {
        public override double Value { get { return _Value; } set { throw new NotImplementedException(); } }
        public override double RawValue {
            get { return _RawValue; }
            set
            {
                if (value != _RawValue) MustSerialize = true;
                _RawValue = value;
                if (!OverRide) _Value = Convert(value);
                else
                {
                    if (UseConvertFunction) _Value = Convert(OverrideValue);
                    else _Value = OverrideValue;
                }
            }
        }

        public AI(int modnr, int id, IOCategory cat, string desc) : base(modnr, id, cat, desc) { }

        public override void CopyValues(RGOBase newVal)
        {
            base.CopyValues(newVal);
        }
    }

    [Serializable]
    public class AO : IO<double>
    {
        public override double Value {
            get { return _Value; }
            set
            {
                if (value != _Value) MustSerialize = true;
                _Value = value;
                if (!OverRide) _RawValue = Convert(value);
                else
                {
                    if (UseConvertFunction) _RawValue = Convert(OverrideValue);
                    else _RawValue = OverrideValue;
                }
            }
        }
        public override double RawValue { get { return _RawValue; } set { throw new NotImplementedException(); } }

        public AO(int modnr, int id, IOCategory cat, string desc) : base(modnr, id, cat, desc) { }

        public override void CopyValues(RGOBase newVal)
        {
            base.CopyValues(newVal);
        }
    }

    [Serializable]
    public class DI : IO<bool>
    {
        public override bool Value { get { return _Value; } set { throw new NotImplementedException(); } }
        public override bool RawValue {
            get { return _RawValue; }
            set
            {
                if (value != _RawValue) MustSerialize = true;
                _RawValue = value;
                if (!OverRide) _Value = value;
                else _Value = OverrideValue;
            }
        }
 
        public DI(int modnr, int id, IOCategory cat, string desc) : base(modnr, id, cat, desc) { }

        public override void CopyValues(RGOBase newVal)
        {
            base.CopyValues(newVal);
        }
    }

    [Serializable]
    public class DO : IO<bool>
    {
        public override bool Value {
            get { return _Value; }
            set
            {
                if (value != _Value) MustSerialize = true;
                _Value = value;
                if (!OverRide) _RawValue = value;
                else _RawValue = OverrideValue;
            }
        }
        public override bool RawValue { get { return _RawValue; } set { throw new NotImplementedException(); } }

        public DO(int modnr, int id, IOCategory cat, string desc) : base(modnr, id, cat, desc) { }

        public override void CopyValues(RGOBase newVal)
        {
            base.CopyValues(newVal);
        }
    }
}


