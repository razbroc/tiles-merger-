using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLiteDemo
{
    internal class TileMatrix
    {
        public int ZoomLevel { get; } 
        public int MatrixWidth { get; }
        public int MatrixHeight { get; }
        public int TileWidth { get; }
        public int TileHeight { get; }
        public double PixelXSize { get; }
        public double PixelYSize { get; }

        public TileMatrix(int zoomLevel, int matrixWidth, int matrixHeight, int tileWidth, int tileHeight, double pixelXSize, double pixelYSize)
        {
            ZoomLevel = zoomLevel;
            MatrixWidth = matrixWidth;
            MatrixHeight = matrixHeight;
            TileWidth = tileWidth;
            TileHeight = tileHeight;
            PixelXSize = pixelXSize;
            PixelYSize = pixelYSize;
        }
    }
}
