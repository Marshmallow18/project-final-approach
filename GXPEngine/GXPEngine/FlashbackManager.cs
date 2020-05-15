using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
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

        private FlashbackPickup _lastPicked;

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
                        string triggerName = $"flashback {val}";
                        _collectedFlashBacksTriggersNames.Add(triggerName);

                        //Disable triggers
                        var trigger = FlashBackTriggersManager.Instance.FlashTriggersMap[triggerName];
                        trigger.Enabled = false;
                    }
                }
            }

            GameHud.Instance.SetFlashbackHudCounterText(string.Format(Settings.HUD_Flashback_Counter_Format,
                _collectedFlashBacksTriggersNames.Count, _totalFlashbacks));
            //$"{_collectedFlashBacksTriggersNames.Count} of {_totalFlashbacks} flashbacks");
        }

        /// <summary>
        /// Returns true if a flashback was added, false if the flashback already exists
        /// </summary>
        /// <param name="flashName"></param>
        /// <returns></returns>
        public void PlayerPickedupFlashblackTrigger(TiledObject flashbackData)
        {
            bool flashbackExists = _collectedFlashBacksTriggersNames.Contains(flashbackData.Name);

            if (!flashbackExists)
            {
                _collectedFlashBacksTriggersNames.Add(flashbackData.Name);

                GameHud.Instance.SetFlashbackHudCounterText(
                    string.Format(Settings.HUD_Flashback_Counter_Format, _collectedFlashBacksTriggersNames.Count,
                        _totalFlashbacks)
                );

                var flashHud = GameHud.Instance.LoadFlashbackHud(flashbackData, false,
                    Settings.Flashback_Triggers_Allow_Skip_With_Esc_Key);
                CoroutineManager.StartCoroutine(WaitForFlashPanelHudBeDestroyed(flashHud, flashbackData), this);
            }
        }

        private IEnumerator WaitForFlashPanelHudBeDestroyed(FlashBackHud01 flashHud, TiledObject flashbackData)
        {
            while (flashHud != null && flashHud.toDestroy == false)
            {
                yield return null;
            }

            _level.Player.InputEnabled = true;

            //Fadein Music if has one and a property to closeit
            var closeMusicBool = flashbackData.GetBoolProperty("close_music", false);
            if (closeMusicBool)
            {
                GameSoundManager.Instance.FadeOutCurrentMusic(Settings.Flashbacks_Music_Fadein_Duration);
                GameSoundManager.Instance.FadeInMusic(Settings.Base_Music, Settings.Background_Music_Volume,
                    Settings.Flashbacks_Music_Fadein_Duration);
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

        public void PlayerPickedupFlashblack(FlashbackPickup flashbackPickup, bool showPanel = true)
        {
            CoroutineManager.StartCoroutine(PlayerPickedupFlashblackRoutine(flashbackPickup, showPanel), this);
        }

        private IEnumerator PlayerPickedupFlashblackRoutine(FlashbackPickup flashbackPickup, bool showPanel)
        {
            GameHud.Instance.ResetFlashbackButton.collider.Enabled = false;

            //Show FlashbackHud
            if (showPanel)
            {
                yield return FlashbackHudRoutine(flashbackPickup.FlashbackData.Name);

                GameHud.Instance.ResetFlashbackButton.SetActive(false);

                //Fadeout Music if has one and a property to close it
                if (FlashBackTriggersManager.Instance.FlashTriggersMap.TryGetValue(flashbackPickup.FlashbackData.Name, out var flashTrigger))
                {
                    var closeMusicBool = flashTrigger.FlashbackTriggerData.GetBoolProperty("close_music", false);
                    if (closeMusicBool)
                    {
                        GameSoundManager.Instance.FadeOutCurrentMusic(Settings.Flashbacks_Music_Fadein_Duration);
                        GameSoundManager.Instance.FadeInMusic(Settings.Base_Music, Settings.Background_Music_Volume,
                            Settings.Flashbacks_Music_Fadein_Duration);
                    }
                }
            }

            var flashName = flashbackPickup.FlashbackData.Name;
            bool alreadyInList = _collectedFlashPickupsNames.Contains(flashName);

            if (!alreadyInList)
            {
                _collectedFlashPickupsNames.Add(flashName);

                if (flashName == "final flashback")
                {
                    CoroutineManager.StopAllCoroutines(flashbackPickup);
                    DrawableTweener.TweenSpriteAlpha(flashbackPickup, flashbackPickup.alpha, 0,
                        Settings.Default_AlphaTween_Duration,
                        () => { flashbackPickup.Enabled = false; });

                    GameHud.Instance.ShowTextBox("Game Over");
                    
                    Console.WriteLine($"{this} go to END");
                }
                else
                {
                    //Change Indicator
                    if (int.TryParse(flashName.Replace("flashback ", ""), out var flashIndex))
                    {
                        GameHud.Instance.MemoriesHudPanel.EnableIndicator(_collectedFlashPickupsNames.Count - 1);
                    }

                    //The last flashback pickedup is not disabled because we need to test the collision with player
                    //to re-enable it only after player exit the trigger
                    //So make it invisible
                    if (_collectedFlashPickupsNames.Count >= _totalFlashbacks)
                    {
                        _lastPicked = flashbackPickup;
                        CoroutineManager.StopAllCoroutines(flashbackPickup);
                        DrawableTweener.TweenSpriteAlpha(flashbackPickup, flashbackPickup.alpha, 0,
                            Settings.Default_AlphaTween_Duration);

                        yield return CheckPickupsCollectedOrderSquence(flashbackPickup);
                    }
                    else
                    {
                        flashbackPickup.collider.Enabled = false;
                        CoroutineManager.StopAllCoroutines(flashbackPickup);
                        DrawableTweener.TweenSpriteAlpha(flashbackPickup, flashbackPickup.alpha, 0,
                            Settings.Default_AlphaTween_Duration, () => { flashbackPickup.Enabled = false; });

                        GameHud.Instance.ResetFlashbackButton.SetActive(true);
                    }
                }
            }

            _level.Player.InputEnabled = true;
        }

        IEnumerator FlashbackHudRoutine(string flashbackDataName)
        {
            bool speedUp = true;
            TiledObject flashData = null;
            bool allowSkipByKey = true;

            if (FlashBackTriggersManager.Instance.FlashTriggersMap.TryGetValue(flashbackDataName.ToLower(),
                out var flashTrigger))
            {
                flashData = flashTrigger.FlashbackTriggerData;
            }
            //For the final flahback scene, its not a trigger so needs a different load
            else if (flashbackDataName == "final flashback")
            {
                flashData = FlashbackPickupsManager.Instance.FinalPickup.FlashbackData;
                allowSkipByKey = false;
                speedUp = false;
            }

            if (flashData != null)
            {
                var flashHud = GameHud.Instance.LoadFlashbackHud(flashData, speedUp, allowSkipByKey);

                while (flashHud != null && flashHud.toDestroy == false)
                {
                    yield return null;
                }
            }
        }

        private IEnumerator CheckPickupsCollectedOrderSquence(FlashbackPickup flashbackPickup)
        {
            _level.Player.InputEnabled = false;

            yield return new WaitForMilliSeconds(Settings.Default_AlphaTween_Duration + 200);

            //Get the original list in lowercase
            var correctOrder =
                FlashbackPickupsManager.Instance.FlashPickupsMap.Values.OrderBy(s => s.FlashbackData.Name)
                    .Select(s => s.FlashbackData.Name.ToLower());
            var collectOrder = _collectedFlashPickupsNames.Select(s => s.ToLower());

            if (correctOrder.SequenceEqual(collectOrder))
            {
                //Correct!
                Utils.print(this, " order correct ", string.Join(", ", collectOrder));

                flashbackPickup.Enabled = false;

                yield return CorrectFlashPickupsOrderSequence(flashbackPickup);
            }
            else
            {
                //Incorrect order
                Utils.print(this, " order incorrect ", string.Join(", ", collectOrder));
                yield return IncorrectFlashPickupsOrderSequence(flashbackPickup);

                GameHud.Instance.ResetFlashbackButton.SetActive(true);
            }
        }

        private IEnumerator CorrectFlashPickupsOrderSequence(FlashbackPickup flashbackPickup)
        {
            yield return new WaitForMilliSeconds(1000);

            var hiddenRoomCover = HiddenRoomCoverManager.Instance.HiddenRoomCover;
            var hiddenCollider = HiddenRoomCoverManager.Instance.HiddenRoomCoverCollider;

            DrawableTweener.TweenSpriteAlpha(hiddenRoomCover, 1, 0, 1000, Easing.Equation.QuadEaseOut);

            GameSoundManager.Instance.PlayFx(Settings.Hidden_Room_Revealed_SFX);

            yield return new WaitForMilliSeconds(1400);

            hiddenCollider.Enabled = false;

            _level.Player.InputEnabled = true;
        }

        private IEnumerator IncorrectFlashPickupsOrderSequence(FlashbackPickup flashbackPickup)
        {
            //Message to player
            GameHud.Instance.ShowTextBox(Settings.Flashback_Pickups_Incorrect_Order_Message);

            //Disable all indicators
            GameHud.Instance.MemoriesHudPanel.DisableAllIndicators();

            yield return new WaitForMilliSeconds(1000);

            _collectedFlashPickupsNames.Clear();

            _level.Player.InputEnabled = true;

            //Wait for player get out of the last pickedup to reenable all 
            while (flashbackPickup.HitTest(_level.Player))
            {
                yield return null;
            }

            yield return new WaitForMilliSeconds(Settings.Default_AlphaTween_Duration);

            flashbackPickup.visible = true;
            DrawableTweener.TweenSpriteAlpha(flashbackPickup, 0, 1, Settings.Default_AlphaTween_Duration);

            FlashbackPickupsManager.Instance.EnableFlashbackPickups();
        }

        TextBox textBox = null;

        void Update()
        {
            if (Input.GetKeyDown(Key.I))
            {
                textBox = GameHud.Instance.ShowTextBox(
                    "Vivamus commodo suscipit neque. Donec fermentum leo a risus volutpat, sit amet malesuada magna tincidunt. Ut lacinia tellus enim, vel ullamcorper odio aliquam sed. Sed id metus neque. Vestibulum sed dolor vel massa finibus vulputate. Donec quis viverra nulla, dapibus blandit turpis. Class aptent taciti sociosqu ad litora torquent per conubia nostra, per inceptos himenaeos."
                );
            }

            if (textBox != null)
            {
                Console.WriteLine(
                    $"{this}: {textBox.x} {textBox.y} | {textBox.width} {textBox.height} | gameW: {game.width} | gameH: {game.height} | hudRatioX: {GameHud.Instance.HudRatioX} hudRatioY: {GameHud.Instance.HudRatioY}");
            }
        }

        public void ResetMemorySequence()
        {
            _collectedFlashPickupsNames.Clear();
            FlashbackPickupsManager.Instance.EnableFlashbackPickups();
        }

        public int TotalFlashbacks => _totalFlashbacks;
        public int FlashbackTriggersCollectedCount => _collectedFlashBacksTriggersNames.Count;
        public int CollectedFlashPickupsCount => _collectedFlashPickupsNames.Count;

        public List<string> CollectedFlashPickupsNames => _collectedFlashPickupsNames;
    }
}