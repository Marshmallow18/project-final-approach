using System; // System contains a lot of default C# libraries 
using System.Drawing;
using System.Linq;
// System.Drawing contains a library used for canvas drawing below
using GXPEngine;
using GXPEngine.Components;
using GXPEngine.Core;
using GXPEngine.Extensions;
using GXPEngine.HUD;
using TiledMapParserExtended;
using Rectangle = System.Drawing.Rectangle;

// GXPEngine contains the engine

public class MyGame : Game
{
    public static bool Debug;

    //Defined in void Main()
    public static int SCREEN_WIDTH;
    public static int SCREEN_HEIGHT;

    public static int HALF_SCREEN_WIDTH;
    public static int HALF_SCREEN_HEIGHT;

    private CaveLevelMapGameObject _caveLevelMap;

    private BaseLevel _level;

    private FollowCamera _cam;
    public static FollowCamera Cam;

    public static Vector2 WorldMousePosition;

    private FpsCounter _fpsCounter;

    public static int[] Keys_Used = new int[]
    {
        Key.O,
        Key.R,
        Key.E,
        Key.C,
        Key.U
    };

    public static int AlphaTweenDuration;

    /// <summary>
    /// Debug GameObjects
    /// </summary>
    private DebugTextBox _debugText;

    private CircleGameObject _circleGo;

    private GameHud _gameHud;

    public float oil = 100f;

    private int _timer;

    public MyGame() : base(SCREEN_WIDTH, SCREEN_HEIGHT, Settings.FullScreen) // Create a window that's 800x600 and NOT fullscreen
    {
        string[] tmxFiles = TmxFilesLoader.GetTmxFileNames("Level*.tmx");
        var mapData = TiledMapParserExtended.MapParser.ReadMap(tmxFiles[0]);
        var caveLevelMap = new CaveLevelMapGameObject(mapData);

        _cam = new FollowCamera(0, 0, SCREEN_WIDTH, SCREEN_HEIGHT);
        Cam = _cam;

        LoadLevel(caveLevelMap);

        //Debug
        _circleGo = new CircleGameObject(64, 64, (uint) Color.Red.ToArgb());
        var circleGo2 = new CircleGameObject(4, 4);
        AddChild(_circleGo);
        _circleGo.SetXY(1192, 4664);
        _circleGo.AddChild(circleGo2);

        _fpsCounter = new FpsCounter();
        AddChild(_fpsCounter);

        _debugText = new DebugTextBox("Hello World", width, 50);
        _cam.AddChild(_debugText);
        _debugText.x = -SCREEN_WIDTH / 2;
        _debugText.y = -SCREEN_HEIGHT / 2;
        _debugText.SetActive(false);
    }

    private void LoadLevel(CaveLevelMapGameObject pCaveLevelMap)
    {
        string[] tmxFiles = TmxFilesLoader.GetTmxFileNames("Level*.tmx");
        var mapData = TiledMapParserExtended.MapParser.ReadMap(tmxFiles[0]);

        _caveLevelMap = pCaveLevelMap;
        _level = new BaseLevel(_caveLevelMap, _cam);

        AddChild(_level);

        _gameHud = new GameHud(_level, _cam);

        DebugDrawBoundBox.level = _level;

        foreach (var sprite in _level.GetChildren().Where(s => s is Sprite))
        {
            DebugDrawBoundBox.AddSprite((Sprite) sprite);
        }

        foreach (var sprite in _gameHud.GetChildrenRecursive().Where(s => s is Sprite))
        {
            DebugDrawBoundBox.AddSprite((Sprite) sprite);
        }
    }

    private void ResetLevel()
    {
        _cam.parent = null;
        _caveLevelMap.parent = null;

        _gameHud.Destroy();
        _level.Destroy();

        GameHud.Instance = null;
        FlashbackPickupsManager.Instance = null;
        FlashbackManager.Instance = null;

        LoadLevel(_caveLevelMap);
    }

    void Update()
    {
        CoroutineManager.Tick(Time.deltaTime);

        if (Input.GetKeyDown(Key.R))
        {
            ResetLevel();
        }

        if (Input.GetKeyDown(Key.O))
        {
            this._glContext.Close();
        }

        /*
         * DEBUG
         */

        var mousePos = new Vector2Int(Input.mouseX, Input.mouseY);
        WorldMousePosition = _cam.TransformPoint(mousePos.x, mousePos.y) - Vector2.right * (width / 2) * _cam.scaleX -
                             Vector2.up * (height / 2) * _cam.scaleY;

        _debugText.TextValue =
            $"playerPos: {_level?.Player?.Position} | mouseWorld: {WorldMousePosition} | mapSize: {_caveLevelMap.TotalWidth} x {_caveLevelMap.TotalHeight}| oil: {oil}";
        //$"camScale: {_cam.scale:0.00} | mousePos: {mousePos} | worldMousePos: {worldMousePos} | isWalk: {isWalkable}";

        _level?.Player?.EnableRun(Input.GetKey(Key.LEFT_SHIFT));

        if (Input.GetKeyDown(Key.U))
        {
            ToogleDebug(WorldMousePosition);
        }

        if (Input.GetMouseButtonDown(0))
        {
            ParticleManager.Instance.PlayCoinsExplosion(_level.Player);
        }

        DebugDrawBoundBox.DrawBounds();

        LampReduceLight();
    }

    private void ToogleDebug(Vector2 worldMousePos)
    {
        Debug = !Debug;

        _debugText.SetActive(Debug);

        int playerIndex = _caveLevelMap.Index + 1;

        //Draw walkable image layers
        var debugWalkable = _level.GetChildren().FirstOrDefault(g => g.name == "Debug_Walkable");
        if (Debug && debugWalkable == null)
        {
            var bitmapData = _caveLevelMap._walkableImageLayer.GetBitmapDataFromWorldPos(worldMousePos);
            if (bitmapData.bitMap != null)
            {
                debugWalkable = new Canvas(bitmapData.bitMap, false)
                {
                    name = "Debug_Walkable"
                };

                _level.AddChildAt(debugWalkable, playerIndex);
                debugWalkable.SetXY(bitmapData.offSetX, bitmapData.offSetY);
            }
        }
        else
        {
            debugWalkable?.Destroy();
        }

        _level.PlayerCollision?.ToogleDebug();
    }

    static void Main() // Main() is the first method that's called when the program is run
    {
        Settings.Load();

        SCREEN_WIDTH = Settings.ScreenResolutionX;
        SCREEN_HEIGHT = Settings.ScreenResolutionY;

        HALF_SCREEN_WIDTH = SCREEN_WIDTH / 2;
        HALF_SCREEN_HEIGHT = SCREEN_HEIGHT / 2;

        AlphaTweenDuration = Settings.Default_AlphaTween_Duration;

        new MyGame().Start(); // Create a "MyGame" and start it
    }

    public void SetOil(float value)
    {
        oil = value;
    }

    public float GetOil()
    {
        return oil;
    }

    public void LampReduceLight()
    {
        _timer++;
        if (_timer > 60)
        {
            oil--;
            _timer = 0;
        }
    }
}