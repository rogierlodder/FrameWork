using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace RGF
{
    public static class ControlsHelper
    {
        public static SolidColorBrush OnColor = new SolidColorBrush(Colors.LawnGreen);
        public static SolidColorBrush ErrorColor = new SolidColorBrush(Colors.Red);
        public static SolidColorBrush TransitionColor = new SolidColorBrush(Colors.Yellow);
        public static SolidColorBrush Transparent = new SolidColorBrush(Colors.Transparent);

        public static string RemoveSpaces(string input)
        {
            return new string(input.ToCharArray().Where(c => !Char.IsWhiteSpace(c)).ToArray());
        }
    }
}
