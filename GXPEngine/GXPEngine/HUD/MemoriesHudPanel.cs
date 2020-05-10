namespace GXPEngine.HUD
{
    public class MemoriesHudPanel : HudPanel
    {
        private MemoryIndicatorPanel[] _indicatorPanels;

        public MemoriesHudPanel() : base("data/Hud Memories Panel.png", true, false)
        {
            _indicatorPanels = new MemoryIndicatorPanel[6];

            for (int i = 0; i < _indicatorPanels.Length; i++)
            {
                var indicator = new MemoryIndicatorPanel(true, true);
                AddChild(indicator);
                indicator.SetXY((142 - 81) * i, 28 - 36);

                indicator.name = $"MemoryIndicatorHud_{i}";
                
                _indicatorPanels[i] = indicator;
            }
        }

        public void EnableIndicator(int index)
        {
            if (index < 0 || index >= _indicatorPanels.Length)
                return;

            _indicatorPanels[index].EnableIndicator();
            ParticleManager.Instance.PlayCoinsExplosion(_indicatorPanels[index], _indicatorPanels[index].height / 2f, _indicatorPanels[index].height / 2f);
        }
    }
}