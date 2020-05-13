using System.Collections.Generic;
using System.Linq;

namespace GXPEngine
{
    public class TriggerBehavior
    {
        public bool enabled = true;
        private IHasTrigger _triggerListener;
        private bool _hasTriggerListener;

        private HashSet<GameObject> _others;
        private HashSet<GameObject> _otherToRemove;
        
        public TriggerBehavior(IHasTrigger listener)
        {
            TriggerListener = listener;
            _others = new HashSet<GameObject>();
            _otherToRemove = new HashSet<GameObject>();
        }

        public void OnTrigger(GameObject other)
        {
            if (!enabled || !_hasTriggerListener)
                return;

            if (!_others.Contains(other))
            {
                _others.Add(other);
                _triggerListener.OnEnterTrigger(other);
            }
        }

        public IHasTrigger TriggerListener
        {
            get => _triggerListener;
            set
            {
                _triggerListener = value;
                _hasTriggerListener = value != null;
            }
        }

        public void HitTest()
        {
            if (!_hasTriggerListener)
                return;

            _otherToRemove.Clear();
            foreach (var other in _others)
            {
                if (_triggerListener.gameObject.HitTest(other) == false)
                {
                    _triggerListener.OnExitTrigger(other);
                    _otherToRemove.Add(other);
                }
            }
            
            _others.ExceptWith(_otherToRemove);
        }
        
        public HashSet<GameObject> Others => _others;
    }
}