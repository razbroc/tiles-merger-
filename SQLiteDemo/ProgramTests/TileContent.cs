using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbTests
{
    public class TileContent
    {
        public string tableName { get; }
        public string dataType { get; }
        public string identifier { get; }
        public string description { get; }
        public string lastChange { get; }
        public double minX { get; }
        public double minY { get; }
        public double maxX { get; }
        public double maxY { get; }
        public double srsId { get; }

        public TileContent()
        {
        }

        public TileContent(string tableName, string dataType, double minX, double minY, double maxX, double maxY)
        {
            this.tableName = tableName;
            this.dataType = dataType;
            this.minX = minX;
            this.minY = minY;
            this.maxX = maxX;
            this.maxY = maxY;
        }

        public TileContent(string tableName, string dataType, string identifier, string description, string lastChange, double minX, double minY, double maxX, double maxY, double srsId)
        {
            this.tableName = tableName;
            this.dataType = dataType;
            this.identifier = identifier;
            this.description = description;
            this.lastChange = lastChange;
            this.minX = minX;
            this.minY = minY;
            this.maxX = maxX;
            this.maxY = maxY;
            this.srsId = srsId;
        }
    }
}
