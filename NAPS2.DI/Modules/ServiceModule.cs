using System;
using System.Collections.Generic;
using System.Linq;
using NAPS2.Automation;
using NAPS2.Automation.Service;
using NAPS2.Dependencies;
using NAPS2.ImportExport.Pdf;
using NAPS2.Operation;
using NAPS2.Scan.Images;
using NAPS2.Util;
using Ninject.Modules;

namespace NAPS2.DI.Modules
{
    public class ServiceModule : NinjectModule
    {
        public override void Load()
        {
            Bind<IPdfPasswordProvider>().To<ServicePdfPasswordProvider>();
            Bind<IErrorOutput>().To<ServiceErrorOutput>();
            Bind<IOverwritePrompt>().To<ServiceOverwritePrompt>();
            Bind<IOperationProgress>().To<ServiceOperationProgress>();
            Bind<IComponentInstallPrompt>().To<ServiceComponentInstallPrompt>();
            Bind<ThumbnailRenderer>().To<NullThumbnailRenderer>();
        }
    }
}
