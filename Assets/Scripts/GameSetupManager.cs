using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Services.Authentication;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class GameSetupManager : MonoBehaviour
{
    private List<Color> selectedColors = new List<Color>();

    [Header("Information")]
    public GameSetupManager instance;
    public int numberOfPlayers;

    [Header("Player 1")]
    public bool player1Over12 = false;
    public bool player1ColourPickerActive = false;
    public string player1Name;
    public Color player1Color;
    Transform player1Transform;

    [Header("Player 2")]
    public bool player2Over12 = false;
    public bool player2ColourPickerActive = false;
    public string player2Name;
    public Color player2Color;
    Transform player2Transform;


    [Header("Player 3")]
    public bool player3Over12 = false;
    public bool player3ColourPickerActive = false;
    public string player3Name ;
    public Color player3Color;
    Transform player3Transform;


    [Header("Player 4")]
    public bool player4Over12 = false;
    public bool player4ColourPickerActive = false;
    public string player4Name;
    public Color player4Color;
    Transform player4Transform;


    [Header("Colour Picker")]
    public Sprite redImage;
    public Sprite greenImage;
    public Sprite blueImage;
    public Sprite purpleImage;

    void Start()
    {
        numberOfPlayers = 2;
        player1Transform = GameObject.Find("Player 1").transform;
        player2Transform = GameObject.Find("Player 2").transform;
        player3Transform = GameObject.Find("Player 3").transform;
        player4Transform = GameObject.Find("Player 4").transform;
        player1Name = "Player 1";
        player2Name = "Player 2";
        player3Name = "Player 3";
        player4Name = "Player 4";
        player1Color = Color.red;
        player2Color = Color.green;
        player3Color = Color.blue;
        player4Color = Color.magenta;

        PlayerPrefs.SetInt("NumberOfPlayers", numberOfPlayers);
        PlayerPrefs.SetString("Player1Name", player1Name);
        string colour = $"{player1Color.r}, {player1Color.g}, {player1Color.b}";
        PlayerPrefs.SetString("Player1Color", colour);


        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    // Player 1
    public void ShowPlayer1ColourPicker()
    {
        Transform colourPicker = player1Transform.Find("Colour Picker");
        player1ColourPickerActive = !player1ColourPickerActive;
        colourPicker.gameObject.SetActive(player1ColourPickerActive);
    }

    public void ChangePlayer1Age()
    {
        GameObject under12 = player1Transform.Find("Under 12").gameObject;
        GameObject over12 = player1Transform.Find("Over 12").gameObject;
        player1Over12 = !player1Over12;

        if (player1Over12)
        {
            over12.SetActive(true);
            under12.SetActive(false);
        }
        else
        {
            under12.SetActive(true);
            over12.SetActive(false);
        }
    }

    public void GetPlayer1NameFromInputBox()
    {
        TMP_InputField inputField = player1Transform.Find("Input Box").GetComponent<TMP_InputField>();
        player1Name = inputField.text;

        if (player1Name == "")
        {
            player1Name = "Player 1";
        }
        PlayerPrefs.SetString("Player1Name", player1Name);
    }

    // Player 2

    public void ShowPlayer2ColourPicker()
    {
        Transform colourPicker = player2Transform.Find("Colour Picker");
        player2ColourPickerActive = !player2ColourPickerActive;
        colourPicker.gameObject.SetActive(player2ColourPickerActive);
    }

    public void ChangePlayer2Age()
    {
        GameObject under12 = player2Transform.Find("Under 12").gameObject;
        GameObject over12 = player2Transform.Find("Over 12").gameObject;
        player2Over12 = !player2Over12;

        if (player2Over12)
        {
            over12.SetActive(true);
            under12.SetActive(false);
        }
        else
        {
            under12.SetActive(true);
            over12.SetActive(false);
        }
    }

    public void GetPlayer2NameFromInputBox()
    {
        TMP_InputField inputField = player2Transform.Find("Input Box").GetComponent<TMP_InputField>();
        player2Name = inputField.text;

        if (player2Name == "")
        {
            player2Name = "Player 2";

           
        }
        PlayerPrefs.SetString("Player2Name", player2Name);
    }

    // Player 3

    public void AddPlayer3()
    {
        numberOfPlayers = 3;
        PlayerPrefs.SetInt("NumberOfPlayers", numberOfPlayers);
        Transform player3Transform = GameObject.Find("Player 3").transform;
        Transform addPlayerTransform = player3Transform.Find("Add Player");
        Transform existingPlayerTransform = player3Transform.Find("Existing Player");
        addPlayerTransform.gameObject.SetActive(false);
        existingPlayerTransform.gameObject.SetActive(true);
        player4Transform.gameObject.SetActive(true);
        Transform addPlayer4Transform = player4Transform.Find("Add Player");
        addPlayer4Transform.gameObject.SetActive(true);

    }

    public void ShowPlayer3ColourPicker()
    {
        Transform colourPicker = player3Transform.Find("Colour Picker");
        player3ColourPickerActive = !player3ColourPickerActive;
        colourPicker.gameObject.SetActive(player3ColourPickerActive);
    }

    public void ChangePlayer3Age()
    {
        Transform existingPlayerTransform = player3Transform.Find("Existing Player");
        GameObject under12 = existingPlayerTransform.Find("Under 12").gameObject;
        GameObject over12 = existingPlayerTransform.Find("Over 12").gameObject;

        player3Over12 = !player3Over12;

        if (player3Over12)
        {
            over12.SetActive(true);
            under12.SetActive(false);
        }
        else
        {
            under12.SetActive(true);
            over12.SetActive(false);
        }
    }

    public void GetPlayer3NameFromInputBox()
    {
        Transform existingPlayerTransform = player3Transform.Find("Existing Player");
        TMP_InputField inputField = existingPlayerTransform.Find("Input Box").GetComponent<TMP_InputField>();
        player3Name = inputField.text;

        if (player3Name == "")
        {
            player3Name = "Player 3";
        }
        PlayerPrefs.SetString("Player3Name", player3Name);
    }

    // Player 4

    public void AddPlayer4()
    {
        numberOfPlayers = 4;
        PlayerPrefs.SetInt("NumberOfPlayers", numberOfPlayers);
        Transform player4Transform = GameObject.Find("Player 4").transform;
        Transform addPlayerTransform = player4Transform.Find("Add Player");
        Transform existingPlayerTransform = player4Transform.Find("Existing Player");
        addPlayerTransform.gameObject.SetActive(false);
        existingPlayerTransform.gameObject.SetActive(true);
    }

    public void ShowPlayer4ColourPicker()
    {
        Transform colourPicker = player4Transform.Find("Colour Picker");
        player4ColourPickerActive = !player4ColourPickerActive;
        colourPicker.gameObject.SetActive(player4ColourPickerActive);
    }

    public void ChangePlayer4Age()
    {
        Transform existingPlayerTransform = player4Transform.Find("Existing Player");
        GameObject under12 = existingPlayerTransform.Find("Under 12").gameObject;
        GameObject over12 = existingPlayerTransform.Find("Over 12").gameObject;
        player4Over12 = !player4Over12;

        if (player4Over12)
        {
            over12.SetActive(true);
            under12.SetActive(false);
        }
        else
        {
            under12.SetActive(true);
            over12.SetActive(false);
        }
    }
    public void GetPlayer4NameFromInputBox()
    {
        Transform existingPlayerTransform = player4Transform.Find("Existing Player");
        TMP_InputField inputField = existingPlayerTransform.Find("Input Box").GetComponent<TMP_InputField>();
        player4Name = inputField.text;

        if(player4Name == "")
        {
            player4Name = "Player 4";
        }
        PlayerPrefs.SetString("Player4Name", player4Name);
    }

    public void SetPlayer1Red()
    {
        if (selectedColors.Contains(player1Color))
        {
            selectedColors.Remove(player1Color);
        }

        if (!selectedColors.Contains(Color.red))
        {
            Transform pawnColour = player1Transform.Find("Pawn Colour");
            pawnColour.gameObject.GetComponent<Image>().sprite = redImage;
            player1Color = Color.red;
            selectedColors.Add(player1Color);
        }
    }

    public void SetPlayer1Green()
    {
        if (selectedColors.Contains(player1Color))
        {
            selectedColors.Remove(player1Color);
        }

        if (!selectedColors.Contains(Color.green))
        {
            Transform pawnColour = player1Transform.Find("Pawn Colour");
            pawnColour.gameObject.GetComponent<Image>().sprite = greenImage;
            player1Color = Color.green;
            selectedColors.Add(player1Color);
        }
    }

    public void SetPlayer1Blue()
    {
        if (selectedColors.Contains(player1Color))
        {
            selectedColors.Remove(player1Color);
        }

        if (!selectedColors.Contains(Color.blue))
        {
            Transform pawnColour = player1Transform.Find("Pawn Colour");
            pawnColour.gameObject.GetComponent<Image>().sprite = blueImage;
            player1Color = Color.blue;
            selectedColors.Add(player1Color);
        }
    }

    public void SetPlayer1Purple()
    {

        if (selectedColors.Contains(player1Color))
        {
            selectedColors.Remove(player1Color);
        }

        if (!selectedColors.Contains(Color.magenta))
        {
            Transform pawnColour = player1Transform.Find("Pawn Colour");
            pawnColour.gameObject.GetComponent<Image>().sprite = purpleImage;
            player1Color = Color.magenta;
            selectedColors.Add(player1Color);
        }
    }

    // Player 2
    public void SetPlayer2Red()
    {
        if (selectedColors.Contains(player2Color))
        {
            selectedColors.Remove(player2Color);
        }

        if (!selectedColors.Contains(Color.red))
        {
            Transform pawnColour = player2Transform.Find("Pawn Colour");
            pawnColour.gameObject.GetComponent<Image>().sprite = redImage;
            player2Color = Color.red;
            selectedColors.Add(player2Color);
        }
    }

    public void SetPlayer2Green()
    {

        if (selectedColors.Contains(player2Color))
        {
            selectedColors.Remove(player2Color);
        }

        if (!selectedColors.Contains(Color.green))
        {
            Transform pawnColour = player2Transform.Find("Pawn Colour");
            pawnColour.gameObject.GetComponent<Image>().sprite = greenImage;
            player2Color = Color.green;
            selectedColors.Add(player2Color);
        }
    }

    public void SetPlayer2Blue()
    {
        if (selectedColors.Contains(player2Color))
        {
            selectedColors.Remove(player2Color);
        }

        if (!selectedColors.Contains(Color.blue))
        {
            Transform pawnColour = player2Transform.Find("Pawn Colour");
            pawnColour.gameObject.GetComponent<Image>().sprite = blueImage;
            player2Color = Color.blue;
            selectedColors.Add(player2Color);
        }
    }

    public void SetPlayer2Purple()
    {

        if (selectedColors.Contains(player2Color))
        {
            selectedColors.Remove(player2Color);
        }

        if (!selectedColors.Contains(Color.magenta))
        {
            Transform pawnColour = player2Transform.Find("Pawn Colour");
            pawnColour.gameObject.GetComponent<Image>().sprite = purpleImage;
            player2Color = Color.magenta;
            selectedColors.Add(player2Color);
        }
    }
    //
    public void SetPlayer3Red()
    {

        if (selectedColors.Contains(player3Color))
        {
            selectedColors.Remove(player3Color);
        }

        if (!selectedColors.Contains(Color.red))
        {
            Transform pawnColour = player3Transform.Find("Pawn Colour");
            pawnColour.gameObject.GetComponent<Image>().sprite = redImage;
            player3Color = Color.red;
            selectedColors.Add(player3Color);
        }
    }

    public void SetPlayer3Green()
    {
        if (selectedColors.Contains(player3Color))
        {
            selectedColors.Remove(player3Color);
        }

        if (!selectedColors.Contains(Color.green))
        {
            Transform existingPlayerTransform = player3Transform.Find("Existing Player");
            Transform pawnColour = existingPlayerTransform.Find("Pawn Colour");
            pawnColour.gameObject.GetComponent<Image>().sprite = greenImage;
            player3Color = Color.green;
            selectedColors.Add(player3Color);
        }
    }

    public void SetPlayer3Blue()
    {
        if (selectedColors.Contains(player3Color))
        {
            selectedColors.Remove(player3Color);
        }

        if (!selectedColors.Contains(Color.blue))
        {
            Transform existingPlayerTransform = player3Transform.Find("Existing Player");
            Transform pawnColour = existingPlayerTransform.Find("Pawn Colour");
            pawnColour.gameObject.GetComponent<Image>().sprite = blueImage;
            player3Color = Color.blue;
            selectedColors.Add(player3Color);
        }
    }

    public void SetPlayer3Purple()
    {
        if (selectedColors.Contains(player3Color))
        {
            selectedColors.Remove(player3Color);
        }

        if (!selectedColors.Contains(Color.magenta))
        {
            Transform existingPlayerTransform = player3Transform.Find("Existing Player");
            Transform pawnColour = existingPlayerTransform.Find("Pawn Colour");
            pawnColour.gameObject.GetComponent<Image>().sprite = purpleImage;
            player3Color = Color.magenta;
            selectedColors.Add(player3Color);
        }
    }
    //
    public void SetPlayer4Red()
    {
        if (selectedColors.Contains(player4Color))
        {
            selectedColors.Remove(player4Color);
        }

        if (!selectedColors.Contains(Color.red))
        {
            Transform existingPlayerTransform = player4Transform.Find("Existing Player");
            Transform pawnColour = existingPlayerTransform.Find("Pawn Colour");
            pawnColour.gameObject.GetComponent<Image>().sprite = redImage;
            player4Color = Color.red;
            selectedColors.Add(player4Color);
        }
    }

    public void SetPlayer4Green()
    {
        if (selectedColors.Contains(player4Color))
        {
            selectedColors.Remove(player4Color);
        }

        if (!selectedColors.Contains(Color.green))
        {
            Transform existingPlayerTransform = player4Transform.Find("Existing Player");
            Transform pawnColour = existingPlayerTransform.Find("Pawn Colour");
            pawnColour.gameObject.GetComponent<Image>().sprite = greenImage;
            player4Color = Color.green;
            selectedColors.Add(player4Color);
        }

    }

    public void SetPlayer4Blue()
    {
        if (selectedColors.Contains(player4Color))
        {
            selectedColors.Remove(player4Color);
        }

        if (!selectedColors.Contains(Color.blue))
        {
            Transform existingPlayerTransform = player4Transform.Find("Existing Player");
            Transform pawnColour = existingPlayerTransform.Find("Pawn Colour");
            pawnColour.gameObject.GetComponent<Image>().sprite = blueImage;
            player4Color = Color.blue;
            selectedColors.Add(player4Color);
        }
    }

    public void SetPlayer4Purple()
    {
        if (selectedColors.Contains(player4Color))
        {
            selectedColors.Remove(player4Color);
        }

        if (!selectedColors.Contains(Color.magenta))
        {
            Transform existingPlayerTransform = player4Transform.Find("Existing Player");
            Transform pawnColour = existingPlayerTransform.Find("Pawn Colour");
            pawnColour.gameObject.GetComponent<Image>().sprite = purpleImage;
            player4Color = Color.magenta;
            selectedColors.Add(player4Color);
        }
    }

    public string GetPlayerName(int currentPlayer)
    {
        if(currentPlayer == 1)
        {
            return player1Name;
        }
        else if(currentPlayer == 2)
        {
            return player2Name;
        }
        else if(currentPlayer == 3)
        {
            return player3Name;
        }
        else
        { 
            return player4Name;
        }
    }
}
