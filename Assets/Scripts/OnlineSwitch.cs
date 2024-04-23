using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Unity.Netcode;

public class OnlineSwitch : MonoBehaviour
{
    public void SwitchQuestionSubject()
    {
        var clicked = EventSystem.current.currentSelectedGameObject;

        string chosenSubject = clicked.tag;
        switch (chosenSubject)
        {
            case "General":
                FindObjectOfType<OnlineGameManager>().SendServerRpc("NextSubject", "General", 0);
                break;
            case "Geography":
                FindObjectOfType<OnlineGameManager>().SendServerRpc("NextSubject", "Geography", 0);
                break;
            case "Science":
                FindObjectOfType<OnlineGameManager>().SendServerRpc("NextSubject", "Science", 0);
                break;
            case "History":
                FindObjectOfType<OnlineGameManager>().SendServerRpc("NextSubject", "History", 0);
                break;
            case "Maths":
                FindObjectOfType<OnlineGameManager>().SendServerRpc("NextSubject", "Maths", 0);
                break;
            case "English":
                FindObjectOfType<OnlineGameManager>().SendServerRpc("NextSubject", "English", 0);
                break;
            default:
                break;
        }

        FindObjectOfType<OnlineGameManager>().ResetQuestionTimer();
        FindObjectOfType<OnlineGameManager>().StartQuestionTimer();

        for (int j = 0; j < FindObjectOfType<OnlineGameManager>().answerButtons.Length; j++)
        {
            FindObjectOfType<OnlineGameManager>().answerButtons[j].SetActive(true);
        }

        FindObjectOfType<OnlineGameManager>().SendServerRpc("IsQuestionAsked", "1", 1); //In this case the string 1 is used for true

        for (int i = 0; i < FindObjectOfType<OnlineGameManager>().players.Count; i++)
        {
            if (FindObjectOfType<OnlineGameManager>().players[i].GetComponent<OnlinePlayerController>().isActivePlayer.Value)
            {
                if (FindObjectOfType<OnlineGameManager>().players[i].GetComponent<OnlinePlayerController>().playerOver12)
                {
                    GetComponent<OnlineQuestions>().SelectQuestionOver12ServerRpc();
                }
                else
                {
                    GetComponent<OnlineQuestions>().SelectQuestionUnder12ServerRpc();
                }
            }

            FindObjectOfType<OnlineGameManager>().players[i].GetComponent<OnlinePlayerController>().switchToGKButton.SetActive(false);
            FindObjectOfType<OnlineGameManager>().players[i].GetComponent<OnlinePlayerController>().switchToGeographyButton.SetActive(false);
            FindObjectOfType<OnlineGameManager>().players[i].GetComponent<OnlinePlayerController>().switchToScienceButton.SetActive(false);
            FindObjectOfType<OnlineGameManager>().players[i].GetComponent<OnlinePlayerController>().switchToHistoryButton.SetActive(false);
            FindObjectOfType<OnlineGameManager>().players[i].GetComponent<OnlinePlayerController>().switchToMathsButton.SetActive(false);
            FindObjectOfType<OnlineGameManager>().players[i].GetComponent<OnlinePlayerController>().switchToEnglishButton.SetActive(false);
        }
    }
}
