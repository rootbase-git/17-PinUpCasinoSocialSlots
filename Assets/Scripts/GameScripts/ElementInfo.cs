using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace GameScripts
{
    public class ElementInfo : MonoBehaviour
    {
        [FormerlySerializedAs("spinElementInfo")] [FormerlySerializedAs("spinPartInfo")] public SpinElement spinElement;
        private UnityEngine.Animation animator;

        private void Awake()
        {
            animator = GetComponent<UnityEngine.Animation>();
        }

        public void PlayAnimation(bool isPlaying)
        {
            animator.enabled = isPlaying;
        }
    }

    [Serializable]
    public struct SpinElement
    {
        public string name;
        public int points;
    }
}