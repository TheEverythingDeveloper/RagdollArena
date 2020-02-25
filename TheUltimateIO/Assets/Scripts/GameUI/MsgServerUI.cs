using System.Collections;
using UnityEngine;
using TMPro;

namespace GameUI
{
    public class MsgServerUI : MonoBehaviour
    {
        public static MsgServerUI Instance;
        public GameObject poster;
        float _initialPositionY;
        float _finishPositionY;
        public float speedAppearPoster;
        public float speedBackPoster;
        float _duration;
        public TextMeshProUGUI textPoster;
        Coroutine coroutinePoster;

        private void Awake()
        {
            Instance = this;
            _initialPositionY = poster.transform.localPosition.y;
        }
        public void NewMsg(string text, float durationInSeconds)
        {
            textPoster.text = text;
            _duration = durationInSeconds;

            if (coroutinePoster == null)
            {
                poster.SetActive(true);
                StopAllCoroutines();
                coroutinePoster = StartCoroutine(Poster());
            }
        }

        IEnumerator Poster()
        {
            var WaitForEndOfFrame = new WaitForEndOfFrame();
            while (poster.transform.localPosition.y > _finishPositionY)
            {
                poster.transform.localPosition -= Vector3.up * speedAppearPoster * Time.deltaTime;
                yield return WaitForEndOfFrame;
            }

            coroutinePoster = null;

            yield return new WaitForSeconds(_duration);

            while (poster.transform.localPosition.y < _initialPositionY)
            {
                poster.transform.localPosition += Vector3.up * speedBackPoster * Time.deltaTime;
                yield return WaitForEndOfFrame;
            }
            poster.SetActive(false);
        }
    }
}