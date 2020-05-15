namespace GXPEngine.Components
{
    public class FpsCounter : GameObject
    {
        private float _fps;
        private float _updateInterval;
        private float _timeCounter;

        public FpsCounter(float pUpdateInterval = 0.400f) : base(false)
        {
            _updateInterval = pUpdateInterval;
        }
        
        void Update()
        {
            if (_timeCounter >= _updateInterval)
            {
                _fps = game.currentFps;
                _timeCounter = 0;
            }

            _timeCounter += Time.delta;
        }
        
        public float Fps => _fps;
    }
}