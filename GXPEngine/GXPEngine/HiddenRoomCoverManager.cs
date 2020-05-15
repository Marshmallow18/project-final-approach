using System;
using System.Collections;
using System.Linq;

namespace GXPEngine
{
    public class HiddenRoomCoverManager : GameObject
    {
        public static HiddenRoomCoverManager Instance;

        private BaseLevel _level;

        private Sprite _hiddenRoomCover;
        private Sprite _hiddenRoomCoverCollider;

        public HiddenRoomCoverManager(BaseLevel pLevel) : base(false)
        {
            Instance = this;
            _level = pLevel;

            //Load hidden room from tmx
            var map = _level.LevelMap.MapData;

            var hiddenRoomData = map.ObjectGroups?.Where(og => og.Name == "Hidden Room Cover").FirstOrDefault()?.Objects
                .FirstOrDefault(ob => ob.Name == "Hidden Room Cover");

            var hiddenRoomColliderData = map.ObjectGroups?.Where(og => og.Name == "Hidden Room Cover").FirstOrDefault()
                ?.Objects
                .FirstOrDefault(ob => ob.Name == "Hidden Room Collider");

            if (hiddenRoomData == null || hiddenRoomColliderData == null)
            {
                Console.WriteLine(
                    $"ERROR: Hidden Room Cover or Collider Tmx objects not found in Hidden Room Cover object layer");
            }
            else
            {
                _hiddenRoomCover = new Sprite("data/Hidden Room Cover.png", false, false);

                _hiddenRoomCover.width = Mathf.Round(hiddenRoomData.Width);
                _hiddenRoomCover.height = Mathf.Round(hiddenRoomData.Height);
                _hiddenRoomCover.SetOrigin(0, _hiddenRoomCover.texture.height);

                _level.AddChild(_hiddenRoomCover);
                _hiddenRoomCover.rotation = hiddenRoomData.rotation;
                _hiddenRoomCover.SetXY(hiddenRoomData.X, hiddenRoomData.Y);
                
                _hiddenRoomCoverCollider = new Sprite("data/White Texture.png");

                _hiddenRoomCoverCollider.width = Mathf.Round(hiddenRoomColliderData.Width);
                _hiddenRoomCoverCollider.height = Mathf.Round(hiddenRoomColliderData.Height);
                _hiddenRoomCoverCollider.SetOrigin(0, _hiddenRoomCoverCollider.texture.height);

                _level.AddChild(_hiddenRoomCoverCollider);
                _hiddenRoomCoverCollider.rotation = hiddenRoomColliderData.rotation;
                _hiddenRoomCoverCollider.SetXY(hiddenRoomColliderData.X, hiddenRoomColliderData.Y);
                _hiddenRoomCoverCollider.visible = false;

                Console.WriteLine(
                    $"{_hiddenRoomCoverCollider}: {_hiddenRoomCoverCollider.scaleX} | {_hiddenRoomCoverCollider.scaleY}");

                CoroutineManager.StartCoroutine(Start(), this);
            }
        }

        IEnumerator Start()
        {
            //Set player objects to collide after player is set
            while (_level.Player == null)
            {
                yield return null;
            }

            _level.Player.objectsToCheck = _level.Player.objectsToCheck
                .Concat(new GameObject[] {_hiddenRoomCoverCollider}).ToArray();

            Utils.print("player index", _level.Player.Index, "fog1 index", _level.Player.Fog1.Index, "fog2 index",
                _level.Player.Fog2.Index);

            //Draw over player layer
            _level.AddChildAt(_hiddenRoomCover, _level.Player.Fog2.Index);

            //Change final pickup flashback index to be below this
            while (FlashbackPickupsManager.Instance?.FinalPickup == null)
            {
                yield return null;
            }

            HierarchyManager.Instance.LateAdd(_level, FlashbackPickupsManager.Instance?.FinalPickup,
                _hiddenRoomCover.Index);
        }

        public Sprite HiddenRoomCover => _hiddenRoomCover;

        public Sprite HiddenRoomCoverCollider => _hiddenRoomCoverCollider;
    }
}