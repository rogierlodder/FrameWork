using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows;
using FWO;

namespace FWO
{
    public class FWOButton : Button, IUpdatableControl
    {
        private GuiCMD command;

        public string CommandString { get; set; } = "";
        public int ModNr { get; set; } = -1;
        public int ID { get; set; } = -1;
        public string ScreenKey { get; set; }
        
        public List<ObjectID> ObjectIDs { get; set; }

        public void Setup(string key)
        {
            ScreenKey = key;
            Init(ModNr, ID);
        }

        public void Setup(string key, int modnr)
        {
            ScreenKey = key;

            //only use the module number from the arguement if the local property is not set by the user in the XAML.
            if (ModNr == -1) Init(modnr, ID);
            else Init(ModNr, ID);
        }

        public void Setup(string key, int modnr, int id)
        {
            ScreenKey = key;
            Init(modnr, id);
        }


        private void Init(int modnr, int id)
        {
            ObjectIDs = new List<ObjectID>();
            ObjectIDs.Add(new ObjectID(FWOBase.CalcID(modnr, id), ElementTypeEnum.ReadWrite));
            command = FWOBase.GetFromID(modnr, id) as GuiCMD;

            if (command != null)
            {
                UDC.AllControls.Add(this);
                command.Update += Update;
            }
        }

        public void Update()
        {
            IsEnabled = command.CanExecute;
        }

        private void ButtonPressed(object sender, RoutedEventArgs e)
        {
            if (command != null) command.Args = CommandString;
        }

        public FWOButton()
        {
            Click += ButtonPressed;
        }
    }
}
