using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class LevelTimer : MonoBehaviour {

    public Text txtToolTip;
    public Text txtTimePassed;
    public Text txtTimeNeeded;

    public Color dangerColor;
    public Color safeColor;

    public List<Image> changeColorImages = new List<Image>();

    int timeNeeded;
    public int timePassed = 0;
    bool timerIsOn = false;
    public bool showHundreths = false;
    public bool enoughTime = false;

    // Use this for initialization
    void Start () {
    
    }

    public void SetColors()
    {
        foreach (Image image in changeColorImages)
        {
            image.color = dangerColor;
        }
    }

    public void LevelIsSafe()
    {
        foreach (Image image in changeColorImages)
        {
            image.color = safeColor;
        }
        txtToolTip.text = "You are safe!";
        enoughTime = true;
    }

    public void Init(int timeNeeded){
        Debug.Log(timeNeeded);
        this.timeNeeded = timeNeeded;
        txtTimePassed.text = "00:00";
        txtTimeNeeded.text = FormatTime(timeNeeded);
        SetColors();
    }

    public void StartTimer()
    {
        timerIsOn = true;
    }

    public void StopTimer()
    {
        timerIsOn = false;
    }


    // Update is called once per frame
    void Update () {
        if (timerIsOn)
        {
            timePassed += (int)(Time.deltaTime * 1000);
            if (timePassed >= timeNeeded)
            {
                LevelIsSafe();
            }
            txtTimePassed.text = FormatTime(timePassed);
        }
    }

    public string FormatTime(int time)
    {
        int minutes = time / 60000;
        int seconds = time % 60000 / 1000;
        int hundreths_of_a_second = seconds / 10;
        string formattedTime = "";

        if (minutes > 0)
        {
            if (minutes < 10) {
                formattedTime += "0";
            }
            formattedTime += minutes + ":";
        }
        else
        {
            formattedTime += "00:";
        }
        if (seconds < 10)
        {
            formattedTime += "0";
        }
        formattedTime += seconds;
        if (showHundreths)
        {
            formattedTime += ".";
            if (hundreths_of_a_second < 10)
            {
                formattedTime += "0";
            }
            formattedTime += hundreths_of_a_second;
        }
        return formattedTime;
    }
}
