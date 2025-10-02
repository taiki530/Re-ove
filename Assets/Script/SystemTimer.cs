using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SystemTimer : MonoBehaviour
{
    public TextMeshProUGUI timertext;
    private bool timecheck;
    private float elapsedtime = 0.0f;

    // elapsedtime を読み取り専用のpublicプロパティとして公開
    public float ElapsedTime //
    {
        get { return elapsedtime; } //
    }
    void Start() 
    {
        timecheck = true;
    }
    void Update()
    {
        if (timecheck)
        {
            elapsedtime += Time.deltaTime;
            //timertext.text = $"Time: {elapsedtime:F2}";
        }
    }
    void Reset()
    {
        elapsedtime = 0.0f;
    }
    float TimeNow()
    {
        float timenow = 0.0f;
        timenow = elapsedtime;

        return timenow;

    }
    void TimeStop()
    {
        timecheck = false;
    }
    void TimeRestart()
    {
        timecheck = true;

    }
}
