using System.Collections;
using System.Collections.Generic;
using System.Threading;
using GXPEngine.HUD;
using GXPEngine.HUD.FlashBack_Huds;
using TiledMapParserExtended;

namespace GXPEngine
{
    public class FlashbackManager : GameObject
    {
        public static FlashbackManager Instance;

        private int _totalFlashbacks;
        private HashSet<string> _collectedFlashBacksTriggersNames;
        private List<string> _collectedFlashPickupsNames;

        private BaseLevel _level;
        
        

        public FlashbackManager(BaseLevel pLevel, int pTotalFlashbacks) : base(false)
        {
            Instance = this;

            _level = pLevel;

            _collectedFlashBacksTriggersNames = new HashSet<string>();
            _collectedFlashPickupsNames = new List<string>();

            _totalFlashbacks = pTotalFlashbacks;

            CoroutineManager.StartCoroutine(Start(), this);
        }

        private IEnumerator Start()
        {
            while (GameHud.Instance == null)
            {
                yield return null;
            }

            if (Settings.Flashback_Triggers_Collected != "0")
            {
                string[] flashTriggersStr = Settings.Flashback_Triggers_Collected.Split(' ');
                for (int i = 0; i < flashTriggersStr.Length; i++)
                {
                    var valStr = flashTriggersStr[i].Trim();
                    if (int.TryParse(valStr, out var val))
                    {
                        _collectedFlashBacksTriggersNames.Add($"Flashback {val}");
                    }
                }
            }

            GameHud.Instance.SetFlashbackHudCounterText(
                $"{_collectedFlashBacksTriggersNames.Count} of {_totalFlashbacks}");
        }

        /// <summary>
        /// Returns true if a flashback was added, false if the flashback already exists
        /// </summary>
        /// <param name="flashName"></param>
        /// <returns></returns>
        public void PlayerPickedupFlashblackTrigger(TiledObject flashbackData)
        {
            bool flashbackExists = _collectedFlashBacksTriggersNames.Contains(flashbackData.Name);
            
            GameHud.Instance.SetFlashbackHudCounterText(
                $"{_collectedFlashBacksTriggersNames.Count} of {_totalFlashbacks}");

            if (!flashbackExists)
            {
                _collectedFlashBacksTriggersNames.Add(flashbackData.Name);
                var flashHud = GameHud.Instance.LoadFlashbackHud(flashbackData);
                CoroutineManager.StartCoroutine(WaitForFlashPanelHudBeDestroyed(flashHud), this);
            }
        }

        private IEnumerator WaitForFlashPanelHudBeDestroyed(FlashBackHud01 flashHud)
        {
            while (flashHud != null && flashHud.toDestroy == false)
            {
                yield return null;
            }

            if (_collectedFlashBacksTriggersNames.Count >= _totalFlashbacks)
            {
                //All found

                //Show message to user
                _level.Player.InputEnabled = false;

                yield return new WaitForMilliSeconds(1000);

                var textBox = new TextBox(Settings.Message_To_Player_After_Collect_Flashbacks, game.width - 120, 30);
                textBox.CenterOnBottom();
                GameHud.Instance.AddChild(textBox);

                yield return textBox.TweenTextRoutine(0, Settings.Flashbacks_TextBoxTweenSpeed, true);

                textBox.Destroy();

                //Change FlashbackPanel
                yield return GameHud.Instance.ShowFlashbackDetectivePanel();

                //Show Flashbacks pickups
                FlashbackPickupsManager.Instance.EnableFlashbackPickups();

                _level.Player.InputEnabled = true;
            }
        }

        public void PlayerPickedupFlashblack(FlashbackPickup flashbackPickup)
        {
            var flashName = flashbackPickup.FlashbackData.Name;
            bool alreadyInList = _collectedFlashPickupsNames.Contains(flashName);

            if (!alreadyInList)
            {
                _collectedFlashPickupsNames.Add(flashName);
                
                //Change Indicator
                if (int.TryParse(flashName.Replace("Flashback Pickup ", ""), out var flashIndex))
                {
                    GameHud.Instance.MemoriesHudPanel.EnableIndicator(_collectedFlashPickupsNames.Count-1);

                    flashbackPickup.collider.Enabled = false;
                    CoroutineManager.StopAllCoroutines(flashbackPickup);
                    DrawableTweener.TweenSpriteAlpha(flashbackPickup, flashbackPickup.alpha, 0, Settings.Default_AlphaTween_Duration);
                }

                if (_collectedFlashPickupsNames.Count == _totalFlashbacks)
                {
                    CoroutineManager.StartCoroutine(CheckPickusCollectedOrder(), this);
                }
            }
        }

        private IEnumerator CheckPickusCollectedOrder()
        {
            _level.Player.InputEnabled = false;
            
            yield return new WaitForMilliSeconds(Settings.Default_AlphaTween_Duration + 200);
            
        }

        void Update()
        {
            if (Input.GetKeyDown(Key.I))
            {
                GameHud.Instance.ShowTextBox("Test test box", 500, 60, 0, 0, true);
            }
        }

        public int TotalFlashbacks => _totalFlashbacks;
        public int FlashbackTriggersCollectedCount => _collectedFlashBacksTriggersNames.Count;
        public int CollectedFlashPickupsCount => _collectedFlashPickupsNames.Count;
    }
}