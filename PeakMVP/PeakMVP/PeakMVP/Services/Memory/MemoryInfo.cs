namespace PeakMVP.Services.Memory {
    public sealed class MemoryInfo {
        public long FreeMemory { get; set; }

        public long MaxMemory { get; set; }

        public long TotalMemory { get; set; }

        public long UsedMemory => TotalMemory - FreeMemory;

        /// <summary>
        ///     ctor().
        /// </summary>
        public MemoryInfo() { }

        public double HeapUsage() => UsedMemory / (double)TotalMemory;

        public double Usage() => UsedMemory / (double)MaxMemory;
    }
}
