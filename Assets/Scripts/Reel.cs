using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 
public class Reel : MonoBehaviour {
 
    public bool isSpinning;
 
    [SerializeField] private int speed;
    private const float LowerSlotBound = -300; 
    private const float UpperSlotBound = 600;
  
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
                rectTransform.anchoredPosition = new Vector3(rectTransform.anchoredPosition.x, rectTransform.anchoredPosition.y + UpperSlotBound);
        }
    }

    public void SetOriginSlotPosition()
    {
        //default slots position
        var parts = new List<int>
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
            var rand = Random.Range(0, parts.Count);
            
            var anchoredPosition = image.GetComponent<RectTransform>().anchoredPosition;
            image.GetComponent<RectTransform>().anchoredPosition = new Vector3(anchoredPosition.x, parts[rand]);
 
            parts.RemoveAt(rand);
        }
    }
}