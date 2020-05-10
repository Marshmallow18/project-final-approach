using System;

namespace GXPEngine
{
    public class PlayerCheckCollision : GameObject
    {
        private Player _player;
        private CaveLevelMapGameObject _caveLevel;

        public PlayerCheckCollision(Player pPlayer, CaveLevelMapGameObject pCaveLevel)
        {
            _player = pPlayer;
            _caveLevel = pCaveLevel;
        }

        void Update()
        {
            var isWalkable = _caveLevel.IsWalkablePosition(_player.Position);

            if (!isWalkable)
            {
                var nextPos = _caveLevel.GetCollisionPOI(_player.Position, _player.lastPos);

                var normalCollision = _caveLevel.GetCollisionNormal(nextPos);
            
                Console.WriteLine($"nextpos: {nextPos} | normal: {normalCollision}");

                _player.SetXY(_player.lastPos.x, _player.lastPos.y);
            }
        }
    }
    
}