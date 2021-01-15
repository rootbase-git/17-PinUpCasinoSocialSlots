using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace GameScripts
{
    public class SlotReel : MonoBehaviour 
    {
        [FormerlySerializedAs("isSpinning")] public bool isSpin;
        public static Action<GameObject> OnCenterRow;
 
        [FormerlySerializedAs("speed")] [SerializeField] private int slotSpeed;
        private const float LowerSlot = -300; 
        private const float ResetSlotY = 500;
        private const int Zero = 0;
    
        private void Awake()
        {
            isSpin = false;
            slotSpeed = Random.Range(2500,3000);
        }

        private void Update()
        {
            if (!isSpin) return;
            //get all children from main parent
            foreach (Transform image in transform)
            {
                var rectTransform = image.GetComponent<RectTransform>();
                //moving slots direction and slotSpeed
                rectTransform.Translate(Vector3.down * (Time.smoothDeltaTime * slotSpeed), Space.World);
 
                //reset image position
                if (!(rectTransform.anchoredPosition.y <= LowerSlot)) continue;
            
                var imagePosition = rectTransform.anchoredPosition;
                imagePosition = new Vector3(imagePosition.x, imagePosition.y + ResetSlotY);
                rectTransform.anchoredPosition = imagePosition;
            }
        }

        public void SetOrigin()
        {
            var defaultSlotsPosition = new List<int>
            {
                200,
                100,
                0,
                -100,
                -200
            };

            //set random original position 
            foreach (RectTransform image in transform)
            {
                var rand = Random.Range(Zero, defaultSlotsPosition.Count);
            
                var anchoredPosition = image.GetComponent<RectTransform>().anchoredPosition;
                image.GetComponent<RectTransform>().anchoredPosition = new Vector3(anchoredPosition.x, defaultSlotsPosition[rand]);
            
                if (defaultSlotsPosition[rand] == Zero)
                {
                    OnCenterRow?.Invoke(image.gameObject);
                }
            
                defaultSlotsPosition.RemoveAt(rand);
            }
        }
    }
}