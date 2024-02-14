using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NAPS2.WinForms
{
    public class DarkToolStripColorScheme : ProfessionalColorTable
    {
        public override Color ButtonSelectedBorder => Color.Orange;
        public override Color ButtonSelectedGradientMiddle => Color.Orange;
        public override Color ButtonSelectedGradientBegin => Color.Orange;
        public override Color ButtonSelectedGradientEnd => Color.Orange;
        public override Color MenuItemSelectedGradientBegin => Color.Orange;
        public override Color MenuItemPressedGradientBegin => Color.Orange;
        public override Color MenuItemPressedGradientMiddle => Color.Orange;
        public override Color MenuItemPressedGradientEnd => Color.Orange;
        public override Color ButtonPressedGradientBegin => Color.Orange;
        public override Color ButtonPressedGradientMiddle => Color.Orange;
        public override Color ButtonPressedGradientEnd => Color.Orange;
        public override Color ButtonPressedBorder => Color.Orange;
    }
}
