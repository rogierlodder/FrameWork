using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace FWO
{
    //The EQP class
    //EQPs are used by the server.They have default values, but the GUI can overwrite the value.On startup, the server loads the new values set by the GUI.

    [Serializable]
    public class EQP<T> : FWOBase where T : struct, IComparable<T>
    {
        private T _Value;
        public T Value
        {
            get { return _Value; }
            set
            {
                OldVal = _Value;
                if (value.CompareTo(_Value) != 0 && value.CompareTo(Min) >= 0 && value.CompareTo(Max) <= 0)
                {
                    _Value = value;
                    MustSerialize = true;
                }
            }
        }
        private T OldVal;

        public string SubSys { get; private set; }
        public string ParName { get; private set; }
        public T Min { get; private set; }
        public T Max { get; private set; }
        public T Default { get; private set; }
        public Units Unit { get; private set; }

        public EQP(int modnr, int id, string subSys, string parName, T def, T min, T max, Units unit, string desc) : base(modnr, id, desc)
        {
            SubSys = subSys;
            ParName = parName;
            Min = min;
            Max = max;
            Unit = unit;

            _Value = Default = def;
        }

        public override void CopyValues(FWOBase newVal)
        {
            if (newVal == null || this == null) return;

            //new EQP values must be acknowledged
            if ((this as dynamic)._Value != ((dynamic)newVal)._Value)
            {
                (this as dynamic)._Value = ((dynamic)newVal)._Value;
                MustSerialize = true;
            }
        }

        public void SetValueFromString(string value)
        {
            if (this is EQP<byte>) (this as EQP<byte>).Value = Convert.ToByte(value);
            if (this is EQP<int>) (this as EQP<int>).Value = Convert.ToInt32(value);
            if (this is EQP<double>) (this as EQP<double>).Value = Convert.ToDouble(value);
            if (this is EQP<bool>) (this as EQP<bool>).Value = Convert.ToBoolean(value);
            if (this is EQP<long>) (this as EQP<long>).Value = Convert.ToInt64(value);
            if (this is EnumEQP) (this as EnumEQP).Value = Convert.ToByte(value);
        }

        public EQPHistData GetEQPHistData()
        {
            EQPHistData hd = new EQPHistData
            {
                ID = this.ID,
                Dt = DateTime.Now,
                SubSys = this.SubSys,
                ParName = this.ParName,
                NewValue = Value.ToString(),
                OldValue = OldVal.ToString(),

                Type = typeof(T).ToString(),
                Enums = ""
            };
            if (this is EnumEQP) hd.Enums = (this as EnumEQP).Enums;

            return hd;
        }
    }

    [Serializable]
    public class EnumEQP : EQP<byte>
    {
        public string Enums { get; private set; }

        public EnumEQP(int modnr, int id, string subSys, string parName, byte val, string enums, Units unit, string desc) : base(modnr, id, subSys, parName, val, 0, 255, unit, desc)
        {
            if (enums != "") Enums = enums.Replace(" ", "");
        }
    }

    [Serializable]
    public class EQPHistData
    {
        public int ID { get; set; }

        public DateTime Dt { get; set; }

        public string SubSys { get;  set; }
        public string ParName { get;  set; }
        public string NewValue { get;  set; }
        public string OldValue { get;  set; }

        public string Type { get;  set; }

        public string Enums { get;  set; }

        public EQPHistData() { }

        public string GetSerializationString()
        {
            XmlSerializer xmlSer = new XmlSerializer(this.GetType());
            using (StringWriter swriter = new StringWriter())
            {
                xmlSer.Serialize(swriter, this);
                string S = swriter.ToString();
                S = S.Replace("\r\n", "");
                return S;
            }
        }

        public static EQPHistData GetInstance(string S)
        {
            XmlSerializer xmlSer = new XmlSerializer(typeof(EQPHistData));
            TextReader tread = new StringReader(S);
            return  (EQPHistData)xmlSer.Deserialize(tread);
        }
    }
}
