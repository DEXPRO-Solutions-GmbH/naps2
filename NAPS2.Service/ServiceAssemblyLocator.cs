using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace NAPS2.Service
{
    public static class ServiceAssemblyLocator
    {
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static string Locate() => Assembly.GetExecutingAssembly().Location;
    }
}
