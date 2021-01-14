using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

namespace GameScripts
{
    public class SlotsLogic : MonoBehaviour
    {
        public static Action CalculateWin;
        public static Action ActivateAd;
        public Reel[] reel;
        public bool IsPlaying => _isPlaying;

        private bool _isPlaying;
        private const int SpinsToAd = 10;
        private int _currentSpinsCount;

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
            CheckSpinsToActivateAd();
            
            CalculateWin?.Invoke();
        }

        private void CheckSpinsToActivateAd()
        {
            _currentSpinsCount++;
            if (_currentSpinsCount < SpinsToAd) return;
            ActivateAd?.Invoke();
            _currentSpinsCount = 0;
        }

        public void StartSpinning()
        {
            if(_isPlaying) return;
            StartCoroutine(Spinning());
        }
    }
}
