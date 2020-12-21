using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PointsCalculator : MonoBehaviour, IHashTableUpdate<GameObject>
{
    public static Action<int> OnPointCalculated;
    public static Action<Dictionary<SlotElement, GameObject>> TriggerAnimation;
    
    //use spinPart=(same instance) to assign similar slots
    private Dictionary<SpinPart, int> _slotElementsInfo = new Dictionary<SpinPart, int>();
    //use SlotElement=(different instances) to assign All slots with
    private Dictionary<SlotElement, GameObject> _pairedElements = new Dictionary<SlotElement, GameObject>();
    //List<KeyValuePair<T1,T2>>
    [SerializeField] private int _winPoints;
    [SerializeField] private int _bet = 0;

    public void UpdateHashTableInfo(GameObject centerSpinElement)
    {
        var slotElementKey = centerSpinElement.GetComponent<SlotElement>();
        if (_slotElementsInfo.ContainsKey(slotElementKey.spinPartInfo))
        {
            _slotElementsInfo[slotElementKey.spinPartInfo] += 1;
        }
        else
        {
            _slotElementsInfo.Add(slotElementKey.spinPartInfo, 1);
        }
        
        _pairedElements.Add(slotElementKey, centerSpinElement);
    }
    [ContextMenu("showInfo")]
    public void ShowInfo()
    {
        foreach (var slot in _pairedElements)
        {
            Debug.Log(slot.Key + " "+ slot.Value);
        }
        Debug.Log(_pairedElements.Count);
    }

    private void CalculatePoints()
    {
        foreach (var slot in _slotElementsInfo.Where(slot => slot.Value > 1))
        {
            foreach (var pairedSlot in _pairedElements.ToList()
                .Where(pairedSlot => !pairedSlot.Key.spinPartInfo.name.Equals(slot.Key.name)))
            {
                //Debug.Log(pairedSlot.Key.spinPartInfo.name + " "+slot.Key.name);
                _pairedElements.Remove(pairedSlot.Key);
            }

            _winPoints += slot.Key.points * slot.Value  * _bet;
        }
        OnPointCalculated?.Invoke(_winPoints);
        TriggerAnimation?.Invoke(_pairedElements);
        
        _winPoints = 0;
        _slotElementsInfo.Clear();
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
