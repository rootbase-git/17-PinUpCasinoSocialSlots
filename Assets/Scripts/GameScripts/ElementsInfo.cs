using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace GameScripts
{
    public class ElementsInfo : MonoBehaviour
    {
        [FormerlySerializedAs("spinElement")] [FormerlySerializedAs("spinElementInfo")] [FormerlySerializedAs("spinPartInfo")] public SpinElements spinElements;
        private UnityEngine.Animation animations;

        private void Awake()
        {
            animations = GetComponent<UnityEngine.Animation>();
        }

        public void PlayAnimations(bool isPlaying)
        {
            animations.enabled = isPlaying;
        }
    }

    [Serializable]
    public struct SpinElements
    {
        public string name;
        public int points;
    }
}