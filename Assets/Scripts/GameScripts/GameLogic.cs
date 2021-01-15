using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace GameScripts
{
    public class GameLogic : MonoBehaviour
    {
        public static Action CalculateMoney;
        public static Action ActivateAdds;
        [FormerlySerializedAs("reel")] public SlotReel[] reels;
        public bool IsPlaying => isPlaying;

        private bool isPlaying;
        private const int SpinsToAds = 10;
        private int _currentSpins;

        private void Awake ()
        {
            isPlaying = false;
        }
#if UNITY_EDITOR     
        private void Update ()
        {
            if (isPlaying) return;
            if (Input.GetKeyDown(KeyCode.Space))
            {
                SpinningRoutine();
            }
        }
#endif
        private IEnumerator StartPlaying()
        {
            isPlaying = true;
            foreach (var individualReel in reels)
            {
                if (individualReel == null)
                {
                    Debug.LogError("Some reels are not assigned");
                    yield break;
                }
                //all reels start spinning
                individualReel.isSpin = true;
            }
 
            foreach (var individualReel in reels)
            {
                //random spinning time for each reels
                yield return new WaitForSeconds(Random.Range(1, 3));
            
                individualReel.isSpin = false;
                individualReel.SetOrigin();
            }
            isPlaying = false;
            ActivateAds();
            
            CalculateMoney?.Invoke();
        }

        private void ActivateAds()
        {
            _currentSpins++;
            if (_currentSpins < SpinsToAds) return;
            ActivateAdds?.Invoke();
            _currentSpins = 0;
        }

        public void SpinningRoutine()
        {
            if(isPlaying) return;
            StartCoroutine(StartPlaying());
        }
    }
}
