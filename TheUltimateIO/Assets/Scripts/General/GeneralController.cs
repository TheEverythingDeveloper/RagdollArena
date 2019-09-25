using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class GeneralController : MonoBehaviour {

    [SerializeField]
    private TextMeshProUGUI _dayText;
    private int actualDay;

    private void Start()
    {
        TimeMaster.instance.ResetDate += DayTextUpdate;
        Load();
        _dayText.text = actualDay.ToString();
        Save();
        if (actualDay == 0)
        {
            DayTextUpdate();
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Delete))
        {
            TimeMaster.instance.ResetClockCall();
            Delete();
        }
    }

    public void DayTextUpdate()
    {
        actualDay++;
        _dayText.text = actualDay.ToString();
        Save();
    }

    public void Save()
    {
        PlayerPrefs.SetInt("actualDay", actualDay);
    }

    public void Load()
    {
        actualDay = PlayerPrefs.GetInt("actualDay");
    }

    public void Delete()
    {
        PlayerPrefs.DeleteKey("actualDay");
        actualDay = 0;
        DayTextUpdate();
    }
}
