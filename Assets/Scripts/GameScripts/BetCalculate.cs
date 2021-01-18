using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

namespace GameScripts
{
    public class BetCalculate : MonoBehaviour, IInfoUpdatable<GameObject>
    {
        public static Action<float> OnCalculate;
        public static Action<List<KeyValuePair<string, GameObject>>> ActivateAnimation;
    
        //use spinPart=(same instance) to assign similar slots
        private Dictionary<SpinElements, int> _centeredSlot = new Dictionary<SpinElements, int>();
        //store all pairs of middle row
        private List<KeyValuePair<string,GameObject>> _pairedSlotElements = new List<KeyValuePair<string, GameObject>>();
    
        private float winVal;
        private float betVal = 0;
        private AudioController _audio;

        private void Awake()
        {
            _audio = FindObjectOfType<AudioController>();
        }

        public void UpdateElement(GameObject centerSpinElement)
        {
            var slotElementKey = centerSpinElement.GetComponent<ElementsInfo>();
            if (_centeredSlot.ContainsKey(slotElementKey.spinElements))
            {
                _centeredSlot[slotElementKey.spinElements] += 1;
            }
            else
            {
                _centeredSlot.Add(slotElementKey.spinElements, 1);
            }
            //add all elements to paired, then sort after calculation
            _pairedSlotElements.Add(new KeyValuePair<string, GameObject>(slotElementKey.spinElements.name,centerSpinElement));
        }

        private void CalculatePoints()
        {
            //calculate win points
            foreach (var slot in _centeredSlot.Where(slot => slot.Value > 1))
            {
                switch (slot.Value)
                {
                    case 2: winVal += 1.3f  * betVal;
                        break;
                    case 3: winVal += 2.5f  * betVal;
                        break;
                    case 4: winVal += 5f  * betVal;
                        break;
                    case 5: winVal += 20f  * betVal;
                        break;
                }
            }
            //delete all elements that haven`t pair
            foreach (var slot in _centeredSlot.Where(slot => slot.Value <= 1))
            {
                foreach (var pairedSlot in _pairedSlotElements.ToList()
                    .Where(pairedSlot => pairedSlot.Key.Equals(slot.Key.name)))
                {
                    _pairedSlotElements.Remove(new KeyValuePair<string, GameObject>(pairedSlot.Key, pairedSlot.Value));
                }
            }
            //send callback to Balance and UiBalance
            OnCalculate?.Invoke(Mathf.RoundToInt(winVal));
            
            //send callback to animator
            ActivateAnimation?.Invoke(_pairedSlotElements);
            
            if (winVal > 0)
                _audio.PlayAudio(_audio.win, false);
            else
                _audio.StopAudio();

            winVal = 0;
            _centeredSlot.Clear();
        }

        private void SetWinMoney(float newBet)
        {
            betVal = newBet;
        }

        private void OnEnable()
        {
            BetMoney.OnBetMade += SetWinMoney;
            Reel.OnCenter += UpdateElement;
            CoreLogic.CalculateWin += CalculatePoints;
        }
        private void OnDisable()
        {
            BetMoney.OnBetMade -= SetWinMoney;
            Reel.OnCenter -= UpdateElement;
            CoreLogic.CalculateWin -= CalculatePoints;
        }
    }
}
