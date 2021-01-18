using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace GameScripts
{
    public class Reel : MonoBehaviour 
    {
        [FormerlySerializedAs("isSpin")] public bool isSpinning;
        public static Action<GameObject> OnCenter;
 
        [FormerlySerializedAs("slotSpeed")] [SerializeField] private int speed;
        private const float LowerSlotPos = -300; 
        private const float ResetSlotYPos = 500;
        private const int Zero = 0;
    
        private void Awake()
        {
            isSpinning = false;
            speed = Random.Range(2500,3000);
        }

        private void Update()
        {
            if (!isSpinning) return;
            //get all children from main parent
            foreach (Transform image in transform)
            {
                var rectTransform = image.GetComponent<RectTransform>();
                //moving slots direction and speed
                rectTransform.Translate(Vector3.down * (Time.smoothDeltaTime * speed), Space.World);
 
                //reset image position
                if (!(rectTransform.anchoredPosition.y <= LowerSlotPos)) continue;
            
                var imagePosition = rectTransform.anchoredPosition;
                imagePosition = new Vector3(imagePosition.x, imagePosition.y + ResetSlotYPos);
                rectTransform.anchoredPosition = imagePosition;
            }
        }

        public void SetOriginPos()
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
                    OnCenter?.Invoke(image.gameObject);
                }
            
                defaultSlotsPosition.RemoveAt(rand);
            }
        }
    }
}