﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Reel : MonoBehaviour {
 
    public bool isSpinning;
    public static Action<GameObject> OnCenterRowStop;
    //public static Action<SlotElement> TriggerAnimation;
 
    [SerializeField] private int speed;
    private const float LowerSlotBound = -300; 
    private const float ResetSlotYValue = 600;
    private const int ZeroValue = 0;
    
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
            if (rectTransform.anchoredPosition.y <= LowerSlotBound)
                rectTransform.anchoredPosition = new Vector3(rectTransform.anchoredPosition.x, rectTransform.anchoredPosition.y + ResetSlotYValue);
        }
    }

    public void SetOriginSlotPosition()
    {
        var defaultSlotsPosition = new List<int>
            {
                200,
                100,
                0,
                -100,
                -200,
                -300
            };

        //set random original position 
        foreach (RectTransform image in transform)
        {
            var rand = Random.Range(ZeroValue, defaultSlotsPosition.Count);
            
            var anchoredPosition = image.GetComponent<RectTransform>().anchoredPosition;
            image.GetComponent<RectTransform>().anchoredPosition = new Vector3(anchoredPosition.x, defaultSlotsPosition[rand]);
            
            if (defaultSlotsPosition[rand] == ZeroValue)
            {
                OnCenterRowStop?.Invoke(image.gameObject);
                //TriggerAnimation?.Invoke(image.GetComponent<SlotElement>());
            }
            
            defaultSlotsPosition.RemoveAt(rand);
        }
    }
}