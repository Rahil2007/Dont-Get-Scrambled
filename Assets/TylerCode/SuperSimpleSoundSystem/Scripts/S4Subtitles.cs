using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


namespace TylerCode.SoundSystem
{
    /// <summary>
    /// Simple subtitle system that I bundle with S4
    /// It still supports you using your own if you have advanced features
    /// you need. This is done by subscribing to the OnSubtitlePlay event 
    /// on the S4SoundManager
    /// </summary>
    public class S4Subtitles : MonoBehaviour
    {
        [SerializeField]
        private bool _useS4Subtitles = true;
        [SerializeField]
        private TextMeshProUGUI _messageText;
        [SerializeField]
        private GameObject _subtitleParent;
        [SerializeField]
        private bool _adaptiveSubtitleTiming = true;
        [SerializeField]
        [Tooltip("Minimum time a subtitle should stay on screen when using adaptive timing, if you are NOT using " + 
            "adaptive timing, then this is the time that the subtitle will stay on screen always.")]
        private float _subtitleMinimumTime = 2;

        private float _adaptiveTimePerWord = 0.3f;

        private void Start()
        {
            if(!_useS4Subtitles)
            {
                Destroy(this);
                return;
            }
            
            if (_messageText == null)
            {
                _messageText = GetComponentInChildren<TextMeshProUGUI>();

                if (_messageText == null)
                    Debug.LogError("MessageText is unassigned, please ensure a TMProUGUI text is assigned.");
            }

            FindAnyObjectByType<S4SoundManager>().OnSubtitlePlay += SoundManager_DisplaySubtitle;
        }

        private void SoundManager_DisplaySubtitle(object sender, SubtitleEventArgs subtitleEventArgs)
        {
            DisplaySub(subtitleEventArgs.SubtitleData);
        }

        public void DisplaySub(SubtitleData caption)
        {
            string subtitleString = "";

            if(caption.IsDialog)
            {
                if (string.IsNullOrEmpty(subtitleString) == false)
                {
                    subtitleString = $"{caption.Speaker}: {caption.Subtitle}";
                }
                else
                {
                    subtitleString = caption.Subtitle;
                }
            }
            else
            {
                //TODO: 6.0 remove the jank found below!
                //subtitleString = $"[{caption.Subtitle}]";
                subtitleString = $"{caption.Subtitle}";
            }

            _messageText.text = subtitleString;
            StartShowingText();

            //Subtitle Hiding and Length
            float subtitleTiming = 0;

            if(_adaptiveSubtitleTiming)
            {
                subtitleTiming = subtitleString.Split(' ').Length * _adaptiveTimePerWord;

                if(subtitleTiming < _subtitleMinimumTime)
                {
                    subtitleTiming = _subtitleMinimumTime;
                }
            }

            Invoke("StopShowingText", subtitleTiming);
        }

        //IEnumerator FadeOutText()
        //{
        //    Color c = _messageText.color;
        //    for (float alpha = 1f; alpha >= 0; alpha -= (_fadeSpeed * Time.deltaTime))
        //    {
        //        c.a = alpha;
        //        _messageText.color = c;
        //        yield return null;
        //    }
        //}

        //IEnumerator FadeInText()
        //{
        //    Color c = _messageText.color;
        //    for (float alpha = 0f; alpha <= 1; alpha += (_fadeSpeed * Time.deltaTime))
        //    {
        //        c.a = alpha;
        //        _messageText.color = c;
        //        yield return null;
        //    }
        //}

        private void StopShowingText()
        {
            _subtitleParent.gameObject.SetActive(false);
        }

        private void StartShowingText()
        {
            _subtitleParent.gameObject.SetActive(true);
        }
    }
}
