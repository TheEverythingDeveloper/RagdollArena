using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System; 
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class TimeMaster : MonoBehaviour {

    public static TimeMaster instance;
    [SerializeField]
    private TextMeshProUGUI _timeText;
    public float timeLeft;
    private float _totalTime;

    public event Action ResetDate;

    private void Awake()
    {
        _totalTime = timeLeft;
        instance = this;
    }

    private void Start()
    {
        timeLeft -= GetRemainingTime();
    }

    private void Update()
    {
        timeLeft -= Time.unscaledDeltaTime;
        while (timeLeft < 0)
        {
            timeLeft += _totalTime;
            ResetDate();
        }

        float hours = Mathf.Floor(timeLeft / 3600);
        float minutes = Mathf.Floor((timeLeft / 60) % 60);
        float seconds = Mathf.RoundToInt(timeLeft % 60);

        _timeText.text = hours + ":" + minutes + ":" + seconds;
    }

    public void ResetClockCall() => ResetDate();

    private float GetRemainingTime() => (float)DateTime.Now.Subtract(DateTime.Today).TotalSeconds;
}
