using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace RGF
{
    public enum Units { none, s, mm, mBar, minutes, W, kW, SCCM, SLM, A, mA, mm_per_s, mm_per_s_square, RPM }

    [Serializable]
    public abstract class RGOBase
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        [NonSerialized]
        public static Action<string> initDebug;
        protected void Debug(string msg) { initDebug?.Invoke(msg); }

        [NonSerialized]
        public Action Update;
  

        public string ID { get; private set; }
        public int ModNr { get; protected set; }

        [NonSerialized]
        private bool _MustSerialize;
        public bool MustSerialize {
            get { return _MustSerialize; }
            set
            {
                _MustSerialize = value;
                if (RunsOnServer) foreach (var id in MustSerializeDict.ToArray()) MustSerializeDict[id.Key] = value;
            }
        } 

        public bool ClientMustSerialize(uint ID)
        {
            if (MustSerializeDict[ID])
            {
                MustSerializeDict[ID] = false;
                return true;
            }
            else return false;
        }

        [NonSerialized]
        public Dictionary<uint, bool> MustSerializeDict = new Dictionary<uint, bool>();

        public static bool RunsOnServer { get; set; } = true;

        [NonSerialized]
        public string Description;

        static public Dictionary<string, RGOBase> AllRGO = new Dictionary<string, RGOBase>();

        /// <summary>
        /// Contructor
        /// </summary>
        /// <param name="modnr"></param>
        /// <param name="id"></param>
        /// <param name="desc"></param>
        public RGOBase(int modnr, string id, string desc)
        {
            ModNr = modnr;
            ID = CalcID(modnr, id);
            Description = desc;
            AddToFWOList();
        }

        public static void AddClientID(uint ID)
        {
            foreach (var F in AllRGO.Values) F.MustSerializeDict.Add(ID, true);
        }

        public static void RemoveClientID(uint ID)
        {
            foreach (var F in AllRGO.Values) F.MustSerializeDict.Remove(ID);
        }


        /// <summary>
        /// Clear the RGO list. Used by the GUI
        /// </summary>
        public static void ClearAll()
        {
            AllRGO.Clear();
        }

        /// <summary>
        /// Get a reference to an RGO object from the full ID
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public static RGOBase GetFromID(string ID)
        {
            if (AllRGO.ContainsKey(ID)) return AllRGO[ID]; else return null;
        }

        public static RGOBase GetFromID(int modnr, string ID)
        {
            if (AllRGO.ContainsKey(CalcID(modnr,  ID))) return AllRGO[CalcID(modnr, ID)]; else return null;
        }

        public static string CalcID(int modnr, string id)
        {
            return $"{id}{modnr}";
        }


        /// <summary>
        /// Add a new RGO to the static list
        /// </summary>
        public void AddToFWOList()
        {
            if (AllRGO.ContainsKey(ID))
            {
                throw new Exception($"A FrameWorkVariable with this key {ID} already exists");
            }
            else
            {
                AllRGO.Add(ID, this);
            }
        }

        /// <summary>
        /// Copy the relevant values from a deserialized object to the object in the AllFWO list with the same ID
        /// </summary>
        /// <param name="newVal"></param>
        public abstract void CopyValues(RGOBase newVal);

        /// <summary>
        /// Get the value of the RGO as a string
        /// </summary>
        /// <returns></returns>
        public string GetValueAsString()
        {
            if (this == null) return "";
            try
            {
                return (this as dynamic).Value.ToString();
            }
            catch { return ""; }
        }

        /// <summary>
        /// Get the value of a RGO values as a string when it is known that the RGO has a double type
        /// </summary>
        /// <param name="nrDigits"></param>
        /// <returns></returns>
        public string GetValueAsString(int nrDigits)
        {
            if (this == null) return "";
            try
            {
                return Math.Round((double)((this as dynamic).Value), nrDigits).ToString();
            }
            catch { return ""; }
        }


        /// <summary>
        /// Sets the MustSerialize property to true in case the GUI connects. Whithout this, a reconnecting GUI would not see the values of parameters that are not being updated
        /// </summary>
        public static void SetAllToMustSerialize()
        {
            foreach (var F in AllRGO) F.Value.MustSerialize = true;
        }

  
    }
}
