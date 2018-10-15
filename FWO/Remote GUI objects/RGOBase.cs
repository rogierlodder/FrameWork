using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace RGO
{
    public enum Units { none, s, mm, mBar, minutes, W, kW, SCCM, SLM, A, mA, mm_per_s, mm_per_s_square, RPM }

    [Serializable]
    public abstract class RGOBase
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public delegate void InitdebugOutput(string msg);
        [NonSerialized]
        public static InitdebugOutput initDebug;
        protected void Debug(string msg) { initDebug?.Invoke(msg); }

        public delegate void UpdateDelegate();
        [NonSerialized]
        public UpdateDelegate Update;
  

        public int ID { get; private set; }
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

        static public Dictionary<int, RGOBase> AllFWO = new Dictionary<int, RGOBase>();

        /// <summary>
        /// Contructor
        /// </summary>
        /// <param name="modnr"></param>
        /// <param name="id"></param>
        /// <param name="desc"></param>
        public RGOBase(int modnr, int id, string desc)
        {
            ModNr = modnr;
            ID = CalcID(modnr, id);
            Description = desc;
            AddToFWOList();
        }

        public static void AddClientID(uint ID)
        {
            foreach (var F in AllFWO.Values) F.MustSerializeDict.Add(ID, true);
        }

        public static void RemoveClientID(uint ID)
        {
            foreach (var F in AllFWO.Values) F.MustSerializeDict.Remove(ID);
        }


        /// <summary>
        /// Clear the FWO list. Used by the GUI
        /// </summary>
        public static void ClearAll()
        {
            AllFWO.Clear();
        }

        /// <summary>
        /// Get a reference to an FWO object from the full ID
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public static RGOBase GetFromID(int ID)
        {
            if (AllFWO.ContainsKey(ID)) return AllFWO[ID]; else return null;
        }

        public static RGOBase GetFromID(int modnr, int ID)
        {
            if (AllFWO.ContainsKey(CalcID(modnr,  ID))) return AllFWO[CalcID(modnr, ID)]; else return null;
        }

        public static int CalcID(int modnr, int id)
        {
            return id * 100 + modnr;
        }


        /// <summary>
        /// Add a new FWO to the static list
        /// </summary>
        public void AddToFWOList()
        {
            if (AllFWO.ContainsKey(ID))
            {
                throw new Exception($"A FrameWorkVariable with this key {ID} already exists");
            }
            else
            {
                AllFWO.Add(ID, this);
            }
        }

        /// <summary>
        /// Copy the relevant values from a deserialized object to the object in the AllFWO list with the same ID
        /// </summary>
        /// <param name="newVal"></param>
        public abstract void CopyValues(RGOBase newVal);

        /// <summary>
        /// Get the value of the FWO as a string
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
        /// Get the value of a FWO values as a string when it is known that the FWO has a double type
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
            foreach (var F in AllFWO) F.Value.MustSerialize = true;
        }

  
    }
}
