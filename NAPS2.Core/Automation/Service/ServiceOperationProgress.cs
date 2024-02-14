using NAPS2.Operation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NAPS2.Automation.Service
{
    public class ServiceOperationProgress : IOperationProgress
    {
        public void Attach(IOperation op)
        {
        }

        public void ShowProgress(IOperation op)
        {
            op.Wait();
        }

        public void ShowModalProgress(IOperation op)
        {
        }

        public void ShowBackgroundProgress(IOperation op)
        {
        }

        public void RenderStatus(IOperation op, Label textLabel, Label numberLabel, ProgressBar progressBar) => throw new InvalidOperationException();

        public List<IOperation> ActiveOperations => throw new InvalidOperationException();
    }
}
