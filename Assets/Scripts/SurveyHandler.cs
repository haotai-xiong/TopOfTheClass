using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static System.Net.WebRequestMethods;

public class SurveyHandler : MonoBehaviour
{
    public string surveyURL = "https://www.surveymonkey.com/r/6YHXW7Q?p="; //[p_value] == device ID

    public void OpenSurvey()
    {
        surveyURL = surveyURL + SystemInfo.deviceUniqueIdentifier;
        //Debug.Log(surveyURL);
        Application.OpenURL(surveyURL);
    }
}
