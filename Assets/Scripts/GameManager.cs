using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    public DebugLogger logger;
    public MenuManager menuManager;

    [Header("Data Collection")]
    public int turnsTaken;
    public float gameCompleteTime;
    public int numberOfPlayers;
    public string player1Name;
    public Color player1Color;
    public bool isPlayer1Over12;
    private string finalStandings;

    [Header("Game Lists")]
    public List<GameObject> gameSquares;
    public List<GameObject> officeSquares;
    public List<GameObject> players;
    public List<GameObject> playersForText;
    public List<GameObject> playersCompletedGame;
    public List<Vector3> buttonPositions;

    [Header("Game Arrays")]
    public int[] officeNumbers = { 1, 11, 16, 20, 27, 31, 36, 41, 46 };
    public int[] missTurnNumbers = { 10, 26, 40 };
    public int[] move3SpacesNumbers = { 6, 15, 30, 45 };

    [Header("UI")]
    public GameObject[] answerButtons;
    public GameObject continueToPrincipalGo;
    public GameObject continueButton;
    public GameObject continueButton_2;
    public GameObject currentPlayerTurn;
    public Image currentPlayersBorder;
    public GameObject principalCard;
    public Image principalCardBorder;
    public TextMeshProUGUI principalText;
    public TextMeshProUGUI rolledText;
    public GameObject rolledTextBackground;
    public GameObject missedTurnText;
    public TextMeshProUGUI leaderboardText;
    public TextMeshProUGUI currentPlayerText;

    [Header("Question Card UI")]
    public Transform questionCardTransform;
    public GameObject okayButton;
    public GameObject questionCardBackground;
    public GameObject questionCardBorder;
    public TextMeshProUGUI questionText;
    public TextMeshProUGUI subjectText;
    public TextMeshProUGUI correctText;
    public Button button1;
    public Button button2;
    public Button button3;
    public Button button4;

    Canvas canvas;
    GameObject incorrectScreen;
    Transform endgameScreen;

    private TextMeshProUGUI correctAnswersText;
    private TextMeshProUGUI spacesMovedText;

    int correctAnswersCount = 0;
    int spacesMovedCount = 0;
    int positionOnStartOfTurn = 0;
    int positionOnEndOfTurn = 0;

    [Header("Active Player Position")]
    public float activePlayerPosX;
    public float activePlayerPosY;

    [Header("Dice Sprites")]
    public Sprite gkSprite;
    public Sprite geoSprite;
    public Sprite scienceSprite;
    public Sprite historySprite;
    public Sprite mathSprite;
    public Sprite englishSprite;

    public Sprite oneSprite;
    public Sprite twoSprite;
    public Sprite threeSprite;
    public Sprite fourSprite;
    public Sprite fiveSprite;
    public Sprite sixSprite;

    [Header("Exit Panel")]
    [SerializeField]
    private GameObject exitPanel;
    [SerializeField]
    private TextMeshProUGUI exitText;
    [SerializeField]
    private GameObject cancelButton;
    [SerializeField]
    private GameObject confirmButton;
    [SerializeField]
    private GameObject homeButton;
    private bool exitPanelToggle = false;

    [Header("Button Colour Change")]
    private Color currentColour = Color.white;
    private bool toGreen = true;
    private bool toBig = true;
    [SerializeField] bool continuePulse = true;

    [Header("Player Select Boolean")]
    public bool allowSelect = true;
    public List<int> selectedPlayerId;
    public int numOfPlayerMoveToMonth = -1;
    public List<GameObject> monthButton;
    public List<GameObject> playerNumberButton;
    public bool monthSelected = false;
    public string selectedMonth = "";

    [Header("3D Dice")]
    public GameObject diceToInstantiate;
    public Transform positionToDrop;
    bool diceFinishedRollingAnimated = false;
    bool startedAnimatedDiceRoll = false;
    int animatedNumberRolled = 1;

    // Start is called before the first frame update
    void Start()
    {
        canvas = FindObjectOfType<Canvas>();

        InstantiatePrefabInCanvas();
        findQuestionCardVariables();

        questionCardTransform = canvas.transform.Find("Question Card");
        endgameScreen = canvas.transform.Find("EndScreen");

        PrepareLists();
        PrepareTimer();
        AssignSprites();
        PreparePlayers();

        ServiceLocator.EventManager.Subscribe(GameEvent.QuestionTimerElapsed, testListener);
        ServiceLocator.EventManager.Subscribe(GameEvent.GameOverTrigger, ShowEndScreen);

        PrepareAnswersUI();
        incorrectScreen = PrepareIncorrectScreenUI();
        incorrectScreen.SetActive(false);
        SetGameObjectsToFalse();

        InitialiseShakeDetector();

        if (UIInteractionSystem.Instance != null)
        {
            CC_Timer.Instance.timeForQuestion = UIInteractionSystem.Instance.questionTimeLimit;
            CC_Timer.Instance.audioSource.volume = UIInteractionSystem.Instance.soundVolumeCoefficient;
        }
    }

    void testListener(int p)
    {
        Debug.Log("EventManager says Timer has elapsed");
        //This line can be moved from Timer allowing Timer to be separated from GameManager:
        //StartCoroutine("TurnButtonOneRed");
    }

    // Update is called once per frame
    void Update()
    {

        for (int i = 0; i < numberOfPlayers; i++)
        {
            if (players[i].GetComponent<PlayerController>().isActivePlayer)
            {
                players[i].transform.position = new Vector3(players[i].transform.position.x, 1.17f, players[i].transform.position.z);

                //Update Player Positions
                activePlayerPosX = players[i].transform.position.x;
                activePlayerPosY = players[i].transform.position.z;

                //Make buttons stand out
                FlashButtonGreen(i);
                PulseActiveButton(i);

                //Check for diceroll shake
                if (players[i].GetComponent<PlayerController>().diceRollButton.GetComponent<Button>().enabled)
                {
                    UpdateShakeDetector();
                    if (timeSinceLastShake == 0)
                    {
                        DiceRoll();
                    }
                }

                if (continuePulse)
                {
                    //Make Active Player Stand Out
                    StartCoroutine("PulseActivePlayer");
                }

                //Check if player is on go to office square
                for (int j = 0; j < officeNumbers.Length; j++)
                {
                    if (officeNumbers[j] == players[i].GetComponent<PlayerController>().currentPosition)
                    {
                        GoToOffice();
                    }
                }

                players[i].transform.position = new Vector3(players[i].transform.position.x, 1.17f, players[i].transform.position.z);
                //Check if player is on move forward 3 square
                for (int l = 0; l < move3SpacesNumbers.Length; l++)
                {
                    if (move3SpacesNumbers[l] == players[i].GetComponent<PlayerController>().currentPosition)
                    {
                        StartCoroutine(Move3(i, move3SpacesNumbers[l]));
                    }
                }

                players[i].transform.position = new Vector3(players[i].transform.position.x, 1.17f, players[i].transform.position.z);
                //Check if player is on POTY square
                if (players[i].GetComponent<PlayerController>().currentPosition == 50)
                {
                    StartCoroutine("POTY", i);
                }

                players[i].transform.position = new Vector3(players[i].transform.position.x, 1.17f, players[i].transform.position.z);
                //Check if player completed game
                if (players[i].GetComponent<PlayerController>().gameComplete)
                {
                    AnalyticsManager.Instance().LogEvent("Player: " + players[i].GetComponent<PlayerController>().playerId + " Completed Game", turnsTaken, gameCompleteTime);
                    playersCompletedGame.Add(players[i]);
                    if (playersCompletedGame.Count == numberOfPlayers)
                    {
                        finalStandings += "Player: " + playersCompletedGame[i].GetComponent<PlayerController>().playerId + ", ";
                        AnalyticsManager.Instance().LogEvent("Game Finished, First to Last Place: " + finalStandings, turnsTaken, gameCompleteTime);
                        AnalyticsManager.Instance().gamesFinished++;
                        menuManager.HomeFromGameboard();
                    }

                    players[i].transform.position = new Vector3(players[i].transform.position.x, 1.17f, players[i].transform.position.z);
                    NextPlayerTurn();
                    players.RemoveAt(i);
                }
                players[i].transform.position = new Vector3(players[i].transform.position.x, 1.17f, players[i].transform.position.z);
                UpdateSwitchSubjectText(i);

                // check if player should miss this turn or not
                IsMissingTurn(i);
            }
        }

        SetGameWinner();
        UpdateTimer(); //Question Timer
        DisplayCurrentPlayerText();
    }

    // Shake Detector ///////////////////////////////////////////////////////////////////////////////

    float shakeDetectionThreshold = 2.0f;

    float shakeInterval = 1.0f;

    float sqrShakeDetectionThreshold;
    float timeSinceLastShake;

    Vector3 acceleration;
    Vector3 lastAcceleration;

    void InitialiseShakeDetector()
    {
        sqrShakeDetectionThreshold = shakeDetectionThreshold * shakeDetectionThreshold;
        lastAcceleration = Input.acceleration;
        timeSinceLastShake = 0;
    }

    void UpdateShakeDetector()
    {
        timeSinceLastShake += Time.deltaTime;
        acceleration = Input.acceleration;
        

        Vector3 deltaAcceleration = acceleration - lastAcceleration;
        if (deltaAcceleration.sqrMagnitude >= sqrShakeDetectionThreshold && timeSinceLastShake >= shakeInterval)
        {
            timeSinceLastShake = 0;
        }

        lastAcceleration = acceleration;
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////

    private void PrepareLists()
    {
        gameSquares = new List<GameObject> { };
        officeSquares = new List<GameObject> { };
        playersCompletedGame = new List<GameObject> { };
        buttonPositions = new List<Vector3> { };

        GameObject[] objects = GameObject.FindGameObjectsWithTag("Square");
        var orderedList = objects.OrderBy(objects => objects.GetComponent<SquarePositionHolder>().squareBoardPosition);
        gameSquares = orderedList.ToList();

        GameObject[] offices = GameObject.FindGameObjectsWithTag("OfficeSquare");
        var officesOrderedList = offices.OrderBy(offices => offices.GetComponent<OfficePositionOnBoard>().officeBoardPosition);
        officeSquares = officesOrderedList.ToList();

        GameObject[] playerArray = GameObject.FindGameObjectsWithTag("PlayerPiece");
        players = playerArray.ToList();
        playersForText = playerArray.ToList();

        numberOfPlayers = PlayerPrefs.GetInt("NumberOfPlayers");
        player1Name = PlayerPrefs.GetString("Player1Name");
        
    }

    private void PrepareTimer()
    {
        CC_Timer.Instance.ShowTimer(false);
        gameCompleteTime = 0;
    }

    private void AssignSprites()
    {
        gkSprite = Resources.Load<Sprite>("DiceGk");
        geoSprite = Resources.Load<Sprite>("DiceGeo");
        scienceSprite = Resources.Load<Sprite>("DiceScience");
        historySprite = Resources.Load<Sprite>("DiceHistory");
        mathSprite = Resources.Load<Sprite>("DiceMaths");
        englishSprite = Resources.Load<Sprite>("DiceEnglish");

        oneSprite = Resources.Load<Sprite>("DiceOne");
        twoSprite = Resources.Load<Sprite>("Dice2");
        threeSprite = Resources.Load<Sprite>("Dice3");
        fourSprite = Resources.Load<Sprite>("Dice4");
        fiveSprite = Resources.Load<Sprite>("Dice5");
        sixSprite = Resources.Load<Sprite>("Dice6");
    }

    public void PrepareSinglePlayer()
    {
        if(numberOfPlayers == 1) 
        { 

        }
    }


    private void PreparePlayers()
    {
        numberOfPlayers = PlayerPrefs.GetInt("NumberOfPlayers");

        if (numberOfPlayers == 1)
        {
            players[3].SetActive(false);
            players[2].SetActive(false);
            players[1].SetActive(false);
            players.RemoveAt(3);
            players.RemoveAt(2);
            players.RemoveAt(1);

        }

        if (numberOfPlayers == 2)
        {
            players[3].SetActive(false);
            players[2].SetActive(false);
            players.RemoveAt(3);
            players.RemoveAt(2);

        }
        else if (numberOfPlayers == 3)
        {
            players[3].SetActive(false);
            players.RemoveAt(3);
        }
        else if (numberOfPlayers == 4)
        {

        }

        for (int i = 0; i < numberOfPlayers; i++)
        {
            if (players[i].GetComponent<PlayerController>().playerId == 0)
            {
                players[i].GetComponent<PlayerController>().isActivePlayer = true;
                players[i].GetComponent<PlayerController>().diceRollButton.GetComponent<Button>().enabled = false;
                players[i].GetComponent<PlayerController>().subjectRollButton.GetComponent<Button>().enabled = true;
                players[i].GetComponent<PlayerController>().playerOver12 = Convert.ToBoolean(PlayerPrefs.GetInt("Player1Over12"));
                player1Color = LoadColor("Player1Color", Color.blue);
                players[i].GetComponent<MeshRenderer>().materials[0].color = player1Color;
            }
            else
            {
                players[i].GetComponent<PlayerController>().isActivePlayer = false;
            }

            if (players[i].GetComponent<PlayerController>().playerId == 1)
            {
                players[i].GetComponent<PlayerController>().playerOver12 = FindObjectOfType<GameSetupManager>().player2Over12;
                players[i].GetComponent<MeshRenderer>().materials[0].color = FindObjectOfType<GameSetupManager>().player2Color;
            }
            else if (players[i].GetComponent<PlayerController>().playerId == 2)
            {
                players[i].GetComponent<PlayerController>().playerOver12 = FindObjectOfType<GameSetupManager>().player3Over12;
                players[i].GetComponent<MeshRenderer>().materials[0].color = FindObjectOfType<GameSetupManager>().player3Color;
            }
            else if (players[i].GetComponent<PlayerController>().playerId == 3)
            {
                players[i].GetComponent<PlayerController>().playerOver12 = FindObjectOfType<GameSetupManager>().player4Over12;
                players[i].GetComponent<MeshRenderer>().materials[0].color = FindObjectOfType<GameSetupManager>().player4Color;
            }
        }
    }

    private void PrepareAnswersUI()
    {
        answerButtons = GameObject.FindGameObjectsWithTag("AnswerButton");
        var orderedAnswers = answerButtons.OrderBy(answerButtons => answerButtons.GetComponent<AnswerIdentifier>().placeInList);
        answerButtons = orderedAnswers.ToArray();

        for (int i = 0; i < answerButtons.Length; i++)
        {
            answerButtons[i].SetActive(false);
            buttonPositions.Add(answerButtons[i].transform.position);
        }
        questionCardBorder = GameObject.FindGameObjectWithTag("QuestionCard2");
        okayButton = GameObject.FindGameObjectWithTag("OkayButton");
    }

    private GameObject PrepareIncorrectScreenUI()
    {
        GameObject incorrectPrefab;

        canvas = FindObjectOfType<Canvas>();
        incorrectPrefab = Resources.Load<GameObject>("Prefabs/incorrect");

        GameObject incorrectScreenPrefab = Instantiate(incorrectPrefab, new Vector2(0, 0), Quaternion.identity);
        incorrectScreenPrefab.transform.SetParent(canvas.transform, false); // False to not world position stays

        RectTransform rectTransform = incorrectScreenPrefab.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = new Vector2(0, 0); // Center the prefab
        rectTransform.rotation = Quaternion.Euler(0, 0, -90);

        // Make the prefab fill the entire canvas
        //rectTransform.anchorMin = new Vector2(0, 0); // Bottom left
        //rectTransform.anchorMax = new Vector2(1, 1); // Top right
        //rectTransform.offsetMin = new Vector2(0, 0); // No offset
        //rectTransform.offsetMax = new Vector2(0, 0); // No offset

        correctAnswersText = incorrectScreenPrefab.transform.Find("QuestionsCorrectText").GetComponent<TextMeshProUGUI>();
        spacesMovedText = incorrectScreenPrefab.transform.Find("SpacesMovedText").GetComponent<TextMeshProUGUI>();

        return incorrectScreenPrefab;
    }

    private void SetGameObjectsToFalse()
    {
        ShowQuestionCard(false);
        ShowOkayButton(false);
        ShowIncorrectScreen(false);
        CC_Timer.Instance.ShowTimer(false);
        ShowCorrectText(false);

        continueButton.SetActive(false);
        continueButton_2.SetActive(false);
        rolledText.gameObject.SetActive(false);
        missedTurnText.gameObject.SetActive(false);
        leaderboardText.gameObject.SetActive(false);
        principalCard.SetActive(false);
        principalText.gameObject.SetActive(false);
        exitPanel.SetActive(false);
        exitText.gameObject.SetActive(false);
        cancelButton.gameObject.SetActive(false);
        confirmButton.gameObject.SetActive(false);
        endgameScreen.gameObject.SetActive(false);

        foreach (var temp in monthButton)
        {
            temp.SetActive(false);
        }

        foreach (var temp in playerNumberButton)
        {
            temp.SetActive(false);
        }
    }

    private void UpdateTimer()
    {
        CC_Timer.Instance.TimerUpdate();
        if (CC_Timer.Instance.timerRunning)
        {
            if (CC_Timer.Instance.timeForQuestion > 0)
            {
                CC_Timer.Instance.timeForQuestion -= Time.deltaTime;
                CC_Timer.Instance.UpdateTimerColorAndText();
            }
            else
            {
                ServiceLocator.EventManager.RaiseEvent(GameEvent.QuestionTimerElapsed, 0);
                //This line doesn't need to be here anymore so you can move all the Timer code out
                //of GameManager
                for (int i = 0; i < numberOfPlayers; i++)
                {
                    if (players[i].GetComponent<PlayerController>().isActivePlayer)
                    {
                        players[i].GetComponent<PlayerController>().switchSubjectScreen.SetActive(false);
                    }
                }
                StartCoroutine("TurnButtonOneRed");
            }
        }
        gameCompleteTime += Time.deltaTime;
    }

    /// <summary>
    /// Make the dice roll button and subject roll button flash green for active player
    /// </summary>
    private void FlashButtonGreen(int activePlayer)
    {
        if (players[activePlayer].GetComponent<PlayerController>().isActivePlayer)
        {
            if (toGreen)
            {
                currentColour += new Color(-1.5f, 0, -1.5f, 0) * Time.deltaTime;

                if (players[activePlayer].GetComponent<PlayerController>().diceRollButton.GetComponent<Button>().enabled)
                {
                    players[activePlayer].GetComponent<PlayerController>().diceRollButton.GetComponent<Image>().color = currentColour;
                }
                else
                {
                    players[activePlayer].GetComponent<PlayerController>().diceRollButton.GetComponent<Image>().color = new Color(1, 0.49f, 0, 1);
                }

                if (players[activePlayer].GetComponent<PlayerController>().subjectRollButton.GetComponent<Button>().enabled)
                {
                    players[activePlayer].GetComponent<PlayerController>().subjectRollButton.GetComponent<Image>().color = currentColour;
                }
                else
                {
                    players[activePlayer].GetComponent<PlayerController>().subjectRollButton.GetComponent<Image>().color = new Color(1, 0.49f, 0, 1);
                }
            }
            else
            {
                currentColour += new Color(1.5f, 0, 1.5f, 0) * Time.deltaTime;
                if (players[activePlayer].GetComponent<PlayerController>().diceRollButton.GetComponent<Button>().enabled)
                {
                    players[activePlayer].GetComponent<PlayerController>().diceRollButton.GetComponent<Image>().color = currentColour;
                }
                else
                {
                    players[activePlayer].GetComponent<PlayerController>().diceRollButton.GetComponent<Image>().color = new Color(1, 0.49f, 0, 1);
                }

                if (players[activePlayer].GetComponent<PlayerController>().subjectRollButton.GetComponent<Button>().enabled)
                {
                    players[activePlayer].GetComponent<PlayerController>().subjectRollButton.GetComponent<Image>().color = currentColour;
                }
                else
                {
                    players[activePlayer].GetComponent<PlayerController>().subjectRollButton.GetComponent<Image>().color = new Color(1, 0.49f, 0, 1);
                }
            }

            if (currentColour.r < 0)
            {
                toGreen = false;
            }
            else if (currentColour.r > 1)
            {
                toGreen = true;
            }
        }
    }

    /// <summary>
    /// Make Active Player Pulse
    /// </summary>
    private IEnumerator PulseActivePlayer()
    {
        continuePulse = false;
        for (int i = 0; i < numberOfPlayers; i++)
        {
            if (players[i].GetComponent<PlayerController>().isActivePlayer)
            {
                players[i].transform.localScale *= 1.1f;
                yield return new WaitForSeconds(0.2f);
            }
            else
            {
                players[i].transform.localScale = new Vector3(20f, 20f, 20f);
            }

            if (players[i].GetComponent<PlayerController>().isActivePlayer)
            {

                players[i].transform.localScale /= 1.1f;
                yield return new WaitForSeconds(0.2f);
            }
            else
            {
                players[i].transform.localScale = new Vector3(20f, 20f, 20f);
            }

        }
        continuePulse = true;
    }

    /// <summary>
    /// <Make Dice Buttons Pulse
    /// </summary>
    /// <param name="activePlayer"></param>
    /// <returns></returns>
    private void PulseActiveButton(int activePlayer)
    {
        Transform trasformToChange = players[activePlayer].GetComponent<PlayerController>().diceRollButton.transform;

        if (players[activePlayer].GetComponent<PlayerController>().isActivePlayer)
        {
            if (toBig)
            {
                if (players[activePlayer].GetComponent<PlayerController>().diceRollButton.GetComponent<Button>().enabled)
                {
                    trasformToChange = players[activePlayer].GetComponent<PlayerController>().diceRollButton.transform;
                    players[activePlayer].GetComponent<PlayerController>().diceRollButton.transform.localScale = new Vector3(
                    (trasformToChange.localScale.x + 0.225f * Time.deltaTime),
                    (trasformToChange.localScale.y + 0.225f * Time.deltaTime),
                    (trasformToChange.localScale.z + 0.225f * Time.deltaTime));
                }
                else if (players[activePlayer].GetComponent<PlayerController>().subjectRollButton.GetComponent<Button>().enabled)
                {
                    trasformToChange = players[activePlayer].GetComponent<PlayerController>().subjectRollButton.transform;
                    players[activePlayer].GetComponent<PlayerController>().subjectRollButton.transform.localScale = new Vector3(
                    (trasformToChange.localScale.x + 0.225f * Time.deltaTime),
                    (trasformToChange.localScale.y + 0.225f * Time.deltaTime),
                    (trasformToChange.localScale.z + 0.225f * Time.deltaTime));
                }
                else
                {
                    players[activePlayer].GetComponent<PlayerController>().diceRollButton.transform.localScale = new Vector3(1, 1, 1);
                    players[activePlayer].GetComponent<PlayerController>().subjectRollButton.transform.localScale = new Vector3(1, 1, 1);
                }
            }
            else
            {
                if (players[activePlayer].GetComponent<PlayerController>().diceRollButton.GetComponent<Button>().enabled)
                {
                    players[activePlayer].GetComponent<PlayerController>().diceRollButton.transform.localScale = new Vector3(1, 1, 1);
                }
                else if (players[activePlayer].GetComponent<PlayerController>().subjectRollButton.GetComponent<Button>().enabled)
                {
                    players[activePlayer].GetComponent<PlayerController>().subjectRollButton.transform.localScale = new Vector3(1, 1, 1);
                }
                else
                {
                    players[activePlayer].GetComponent<PlayerController>().diceRollButton.transform.localScale = new Vector3(1, 1, 1);
                    players[activePlayer].GetComponent<PlayerController>().subjectRollButton.transform.localScale = new Vector3(1, 1, 1);
                }
            }

            if (trasformToChange.localScale.x > 1.3f)
            {
                toBig = false;
            }
            else if (trasformToChange.localScale.x <= 1.0f)
            {
                toBig = true;
            }
        }
    }

    private IEnumerator POTY(int player)
    {
        rolledText.gameObject.SetActive(true);
        rolledText.text = "Pupil Of The Year \nGo To Summer Tests";
        yield return new WaitForSeconds(2.0f);

        rolledText.gameObject.SetActive(false);
        players[player].GetComponent<PlayerController>().currentPosition = 53;
        players[player].transform.position = gameSquares[53].transform.position;
        players[player].GetComponent<PlayerController>().subjectRollButton.GetComponent<Button>().enabled = true;
        yield break;
    }

    private IEnumerator Move3(int player, int spaceOccupied)
    {
        rolledText.gameObject.SetActive(true);
        rolledTextBackground.SetActive(true);
        rolledText.text = "Move 3 Spaces Forward";
        yield return new WaitForSeconds(3.0f);

        rolledText.gameObject.SetActive(false);
        rolledTextBackground.SetActive(false);
        if (spaceOccupied == 6)
        {
            players[player].GetComponent<PlayerController>().currentPosition = 9;
            players[player].transform.position = gameSquares[9].transform.position;
        }
        else if (spaceOccupied == 15)
        {
            players[player].GetComponent<PlayerController>().currentPosition = 18;
            players[player].transform.position = gameSquares[18].transform.position;
        }
        else if (spaceOccupied == 30)
        {
            players[player].GetComponent<PlayerController>().currentPosition = 33;
            players[player].transform.position = gameSquares[33].transform.position;
        }
        else if (spaceOccupied == 45)
        {
            players[player].GetComponent<PlayerController>().currentPosition = 48;
            players[player].transform.position = gameSquares[48].transform.position;
        }

        players[player].GetComponent<PlayerController>().subjectRollButton.GetComponent<Button>().enabled = true;

        yield break;
    }

    private IEnumerator ShowRolledNum(string text, int player)
    {
        yield return new WaitForSeconds(1.0f);

        // UN COMMENT FOR EASY WIN
        //if (players[player].GetComponent<PlayerController>().currentPosition < 50)
        //{
        //    players[player].GetComponent<PlayerController>().currentPosition = 57;

        //}
        players[player].GetComponent<PlayerController>().MovePlayer();
        players[player].GetComponent<PlayerController>().diceRollButton.GetComponent<Button>().enabled = false;
        players[player].GetComponent<PlayerController>().subjectRollButton.GetComponent<Button>().enabled = true;
        CheckIfMissTurn(player);
        yield break;
    }

    private IEnumerator ShowRolledSubject(string text, int player)
    {
        yield return new WaitForSeconds(1.0f);
        if (players[player].GetComponent<PlayerController>().playerOver12)
        {
            Over12Question();
        }
        else
        {
            Under12Question();
        }
        players[player].GetComponent<PlayerController>().subjectRollButton.GetComponent<Button>().enabled = false;

        yield break;
    }

    private IEnumerator MissTurn()
    {
        missedTurnText.SetActive(true);
        yield return new WaitForSeconds(4.0f);
        missedTurnText.SetActive(false);
        yield break;
    }

    private void SetGameWinner()
    {
        if (playersCompletedGame.Count > 0)
        {
            leaderboardText.gameObject.SetActive(true);

            switch (playersCompletedGame[0].GetComponent<PlayerController>().playerId)
            {
                case 0:
                    leaderboardText.color = Color.magenta;
                    leaderboardText.text = "Pink Player Wins";
                    break;
                case 1:
                    leaderboardText.color = Color.blue;
                    leaderboardText.text = "Blue Player Wins";
                    break;
                case 2:
                    leaderboardText.color = Color.red;
                    leaderboardText.text = "Red Player Wins";
                    break;
                case 3:
                    leaderboardText.color = Color.green;
                    leaderboardText.text = "Green Player Wins";
                    break;
                default:
                    break;
            }
        }
    }

    public void DiceRoll()
    {
        StartCoroutine(RollingDiceAnimated());
        StartCoroutine(NumberRollCoroutine());
    }

    IEnumerator NumberRollCoroutine()
    {
        yield return new WaitUntil(() => diceFinishedRollingAnimated);

        int roll = animatedNumberRolled;
        spacesMovedCount += roll;

        for (int i = 0; i < numberOfPlayers; i++)
        {
            if (players[i].GetComponent<PlayerController>().isActivePlayer)
            {
                print("HELLO POP " + players[i].GetComponent<PlayerController>().currentPosition);
            }
        }

        for (int i = 0; i < numberOfPlayers; i++)
        {
            if (players[i].GetComponent<PlayerController>().isActivePlayer)
            {
                if (players[i].GetComponent<PlayerController>().finalExamsStage == false)
                {
                    switch (roll)
                    {
                        case 1:
                            players[i].GetComponent<PlayerController>().diceRollButton.GetComponent<Image>().sprite = oneSprite;
                            break;
                        case 2:
                            players[i].GetComponent<PlayerController>().diceRollButton.GetComponent<Image>().sprite = twoSprite;
                            break;
                        case 3:
                            players[i].GetComponent<PlayerController>().diceRollButton.GetComponent<Image>().sprite = threeSprite;
                            break;
                        case 4:
                            players[i].GetComponent<PlayerController>().diceRollButton.GetComponent<Image>().sprite = fourSprite;
                            break;
                        case 5:
                            players[i].GetComponent<PlayerController>().diceRollButton.GetComponent<Image>().sprite = fiveSprite;
                            break;
                        case 6:
                            players[i].GetComponent<PlayerController>().diceRollButton.GetComponent<Image>().sprite = sixSprite;
                            break;
                        default:
                            break;
                    }


                    logger.LogInformation("Player: " + i + " Rolled dice ");
                    players[i].GetComponent<PlayerController>().rolledNumber = roll;
                    StartCoroutine(ShowRolledNum(roll.ToString(), i));
                    players[i].GetComponent<PlayerController>().diceRollButton.GetComponent<Button>().enabled = false;
                }
                else
                {
                    roll = 1;
                    logger.LogInformation("Player: " + i + " In exam stage rolled dice ");
                    players[i].GetComponent<PlayerController>().rolledNumber = 1;
                    StartCoroutine(ShowRolledNum(roll.ToString(), i));
                    players[i].GetComponent<PlayerController>().diceRollButton.GetComponent<Image>().sprite = oneSprite;
                    players[i].GetComponent<PlayerController>().diceRollButton.GetComponent<Button>().enabled = false;
                }
            }
        }
        diceFinishedRollingAnimated = false;
    }

    IEnumerator RollingDiceAnimated()
    {
        // check for final exams
        for (int i = 0; i < players.Count; i++)
        {
            if (players[i].GetComponent<PlayerController>().isActivePlayer)
            {
                positionToDrop.position = new Vector3(players[i].transform.position.x, players[i].transform.position.y + 5f, players[i].transform.position.z); // put above player
                if (players[i].GetComponent<PlayerController>().finalExamsStage == true)
                {
                    diceFinishedRollingAnimated = true;
                }
            }
        }

        // is camera off?
        if (!startedAnimatedDiceRoll)
        {
            startedAnimatedDiceRoll = true;

            GameObject go = Instantiate(diceToInstantiate, positionToDrop.position, Quaternion.identity);

            yield return new WaitForSeconds(2f);

            Dice d = go.GetComponentInChildren<Dice>();
            d.Roll();
            Rigidbody rb = go.AddComponent<Rigidbody>();

            yield return new WaitUntil(() => rb.velocity == Vector3.zero);

            yield return new WaitForSeconds(1f);

            yield return new WaitUntil(() => rb.velocity == Vector3.zero);

            animatedNumberRolled = d.GetNumberFromRoll();

            yield return new WaitForSeconds(1f);

            diceFinishedRollingAnimated = true;
            yield return new WaitForSeconds(1f);
            Destroy(go);
            startedAnimatedDiceRoll = false;
        }
    }

    public void SubjectDiceRoll()
    {
        int roll = 0;
        roll += UnityEngine.Random.Range(0, 6);

        for (int i = 0; i < numberOfPlayers; i++)
        {
            if (players[i].GetComponent<PlayerController>().isActivePlayer)
            {
                if (players[i].GetComponent<PlayerController>().finalExamsStage == false)
                {
                    switch (roll)
                    {
                        case 0:
                            players[i].GetComponent<PlayerController>().subjectRollButton.GetComponent<Image>().sprite = gkSprite;
                            break;
                        case 1:
                            players[i].GetComponent<PlayerController>().subjectRollButton.GetComponent<Image>().sprite = geoSprite;
                            break;
                        case 2:
                            players[i].GetComponent<PlayerController>().subjectRollButton.GetComponent<Image>().sprite = scienceSprite;
                            break;
                        case 3:
                            players[i].GetComponent<PlayerController>().subjectRollButton.GetComponent<Image>().sprite = historySprite;
                            break;
                        case 4:
                            players[i].GetComponent<PlayerController>().subjectRollButton.GetComponent<Image>().sprite = mathSprite;
                            break;
                        case 5:
                            players[i].GetComponent<PlayerController>().subjectRollButton.GetComponent<Image>().sprite = englishSprite;
                            break;
                        default:
                            break;
                    }

                    FindObjectOfType<Questions>().nextSubject = FindObjectOfType<Questions>().subjects[roll];
                    StartCoroutine(ShowRolledSubject(FindObjectOfType<Questions>().nextSubject, i));

                    players[i].GetComponent<PlayerController>().subjectRollButton.GetComponent<Button>().enabled = false;
                }
                else
                {
                    switch (players[i].GetComponent<PlayerController>().currentPosition)
                    {
                        case 53:
                            FindObjectOfType<Questions>().nextSubject = "General";
                            players[i].GetComponent<PlayerController>().subjectRollButton.GetComponent<Image>().sprite = gkSprite;
                            break;
                        case 54:
                            FindObjectOfType<Questions>().nextSubject = "Geography";
                            players[i].GetComponent<PlayerController>().subjectRollButton.GetComponent<Image>().sprite = geoSprite;
                            break;
                        case 55:
                            FindObjectOfType<Questions>().nextSubject = "Science";
                            players[i].GetComponent<PlayerController>().subjectRollButton.GetComponent<Image>().sprite = scienceSprite;
                            break;
                        case 56:
                            FindObjectOfType<Questions>().nextSubject = "History";
                            players[i].GetComponent<PlayerController>().subjectRollButton.GetComponent<Image>().sprite = historySprite;
                            break;
                        case 57:
                            FindObjectOfType<Questions>().nextSubject = "Maths";
                            players[i].GetComponent<PlayerController>().subjectRollButton.GetComponent<Image>().sprite = mathSprite;
                            break;
                        case 58:
                            FindObjectOfType<Questions>().nextSubject = "English";
                            players[i].GetComponent<PlayerController>().subjectRollButton.GetComponent<Image>().sprite = englishSprite;
                            break;
                        default:
                            break;
                    }
                    StartCoroutine(ShowRolledSubject(FindObjectOfType<Questions>().nextSubject, i));

                    players[i].GetComponent<PlayerController>().subjectRollButton.GetComponent<Button>().enabled = false;
                }
            }
        }
    }

    public static Color LoadColor(string key, Color defaultColor)
    {
        if (PlayerPrefs.HasKey(key))
        {
            string value = PlayerPrefs.GetString(key);
            string[] parts = value.Split(',');
            if (parts.Length == 4)
            {
                return new Color(float.Parse(parts[0]), float.Parse(parts[1]), float.Parse(parts[2]), float.Parse(parts[3]));
            }
        }
        return defaultColor;
    }

    private void DisplayCurrentPlayerText()
    {
        if (playersForText[0].GetComponent<PlayerController>().isActivePlayer)
        {

            if (FindObjectOfType<GameSetupManager>() == null)
            {
                currentPlayerText.text = PlayerPrefs.GetString("Player1Name") + "'s turn";
                player1Color = LoadColor("Player1Color", Color.blue);
                currentPlayersBorder.color = player1Color;
                principalCardBorder.color = player1Color;
            }
            else
            {
                currentPlayerText.text = FindObjectOfType<GameSetupManager>().player1Name + "'s turn";
                currentPlayersBorder.color = FindObjectOfType<GameSetupManager>().player1Color;
                principalCardBorder.color = FindObjectOfType<GameSetupManager>().player1Color;
                currentPlayersBorder.color = FindObjectOfType<GameSetupManager>().player1Color;
            }
        }
        else if (playersForText[1].GetComponent<PlayerController>().isActivePlayer)
        {
            currentPlayerText.text = FindObjectOfType<GameSetupManager>().player2Name + "'s turn";
            questionCardBackground.GetComponent<Image>().color = FindObjectOfType<GameSetupManager>().player2Color;
            currentPlayersBorder.color = FindObjectOfType<GameSetupManager>().player2Color;
            principalCardBorder.color = FindObjectOfType<GameSetupManager>().player2Color;
        }
        else if (playersForText[2].GetComponent<PlayerController>().isActivePlayer)
        {
            currentPlayerText.text = FindObjectOfType<GameSetupManager>().player3Name + "'s turn";
            questionCardBackground.GetComponent<Image>().color = FindObjectOfType<GameSetupManager>().player3Color;
            currentPlayersBorder.color = FindObjectOfType<GameSetupManager>().player3Color;
            principalCardBorder.color = FindObjectOfType<GameSetupManager>().player3Color;
        }
        else if (playersForText[3].GetComponent<PlayerController>().isActivePlayer)
        {
            currentPlayerText.text = FindObjectOfType<GameSetupManager>().player4Name + "'s turn";
            questionCardBackground.GetComponent<Image>().color = FindObjectOfType<GameSetupManager>().player4Color;
            currentPlayersBorder.color = FindObjectOfType<GameSetupManager>().player4Color;
            principalCardBorder.color = FindObjectOfType<GameSetupManager>().player4Color;
        }
    }

    public void SwitchSubject()
    {
        for (int i = 0; i < numberOfPlayers; i++)
        {
            if (players[i].GetComponent<PlayerController>().isActivePlayer)
            {
                players[i].GetComponent<PlayerController>().switchSubjectScreen.SetActive(true);

                for (int j = 0; j < answerButtons.Length; j++)
                {
                    answerButtons[j].SetActive(false);
                }

                subjectText.gameObject.SetActive(false);
                questionText.gameObject.SetActive(false);
                questionCardBackground.SetActive(false);
                questionCardBorder.SetActive(false);
            }
        }
    }

    public void Under12Question()
    {
        for (int j = 0; j < answerButtons.Length; j++)
        {
            answerButtons[j].SetActive(true);
        }
        SetRandomButtonPos();
        ShowQuestionCard(true);
        StartQuestionTimer();

        GetComponent<Questions>().SelectQuestionUnder12();
    }

    public void Over12Question()
    {
        for (int j = 0; j < answerButtons.Length; j++)
        {
            answerButtons[j].SetActive(true);
        }
        SetRandomButtonPos();
        ShowQuestionCard(true);
        StartQuestionTimer();

        GetComponent<Questions>().SelectQuestionOver12();
    }

    private void ShowQuestionCard(bool showCard)
    {
        questionCardTransform.gameObject.SetActive(showCard);
        currentPlayerTurn.SetActive(!showCard);
    }

    public void ShowIncorrectScreen(bool showIncorrectScreen)
    {
        incorrectScreen.SetActive(showIncorrectScreen);
        spacesMovedCount = positionOnEndOfTurn - positionOnStartOfTurn;
        string forwardOrBackwardString = "forward";
        if (spacesMovedCount < 0)
        {
            forwardOrBackwardString = "backwards";
        }
        else if (spacesMovedCount > 0)
        {
            forwardOrBackwardString = "forward";
        }
        else if (spacesMovedCount == 0)
        {
            forwardOrBackwardString = "";
        }
        correctAnswersText.text = "You got " + correctAnswersCount.ToString() + " answers correct!";
        spacesMovedText.text = "You moved " + spacesMovedCount.ToString() + " spaces " + forwardOrBackwardString;
    }

    void ShowCorrectText(bool showCorrectText)
    {
        correctText.gameObject.SetActive(showCorrectText);
    }

    void ShowOkayButton(bool showOkayButton)
    {
        okayButton.gameObject.SetActive(showOkayButton);
    }

    //If Answered Wrong
    public void AnswerButtonOne()
    {
        StartCoroutine("TurnButtonOneRed");
    }
    public void AnswerButtonTwo()
    {
        StartCoroutine("TurnButtonTwoRed");
    }

    //If Answered Correctly
    public void CorrectAnswer()
    {
        TurnButtonGreen();
    }

    private IEnumerator TurnButtonOneRed()
    {
        CC_Timer.Instance.TimerIsRunning(false);
        CC_Timer.Instance.PlaySound(false);
        answerButtons[0].GetComponent<Image>().color = Color.green;
        answerButtons[1].GetComponent<Image>().color = Color.red;
        for (int j = 0; j < answerButtons.Length; j++)
        {
            answerButtons[j].GetComponent<Button>().enabled = false;
        }
        yield return new WaitForSeconds(3.0f);

        for (int j = 0; j < answerButtons.Length; j++)
        {
            answerButtons[j].GetComponent<Button>().enabled = true;
            answerButtons[j].SetActive(false);
            answerButtons[j].GetComponent<Image>().color = Color.white;
        }

        CC_Timer.Instance.ShowTimer(false);

        ResetQuestionTimer();
        ShowIncorrectScreen(true);
        yield return new WaitForSeconds(3.0f);
        ShowIncorrectScreen(false);
        NextPlayerTurn();

        yield break;
    }

    private IEnumerator TurnButtonTwoRed()
    {
        CC_Timer.Instance.TimerIsRunning(false);
        answerButtons[0].GetComponent<Image>().color = Color.green;
        answerButtons[2].GetComponent<Image>().color = Color.red;
        for (int j = 0; j < answerButtons.Length; j++)
        {
            answerButtons[j].GetComponent<Button>().enabled = false;
        }
        yield return new WaitForSeconds(3.0f);

        for (int j = 0; j < answerButtons.Length; j++)
        {
            answerButtons[j].GetComponent<Button>().enabled = true;
            answerButtons[j].SetActive(false);
            answerButtons[j].GetComponent<Image>().color = Color.white;
        }

        CC_Timer.Instance.ShowTimer(false);

        ResetQuestionTimer();
        ShowIncorrectScreen(true);
        yield return new WaitForSeconds(3.0f);
        ShowIncorrectScreen(false);
        NextPlayerTurn();
        yield break;
    }

    private void TurnButtonGreen()
    {
        CC_Timer.Instance.TimerIsRunning(false);
        CC_Timer.Instance.PlaySound(false);
        correctAnswersCount++;

        answerButtons[0].GetComponent<Image>().color = Color.green;

        Transform cardCanvas = canvas.transform.Find("Question Card");
        Transform outline = cardCanvas.transform.Find("Outline");
        outline.gameObject.SetActive(true);
        outline.localPosition = answerButtons[0].transform.localPosition;

        for (int j = 0; j < answerButtons.Length; j++)
        {
            answerButtons[j].GetComponent<Button>().enabled = false;
        }

        CC_Timer.Instance.ShowTimer(false);
        ShowCorrectText(true);
        ShowOkayButton(true);
    }

    public void DoAfterOkayPress()
    {
        Transform cardCanvas = canvas.transform.Find("Question Card");
        Transform outline = cardCanvas.transform.Find("Outline");
        outline.gameObject.SetActive(false);

        for (int j = 0; j < answerButtons.Length; j++)
        {
            answerButtons[j].GetComponent<Button>().enabled = true;
            answerButtons[j].SetActive(false);
            answerButtons[j].GetComponent<Image>().color = Color.white;
        }

        for (int i = 0; i < numberOfPlayers; i++)
        {
            if (players[i].GetComponent<PlayerController>().isActivePlayer)
            {
                players[i].GetComponent<PlayerController>().diceRollButton.GetComponent<Button>().enabled = true;
            }
        }

        ShowQuestionCard(false);
        ShowOkayButton(false);
        ResetQuestionTimer();
    }

    public void GoToOffice()
    {
        for (int i = 0; i < numberOfPlayers; i++)
        {
            if (players[i].GetComponent<PlayerController>().isActivePlayer)
            {
                players[i].GetComponent<PlayerController>().subjectRollButton.GetComponent<Button>().enabled = false;
                continueToPrincipalGo.SetActive(true);
                continueButton.SetActive(true); 
            }
        }
    }

    public void StartQuestionTimer()
    {
        CC_Timer.Instance.ShowTimer(true);
        CC_Timer.Instance.TimerIsRunning(true);
        CC_Timer.Instance.PlaySound(true);
    }

    public void ResetQuestionTimer()
    {
        CC_Timer.Instance.ResetTimer();
        CC_Timer.Instance.timeForQuestion = UIInteractionSystem.Instance.questionTimeLimit;
        correctText.gameObject.SetActive(false);
        ShowOkayButton(false);
        for (int i = 0; i < numberOfPlayers; i++)
        {
            if (players[i].GetComponent<PlayerController>().isActivePlayer)
            {
                positionOnEndOfTurn = players[i].GetComponent<PlayerController>().currentPosition;
                break;
            }
        }
    }

    public void NextPlayerTurn()
    {
        // ServiceLocator.EventManager.RaiseEvent(GameEvent.GameOverTrigger, 0);
        FindObjectOfType<CameraTurn>().NextPlayer();
        ShowQuestionCard(false);
        correctAnswersCount = 0;
        spacesMovedCount = 0;

        int nextActivePlayer = 0;
        int currentActivePlayer = 0;

        for (int i = 0; i < numberOfPlayers; i++)
        {
            if (players[i].GetComponent<PlayerController>().isActivePlayer)
            {
                currentActivePlayer = i;
                nextActivePlayer = i + 1;

                if (nextActivePlayer >= numberOfPlayers)
                {
                    nextActivePlayer = 0;
                }
                if (players[nextActivePlayer].GetComponent<PlayerController>().gameComplete == true)
                {
                    nextActivePlayer += 1;
                }
            }
        }

        players[currentActivePlayer].GetComponent<PlayerController>().isActivePlayer = false;
        AnalyticsManager.Instance().LogEvent("Player: " + currentActivePlayer + " Turn Over", turnsTaken, 0);
        turnsTaken++;
        AnalyticsManager.Instance().totalTurnsMade++;
        AnalyticsManager.Instance().LogEvent("Player: " + nextActivePlayer + " Turn Start", turnsTaken, 0);
        players[nextActivePlayer].GetComponent<PlayerController>().isActivePlayer = true;
        players[nextActivePlayer].GetComponent<PlayerController>().subjectRollButton.GetComponent<Button>().enabled = true;
        positionOnStartOfTurn = players[nextActivePlayer].GetComponent<PlayerController>().currentPosition;
    }

    public void ContinueToPrincipal()
    {
        for (int i = 0; i < numberOfPlayers; i++)
        {
            //Change to switch and/or simplify
            if (players[i].GetComponent<PlayerController>().isActivePlayer)
            {
                switch (players[i].GetComponent<PlayerController>().currentPosition)
                {
                    case 1: //good
                        players[i].transform.position = officeSquares[0].transform.position;
                        players[i].GetComponent<PlayerController>().currentPosition = officeSquares[0].GetComponent<OfficePositionOnBoard>().officeBoardPosition;
                        break;
                    case 11: //good
                        players[i].transform.position = officeSquares[1].transform.position;
                        players[i].GetComponent<PlayerController>().currentPosition = officeSquares[1].GetComponent<OfficePositionOnBoard>().officeBoardPosition;
                        break;
                    case 16: //bad
                        players[i].transform.position = officeSquares[0].transform.position;
                        players[i].GetComponent<PlayerController>().currentPosition = officeSquares[0].GetComponent<OfficePositionOnBoard>().officeBoardPosition;
                        break;
                    case 20: //good
                        players[i].transform.position = officeSquares[1].transform.position;
                        players[i].GetComponent<PlayerController>().currentPosition = officeSquares[1].GetComponent<OfficePositionOnBoard>().officeBoardPosition;
                        break;
                    case 27: //good
                        players[i].transform.position = officeSquares[2].transform.position;
                        players[i].GetComponent<PlayerController>().currentPosition = officeSquares[2].GetComponent<OfficePositionOnBoard>().officeBoardPosition;
                        break;
                    case 31:  //bad
                        players[i].transform.position = officeSquares[1].transform.position;
                        players[i].GetComponent<PlayerController>().currentPosition = officeSquares[1].GetComponent<OfficePositionOnBoard>().officeBoardPosition;
                        break;
                    case 36:  //bad
                        players[i].transform.position = officeSquares[2].transform.position;
                        players[i].GetComponent<PlayerController>().currentPosition = officeSquares[2].GetComponent<OfficePositionOnBoard>().officeBoardPosition;
                        break;
                    case 41: //good
                        players[i].transform.position = officeSquares[3].transform.position;
                        players[i].GetComponent<PlayerController>().currentPosition = officeSquares[3].GetComponent<OfficePositionOnBoard>().officeBoardPosition;
                        break;
                    case 46: //bad
                        players[i].transform.position = officeSquares[2].transform.position;
                        players[i].GetComponent<PlayerController>().currentPosition = officeSquares[2].GetComponent<OfficePositionOnBoard>().officeBoardPosition;
                        break;
                    default:
                        break;
                }
                continueButton.SetActive(false);
                continueToPrincipalGo.SetActive(false);

                GetComponent<PrincipalCards>().DrawRandomPrincipalCard();
                if (GetComponent<PrincipalCards>().type == "principal")
                {
                    principalCard.SetActive(true);
                    principalText.gameObject.SetActive(true);
                    players[i].GetComponent<PlayerController>().acceptRuleButton.SetActive(true);
                }
            }
        }
    }

    public void AcceptRule()
    {
        for (int i = 0; i < numberOfPlayers; i++)
        {
            if (players[i].GetComponent<PlayerController>().isActivePlayer)
            {
                if (GetComponent<PrincipalCards>().nextDirection == "forward")
                {
                    players[i].GetComponent<PlayerController>().currentPosition += GetComponent<PrincipalCards>().nextMove;
                    players[i].transform.position = FindObjectOfType<GameManager>().gameSquares[players[i].GetComponent<PlayerController>().currentPosition].transform.position;

                }
                else if (GetComponent<PrincipalCards>().nextDirection == "back")
                {
                    int newPosMinus = players[i].GetComponent<PlayerController>().currentPosition - GetComponent<PrincipalCards>().nextMove;
                    if (newPosMinus >= 0)
                    {
                        players[i].GetComponent<PlayerController>().currentPosition -= GetComponent<PrincipalCards>().nextMove;
                    }
                    else
                    {
                        players[i].GetComponent<PlayerController>().currentPosition = 0;
                    }
                    players[i].transform.position = FindObjectOfType<GameManager>().gameSquares[players[i].GetComponent<PlayerController>().currentPosition].transform.position;
                    logger.LogInformation("Rule Accepted, Next position: " + newPosMinus);
                }

                players[i].GetComponent<PlayerController>().acceptRuleButton.SetActive(false);
                principalCard.SetActive(false);
                principalText.gameObject.SetActive(false);
                players[i].GetComponent<PlayerController>().subjectRollButton.GetComponent<Button>().enabled = true;
                CheckIfMissTurn(i);
            }
        }
    }

    public void SetRandomButtonPos()
    {
        int roll = UnityEngine.Random.Range(0, 3);
        answerButtons[0].transform.position = buttonPositions[roll];

        switch (roll)
        {
            case 0:
                roll = UnityEngine.Random.Range(1, 3);
                answerButtons[1].transform.position = buttonPositions[roll];
                if (roll == 1)
                {
                    answerButtons[2].transform.position = buttonPositions[2];
                }
                else
                {
                    answerButtons[2].transform.position = buttonPositions[1];
                }
                break;
            case 1:
                answerButtons[1].transform.position = buttonPositions[0];
                answerButtons[2].transform.position = buttonPositions[2];
                break;
            case 2:
                roll = UnityEngine.Random.Range(0, 2);
                answerButtons[1].transform.position = buttonPositions[roll];
                if (roll == 0)
                {
                    answerButtons[2].transform.position = buttonPositions[1];
                }
                else
                {
                    answerButtons[2].transform.position = buttonPositions[0];
                }
                break;
            default:
                break;
        }
    }

    public void CheckIfMissTurn(int playerid)
    {
        for (int k = 0; k < missTurnNumbers.Length; k++)
        {
            Debug.Log("checking square: " + missTurnNumbers[k] + "for player: " + playerid + "who is at square: " +
                players[playerid].GetComponent<PlayerController>().currentPosition);
            if (missTurnNumbers[k] == players[playerid].GetComponent<PlayerController>().currentPosition)
            {
                StartCoroutine(MissTurn());
                ResetQuestionTimer();
                NextPlayerTurn();
            }
        }
    }

    public void ToggleExitScreen()
    {
        exitPanelToggle = !exitPanelToggle;
        exitPanel.SetActive(exitPanelToggle);
        exitText.gameObject.SetActive(exitPanelToggle);
        cancelButton.gameObject.SetActive(exitPanelToggle);
        confirmButton.gameObject.SetActive(exitPanelToggle);
    }

    public void IsMissingTurn(int t_playerId)
    {
        if (players[t_playerId].GetComponent<PlayerController>().isActivePlayer && players[t_playerId].GetComponent<PlayerController>().missingThisTurn)
        {
            StartCoroutine(MissTurn());
            ResetQuestionTimer();
            players[t_playerId].GetComponent<PlayerController>().missingThisTurn = false;
            NextPlayerTurn();
        }
    }

    void ShowEndScreen(int p)
    {
        endgameScreen.gameObject.SetActive(true);

        Transform first = endgameScreen.transform.Find("First");
        Transform second = endgameScreen.transform.Find("Second");
        Transform third = endgameScreen.transform.Find("Third");
        Transform fourth = endgameScreen.transform.Find("Fourth");

        TextMeshProUGUI firstScore = first.transform.Find("Score").GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI secondScore = second.transform.Find("Score").GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI thirdScore = third.transform.Find("Score").GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI fourthScore = fourth.transform.Find("Score").GetComponent<TextMeshProUGUI>();

        TextMeshProUGUI firstName = first.transform.Find("Name").GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI secondName = second.transform.Find("Name").GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI thirdName = third.transform.Find("Name").GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI fourthName = fourth.transform.Find("Name").GetComponent<TextMeshProUGUI>();

        int[] scores = new int[numberOfPlayers];
        string[] names = new string[numberOfPlayers];


        for (int i = 0; i < numberOfPlayers; i++)
        {
            scores[i] = players[i].GetComponent<PlayerController>().currentPosition;
            int temp = i + 1;
            string tempString = "Player" + temp.ToString() + "Name";
            names[i] = PlayerPrefs.GetString(tempString);
        }

        Array.Sort(scores, names);
        Array.Reverse(scores);
        Array.Reverse(names);


        first.gameObject.SetActive(true);
        firstScore.text = scores[0].ToString();
        firstName.text = names[0];

        if (numberOfPlayers > 1)
        {
            second.gameObject.SetActive(true);
            secondScore.text = scores[1].ToString();
            secondName.text = names[1];
        }
        if (numberOfPlayers > 2)
        {
            third.gameObject.SetActive(true);
            thirdScore.text = scores[2].ToString();
            thirdName.text = names[2];
        }
        if (numberOfPlayers > 3)
        {
            fourth.gameObject.SetActive(true);
            fourthScore.text = scores[3].ToString();
            fourthName.text = names[3];
        }
    }

    public void PlayAgain()
    {
        SceneManager.LoadScene("Gameplay");
    }
    public void MainMenu()
    {
        SceneManager.LoadScene("Main Menu");
    }
    public void More()
    {
       
    }

    public void UpdateSwitchSubjectText(int newPlayerIndex)
    {
        for (int i = 0; i < players.Count; i++)
        {
            players[i].GetComponent<PlayerController>().SetActivePlayer(i == newPlayerIndex);
        }
    }

    private void InstantiatePrefabInCanvas()
    {
        // Current cards available

        // ########  DEFAULT CARD  ########
        // CC_Default_Question Card

        // ########  THEMED CARDS  ########
        // H_Theme_Question Card

        Instantiate_Card.Instance.createCard("CC_Default_Question Card", "#1949c2");

    }

    private void findQuestionCardVariables()
    {
        GameObject questionCard = GameObject.Find("Question Card");
        if (questionCard != null)
        {
            okayButton = questionCard.transform.Find("OkayButton").gameObject;
            questionCardBackground = questionCard.transform.Find("Card Background").gameObject;
            questionCardBorder = questionCard.transform.Find("Card Frame").gameObject;
            questionText = questionCard.transform.Find("QuestionText").GetComponent<TextMeshProUGUI>();
            subjectText = questionCard.transform.Find("QuestionSubject").GetComponent<TextMeshProUGUI>();
            correctText = questionCard.transform.Find("CorrectText").GetComponent<TextMeshProUGUI>();

            button1 = questionCard.transform.Find("Answer1").GetComponent<Button>();
            button2 = questionCard.transform.Find("Answer2").GetComponent<Button>();
            button3 = questionCard.transform.Find("Answer3").GetComponent<Button>();
            button4 = questionCard.transform.Find("OkayButton").GetComponent<Button>();

            if (button1 != null)
            {
                button1.onClick.AddListener(() => CorrectAnswer());
            }
            if (button2 != null)
            {
                button2.onClick.AddListener(() => AnswerButtonOne());
            }
            if (button3 != null)
            {
                button3.onClick.AddListener(() => AnswerButtonTwo());
            }
            if (button4 != null)
            {
                button4.onClick.AddListener(() => DoAfterOkayPress());
            }

            var switchQuestion = GetComponent<SwitchQuestion>();
            var questionsScript = GetComponent<Questions>();
            switchQuestion.Setup(questionCard);
            questionsScript.Setup(questionCard);

        }
        else
        {
            Debug.LogError("Question Card not found in the hierarchy.");
        }

        if (okayButton == null) Debug.LogError("OkayButton not found.");
        if (questionCardBorder == null) Debug.LogError("QuestionCardBorder not found.");
        if (questionText == null) Debug.LogError("QuestionText not found.");
        if (subjectText == null) Debug.LogError("SubjectText not found.");
        if (correctText == null) Debug.LogError("CorrectText not found.");
    }

}