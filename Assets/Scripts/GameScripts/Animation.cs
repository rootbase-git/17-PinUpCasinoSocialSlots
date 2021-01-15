using System.Collections.Generic;
using UnityEngine;

namespace GameScripts
{
    public class Animation : MonoBehaviour
    {
        private List<KeyValuePair<string, GameObject>> _pairedSlot = new List<KeyValuePair<string, GameObject>>();
    
        private void Activate(List<KeyValuePair<string, GameObject>> keyValuePairs)
        {
            _pairedSlot = keyValuePairs;
            foreach (var slot in _pairedSlot)
            {
                slot.Value.GetComponent<ElementInfo>().PlayAnimation(true);
            }
        }

        public void Deactivate()
        {
            foreach (var slot in _pairedSlot)
            {
                slot.Value.GetComponent<ElementInfo>().PlayAnimation(false);
            }
            _pairedSlot.Clear();
        }
    
        private void OnEnable()
        {
            BetCalculator.TriggerAnimatio += Activate;
        }
        private void OnDisable()
        {
            BetCalculator.TriggerAnimatio -= Activate;
        }
    }
}
