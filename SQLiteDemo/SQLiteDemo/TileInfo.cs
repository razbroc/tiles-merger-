using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLiteDemo
{
    public class TileInfo
    {
		public int ZoomLevel { get; set; }
		public int TileColumn { get; }
		public int TileRow { get; }
		public byte[] TileData { get; }

		public TileInfo(int zoomLevel, int tileColumn, int tileRow, byte[] tileData)
        {
            this.ZoomLevel = zoomLevel;
			this.TileColumn = tileColumn;
			this.TileRow = tileRow;
			this.TileData = tileData;
		}
		public TileInfo() { }
	}
}
