using System.Collections;
using UnityEngine;
 
public class Slots : MonoBehaviour {
 
    public Reel[] reel;
    private bool _isPlaying;
 
    private void Awake ()
    {
        _isPlaying = false;
    }
     
    private void Update ()
    {
#if UNITY_EDITOR
        if (_isPlaying) return;
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartSpinning();
        }
#endif
    }
 
    private IEnumerator Spinning()
    {
        _isPlaying = true;
        foreach (var individualReel in reel)
        {
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
    }

    public void StartSpinning()
    {
        if(_isPlaying) return;
        StartCoroutine(Spinning());
    }
}
