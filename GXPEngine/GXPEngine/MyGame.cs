using System; // System contains a lot of default C# libraries 
using System.Drawing;
using System.Linq;
// System.Drawing contains a library used for canvas drawing below
using GXPEngine;
using GXPEngine.Components;
using GXPEngine.Core;
using GXPEngine.Extensions;
using TiledMapParserExtended;
using Rectangle = System.Drawing.Rectangle;

// GXPEngine contains the engine

public class MyGame : Game
{
    public static bool Debug;

#if true
    public const int SCREEN_WIDTH = 1280; //1920
    public const int SCREEN_HEIGHT = 720; //1080
    public const bool FULLSCREEN = false;

#else
    public const int SCREEN_WIDTH = 1920;
    public const int SCREEN_HEIGHT = 1080;
    public const bool FULLSCREEN = true;
#endif

    public static int HALF_SCREEN_WIDTH = SCREEN_WIDTH / 2;
    public static int HALF_SCREEN_HEIGHT = SCREEN_HEIGHT / 2;

    private CaveLevelMapGameObject _caveLevelMap;

    private FollowCamera _cam;
    private FpsCounter _fpsCounter;
    private TextBox _debugText;
    private CircleGameObject _circleGo;

    private EasyDraw _debugColl;
    private EasyDraw _debugPOI;
    private Arrow _debugNormalPOI;
    
    public MyGame() : base(SCREEN_WIDTH, SCREEN_HEIGHT, FULLSCREEN) // Create a window that's 800x600 and NOT fullscreen
    {
        //----------------------------------------------------example-code----------------------------
        //create a canvas
        Canvas canvas = new Canvas(SCREEN_WIDTH, SCREEN_HEIGHT);

        //add some content
        canvas.graphics.FillRectangle(new SolidBrush(Color.Red), new Rectangle(0, 0, 400, 300));
        canvas.graphics.FillRectangle(new SolidBrush(Color.Blue), new Rectangle(400, 0, 400, 300));
        canvas.graphics.FillRectangle(new SolidBrush(Color.Yellow), new Rectangle(0, 300, 400, 300));
        canvas.graphics.FillRectangle(new SolidBrush(Color.Gray), new Rectangle(400, 300, 400, 300));

        //add canvas to display list
        AddChild(canvas);
        //------------------------------------------------end-of-example-code-------------------------

        string[] tmxFiles = TmxFilesLoader.GetTmxFileNames("Level*.tmx");
        var mapData = TiledMapParserExtended.MapParser.ReadMap(tmxFiles[0]);

        _caveLevelMap = new CaveLevelMapGameObject(mapData);

        AddChild(_caveLevelMap);

        _cam = new FollowCamera(0, 0, SCREEN_WIDTH, SCREEN_HEIGHT);
        AddChild(_cam);

        _circleGo = new CircleGameObject(64, 64, (uint) Color.Red.ToArgb());
        var circleGo2 = new CircleGameObject(4, 4);
        AddChild(_circleGo);
        _circleGo.SetXY(1192, 4664);
        _circleGo.AddChild(circleGo2);

        _cam.Target = _circleGo;
        _cam.Map = _caveLevelMap;
        _cam.SetXY(1000, 1000);

        _fpsCounter = new FpsCounter();
        AddChild(_fpsCounter);

        _debugText = new TextBox("Hello World", width, 50);
        _cam.AddChild(_debugText);
        _debugText.x = -SCREEN_WIDTH / 2;
        _debugText.y = -SCREEN_HEIGHT / 2;
    }

    void Update()
    {
        CoroutineManager.Tick(Time.deltaTime);

        /*
         * DEBUG
         */

        var mousePos = new Vector2Int(Input.mouseX, Input.mouseY);
        var worldMousePos = _cam.TransformPoint(mousePos.x, mousePos.y) - Vector2.right * (width / 2) * _cam.scaleX -
                            Vector2.up * (height / 2) * _cam.scaleY;
        
        Vector2 lastPos = _circleGo.Position;
        
        float camSpeed = 50;
        Vector2 vel;

        if (Input.GetKey(Key.W))
        {
            _circleGo.Translate(0, -camSpeed * Time.delta);
            
        }
        else if (Input.GetKey(Key.S))
        {
            _circleGo.Translate(0, camSpeed * Time.delta);
        }

        if (Input.GetKey(Key.D))
        {
            _circleGo.Translate(camSpeed * Time.delta, 0);
        }
        else if (Input.GetKey(Key.A))
        {
            _circleGo.Translate(-camSpeed * Time.delta, 0);
        }

        var isWalkable = _caveLevelMap.IsWalkablePosition(_circleGo.Position);

        if (!isWalkable)
        {
            var nextPos = _caveLevelMap.GetCollisionPOI(_circleGo.Position, lastPos);

            var normalCollision = _caveLevelMap.GetCollisionNormal(nextPos);
            
            Console.WriteLine($"nextpos: {nextPos} | normal: {normalCollision}");
            
            _debugPOI?.SetXY(nextPos.x,nextPos.y);
            _debugColl?.SetXY(_circleGo.x, _circleGo.y);

            if (_debugNormalPOI != null)
            {
                _debugNormalPOI.startPoint = nextPos - _cam.Position + new Vector2(HALF_SCREEN_WIDTH,HALF_SCREEN_HEIGHT);
                _debugNormalPOI.vector = normalCollision;
            }

            _circleGo.SetXY(lastPos.x, lastPos.y);
        }
        
        if (Input.GetKey(Key.E))
        {
            _cam.scale += 0.05f;

            for (int i = 0; i < _cam.GetChildren().Count; i++)
            {
            }
        }
        else if (Input.GetKey(Key.C))
        {
            _cam.scale -= 0.05f;
        }

        _debugText.TextValue =
            $"camScale: {_cam.scale:0.00} | mousePos: {mousePos} | worldMousePos: {worldMousePos} | isWalk: {isWalkable}";

        if (Input.GetKeyDown(Key.U))
        {
            ToogleDebug(worldMousePos);
        }
    }

    private void ToogleDebug(Vector2 worldMousePos)
    {
        Debug = !Debug;
        
        //Draw walkable image layers
        var debugWalkable = this.GetChildren().FirstOrDefault(g => g.name == "Debug_Walkable");
        if (Debug && debugWalkable == null)
        {
            var bitmapData = _caveLevelMap._walkableImageLayer.GetBitmapDataFromWorldPos(worldMousePos);
            if (bitmapData.bitMap != null)
            {
                debugWalkable = new Canvas(bitmapData.bitMap)
                {
                    name = "Debug_Walkable"
                };

                int circleIndex = _circleGo.Index;
                
                AddChildAt(debugWalkable, circleIndex);
                debugWalkable.SetXY(bitmapData.offSetX, bitmapData.offSetY);
            }
        }
        else
        {
            debugWalkable?.Destroy();
        }
        _debugColl = this.GetChildren().FirstOrDefault(g => g.name == "Debug_Coll") as EasyDraw;
        if (_debugColl == null)
        {
            _debugColl = new EasyDraw(2, 2, false)
            {
                name = "Debug_Coll"
            };
            _debugColl.CentralizeOrigin();
            _debugColl.Clear(Color.GreenYellow);
            AddChild(_debugColl);
        }
        else
        {
            _debugColl?.Destroy();
        }
        
        _debugPOI = this.GetChildren().FirstOrDefault(g => g.name == "Debug_POI") as EasyDraw;
        if (_debugPOI == null)
        {
            _debugPOI = new EasyDraw(9, 9, false)
            {
                name = "Debug_POI"
            };
            _debugPOI.CentralizeOrigin();
            _debugPOI.Clear(Color.FromArgb(50, Color.Red));
            AddChild(_debugPOI);
        }
        else
        {
            _debugPOI?.Destroy();
        }
        
        _debugNormalPOI = this.GetChildren().FirstOrDefault(g => g.name == "Debug_Normal_POI") as Arrow;
        if (_debugNormalPOI == null)
        {
            _debugNormalPOI = new Arrow(Vector2.zero, Vector2.one, 100, (uint)Color.HotPink.ToArgb())
            {
                name = "Debug_Normal_POI"
            };
            AddChild(_debugNormalPOI);
        }
        else
        {
            _debugNormalPOI?.Destroy();
        }
    }

    static void Main() // Main() is the first method that's called when the program is run
    {
        new MyGame().Start(); // Create a "MyGame" and start it
    }
}