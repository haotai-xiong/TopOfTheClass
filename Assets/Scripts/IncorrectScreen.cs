using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class IncorrectScreen : MonoBehaviour
{
    //public GameManager gameManager;
    //Canvas canvas;

   // public GameObject incorrectPrefab;

    void Start()
    {
     
        //canvas = FindObjectOfType<Canvas>();
       // InstantiateIncorrectScreen();
    }

    // Update is called once per frame
    void Update()
    {
       

       // canvas = CanvasManager.items["Quests"].GetChild(2).GetComponent<TextMeshProUGUI>();
    }

    public void TurnOffInCorrectScreen()
    {
      //  GameManager gameManager = FindObjectOfType<GameManager>();
        //gameManager.ShowIncorrectScreen(false);
    }

    void InstantiateIncorrectScreen()
    {
        //GameObject temp = Instantiate(incorrectPrefab, new Vector2(0, 0), Quaternion.identity);
        //temp.transform.SetParent(canvas.transform, false); // False to not world position stays

        //RectTransform rectTransform = temp.GetComponent<RectTransform>();
        //rectTransform.anchoredPosition = new Vector2(0, 0); // Center the prefab

        //// Make the prefab fill the entire canvas
        //rectTransform.anchorMin = new Vector2(0, 0); // Bottom left
        //rectTransform.anchorMax = new Vector2(1, 1); // Top right
        //rectTransform.offsetMin = new Vector2(0, 0); // No offset
        //rectTransform.offsetMax = new Vector2(0, 0); // No offset
    }

}
