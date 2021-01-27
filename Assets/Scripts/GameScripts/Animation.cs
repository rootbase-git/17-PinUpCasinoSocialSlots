using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameScripts
{
    public class Animation : MonoBehaviour
    {
        private List<KeyValuePair<string, GameObject>> _pairedMidSlot = new List<KeyValuePair<string, GameObject>>();
        public GameObject[] sideParticles;
        private void ActivateMid(List<KeyValuePair<string, GameObject>> keyValuePairs)
        {
            _pairedMidSlot = keyValuePairs;
            if(_pairedMidSlot.Count >= 4)
                ActivateFirePs();
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
                slot.Value.GetComponent<ElementsInfo>().ResetScale();

                var emission = slot.Value.GetComponentInChildren<ParticleSystem>().emission;
                emission.enabled = false;
            }
            _pairedMidSlot.Clear();
        }

        public void ActivateFirePs()
        {
            foreach (var particle in sideParticles)
            {
                particle.SetActive(true);
            }
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
