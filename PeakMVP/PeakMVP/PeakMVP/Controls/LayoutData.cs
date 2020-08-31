using Xamarin.Forms;

namespace PeakMVP.Controls {
    public class LayoutData {
        public int VisibleChildCount { get; set; }

        public Size CellSize { get; set; }

        public int Rows { get; set; }

        public int Columns;

        public LayoutData() {

        }

        /// <summary>
        ///     ctor()
        /// </summary>
        /// <param name="visibleChildCount"></param>
        /// <param name="cellSize"></param>
        /// <param name="rows"></param>
        /// <param name="columns"></param>
        public LayoutData(int visibleChildCount, Size cellSize, int rows, int columns) {
            VisibleChildCount = visibleChildCount;
            CellSize = cellSize;
            Rows = rows;
            Columns = columns;
        }
    }
}
