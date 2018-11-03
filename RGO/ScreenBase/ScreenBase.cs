using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace RGF
{
    public abstract class ScreenBase : UserControl, IUpdatableScreen
    {
        public delegate void DebugDelegate(string msg);
        public static DebugDelegate Debug { get; set; }

        public delegate void RGOElementUpdateDelegat();

        /// <summary>
        /// The module number that is selected by the GUI
        /// </summary>
        public virtual int ModNr { get; set; }

        /// <summary>
        /// The module number that is currently on display
        /// </summary>
        public int DisplayModNr { get; protected set; } = -1;

        /// <summary>
        /// Indicates whether the module should update even when it is not visible
        /// </summary>
        public bool IsPermanent { get; set; } = false;

        /// <summary>
        /// the key that identifies the screen in the screen dictionary
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// A list of IDS of all the FWObjects this screen could potentially write
        /// </summary>
        public List<string> WriteListIDs = new List<string>();

        protected RGOClientRW RGOClient;

        /// <summary>
        /// Constructor
        /// </summary>
        public ScreenBase()
        {

        }


        protected void SetupAllRGOElements(Panel panel, int modnr)
        {
            IterateAllUIElements<IUpdatableControl>(panel, p => p.Setup(Key, modnr));
        }

        /// <summary>
        /// This method is used by the RGO client to create the list of RGO objects that must be written to the server
        /// </summary>
        public void CreateWriteList()
        {
            RGOClient.Request.WriteList.Clear();
            foreach (var I in WriteListIDs)
            {
                if (RGOBase.AllRGO[I].MustSerialize == true)
                {
                    RGOClient.Request.WriteList.Add(RGOBase.AllRGO[I]);
                    RGOBase.AllRGO[I].MustSerialize = false;
                }
            }
        }

        /// <summary>
        /// Collect all the FWOobjects used by all elements in this screen.
        /// </summary>
        public virtual void Setup(string ServerIP)
        {
            //start the RGO client
            RGOClient = new RGOClientRW(ServerIP, "RGOReadWriteService");
            RGOClient.CreateWriteList = CreateWriteList;

            //scan 
            var AllElementsForThisScreen = UpdatableControlCollector.AllControls.Where(p => p.ScreenKey == Key);
            foreach (var Element in AllElementsForThisScreen)
            {
                foreach (ObjectID obj in Element.ObjectIDs)
                {
                    if (obj.Type == ElementTypeEnum.Read || obj.Type == ElementTypeEnum.ReadWrite) RGOClient.Request.ReqList.Add(obj.ID);
                    if (obj.Type == ElementTypeEnum.Write || obj.Type == ElementTypeEnum.ReadWrite) WriteListIDs.Add(obj.ID);
                }
            }
        }

        /// <summary>
        /// Start or restart the RGO client of this screen
        /// </summary>
        public virtual void StartUpdating()
        {
            RGOClient?.Start();
            Debug($"{Key} started the RGO client");
        }

        /// <summary>
        /// Stop the RGO client if this screen to save on ethernet traffic
        /// </summary>
        public virtual void StopUpdating()
        {
            RGOClient?.Stop();
            Debug($"{Key} stopped the RGO client");
        }

        /// <summary>
        /// Recursively iterate through all UIElement items in a Panel and call a method if they are of type <T>
        /// </summary>
        /// <param name="panel"></param>
        static void IterateAllUIElements<T>(Panel panel, Action<T> setup) where T : class
        {
            if (panel == null || panel.Children == null) return;
            foreach (UIElement U in panel.Children)
            {
                if (U is T) setup?.Invoke(U as T);
                else if (U is Panel) IterateAllUIElements(U as Panel, setup);
                else if (U is ContentControl) IterateAllUIElements((U as ContentControl).Content as Panel, setup);
                else if (U is TabControl) foreach (TabItem I in (U as TabControl).Items) IterateAllUIElements((I as ContentControl).Content as Panel, setup);
            }
        }
    }
}
