using System.Diagnostics;
using Xamarin.Forms;

namespace PeakMVP.Services.Memory {
    public static class MemoryHelper {

        private static int _count = default(int);

        public static void DisplayAndroidMemory() {
            _count++;

            MemoryInfo memoryInfo = DependencyService.Get<IMemoryService>().GetInfo();

            Debug.WriteLine($"-------------------------------{_count}----------------------------------");
            Debug.WriteLine($"USED MEMORY:{string.Format("{0:N}", memoryInfo.UsedMemory)}");
            Debug.WriteLine($"FREE MEMORY:{string.Format("{0:N}", memoryInfo.FreeMemory)}");
            Debug.WriteLine($"TOTAL MEMORY:{string.Format("{0:N}", memoryInfo.TotalMemory)}");
            Debug.WriteLine($"--------------------------------------------------------------------------");
            Debug.WriteLine($"MAX MEMORY:{string.Format("{0:N}", memoryInfo.MaxMemory)}");
            Debug.WriteLine($"HEAP USAGE:{string.Format("{0:P}", memoryInfo.HeapUsage())}");
            Debug.WriteLine($"TOTAL USAGE:{string.Format("{0:P}", memoryInfo.HeapUsage())}");
        }
    }
}
