using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLiteDemo
{
    internal class GeoPackage
    {
		int ZoomLevel;
		int TileColumn;
		int TileRow;
		byte[] TileData;

		public GeoPackage(int zoomLevel, int tileColumn, int tileRow, byte[] tileData)
		{
			this.ZoomLevel = zoomLevel;
			this.TileColumn = tileColumn;
			this.TileRow = tileRow;
			this.TileData = tileData;
		}
		public GeoPackage() { }

		public int GetZoomLevel()
		{
			return ZoomLevel;
		}

		public int GetTileColumn()
		{
			return TileColumn;
		}

		public int GetTileRow()
		{
			return TileRow;
		}

		public byte[] GetTileData()
		{
			return TileData;
		}
	}
}
