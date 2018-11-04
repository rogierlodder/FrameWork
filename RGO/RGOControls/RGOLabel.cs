using System.Windows.Controls;
using RGF;
using System.Linq;
using System.Windows.Media;
using System.Collections.Generic;

namespace RGF
{
    public class RGOlabel : Label, IUpdatableControl
    {
        private RGOBase RGObject;

        private string[] errorString, onString, transitionString;
        private bool IsStatusVariableString = false;

        public int ModNr { get; set; } = -1;
        public int NrDigits { get; set; } = -1;
        public string ID { get; set; } = "";
        public List<ObjectID> ObjectIDs { get; set; }
        public string ScreenKey { get; set; }

        public string Error { get; set; } = "";
        public string On { get; set; } = "";
        public string Transition { get; set; } = "";

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
            ObjectIDs.Add(new ObjectID(RGOBase.CalcID(modnr, id), ElementTypeEnum.Read));
            RGObject = RGOBase.GetFromID(modnr, id);

            if (RGObject != null)
            {
                errorString = (ControlsHelper.RemoveSpaces(Error)).Split(',');
                onString = (ControlsHelper.RemoveSpaces(On)).Split(',');
                transitionString = (ControlsHelper.RemoveSpaces(Transition)).Split(',');

                if (Error != "" || On != "" || Transition != "") IsStatusVariableString = true;
                //add this element to the list of all elementrs so that the request and reply classes can be created for the RGOServices
                UpdatableControlCollector.AllControls.Add(this);

                //subscribe to the update delegate of the RGObject
                RGObject.Update += Update;

                Background = ControlsHelper.Transparent;
            }
            else Background = ControlsHelper.ErrorColor;
        }

        public void Update()
        {
            if (IsStatusVariableString)
            {
                string s = RGObject.GetValueAsString();
                Content = s;
                if (errorString != null && errorString.Contains(s)) Background = ControlsHelper.ErrorColor;
                else if (onString != null && onString.Contains(s)) Background = ControlsHelper.OnColor;
                else if (transitionString != null && transitionString.Contains(s)) Background = ControlsHelper.TransitionColor;
                else Background = Brushes.LightGray;
            }
            else
            {
                if (NrDigits == -1) Content = RGObject.GetValueAsString();
                else Content = RGObject.GetValueAsString(NrDigits);
            }
        }
    }
}
