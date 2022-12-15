using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLiteDemo
{
    internal class GeoPackage
    {
		public int ZoomLevel { get; }
		public int TileColumn { get; }
		public int TileRow { get; }
		public byte[] TileData { get; }

		public GeoPackage(int zoomLevel, int tileColumn, int tileRow, byte[] tileData)
		{
			this.ZoomLevel = zoomLevel;
			this.TileColumn = tileColumn;
			this.TileRow = tileRow;
			this.TileData = tileData;
		}
		public GeoPackage() { }
	}
}
