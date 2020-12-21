using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;

public class SlotAnimation : MonoBehaviour
{
    private Dictionary<SlotElement, GameObject> _pairedSlots = new Dictionary<SlotElement, GameObject>();
    
    private void ActivateAnimation(Dictionary<SlotElement, GameObject> centerSpinElement)
    {
        _pairedSlots = centerSpinElement;
        foreach (var slot in _pairedSlots)
        {
            slot.Key.PlayAnimation(true);
        }
    }

    public void DeactivateAnimations()
    {
        foreach (var slot in _pairedSlots)
        {
            slot.Key.PlayAnimation(false);
        }
        _pairedSlots.Clear();
    }
    
    private void OnEnable()
    {
        PointsCalculator.TriggerAnimation += ActivateAnimation;
    }
    private void OnDisable()
    {
       PointsCalculator.TriggerAnimation -= ActivateAnimation;
    }
}
