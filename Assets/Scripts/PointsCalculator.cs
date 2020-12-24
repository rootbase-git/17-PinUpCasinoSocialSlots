using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PointsCalculator : MonoBehaviour, IHashTableUpdate<GameObject>
{
    public static Action<int> OnPointCalculated;
    public static Action<List<KeyValuePair<string, GameObject>>> TriggerAnimation;
    
    //use spinPart=(same instance) to assign similar slots
    private Dictionary<SpinPart, int> _centeredSlotElementsInfo = new Dictionary<SpinPart, int>();
    //store all pairs of middle row
    private List<KeyValuePair<string,GameObject>> _pairedSlots = new List<KeyValuePair<string, GameObject>>();
    
    [SerializeField] private int _winPoints;
    [SerializeField] private int _bet = 0;

    public void UpdateHashTableInfo(GameObject centerSpinElement)
    {
        var slotElementKey = centerSpinElement.GetComponent<SlotElement>();
        if (_centeredSlotElementsInfo.ContainsKey(slotElementKey.spinPartInfo))
        {
            _centeredSlotElementsInfo[slotElementKey.spinPartInfo] += 1;
        }
        else
        {
            _centeredSlotElementsInfo.Add(slotElementKey.spinPartInfo, 1);
        }
        //add all elements to paired, then sort after calculation
        _pairedSlots.Add(new KeyValuePair<string, GameObject>(slotElementKey.spinPartInfo.name,centerSpinElement));
    }
    
    [ContextMenu("showInfo")]
    public void ShowInfo()
    {
        foreach (var slot in _pairedSlots)
        {
            Debug.Log(slot.Key + " "+ slot.Value);
        }
        Debug.Log(_pairedSlots.Count);
    }

    private void CalculatePoints()
    {
        //calculate win points
        foreach (var slot in _centeredSlotElementsInfo.Where(slot => slot.Value > 1))
        {
            _winPoints += slot.Key.points * slot.Value  * _bet;
        }
        //delete all elements that haven`t pair
        foreach (var slot in _centeredSlotElementsInfo.Where(slot => slot.Value <= 1))
        {
            foreach (var pairedSlot in _pairedSlots.ToList()
                .Where(pairedSlot => pairedSlot.Key.Equals(slot.Key.name)))
            {
                _pairedSlots.Remove(new KeyValuePair<string, GameObject>(pairedSlot.Key, pairedSlot.Value));
            }
        }
        //send callback to Balance and UiBalance
        OnPointCalculated?.Invoke(_winPoints);
        //send callback to animator
        TriggerAnimation?.Invoke(_pairedSlots);
        
        _winPoints = 0;
        _centeredSlotElementsInfo.Clear();
    }

    private void SetBet(int newBet)
    {
        _bet = newBet;
    }

    private void OnEnable()
    {
        BetHandler.OnBetMade += SetBet;
        Reel.OnCenterRowStop += UpdateHashTableInfo;
        SlotsLogic.CalculateWin += CalculatePoints;
    }
    private void OnDisable()
    {
        BetHandler.OnBetMade -= SetBet;
        Reel.OnCenterRowStop -= UpdateHashTableInfo;
        SlotsLogic.CalculateWin -= CalculatePoints;
    }
}
