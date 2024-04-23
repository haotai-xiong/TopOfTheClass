using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class SwitchQuestion : MonoBehaviour
{
    private Color32 originalColor = new Color32(255, 255, 255, 255);
    private GameObject lastSelectedButton = null;
    public GameObject switchSubject;
    public TextMeshProUGUI attemptsText;

    public void SwitchQuestionSubject()
    {
        var clicked = EventSystem.current.currentSelectedGameObject;

        if (clicked != null)
        {
            if (lastSelectedButton != null)
            {
                lastSelectedButton.GetComponent<Image>().color = originalColor;
            }

            Color32 newHighlightColor = new Color32(12, 123, 213, 255); // Color #0C7BD5
            clicked.GetComponent<Image>().color = newHighlightColor;

            lastSelectedButton = clicked;
        }
    }

    public void AcceptButton()
    {
        if (lastSelectedButton != null)
        {
            string chosenSubject = lastSelectedButton.tag;
            switch (chosenSubject)
            {
                case "General":
                    FindObjectOfType<Questions>().nextSubject = "General";
                    break;
                case "Geography":
                    FindObjectOfType<Questions>().nextSubject = "Geography";
                    break;
                case "Science":
                    FindObjectOfType<Questions>().nextSubject = "Science";
                    break;
                case "History":
                    FindObjectOfType<Questions>().nextSubject = "History";
                    break;
                case "Maths":
                    FindObjectOfType<Questions>().nextSubject = "Maths";
                    break;
                case "English":
                    FindObjectOfType<Questions>().nextSubject = "English";
                    break;
                default:
                    Debug.LogWarning("Unknown subject selected.");
                    break;
            }

            GameManager gameManager = FindObjectOfType<GameManager>();

            for (int i = 0; i < gameManager.players.Count; i++)
            {
                PlayerController playerController = gameManager.players[i].GetComponent<PlayerController>();
                if (playerController.isActivePlayer && playerController.CanTakeTurn())
                {
                    playerController.UseTurn();
                    attemptsText.text = $"{playerController.turnsLeft}/3";
                }
            }

            lastSelectedButton.GetComponent<Image>().color = originalColor;
            lastSelectedButton = null;

            FindObjectOfType<GameManager>().ResetQuestionTimer();
            FindObjectOfType<GameManager>().StartQuestionTimer();

            for (int j = 0; j < FindObjectOfType<GameManager>().answerButtons.Length; j++)
            {
                FindObjectOfType<GameManager>().answerButtons[j].SetActive(true);
            }

            FindObjectOfType<GameManager>().subjectText.gameObject.SetActive(true);
            FindObjectOfType<GameManager>().questionText.gameObject.SetActive(true);
            FindObjectOfType<GameManager>().questionCardBackground.SetActive(true);

            for (int i = 0; i < FindObjectOfType<GameManager>().players.Count; i++)
            {
                if (FindObjectOfType<GameManager>().players[i].GetComponent<PlayerController>().isActivePlayer)
                {
                    if (FindObjectOfType<GameManager>().players[i].GetComponent<PlayerController>().playerOver12)
                    {
                        GetComponent<Questions>().SelectQuestionOver12();
                    }
                    else
                    {
                        GetComponent<Questions>().SelectQuestionUnder12();
                    }
                }

                FindObjectOfType<GameManager>().players[i].GetComponent<PlayerController>().switchSubjectScreen.SetActive(false);
            }
        }
        else
        {
            Debug.LogWarning("No subject selected.");
        }
    }

    public void UpdateAttemptsText(int turnsLeft)
    {
        if (attemptsText != null)
        {
            attemptsText.text = $"{turnsLeft}/3";
        }
        else
        {
            Debug.LogWarning("Attempts TMPro component is not assigned.", this);
        }
    }

    public void Setup(GameObject questionCard)
    {
        FindAttemptsText(questionCard);
    }

    public void FindAttemptsText(GameObject questionCard)
    {
        switchSubject = questionCard.transform.Find("Switch Subject").gameObject;
        if (switchSubject != null)
        {
            attemptsText = switchSubject.transform.Find("ss Text").GetComponent<TextMeshProUGUI>();
            if (attemptsText == null)
            {
                Debug.LogError("attemptsText not found.");
            }
        }
        else
        {
            Debug.LogError("SwitchSubject not found under Question Card.");
        }
    }

}
