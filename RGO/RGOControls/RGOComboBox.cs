using System.Windows.Controls;
using RGF;
using System.Collections.Generic;

namespace RGF
{
    public class RGOComboBox : ComboBox, IUpdatableControl
    {
        EnumSTP stp;

        public int ModNr { get; set; } = -1;
        public string ID { get; set; }
        public List<ObjectID> ObjectIDs { get; set; }
        public string ScreenKey { get; set; }

        private string[] Options;

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

        public void Setup(string key, int modnr, string id)
        {
            ScreenKey = key;
            Init(modnr, id);
        }

        private void Init(int modnr, string id)
        {
            ObjectIDs = new List<ObjectID>();
            ObjectIDs.Add(new ObjectID(RGOBase.CalcID(modnr, id), ElementTypeEnum.ReadWrite));
            stp = RGOBase.GetFromID(modnr, id) as EnumSTP;

            Options = stp.OptionsString.Split(',');
            ItemsSource = Options;

            if (stp != null)
            {
                UpdatableControlCollector.AllControls.Add(this);
                stp.Update += Update;
            }
        }

        void NewSelection(object sender, SelectionChangedEventArgs e)
        {
            if (stp != null) stp.Value = ((byte)SelectedIndex);
        }

        public void Update()
        {
            SelectedIndex = stp.Value;
            IsEnabled = stp.CanSetValue;
        }

        public RGOComboBox()
        {
            SelectionChanged += NewSelection;
        }
    }
}
