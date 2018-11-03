using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace RGF
{
    public abstract class ContainerScreenBase : UserControl, IUpdatableScreen
    {
        /// <summary>
        /// the Dictionary holding the local screens (IUpdatebleScreen)
        /// </summary>
        protected Dictionary<int, ScreenBase> localScreens = new Dictionary<int, ScreenBase>();

        /// <summary>
        /// Do not forget to set this reference to the MainGrid in your user control
        /// </summary>
        protected Grid MainGridRef;

        private int _ModNr;

        public bool IsPermanent { get; set; }

        public string Key { get; set; }

        public int ModNr
        {
            get { return _ModNr; }
            set
            {
                //stop old screen
                localScreens[_ModNr].StopUpdating();

                //remove the screen from the main grid
                if (MainGridRef != null)
                {
                    MainGridRef.Children.Clear();
                    //Update 
                    _ModNr = value;

                    //load the new screen
                    MainGridRef.Children.Add(localScreens[_ModNr]);

                    //start new screen
                    localScreens[_ModNr].StartUpdating();
                }
            }
        }

        public virtual void Setup() { }

        public virtual void StartUpdating()
        {

        }

        public virtual void StopUpdating()
        {
            localScreens[_ModNr].StopUpdating();
        }
    }
}
