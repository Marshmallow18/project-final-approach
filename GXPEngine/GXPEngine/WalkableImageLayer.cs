using System;
using System.Drawing;
using System.Linq;
using GXPEngine.Components;
using GXPEngine.Core;
using TiledMapParserExtended;

namespace GXPEngine
{
    public class WalkableImageLayer
    {
        private ImageLayer[] _imageLayers;

        private int _tileWidth;
        private int _tileHeight;

        private BitmapData[,] _bitMaps;

        public WalkableImageLayer(ImageLayer[] pImageLayers, Map mapData)
        {
            _imageLayers = pImageLayers;

            _tileWidth = _imageLayers[0].Image.Width;
            _tileHeight = _imageLayers[0].Image.Height;

            int totalColumns = Mathf.Ceiling((float) (mapData.Width * mapData.TileWidth) / _tileWidth);
            int totalRows = Mathf.Ceiling((float) (mapData.Height * mapData.TileHeight) / _tileHeight);

            int index = 0;

            _bitMaps = new BitmapData[totalColumns, totalRows];
            for (int row = 0; row < _bitMaps.GetLength(1); row++)
            {
                for (int col = 0; col < _bitMaps.GetLength(0); col++)
                {
                    var bitMap = new Bitmap(_imageLayers[index].Image.FileName);
                    _bitMaps[col, row] = new BitmapData()
                    {
                        bitMap = bitMap,
                        offSetX = Mathf.Round(_imageLayers[index].offsetX),
                        offSetY = Mathf.Round(_imageLayers[index].offsetY),
                    };
                    index++;
                }
            }
        }

        public BitmapData GetBitmapDataFromWorldPos(Vector2 pos)
        {
            var colRow = WorldToRowColumn(pos);

            if (!IsInsideLimits(colRow))
            {
                return new BitmapData();
            }
            
            return _bitMaps[colRow.x, colRow.y];
        }

        public Color GetPixelFromWorldPos(Vector2 pos)
        {
            var colRow = WorldToRowColumn(pos);

            if (!IsInsideLimits(colRow))
            {
                return Color.Black;
            }

            var bitMapData = _bitMaps[colRow.x, colRow.y];

            //Subtract image bitmap offset to get relative pos
            int posX = Mathf.Floor(pos.x - bitMapData.offSetX);
            int posY = Mathf.Floor(pos.y - bitMapData.offSetY);

            return bitMapData.bitMap.GetPixel(posX, posY);
        }
        
        public Color GetPixelFromWorldPosReturnMagenta(Vector2 pos)
        {
            var colRow = WorldToRowColumn(pos);

            if (!IsInsideLimits(colRow))
            {
                return Color.Magenta;
            }

            var bitMapData = _bitMaps[colRow.x, colRow.y];

            //Subtract image bitmap offset to get relative pos
            int posX = Mathf.Floor(pos.x - bitMapData.offSetX);
            int posY = Mathf.Floor(pos.y - bitMapData.offSetY);

            return bitMapData.bitMap.GetPixel(posX, posY);
        }

        public Vector2Int WorldToRowColumn(Vector2 pos)
        {
            int row = Mathf.Floor(pos.y / _tileHeight);
            int col = Mathf.Floor(pos.x / _tileWidth);

            return new Vector2Int(col, row);
        }

        public bool IsInsideLimits(Vector2Int colRow)
        {
            return colRow.x > -1 && colRow.x < _bitMaps.GetLength(0) && colRow.y > -1 &&
                   colRow.y < _bitMaps.GetLength(1);
        }

        public struct BitmapData
        {
            public Bitmap bitMap;
            public int offSetX;
            public int offSetY;
        }
    }
}