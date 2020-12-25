using System.Collections.Generic;
using UnityEngine;

public class SlotAnimation : MonoBehaviour
{
    private List<KeyValuePair<string, GameObject>> _pairedSlots = new List<KeyValuePair<string, GameObject>>();
    
    private void ActivateAnimation(List<KeyValuePair<string, GameObject>> keyValuePairs)
    {
        _pairedSlots = keyValuePairs;
        foreach (var slot in _pairedSlots)
        {
            slot.Value.GetComponent<SlotElement>().PlayAnimation(true);
        }
    }

    public void DeactivateAnimations()
    {
        foreach (var slot in _pairedSlots)
        {
            slot.Value.GetComponent<SlotElement>().PlayAnimation(false);
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
