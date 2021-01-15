using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

namespace GameScripts
{
    public class BetCalculator : MonoBehaviour, IInfoUpdate<GameObject>
    {
        public static Action<float> OnPointCalculate;
        public static Action<List<KeyValuePair<string, GameObject>>> TriggerAnimatio;
    
        //use spinPart=(same instance) to assign similar slots
        private Dictionary<SpinElement, int> _centeredSlotElements = new Dictionary<SpinElement, int>();
        //store all pairs of middle row
        private List<KeyValuePair<string,GameObject>> _pairedSlot = new List<KeyValuePair<string, GameObject>>();
    
        [FormerlySerializedAs("_winPoints")] [SerializeField] private float winPoints;
        [FormerlySerializedAs("_bet")] [SerializeField] private float bet = 0;

        public void UpdateHash(GameObject centerSpinElement)
        {
            var slotElementKey = centerSpinElement.GetComponent<ElementInfo>();
            if (_centeredSlotElements.ContainsKey(slotElementKey.spinElement))
            {
                _centeredSlotElements[slotElementKey.spinElement] += 1;
            }
            else
            {
                _centeredSlotElements.Add(slotElementKey.spinElement, 1);
            }
            //add all elements to paired, then sort after calculation
            _pairedSlot.Add(new KeyValuePair<string, GameObject>(slotElementKey.spinElement.name,centerSpinElement));
        }

        private void CalculateWin()
        {
            //calculate win points
            foreach (var slot in _centeredSlotElements.Where(slot => slot.Value > 1))
            {
                switch (slot.Value)
                {
                    case 2: winPoints += 1.3f  * bet;
                        break;
                    case 3: winPoints += 2.5f  * bet;
                        break;
                    case 4: winPoints += 5f  * bet;
                        break;
                    case 5: winPoints += 20f  * bet;
                        break;
                }
            }
            //delete all elements that haven`t pair
            foreach (var slot in _centeredSlotElements.Where(slot => slot.Value <= 1))
            {
                foreach (var pairedSlot in _pairedSlot.ToList()
                    .Where(pairedSlot => pairedSlot.Key.Equals(slot.Key.name)))
                {
                    _pairedSlot.Remove(new KeyValuePair<string, GameObject>(pairedSlot.Key, pairedSlot.Value));
                }
            }
            //send callback to Balance and UiBalance
            OnPointCalculate?.Invoke(Mathf.RoundToInt(winPoints));
            //send callback to animator
            TriggerAnimatio?.Invoke(_pairedSlot);
        
            winPoints = 0;
            _centeredSlotElements.Clear();
        }

        private void SetMoney(float newBet)
        {
            bet = newBet;
        }

        private void OnEnable()
        {
            Betting.OnBet += SetMoney;
            SlotReel.OnCenterRow += UpdateHash;
            GameLogic.CalculateMoney += CalculateWin;
        }
        private void OnDisable()
        {
            Betting.OnBet -= SetMoney;
            SlotReel.OnCenterRow -= UpdateHash;
            GameLogic.CalculateMoney -= CalculateWin;
        }
    }
}
