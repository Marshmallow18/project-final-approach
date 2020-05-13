using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using GXPEngine.Core;
using TiledMapParserExtended;
using Rectangle = System.Drawing.Rectangle;

namespace GXPEngine
{
    public class MapGameObject : GameObject
    {
        /// <summary>
        /// MapData object parsed using TiledMapParserExtended
        /// </summary>
        private Map _mapData;

        private Sprite[,] _bgCanvases;

        /// <summary>
        /// Tileset images loaded from Tmx
        /// </summary>
        private Bitmap[] _tileSetImages;

        /// <summary>
        /// Used to draw tiles from TileSet, it's keep the rectangle and the tileset id
        /// </summary>
        private SpriteTileSetData[] _tileSetSpritesData;

        /// <summary>
        /// All the tiles layers in a mu;tidimensional array, example:
        /// [0][12,23] = 47 //means that the tile id in layer "0", at column "12" and row "23" is "47"
        /// This array is used to get the tile data of a layer at some position
        /// </summary>
        private short[][,] _tileArrays;

        private ObjectGroup[] _objectGroups;

        private ImageLayer[] _backGroundImages;

        private Dictionary<string, int> _tilesetIndexMap;
        
        private Dictionary<string, Bitmap> _bitMapReuse;

        protected int _totalWidth;
        protected int _totalHeight;

        public MapGameObject(Map mapData) : base(false)
        {
            _mapData = mapData;

            _totalWidth = _mapData.Width * _mapData.TileWidth;
            _totalHeight = _mapData.Height * _mapData.TileHeight;
            
            _bitMapReuse = new Dictionary<string, Bitmap>();
            
            var backGroundImages = _mapData.Groups.Where(g => g.name == "background image").SelectMany(g => g.ImageLayers).Where(g => !string.IsNullOrWhiteSpace(g.Image?.FileName)).GroupBy(g => g.Image?.FileName);

            _mapData.ImageLayers = backGroundImages.SelectMany(g => g).ToArray();
            
            foreach (var groupImage in backGroundImages)
            {
                var bitmap = new Bitmap(groupImage.Key);
                _bitMapReuse.Add(groupImage.Key, bitmap);
            }

            _bitMapReuse = new Dictionary<string, Bitmap>();

            //Load Object Group
            _objectGroups = _mapData.ObjectGroups;

            //Section to create the tiles Image and data to be drawn
            //The TilesSet Images are loaded and than used to draw each layer according with the layer data

            int totalTiles = mapData.TileSets.Sum(t => t.TileCount);
            _tileSetImages = new Bitmap[_mapData.TileSets.Length];
            _tileSetSpritesData = new SpriteTileSetData[totalTiles];
            int tileCounter = 0;

            _tilesetIndexMap = new Dictionary<string, int>();

            for (int i = 0; i < _mapData.TileSets.Length; i++)
            {
                var watch = StopwatchTool.StartWatch();

                var tileSet = _mapData.TileSets[i];

                _tilesetIndexMap.Add(tileSet.Name, i);

                if (tileSet.TileCount > 0)
                {
                    int tileWidth = tileSet.TileWidth;
                    int tileHeight = tileSet.TileHeight;
                    int margin = tileSet.Margin;
                    int spacing = tileSet.Spacing;
                    int rows = tileSet.Rows;

                    var bm = new Bitmap("data/Clouds_Border_Tileset.png");
                    _tileSetImages[i] = bm;

                    for (int j = 0; j < tileSet.TileCount; j++)
                    {
                        int row = j / tileSet.Columns;
                        int column = j % tileSet.Columns;

                        Rectangle cloneRect = new Rectangle(column * tileWidth + margin + spacing * (column),
                            row * tileHeight + margin + spacing * (row), tileWidth, tileHeight);

                        //Create a data the keeps the ID and the rectangle where the sprite is in tileset
                        _tileSetSpritesData[tileCounter] = new SpriteTileSetData(i, cloneRect);
                        tileCounter++;
                    }

                    Console.WriteLine("tileSet.TileCount: " + tileSet.TileCount + " | rows: " + tileSet.Rows +
                                      " | columns: " + tileSet.Columns);

                    StopwatchTool.StopWatch(watch, tileSet.Name + " loaded: ");
                }
            }

            var drawWatch = StopwatchTool.StartWatch();

            DrawBackgroundLayers();

            StopwatchTool.StopWatch(drawWatch, "DrawBackgroundLayer: ");

            if (_mapData.Layers != null)
            {
                //Set the the layers data in a multidimensional array
                _tileArrays = new short[_mapData.Layers.Length][,];
                for (int i = 0; i < mapData.Layers.Length; i++)
                {
                    _tileArrays[i] = _mapData.Layers[i].GetTileArray();
                }

                drawWatch = StopwatchTool.StartWatch();

                DrawLayers();

                StopwatchTool.StopWatch(drawWatch, "DrawLayers: ");
            }
        }

        private void DrawBackgroundLayers()
        {
            var backGroundImageLayer =
                _mapData.ImageLayers.Where(l => l.Name.StartsWith("background image") && l.visible == true);

            var bitMapReuse = new Dictionary<string, Bitmap>();

            foreach (var bgLayer in backGroundImageLayer)
            {
                Bitmap bitmap;

                if (!bitMapReuse.ContainsKey(bgLayer.Image.FileName))
                {
                    bitmap = new Bitmap(bgLayer.Image.FileName);
                    bitMapReuse.Add(bgLayer.Image.FileName, bitmap);
                }
                else
                {
                    bitmap = bitMapReuse[bgLayer.Image.FileName];
                }

                ImageAttributes imageAtt = new ImageAttributes();

                // int offsetX = SIZE * Mathf.Round(bgLayer.offsetX) / 1920;
                // int offsetY = SIZE * Mathf.Round(bgLayer.offsetY) / 1920;
                //
                // //Get Canvas
                // int canvasRow = offsetY / SIZE;
                // int canvasColumn = offsetX / SIZE;

                var canvas = new Sprite(bitmap, false);
                AddChild(canvas);
                canvas.SetXY(bgLayer.offsetX, bgLayer.offsetY);
            }
        }


        private void DrawLayers()
        {
            for (int i = 0; i < _mapData.Layers.Length; i++)
            {
                Layer layer = _mapData.Layers[i];
                if (layer.Visible)
                {
                    DrawLayer(i, this);
                }
            }
        }

        /// <summary>
        /// Draw a layer by index
        /// </summary>
        /// <param name="layerIndex"></param>
        public void DrawLayer(int layerIndex, GameObject parent)
        {
            int tileWidth = _mapData.TileWidth;
            int tileHeight = _mapData.TileHeight;
            int tileOrigin = _mapData.Layers[layerIndex].GetIntProperty("tileorigin", 0);

            // To draw mapData layer with opacity
            float[][] matrixItems =
            {
                new float[] {1, 0, 0, 0, 0},
                new float[] {0, 1, 0, 0, 0},
                new float[] {0, 0, 1, 0, 0},
                new float[] {0, 0, 0, _mapData.Layers[layerIndex].Opacity, 0},
                new float[] {0, 0, 0, 0, 1}
            };
            ColorMatrix colorMatrix = new ColorMatrix(matrixItems);

            // Create an ImageAttributes object and set its color matrix.
            ImageAttributes imageAtt = new ImageAttributes();
            imageAtt.SetColorMatrix(
                colorMatrix,
                ColorMatrixFlag.Default,
                ColorAdjustType.Bitmap);

            //go through data and get the absolute ID, than with the absolute ID get the correspondent rectangle in tileset image
            int rows = _tileArrays[layerIndex].GetLength(0);
            int columns = _tileArrays[layerIndex].GetLength(1);
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    int tileIndex = _tileArrays[layerIndex][i, j] - 1;

                    if (tileIndex < 0)
                    {
                        continue;
                    }

                    var spriteData = _tileSetSpritesData[tileIndex];
                    var sourceRect = spriteData.rect;

                    var pt = new Point(i * _mapData.TileHeight, j * _mapData.TileWidth - tileOrigin);

                    var canvas = new Canvas(_mapData.TileWidth, _mapData.TileHeight, false);

                    canvas.graphics.DrawImage(_tileSetImages[spriteData.tileSetId],
                        new Rectangle(0, 0, tileWidth,
                            tileHeight), // destination rectangle
                        sourceRect.X, // source rectangle x 
                        sourceRect.Y, // source rectangle y
                        tileWidth, // source rectangle width
                        tileHeight, // source rectangle height
                        GraphicsUnit.Pixel,
                        imageAtt);

                    parent.AddChild(canvas);
                    canvas.SetXY(pt.X, pt.Y);
                }
            }
        }

        public void DrawBorders(GameObject parent, float alpha = 1f)
        {
            var borderCloudesTileSet = _mapData.TileSets.FirstOrDefault(ts => ts.Name == "Clouds Border");

            var topCloudsRects = new Rectangle[]
            {
                new Rectangle(0, 128 * 0, 512, 128),
                new Rectangle(0, 128 * 1, 512, 128),
                new Rectangle(0, 128 * 2, 512, 128),
            };

            var bottomCloudsRects = new Rectangle[]
            {
                new Rectangle(0, 128 * 3, 512, 128),
                new Rectangle(0, 128 * 4, 512, 128),
                new Rectangle(0, 128 * 5, 512, 128),
            };

            var rightCloudsRects = new Rectangle[]
            {
                new Rectangle(512 + 128 * 0, 0, 128, 512),
                new Rectangle(512 + 128 * 1, 0, 128, 512),
                new Rectangle(512 + 128 * 2, 0, 128, 512),
            };

            var leftCloudsRects = new Rectangle[]
            {
                new Rectangle(512 + 128 * 3, 0, 128, 512),
                new Rectangle(512 + 128 * 0, 512, 128, 512),
                new Rectangle(512 + 128 * 1, 512, 128, 512),
            };

            int tilsetIndex = _tilesetIndexMap["Clouds Borders"];
            var tilesetImage = _tileSetImages[tilsetIndex];

            int widthSteps = Mathf.Ceiling((float) _mapData.Width * _mapData.TileWidth / topCloudsRects[0].Width);

            //Draw top border
            int canvasW = topCloudsRects[0].Width;
            int canvasH = topCloudsRects[0].Height;

            for (int i = 0; i < widthSteps; i++)
            {
                int randRect = MRandom.Range(0, topCloudsRects.Length);

                var canvas = new Canvas(canvasW, canvasH, false);

                canvas.graphics.DrawImage(tilesetImage,
                    new Rectangle(0, 0, canvasW, canvasH),
                    topCloudsRects[randRect], GraphicsUnit.Pixel
                );

                parent.AddChild(canvas);

                canvas.x = i * canvasW;
                canvas.y = 0;

                canvas.alpha = alpha;
            }

            //Draw bottom border
            canvasW = bottomCloudsRects[0].Width;
            canvasH = bottomCloudsRects[0].Height;

            int yHeight = (_mapData.Height - 1) * _mapData.TileHeight;

            for (int i = 0; i < widthSteps; i++)
            {
                int randRect = MRandom.Range(0, bottomCloudsRects.Length);

                var canvas = new Canvas(canvasW, canvasH, false);

                canvas.graphics.DrawImage(tilesetImage,
                    new Rectangle(0, 0, canvasW, canvasH),
                    bottomCloudsRects[randRect], GraphicsUnit.Pixel
                );

                parent.AddChild(canvas);

                canvas.x = i * canvasW;
                canvas.y = yHeight;

                canvas.alpha = alpha;
            }

            int heightSteps = Mathf.Ceiling((float) _mapData.Height * _mapData.TileHeight / leftCloudsRects[0].Width);

            //Draw left border
            canvasW = leftCloudsRects[0].Width;
            canvasH = leftCloudsRects[0].Height;

            int xWidth = 0;

            for (int i = 0; i < heightSteps; i++)
            {
                int randRect = MRandom.Range(0, leftCloudsRects.Length);

                var canvas = new Canvas(canvasW, canvasH, false);

                canvas.graphics.DrawImage(tilesetImage,
                    new Rectangle(0, 0, canvasW, canvasH),
                    leftCloudsRects[randRect], GraphicsUnit.Pixel
                );

                parent.AddChild(canvas);

                canvas.x = xWidth;
                canvas.y = i * canvasH;

                canvas.alpha = alpha;
            }

            //Draw right border
            canvasW = rightCloudsRects[0].Width;
            canvasH = rightCloudsRects[0].Height;

            xWidth = (_mapData.Width - 1) * _mapData.TileWidth;

            for (int i = 0; i < heightSteps; i++)
            {
                int randRect = MRandom.Range(0, rightCloudsRects.Length);

                var canvas = new Canvas(canvasW, canvasH, false);

                canvas.graphics.DrawImage(tilesetImage,
                    new Rectangle(0, 0, canvasW, canvasH),
                    rightCloudsRects[randRect], GraphicsUnit.Pixel
                );

                parent.AddChild(canvas);

                canvas.x = xWidth;
                canvas.y = i * canvasH;

                canvas.alpha = alpha;
            }
        }

        public Vector2 WorldToTilePoint(float x, float y)
        {
            int row = Mathf.Floor((y) / _mapData.TileHeight);
            int column = Mathf.Floor((x) / _mapData.TileWidth);

            return new Vector2(column, row);
        }

        public int WorldToTile(float x, float y)
        {
            if (x < 0 || x > _mapData.Height * _mapData.TileHeight || y < 0 || y > _mapData.Width * _mapData.TileWidth)
            {
                return -1;
            }

            int row = Mathf.Floor(y / _mapData.TileHeight);

            if (row < 0 || row >= _mapData.Height)
            {
                return -1;
            }

            int column = Mathf.Floor(x / _mapData.TileWidth);

            if (column < 0 || column >= _mapData.Width)
            {
                return -1;
            }

            return row * _mapData.Height + column;
        }

        public Vector2 TileToWorld(int tile)
        {
            int row = tile / _mapData.Height;
            int column = tile % _mapData.Width;

            return new Vector2(column * _mapData.TileWidth + _mapData.TileWidth * 0.5f,
                row * _mapData.TileHeight + _mapData.TileHeight * 0.5f);
        }

        public Vector2 TileToWorld(int column, int row)
        {
            return new Vector2(column * _mapData.TileWidth + _mapData.TileWidth * 0.5f,
                row * _mapData.TileHeight + _mapData.TileHeight * 0.5f);
        }

        public int GetTileIdFromWorld(int layer, float x, float y)
        {
            int row = Mathf.Floor(y / _mapData.TileHeight);

            if (row < 0 || row >= this._mapData.Height)
            {
                return -1;
            }

            int column = Mathf.Floor(x / _mapData.TileWidth);
            if (column < 0 || column >= this._mapData.Width)
            {
                return -1;
            }

            return _tileArrays[layer][column, row];
        }

        public Map MapData => _mapData;

        public int GetLayerIndex(string pName)
        {
            for (int i = 0; i < _mapData.Layers?.Length; i++)
            {
                if (_mapData.Layers[i].Name == pName)
                {
                    return i;
                }
            }

            return -1;
        }

        public ObjectGroup[] ObjectGroups => _objectGroups;

        public int TotalWidth => _totalWidth;
        public int TotalHeight => _totalHeight;

        /// <summary>
        /// All the tiles layers in a mu;tidimensional array, example:
        /// [0][12,23] = 47 //means that the tile id in layer "0", at column "12" and row "23" is "47"
        /// This array is used to get the tile data of a layer at some position
        /// </summary>
        public short[][,] TileArrays
        {
            get => _tileArrays;
            set => _tileArrays = value;
        }
    }

    /// <summary>
    /// Data to be used when drawing tiles from the tileset
    /// </summary>
    internal struct SpriteTileSetData
    {
        public Rectangle rect;
        public int tileSetId;

        public SpriteTileSetData(int tileSetId, Rectangle rect)
        {
            this.tileSetId = tileSetId;
            this.rect = rect;
        }
    }
}