using System;
using UnityEngine;

public class SlotElement : MonoBehaviour
{
    public SpinPart spinPartInfo;
    private Animation _animator;

    private void Awake()
    {
        _animator = GetComponent<Animation>();
    }

    public void PlayAnimation(bool isPlaying)
    {
        _animator.enabled = isPlaying;
    }
}

[Serializable]
public struct SpinPart
{
    public string name;
    public int points;
}