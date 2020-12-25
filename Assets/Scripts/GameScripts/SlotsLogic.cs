using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class SlotsLogic : MonoBehaviour 
{
    public static Action CalculateWin;
    public Reel[] reel;
    
    private bool _isPlaying;
 
    private void Awake ()
    {
        _isPlaying = false;
    }
#if UNITY_EDITOR     
    private void Update ()
    {
        if (_isPlaying) return;
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartSpinning();
        }
    }
#endif
    private IEnumerator Spinning()
    {
        _isPlaying = true;
        foreach (var individualReel in reel)
        {
            if (individualReel == null)
            {
                Debug.LogError("Some reels are not assigned");
                yield break;
            }
            //all reels start spinning
            individualReel.isSpinning = true;
        }
 
        foreach (var individualReel in reel)
        {
            //random spinning time for each reel
            yield return new WaitForSeconds(Random.Range(1, 3));
            
            individualReel.isSpinning = false;
            individualReel.SetOriginSlotPosition();
        }
        _isPlaying = false;
        //callback for calculator
        CalculateWin?.Invoke();
    }

    public void StartSpinning()
    {
        if(_isPlaying) return;
        StartCoroutine(Spinning());
    }
}
