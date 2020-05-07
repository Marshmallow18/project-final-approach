using System;
using System.Drawing;
using System.Linq;
using GXPEngine.Components;
using GXPEngine.Core;
using TiledMapParserExtended;

namespace GXPEngine
{
    public class CaveLevelMapGameObject : MapGameObject
    {
        public WalkableImageLayer _walkableImageLayer;

        public static Vector2[] Directions = new Vector2[]
        {
            Vector2.up,
            Vector2.right,
            Vector2.up * -1,
            Vector2.right * -1
        };

        public CaveLevelMapGameObject(Map mapData) : base(mapData)
        {
            var walkableImageLayersData = mapData.Groups.Where(g => g.name == "walkable image")
                .SelectMany(g => g.ImageLayers).ToArray();
            _walkableImageLayer = new WalkableImageLayer(walkableImageLayersData, mapData);
        }

        void Update()
        {
        }

        public bool IsWalkablePosition(Vector2 pos)
        {
            if (pos.x < 0 || pos.y < 0 || pos.x >= _totalWidth || pos.y >= _totalHeight)
            {
                return false;
            }

            var colRow = _walkableImageLayer.WorldToRowColumn(pos);

            var c = _walkableImageLayer.GetPixelFromWorldPos(pos);

            return c.ToArgb() == Color.White.ToArgb();
        }

        public Vector2 GetCollisionPOI(Vector2 pos, Vector2 lastPos)
        {
            Vector2 desired = pos - lastPos;
            Vector2 desiredDir = desired.Normalized;
            int desiredLen = Mathf.Round(desired.Magnitude);

            Vector2 nextPos;
            for (int i = 1; i < desiredLen; i++)
            {
                nextPos = lastPos + desiredDir * i;
                Color nextPixel = _walkableImageLayer.GetPixelFromWorldPos(nextPos);
                if (nextPixel.ToArgb() == Color.Black.ToArgb())
                {
                    return nextPos;
                }
            }

            return lastPos;
        }

        public Vector2 GetCollisionNormal(Vector2 poi)
        {
            var firstPixel = poi - Vector2.one * 4;
            firstPixel.x = Mathf.Round(firstPixel.x);
            firstPixel.y = Mathf.Round(firstPixel.y);
            
            Vector2 normal = Vector2.zero;
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    var pixelX = firstPixel.x + j;
                    var pixelY = firstPixel.y + i;

                    var pixelPos = new Vector2(pixelX, pixelY);
                    
                    var colRow = _walkableImageLayer.WorldToRowColumn(pixelPos);
                    if (!_walkableImageLayer.IsInsideLimits(colRow))
                    {
                        continue;
                    }

                    var pixColor = _walkableImageLayer.GetPixelFromWorldPosReturnMagenta(pixelPos);
                    if (pixColor.ToArgb() == Color.White.ToArgb())
                    {
                        continue;
                    }
                    
                    for (int dir = 0; dir < Directions.Length; dir++)
                    {
                        var neihbColor =
                            _walkableImageLayer.GetPixelFromWorldPosReturnMagenta(pixelPos + Directions[dir]);

                        if (neihbColor.ToArgb() == Color.Magenta.ToArgb())
                        {
                            continue;
                        }

                        if (neihbColor.ToArgb() == Color.White.ToArgb())
                        {
                            normal += Directions[dir];
                        }
                    }
                }
            }

            return normal.Normalized;
        }
    }
}