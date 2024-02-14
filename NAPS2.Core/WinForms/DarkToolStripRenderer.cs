using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NAPS2.WinForms
{
    public class DarkToolStripRenderer : ToolStripProfessionalRenderer
    {
        public DarkToolStripRenderer() : base(new DarkToolStripColorScheme()) { }

        protected override void OnRenderArrow(ToolStripArrowRenderEventArgs e)
        {
            if(!(e.Item is ToolStripMenuItem))
                e.ArrowColor = System.Drawing.Color.White;
            base.OnRenderArrow(e);
        }

        protected override void OnRenderItemBackground(ToolStripItemRenderEventArgs e)
        {
            base.OnRenderItemBackground(e);
        }

        protected override void OnRenderToolStripBorder(ToolStripRenderEventArgs e)
        {
            //base.OnRenderToolStripBorder(e);
        }
    }
}
