using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using GXPEngine.Core;

namespace GXPEngine
{
    public class DebugDrawBoundBox
    {
        private static Dictionary<Sprite, LineSegment[]> _linesMap = new Dictionary<Sprite, LineSegment[]>();

        public static GameObject level;

        private static bool lastDebug;

        public static void AddSprite(Sprite s)
        {
            _linesMap.Add(s, new LineSegment[]
            {
                new LineSegment(Vector2.zero, Vector2.one, (uint) Color.LimeGreen.ToArgb()),
                new LineSegment(Vector2.zero, Vector2.one, (uint) Color.LimeGreen.ToArgb()),
                new LineSegment(Vector2.zero, Vector2.one, (uint) Color.LimeGreen.ToArgb()),
                new LineSegment(Vector2.zero, Vector2.one, (uint) Color.LimeGreen.ToArgb()),
            });

            for (int i = 0; i < _linesMap[s].Length; i++)
            {
                level?.AddChild(_linesMap[s][i]);
            }
            
            Console.WriteLine($"DebugDrawBoundBox: {s.name} added");
        }

        public static void DrawBounds()
        {
            if (lastDebug == true && MyGame.Debug == false)
            {
                //remove all Lines game objects
                foreach (var kv in _linesMap)
                {
                    var lines = kv.Value;
                    for (int i = 0; i < lines.Length; i++)
                    {
                        lines[i].SetActive(false);
                    }
                }
            }
            else if (lastDebug == false && MyGame.Debug == true)
            {
                foreach (var kv in _linesMap)
                {
                    var lines = kv.Value;
                    for (int i = 0; i < lines.Length; i++)
                    {
                        lines[i].SetActive(true);
                    }
                }
            }

            lastDebug = MyGame.Debug;

            if (!MyGame.Debug)
                return;

            foreach (var kv in _linesMap)
            {
                var lines = kv.Value;
                
                var extends = kv.Key.GetExtents();

                lines[0].start = extends[0];
                lines[0].end = extends[1];

                lines[1].start = extends[1];
                lines[1].end = extends[2];

                lines[2].start = extends[2];
                lines[2].end = extends[3];

                lines[3].start = extends[3];
                lines[3].end = extends[0];
            }
        }
    }
}