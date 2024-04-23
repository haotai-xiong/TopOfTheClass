using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public SurveyHandler surveyHandler;

    private void Start()
    {
        var screenshotTaker = ScreenshotTaker.Instance;
        if (GameObject.Find("AnalyticsManager") == null)
        {
            AnalyticsManager.Instance();
        }
    }

    public void PlaySinglePlayer()
    {
        PlayerPrefs.SetInt("NumberOfPlayers", 1);
        PlayerPrefs.SetString("Player1Name", "Player1");
        Color player1Color = Color.red;
        string colour = $"{player1Color.r}, {player1Color.g}, {player1Color.b}";
        PlayerPrefs.SetString("Player1Color", colour);
        PlayerPrefs.SetInt("Player1Over12", 1);

        SceneManager.LoadScene("Gameplay");
        
    }
    public void LocalMode()
    {
        AnalyticsManager.Instance().LogEvent("Player Chose Local Mode",0,0);
        SceneManager.LoadScene("GameSetup");
    }

    public void PlayLocalMode()
    {
        
        //SceneLoader.instance.ShowScene("Gameplay");
        SceneManager.LoadScene("Gameplay");
    }

    public void Multiplayer()
    {
        AnalyticsManager.Instance().LogEvent("Player Chose Online Mode",0,0);
        SceneManager.LoadScene("Lobby");
    }

    public void Home()
    {
        if(GameObject.Find("GameSetupManager") != null)
        {
            Destroy(GameObject.Find("GameSetupManager"));
        }
        if (GameObject.Find("NetworkManager") != null)
        {
            Destroy(GameObject.Find("NetworkManager"));
        }
        SceneManager.LoadScene("Main Menu");
        AnalyticsManager.Instance().LogSession("Return to Main Menu", AnalyticsManager.Instance().totalTurnsMade, AnalyticsManager.Instance().gamesFinished, AnalyticsManager.Instance().timeElapsed);
    }

    public void HomeFromGameboard()
    {
        if (GameObject.Find("GameSetupManager") != null)
        {
            Destroy(GameObject.Find("GameSetupManager"));
        }
        if (GameObject.Find("NetworkManager") != null)
        {
            Destroy(GameObject.Find("NetworkManager"));
        }
        SceneManager.LoadScene("Main Menu");
        AnalyticsManager.Instance().LogSession("Return to Main Menu", AnalyticsManager.Instance().totalTurnsMade, AnalyticsManager.Instance().gamesFinished, AnalyticsManager.Instance().timeElapsed);
       // surveyHandler.OpenSurvey();
    }

    public void Exit()
    {
        AnalyticsManager.Instance().LogSession("Exit Game", AnalyticsManager.Instance().totalTurnsMade, AnalyticsManager.Instance().gamesFinished,AnalyticsManager.Instance().timeElapsed);
        Application.Quit();
    }

    public void PlayAsGuestScreen()
    {
        Transform canvas = GameObject.Find("Canvas").transform;
        Transform accountTransform = canvas.transform.Find("Account");
        Transform playTransform = canvas.transform.Find("Play");
        Transform profileManagementTransform = canvas.transform.Find("Profile Management");
        Transform onlineTransform = canvas.transform.Find("Online");

        accountTransform.gameObject.SetActive(false);
        playTransform.gameObject.SetActive(true);
        profileManagementTransform.gameObject.SetActive(false);
    }

    public void ProfileManagementScreen()
    {
        Transform canvas = GameObject.Find("Canvas").transform;
        Transform accountTransform = canvas.transform.Find("Account");
        Transform profileManagementTransform = canvas.transform.Find("Profile Management");
        Transform onlineTransform = canvas.transform.Find("Online");

        accountTransform.gameObject.SetActive(false);
        profileManagementTransform.gameObject.SetActive(true);

        onlineTransform.gameObject.SetActive(false);
    }

    public void BackToAccountScreen()
    {
        Transform canvas = GameObject.Find("Canvas").transform;
        Transform accountTransform = canvas.transform.Find("Account");
        Transform profileManagementTransform = canvas.transform.Find("Profile Management");
        Transform playTransform = canvas.transform.Find("Play");
        Transform onlineTransform = canvas.transform.Find("Online");

        accountTransform.gameObject.SetActive(true);
        profileManagementTransform.gameObject.SetActive(false);
        playTransform.gameObject.SetActive(false);
        onlineTransform.gameObject.SetActive(false);
    }
    public void OnlineScreen()
    {
        Transform canvas = GameObject.Find("Canvas").transform;
        Transform accountTransform = canvas.transform.Find("Account");
        Transform profileManagementTransform = canvas.transform.Find("Profile Management");
        Transform playTransform = canvas.transform.Find("Play");
        Transform onlineTransform = canvas.transform.Find("Online");

        accountTransform.gameObject.SetActive(false);
        profileManagementTransform.gameObject.SetActive(false);
        playTransform.gameObject.SetActive(false);
        onlineTransform.gameObject.SetActive(true);
    }

    public void LoadSettingMenu()
    {
        StartCoroutine(UIInteractionSystem.Instance.ShowSettingMenu());
    }
}
