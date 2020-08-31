using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;

namespace PeakMVP.Helpers {
    public class AzureMobileCenter {

        public string AndroidAppSecret => "8e4bfa2b-e706-4cee-9eb5-93ebbac69da3";
        public string IOSAppSecret => "898e3732-5917-43c1-8136-859f40c5fe75";

        [MethodImpl(MethodImplOptions.NoInlining)]
        public void CurrentMethodName() {
            StackTrace stackTrace = new StackTrace();
            StackFrame stackFrame = stackTrace.GetFrames().Last();

            Console.WriteLine("---> {0}",stackFrame.GetMethod().Name);
        }
    }
}
