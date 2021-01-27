using System;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace GameScripts
{
    public class ElementsInfo : MonoBehaviour
    {
        [FormerlySerializedAs("spinElement")] [FormerlySerializedAs("spinElementInfo")] [FormerlySerializedAs("spinPartInfo")] public SpinElements spinElements;
        private UnityEngine.Animation animations;
        private static float Scale = 0.8f;

        private void Awake()
        {
            animations = GetComponent<UnityEngine.Animation>();
        }

        public void PlayAnimations(bool isPlaying)
        {
            animations.enabled = isPlaying;
        }

        public void ResetScale()
        {
            GetComponentInChildren<RectTransform>().localScale = new Vector3(Scale,Scale,Scale);
        }
    }

    [Serializable]
    public struct SpinElements
    {
        public string name;
        public int points;
    }
}