using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLiteDemo
{
    internal class GeoPackage
    {
		int zoomLevel;
		int tileColumn;
		int tileRow;
		byte[] tileData;

		public GeoPackage(int zoomLevel, int tileColumn, int tileRow, byte[] tileData)
		{
			this.zoomLevel = zoomLevel;
			this.tileColumn = tileColumn;
			this.tileRow = tileRow;
			this.tileData = tileData;
		}
		public GeoPackage() { }

		public int GetZoomLevel()
		{
			return zoomLevel;
		}

		public int GetTileColumn()
		{
			return tileColumn;
		}

		public int GetTileRow()
		{
			return tileRow;
		}

		public byte[] GetTileData()
		{
			return tileData;
		}
	}
}
