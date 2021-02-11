using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace GameScripts
{
    public class CoreLogic : MonoBehaviour
    {
        public static Action CalculateWin;
        public static Action ActivateAdss;
        [FormerlySerializedAs("reels")] [FormerlySerializedAs("reel")] public Reel[] slotReels;
        public bool IsPlay => isPlay;
        [FormerlySerializedAs("middlePs")] public ParticleSystem middleParticleSystem;

        private bool isPlay;
        private const int SpinsToAdds = 10;
        private int _curSpins;
        private AudioController _audioController;

        private void Awake ()
        {
            _audioController = FindObjectOfType<AudioController>();
            isPlay = false;
            Screen.orientation = ScreenOrientation.Landscape;
        }
#if UNITY_EDITOR     
        private void Update ()
        {
            if (isPlay) return;
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Spinning();
            }
        }
#endif
        private IEnumerator StartPlay()
        {
            isPlay = true;
            DisableParticleSystem();
            _audioController.PlayAudio(_audioController.spinning,true);
            foreach (var individualReel in slotReels)
            {
                if (individualReel == null)
                {
                    Debug.LogError("Some slotReels are not assigned");
                    yield break;
                }
                //all slotReels start spinning
                individualReel.isSpinning = true;
            }
 
            foreach (var individualReel in slotReels)
            {
                //random spinning time for each slotReels
                yield return new WaitForSeconds(Random.Range(1, 3));
            
                individualReel.isSpinning = false;
                individualReel.SetOriginPos();
            }
            isPlay = false;
            ActivateAd();
            
            CalculateWin?.Invoke();
        }

        private void DisableParticleSystem()
        {
            var emission = middleParticleSystem.emission;
            if (emission.enabled)
            {
                emission.enabled = false;
            }
        }

        private void ActivateAd()
        {
            _curSpins++;
            if (_curSpins < SpinsToAdds) return;
            ActivateAdss?.Invoke();
            _curSpins = 0;
        }

        public void Spinning()
        {
            if(isPlay) return;
            StartCoroutine(StartPlay());
        }
    }
}
