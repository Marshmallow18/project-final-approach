﻿using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace TiledMapParserExtended
{
    /// <summary>
    /// Call the method MapParser.ReadMap, with as argument a Tiled file exported as xml (file extension: .tmx),
    /// to get an object of type MapData.
    /// This object, together with its nested objects, contains most of the information contained in the Tiled file.
    /// 
    /// The nesting of objects mimics the structure of the Tiled file exactly. 
    /// (For instance, a MapData can contain multiple (tile) Layers, ObjectgroupLayers, ImageLayers, which 
    /// all can have a PropertyList, etc.)
    /// 
    /// You should extend this class yourself if you want to read more information from the Tiled file 
    /// (such as tile rotations, geometry objects, ...). See
    ///   http://docs.mapeditor.org/en/stable/reference/tmx-mapData-format/
    /// for details.
    /// </summary>
    public class MapParser
    {
        static XmlSerializer serial = new XmlSerializer(typeof(Map), new XmlRootAttribute("map"));

        public static Map ReadMap(string filename)
        {
            TextReader reader = new StreamReader(filename);
            Map myMap = serial.Deserialize(reader) as Map;
            reader.Close();

            return myMap;
        }

        public static void WriteMap(string filename, Map map)
        {
            TextWriter writer = new StreamWriter(filename);
            serial.Serialize(writer, map);
            writer.Close();
        }
    }

    [XmlRootAttribute("mapData")]
    public class Map : PropertyContainer
    {
        [XmlAttribute("width")] public int Width;
        [XmlAttribute("height")] public int Height;

        [XmlAttribute("version")] public string Version;
        [XmlAttribute("orientation")] public string Orientation;
        [XmlAttribute("renderorder")] public string RenderOrder;
        [XmlAttribute("tilewidth")] public int TileWidth;
        [XmlAttribute("tileheight")] public int TileHeight;
        [XmlAttribute("nextobjectid")] public int NextObjectId;

        [XmlElement("tileset")] public TileSet[] TileSets;

        [XmlElement("layer")] public Layer[] Layers;

        [XmlElement("objectgroup")] public ObjectGroup[] ObjectGroups;

        [XmlElement("imagelayer")] public ImageLayer[] ImageLayers;
        
        [XmlElement("group")] public Group[] Groups;
        
        

        [XmlText] public string InnerXML; // This should be empty

        public override string ToString()
        {
            string output = "MapData of width " + Width + " and height " + Height + ".\n";

            output += "TILE LAYERS:\n";
            foreach (Layer l in Layers)
                output += l.ToString();

            output += "IMAGE LAYERS:\n";
            foreach (ImageLayer l in ImageLayers)
                output += l.ToString();

            output += "TILE SETS:\n";
            foreach (TileSet t in TileSets)
                output += t.ToString();

            output += "OBJECT GROUPS:\n";
            foreach (ObjectGroup g in ObjectGroups)
                output += g.ToString();

            return output;
        }

        /// <summary>
        /// A helper function that returns the tile set that belongs to the tile ID read from the layer data:
        /// </summary>
        public TileSet GetTileSet(int tileID)
        {
            if (tileID < 0)
                return null;
            int index = 0;
            while (TileSets[index].FirstGId + TileSets[index].TileCount <= tileID)
            {
                index++;
                if (index >= TileSets.Length)
                    return null;
            }

            return TileSets[index];
        }

        public int WorldToTileIndex(int x, int y)
        {
            if (x < 0 || x > (this.Width * this.TileWidth))
            {
                return -1;
            }

            if (y < 0 || y > (this.Height * this.TileHeight))
            {
                return -1;
            }

            int row = y / this.TileHeight;
            int column = x / this.TileWidth;

            return row * this.Width + column;
        }
    }

    [XmlRootAttribute("group")]
    public class Group
    {
        [XmlAttribute("name")] public string name;
        
        [XmlElement("layer")] public Layer[] Layers;

        [XmlElement("objectgroup")] public ObjectGroup[] ObjectGroups;

        [XmlElement("imagelayer")] public ImageLayer[] ImageLayers;
    }

    [XmlRootAttribute("tileset")]
    public class TileSet
    {
        [XmlAttribute("tilewidth")] public int TileWidth;
        [XmlAttribute("tileheight")] public int TileHeight;
        [XmlAttribute("tilecount")] public int TileCount;
        [XmlAttribute("columns")] public int Columns;
        [XmlAttribute("spacing")] public int Spacing;
        [XmlAttribute("margin")] public int Margin;

        public int Rows
        {
            get
            {
                if (TileCount % Columns == 0)
                    return TileCount / Columns;
                else
                    return (TileCount / Columns) + 1;
            }
        }

        /// <summary>
        /// This is the number of the first tile. Usually 1 (so 0 means empty/no tile).
        //// When multiple tilesets are used, this is the total number of previous tiles + 1.
        /// </summary>
        [XmlAttribute("firstgid")] public int FirstGId;

        [XmlAttribute("name")] public string Name;
        [XmlElement("image")] public Image Image;

        public override string ToString()
        {
            return "Tile set: Name: " + Name + " Image: " + Image + " Tile dimensions: " + TileWidth + "x" +
                   TileHeight + " Grid dimensions: " + Columns + "x" + (int) Math.Ceiling(1f * TileCount / Columns) +
                   "\n";
        }
    }

    public class PropertyContainer
    {
        [XmlElement("properties")] public PropertyList propertyList;

        public bool HasProperty(string key, string type)
        {
            if (propertyList == null)
                return false;
            foreach (Property p in propertyList.properties)
            {
                if (p.Name == key && p.Type == type)
                    return true;
            }

            return false;
        }

        public string GetStringProperty(string key, string defaultValue = "")
        {
            if (propertyList == null)
                return defaultValue;
            foreach (Property p in propertyList.properties)
            {
                if (p.Name == key)
                    return p.Value;
            }

            return defaultValue;
        }

        public float GetFloatProperty(string key, float defaultValue = 1)
        {
            if (propertyList == null)
                return defaultValue;
            foreach (Property p in propertyList.properties)
            {
                if (p.Name == key && p.Type == "float")
                    return float.Parse(p.Value, System.Globalization.NumberStyles.Any,
                        System.Globalization.CultureInfo.InvariantCulture);
            }

            return defaultValue;
        }

        public int GetIntProperty(string key, int defaultValue = 1)
        {
            if (propertyList == null)
                return defaultValue;
            foreach (Property p in propertyList.properties)
            {
                if (p.Name == key && p.Type == "int")
                    return int.Parse(p.Value);
            }

            return defaultValue;
        }

        public bool GetBoolProperty(string key, bool defaultValue = false)
        {
            if (propertyList == null)
                return defaultValue;
            foreach (Property p in propertyList.properties)
            {
                if (p.Name == key && p.Type == "bool")
                    return bool.Parse(p.Value);
            }

            return defaultValue;
        }

        public uint GetColorProperty(string key, uint defaultvalue = 0xffffffff)
        {
            if (propertyList == null)
                return defaultvalue;
            foreach (Property p in propertyList.properties)
            {
                if (p.Name == key && p.Type == "color")
                {
                    return TiledUtils.GetColor(p.Value);
                }
            }

            return defaultvalue;
        }
    }

    [XmlRootAttribute("imagelayer")]
    public class ImageLayer : PropertyContainer
    {
        [XmlAttribute("name")] public string Name;
        [XmlElement("image")] public Image Image;
        [XmlAttribute("offsetx")] public float offsetX = 0;
        [XmlAttribute("offsety")] public float offsetY = 0;
        [XmlAttribute("visible")] public bool visible = true;

        public override string ToString()
        {
            return "Image layer: " + Name + " Image: " + Image + "\n";
        }
    }

    [XmlRootAttribute("image")]
    public class Image
    {
        [XmlAttribute("width")] // width in pixels
        public int Width;

        [XmlAttribute("height")] // height in pixels
        public int Height;

        [XmlAttribute("source")] // AnimSprite file name
        public string FileName;

        public override string ToString()
        {
            return FileName + " (dim: " + Width + "x" + Height + ")";
        }
    }

    [XmlRootAttribute("layer")]
    public class Layer : PropertyContainer
    {
        [XmlAttribute("name")] public string Name;
        [XmlAttribute("width")] public int Width;
        [XmlAttribute("height")] public int Height;
        [XmlElement("data")] public Data Data;
        public float _xmlOpacity;
        public float Opacity = 1f;
        private int _xmlVisible;
        public bool Visible = true;

        public override string ToString()
        {
            string output = " Layer name: " + Name;
            output += "Properties:\n" + propertyList.ToString();
            output += "Data:" + Data.ToString();
            return output;
        }

        /// <summary>
        /// Returns the tile data from this layer as a 2-dimensional array of shorts. 
        /// It's a column-major array, so use [column,row] as indices.
        /// 
        /// This method does a lot of string parsing and memory allocation, so use it only once,
        /// during level loading.
        /// </summary>
        /// <returns>The tile array.</returns>
        public short[,] GetTileArray()
        {
            short[,] grid = new short[Width, Height];
            string[] lines = Data.innerXML.Split('\n');
            int row = 0;

            foreach (string line in lines)
            {
                if (line.Length <= 1)
                    continue;
                string parseLine = line;
                if (line[line.Length - 1] == ',')
                    parseLine = line.Substring(0, line.Length - 1);

                string[] chars = parseLine.Split(',');
                for (int col = 0; col < chars.Length; col++)
                {
                    if (col < Width)
                    {
                        short tileNum = short.Parse(chars[col]);
                        grid[col, row] = tileNum;
                    }
                }

                row++;
            }

            return grid;
        }

        [XmlAttribute("visible")]
        public int XmlVisible
        {
            get { return _xmlVisible; }
            set
            {
                _xmlVisible = value;
                Visible = value != 0;
            }
        }

        [XmlAttribute("opacity")]
        public float XmlOpacity
        {
            get { return _xmlOpacity;}
            set
            {
                _xmlOpacity = value;
                Opacity = value != 0 ? _xmlOpacity : 1;
            }
        }
    }

    [XmlRootAttribute("data")]
    public class Data
    {
        [XmlAttribute("encoding")] public string Encoding;
        [XmlText] public string innerXML;

        public override string ToString()
        {
            return innerXML;
        }
    }

    [XmlRootAttribute("properties")]
    public class PropertyList
    {
        [XmlElement("property")] public Property[] properties;

        public override string ToString()
        {
            string output = "";
            foreach (Property p in properties)
                output += p.ToString();
            return output;
        }
    }

    [XmlRootAttribute("property")]
    public class Property
    {
        [XmlAttribute("name")] public string Name = "";
        [XmlAttribute("type")] public string Type = "string";
        [XmlText] public string innerXML = "";

        private string _value;
        
        [XmlAttribute("value")]
        public string Value
        {
            get { return _value == null && !string.IsNullOrEmpty( innerXML) ? innerXML : _value; }
            set { _value = value; }
        }
        
        public override string ToString()
        {
            return "Property: Name: " + Name + " Type: " + Type + " Value: " + Value + "\n";
        }
    }

    [XmlRootAttribute("objectgroup")]
    public class ObjectGroup : PropertyContainer
    {
        [XmlAttribute("name")] public string Name;
        [XmlElement("object")] public TiledObject[] Objects;

        public override string ToString()
        {
            string output = "Object group: Name: " + Name + " Objects:\n";
            foreach (TiledObject obj in Objects)
                output += obj.ToString();

            return output;
        }
    }

    [XmlRootAttribute("text")]
    public class Text
    {
        [XmlAttribute("fontfamily")] public string font;
        [XmlAttribute("wrap")] public int wrap = 0;
        [XmlAttribute("bold")] public int bold = 0;
        [XmlAttribute("italic")] public int italic = 0;
        [XmlAttribute("pixelsize")] public int fontSize = 16;
        [XmlText] public string text;
        [XmlAttribute("halign")] public string horizontalAlign = "left";
        [XmlAttribute("valign")] public string verticalAlign = "top";
        [XmlAttribute("color")] public string color = "#FF000000"; // Tiled default

        public uint Color
        {
            get { return TiledUtils.GetColor(color); }
        }

        public override string ToString()
        {
            return text;
        }
    }

    [XmlRootAttribute("object")]
    public class TiledObject : PropertyContainer
    {
        [XmlAttribute("id")] public int ID;
        [XmlAttribute("gid")] public int GID = -1;
        [XmlAttribute("name")] public string Name = "";
        [XmlAttribute("type")] public string Type = "";

        [XmlAttribute("width")] // width in pixels
        public float Width;

        [XmlAttribute("height")] // height in pixels
        public float Height;

        [XmlAttribute("x")] public float X;
        [XmlAttribute("y")] public float Y;
        [XmlAttribute("rotation")] public float rotation;
        [XmlElement("text")] public Text textField;
        [XmlElement("polyline")] public PolylineObject[] polylines;
        [XmlElement("polygon")] public PolygonObject[] polygons;

        public override string ToString()
        {
            return "Object: " + Name + " ID: " + ID + " Type: " + Type + " coordinates: (" + X + "," + Y +
                   ") dimensions: (" + Width + "," + Height + ")\n";
        }
    }

    public class PolylineObject : PropertyContainer
    {
        private string[] _xmlPoints;

        public GXPEngine.Core.Vector2[] points;

        private NumberFormatInfo numberFormat;

        public PolylineObject()
        {
            var currentCulture = System.Globalization.CultureInfo.InstalledUICulture;
            numberFormat = (System.Globalization.NumberFormatInfo) currentCulture.NumberFormat.Clone();
            numberFormat.NumberDecimalSeparator = ".";
        }
        
        [XmlAttribute("points")]
        public string[] xmlPoints
        {
            get { return _xmlPoints; }

            set
            {
                _xmlPoints = value;
                points = _xmlPoints.Select(s => s.Trim().Split(',')).Select(str =>
                        new GXPEngine.Core.Vector2(float.Parse(str[0], numberFormat),
                            float.Parse(str[1], numberFormat)))
                    .ToArray();
            }
        }
    }

    public class PolygonObject : PropertyContainer
    {
        private string[] _xmlPoints;

        public GXPEngine.Core.Vector2[] points;

        private NumberFormatInfo numberFormat;

        public PolygonObject()
        {
            var currentCulture = System.Globalization.CultureInfo.InstalledUICulture;
            numberFormat = (System.Globalization.NumberFormatInfo) currentCulture.NumberFormat.Clone();
            numberFormat.NumberDecimalSeparator = ".";
        }

        [XmlAttribute("points")]
        public string[] xmlPoints
        {
            get { return _xmlPoints; }

            set
            {
                _xmlPoints = value;
                points = _xmlPoints.Select(s => s.Trim().Split(',')).Select(str =>
                        new GXPEngine.Core.Vector2(float.Parse(str[0], numberFormat),
                            float.Parse(str[1], numberFormat)))
                    .ToArray();
            }
        }
    }

    public class TiledUtils
    {
        /// <summary>
        /// This translates a Tiled color string to a uint that can be used as a GXPEngine Sprite color.
        /// </summary>
        public static uint GetColor(string htmlColor)
        {
            if (htmlColor.Length == 9)
            {
                return (uint) (
                    (Convert.ToInt32(htmlColor.Substring(3, 2), 16) << 24) + // R
                    (Convert.ToInt32(htmlColor.Substring(5, 2), 16) << 16) + // G
                    (Convert.ToInt32(htmlColor.Substring(7, 2), 16) << 8) + // B
                    (Convert.ToInt32(htmlColor.Substring(1, 2), 16))); // Alpha
            }
            else if (htmlColor.Length == 7)
            {
                return (uint) (
                    (Convert.ToInt32(htmlColor.Substring(1, 2), 16) << 24) + // R
                    (Convert.ToInt32(htmlColor.Substring(3, 2), 16) << 16) + // G
                    (Convert.ToInt32(htmlColor.Substring(5, 2), 16) << 8) + // B
                    0xFF); // Alpha
            }
            else
            {
                throw new Exception("Cannot recognize color string: " + htmlColor);
            }
        }
    }
}