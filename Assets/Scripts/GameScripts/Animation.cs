using System.Collections.Generic;
using UnityEngine;

namespace GameScripts
{
    public class Animation : MonoBehaviour
    {
        private List<KeyValuePair<string, GameObject>> _pairedMidSlot = new List<KeyValuePair<string, GameObject>>();
    
        private void ActivateMid(List<KeyValuePair<string, GameObject>> keyValuePairs)
        {
            _pairedMidSlot = keyValuePairs;
            foreach (var slot in _pairedMidSlot)
            {
                slot.Value.GetComponent<ElementsInfo>().PlayAnimations(true);
                var emission = slot.Value.GetComponentInChildren<ParticleSystem>().emission;
                emission.enabled = true;
            }
        }

        public void DeactivateMid()
        {
            foreach (var slot in _pairedMidSlot)
            {
                slot.Value.GetComponent<ElementsInfo>().PlayAnimations(false);
                var emission = slot.Value.GetComponentInChildren<ParticleSystem>().emission;
                emission.enabled = false;
            }
            _pairedMidSlot.Clear();
        }
    
        private void OnEnable()
        {
            BetCalculate.ActivateAnimation += ActivateMid;
        }
        private void OnDisable()
        {
            BetCalculate.ActivateAnimation -= ActivateMid;
        }
    }
}
