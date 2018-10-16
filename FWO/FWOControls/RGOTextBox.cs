using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Input;
using RGF;
using System.Windows.Media;

namespace RGF
{
    public class RGOTextBox : TextBox, IUpdatableControl
    {
        private RGOBase stp;
        private RGOBase MinEQP, MaxEQP;

        public int ModNr { get; set; } = -1;
        public int ID { get; set; }
        public List<ObjectID> ObjectIDs { get; private set; }

        public int MinEQPID { get; set; } = -1;
        public int MaxEQPID { get; set; } = -1;
        public string ScreenKey { get; set; }
        public int NrDigits { get; set; }

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

        public void Update()
        {
            IsEnabled = (stp as dynamic).CanSetValue;
            Text = stp.GetValueAsString(NrDigits);
        }

        private void Init(int modnr, int id)
        {
            ObjectIDs = new List<ObjectID>();
            ObjectIDs.Add(new ObjectID(RGOBase.CalcID(modnr, id), ElementTypeEnum.ReadWrite));
            stp = RGOBase.GetFromID(modnr, id);

            if (MinEQPID != -1) MinEQP = RGOBase.GetFromID(RGOBase.CalcID(modnr, MinEQPID));
            if (MaxEQPID != -1) MaxEQP = RGOBase.GetFromID(RGOBase.CalcID(modnr, MaxEQPID));

            if (stp != null && stp.GetType().Name.Contains("STP"))
            {
                UDC.AllControls.Add(this);
                stp.Update += Update;
            }
        }

        protected void Key_pressed(object sender, KeyEventArgs e) { StartEvaluate(); }
        protected void Lost_focus(object sender,  EventArgs e)    { StartEvaluate(); }

        public void StartEvaluate()
        {
            bool evalFailed = true;
            bool MinMaxFail = true;

            if (stp is STP<int>) 
            {
                STP<int> I = stp as STP<int>;

                double min, max;
                if (MinEQP != null) min = (MinEQP as EQP<int>).Value; else min = int.MinValue;
                if (MaxEQP != null) max = (MaxEQP as EQP<int>).Value; else max = int.MaxValue;

                int entryVal = 0;
                if (int.TryParse(Text, out entryVal)) evalFailed = false;
                else ToolTip = "This entry has the wrong format (must be integer)";
                if (entryVal >= min && entryVal <= max) MinMaxFail = false;
                else ToolTip = $"This value is outside the min/max values: {min}/{max}";

                if (!evalFailed && !MinMaxFail) I.Value = entryVal;
            }
            if (stp is STP<byte>)
            {
                STP<byte> B = stp as STP<byte>;
                byte min, max;
                if (MinEQP != null) min = (MinEQP as EQP<byte>).Value; else min = byte.MinValue;
                if (MaxEQP != null) max = (MaxEQP as EQP<byte>).Value; else max = byte.MaxValue;

                byte entryVal;
                if (byte.TryParse(Text, out entryVal)) evalFailed = false;
                else ToolTip = "This entry has the wrong format (must be byte)";
                if (entryVal >= min && entryVal <= max) MinMaxFail = false;
                else ToolTip = $"This value is outside the min/max values: {min}/{max}";
                if (!evalFailed && !MinMaxFail) B.Value = entryVal;
            }
            if (stp is STP<double>)
            {
                STP<double> D = stp as STP<double>;
                double min, max;
                if (MinEQP != null) min = (MinEQP as EQP<double>).Value; else min = double.MinValue;
                if (MaxEQP != null) max = (MaxEQP as EQP<double>).Value; else max = double.MaxValue;

                double entryVal;
                if (double.TryParse(Text, out entryVal)) evalFailed = false;
                else ToolTip = "This entry has the wrong format (must be floating point)";
                if (entryVal >= min && entryVal <= max) MinMaxFail = false;
                else ToolTip = $"This value is outside the min/max values: {min}/{max}";
                if (!evalFailed && !MinMaxFail) D.Value = entryVal;
            }

            if (evalFailed)
            {
                Foreground = Brushes.Red;
                FontWeight = FontWeights.Bold;
            }
            if (MinMaxFail)
            {
                Foreground = Brushes.Red;
                FontWeight = FontWeights.Bold;
            }
            if (evalFailed == false && MinMaxFail == false)
            {
                Foreground = Brushes.Black;
                FontWeight = FontWeights.Regular;
                ToolTip = "This entry is valid";
            }
        }

        public RGOTextBox()
        {
            //inspect the STP
            HorizontalContentAlignment = HorizontalAlignment.Right;
            VerticalContentAlignment = VerticalAlignment.Center;

            IsEnabled = false;
            KeyUp += Key_pressed;
            LostFocus += Lost_focus;
        }
    }
}
