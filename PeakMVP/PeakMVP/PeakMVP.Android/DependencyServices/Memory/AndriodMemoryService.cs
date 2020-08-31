using Android.Content;
using PeakMVP.Droid.DependencyServices.Memory;
using PeakMVP.Services.Memory;
using Xamarin.Forms;

[assembly: Dependency(typeof(AndriodMemoryService))]
namespace PeakMVP.Droid.DependencyServices.Memory {
    public class AndriodMemoryService : IMemoryService {

        private static Context _context;

        /// <summary>
        ///     ctor().
        /// </summary>
        public AndriodMemoryService() {
            _context = MainActivity.Self;
        }

        public MemoryInfo GetInfo() => MemoryHelper.GetMemoryInfo(_context);
    }
}