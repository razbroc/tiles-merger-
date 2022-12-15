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

		public GeoPackage(int ZoomLevel, int TileColumn, int TileRow, byte[] TileData)
		{
			this.ZoomLevel = ZoomLevel;
			this.TileColumn = TileColumn;
			this.TileRow = TileRow;
			this.TileData = TileData;
		}
		public GeoPackage() { }
	}
}
