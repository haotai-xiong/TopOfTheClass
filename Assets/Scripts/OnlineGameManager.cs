using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;
using UnityEngine.UI;
using Unity.Netcode;
using Unity.Collections;

public class OnlineGameManager : NetworkBehaviour
{
    public DebugLogger logger;
    public MenuManager menuManager;

    [Header("Data Collection")]
    public int turnsTaken;
    public float gameCompleteTime;
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
    public NetworkVariable<FixedString128Bytes> answerTextOne = new NetworkVariable<FixedString128Bytes>("");
    public NetworkVariable<FixedString128Bytes> answerTextTwo = new NetworkVariable<FixedString128Bytes>("");
    public NetworkVariable<FixedString128Bytes> answerTextThree = new NetworkVariable<FixedString128Bytes>("");
    public NetworkVariable<bool> isQuestionAsked = new NetworkVariable<bool>(false);
    public GameObject[] timerComponent;

    public GameObject[] diceRollButtons;
    public GameObject[] subjectRollButtons;

    public GameObject questionCard;
    public TextMeshProUGUI questionText;
    public NetworkVariable<FixedString128Bytes> questionTextNetString = new NetworkVariable<FixedString128Bytes>("");
    public TextMeshProUGUI subjectText;
    public NetworkVariable<FixedString128Bytes> subjectTextNetString = new NetworkVariable<FixedString128Bytes>("");

    public GameObject principalCard;
    public TextMeshProUGUI principalText;

    public TextMeshProUGUI rolledText;
    public TextMeshProUGUI missedTurnText;
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI leaderboardText;
    public TextMeshProUGUI currentPlayerText;

    [Header("Player Position")]
    public float activePlayerPosX;
    public float activePlayerPosY;

    [Header("Timer")]
    public float timeForQuestion = 30.0f;
    public bool timerRunning = false;

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
    Color currentColour = Color.white;
    private bool toGreen = true;
    private bool toBig = true;
    [SerializeField] bool continuePulse = true;

    GameManager gameManager;


    // Start is called before the first frame update
    void Start()
    {
        logger.LogInformation("Start called");

        diceRollButtons = GameObject.FindGameObjectsWithTag("NumberDice");
        subjectRollButtons = GameObject.FindGameObjectsWithTag("SubjectDice");

        for (int i = 0; i < diceRollButtons.Length; i++)
        {
            diceRollButtons[i].SetActive(false);
            subjectRollButtons[i].SetActive(false);
        }

        PrepareLists();
        AssignSprites(); //Assign all images from Resource

        timerComponent = GameObject.FindGameObjectsWithTag("Timer");
        for (int i = 0; i < timerComponent.Length; i++)
        {
            timerComponent[i].SetActive(false);
        }
        gameCompleteTime = 0;

        PreparePlayers();

        answerButtons = GameObject.FindGameObjectsWithTag("AnswerButton");
        var orderedAnswers = answerButtons.OrderBy(answerButtons => answerButtons.GetComponent<AnswerIdentifier>().placeInList);
        answerButtons = orderedAnswers.ToArray();
        for (int i = 0; i < answerButtons.Length; i++)
        {
            answerButtons[i].SetActive(false);
            buttonPositions.Add(answerButtons[i].transform.position);
            Debug.Log(buttonPositions[i]);
        }
        questionCard = GameObject.FindGameObjectWithTag("QuestionCard");

        SetGameObjectsToFalse();


        for (int i = 0; i < players.Count; i++)
        {
            if (players[i].GetComponent<OnlinePlayerController>().IsOwner)
            {
                players[i].GetComponent<OnlinePlayerController>().diceRollButton.SetActive(true);
                players[i].GetComponent<OnlinePlayerController>().subjectRollButton.SetActive(true);
            }
            else
            {
                players[i].GetComponent<OnlinePlayerController>().diceRollButton.SetActive(false);
                players[i].GetComponent<OnlinePlayerController>().subjectRollButton.SetActive(false);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

        if (players.Count < 1)
        {
            AnalyticsManager.Instance().LogEvent("Game Finished, First to Last Place: " + finalStandings, turnsTaken, gameCompleteTime);
            AnalyticsManager.Instance().gamesFinished++;
            menuManager.HomeFromGameboard();
        }

        for (int i = 0; i < players.Count; i++)
        {
            if (players[i].GetComponent<OnlinePlayerController>().gameComplete.Value)
            {
                SendServerRpc("CurrentPlayerStopTurn", i.ToString(), 0);
                players[i].GetComponent<OnlinePlayerController>().DisableAllButtons();
                logger.LogInformation("Currently all are buttons are disabled for: " + i);
                playersCompletedGame.Add(players[i]);
                players.RemoveAt(i);
                NextPlayerAfterOneCompleted(i);
            }
            else
            {
                if (players[i].GetComponent<OnlinePlayerController>().isActivePlayer.Value)
                {
                    if (players[i] == null)
                    {
                        AnalyticsManager.Instance().LogEvent("Game Finished, First to Last Place: " + finalStandings, turnsTaken, gameCompleteTime);
                        AnalyticsManager.Instance().gamesFinished++;
                        menuManager.HomeFromGameboard();
                    }
                    if (players[i].GetComponent<NetworkObject>().IsOwner)
                    {
                        for (int j = 0; j < answerButtons.Length; j++)
                        {
                            answerButtons[j].GetComponent<Button>().enabled = true;
                        }
                    }
                    else
                    {
                        for (int j = 0; j < answerButtons.Length; j++)
                        {
                            answerButtons[j].GetComponent<Button>().enabled = false;
                        }
                    }
                    //Update position
                    activePlayerPosX = players[i].transform.position.x;
                    activePlayerPosY = players[i].transform.position.y;

                    //Make buttons stand out
                    FlashButtonGreen(i);
                    PulseActiveButton(i);

                    if (continuePulse)
                    {
                        //Make Active Player Stand Out
                        StartCoroutine("PulseActivePlayer");
                    }

                    //Check if player is on go to office square
                    for (int j = 0; j < officeNumbers.Length; j++)
                    {
                        if (officeNumbers[j] == players[i].GetComponent<OnlinePlayerController>().currentPosition.Value)
                        {
                            GoToOffice();
                        }
                    }

                    //Check if player is on move forward 3 square
                    for (int l = 0; l < move3SpacesNumbers.Length; l++)
                    {
                        if (move3SpacesNumbers[l] == players[i].GetComponent<OnlinePlayerController>().currentPosition.Value)
                        {
                            StartCoroutine(Move3(i, move3SpacesNumbers[l]));
                        }
                    }

                    //Check if player is on POTY square
                    if (players[i].GetComponent<OnlinePlayerController>().currentPosition.Value == 50)
                    {
                        StartCoroutine("POTY", i);
                    }

                    CheckIfQuestionAsked();
                }
            
            }
        }

        //Question Timer
        if (timerRunning)
        {
            if (timeForQuestion > 0)
            {
                timeForQuestion -= Time.deltaTime;
            }
            else
            {
                StartCoroutine("TurnButtonOneRed");
            }
        }
        timerText.text = ((int)timeForQuestion).ToString();
        gameCompleteTime += Time.deltaTime;

        SetGameWinner();
        DisplayCurrentPlayerText();
    }

    private IEnumerator POTY(int player)
    {
        rolledText.gameObject.SetActive(true);
        rolledText.text = "Pupil Of The Year \nGo To Summer Tests";
        yield return new WaitForSeconds(2.0f);

        rolledText.gameObject.SetActive(false);
        SendServerRpc("POTY", player.ToString(), 53);
        players[player].transform.position = gameSquares[53].transform.position;

        yield break;
    }

    private IEnumerator Move3(int player, int spaceOccupied)
    {
        logger.LogInformation("Moved 3 spaces forward");

        rolledText.gameObject.SetActive(true);
        rolledText.text = "Move 3 Spaces Forward";
        yield return new WaitForSeconds(2.0f);

        rolledText.gameObject.SetActive(false);
        if (spaceOccupied == 6)
        {
            SendServerRpc("MoveBy3", "9", player);
            players[player].transform.position = gameSquares[9].transform.position;
        }
        else if (spaceOccupied == 15)
        {
            SendServerRpc("MoveBy3", "18", player);
            players[player].transform.position = gameSquares[18].transform.position;
        }
        else if (spaceOccupied == 30)
        {
            SendServerRpc("MoveBy3", "33", player);
            players[player].transform.position = gameSquares[33].transform.position;
        }
        else if (spaceOccupied == 45)
        {
            SendServerRpc("MoveBy3", "48", player);

            players[player].transform.position = gameSquares[48].transform.position;
        }

        yield break;
    }

    private IEnumerator ShowRolledNum(string text, int player)
    {
        yield return new WaitForSeconds(1.0f);
        players[player].GetComponent<OnlinePlayerController>().MovePlayer();
        CheckIfMissTurn(player);
        logger.LogInformation("Rolled Number Shown playerID:" + player);
        yield break;

    }

    private IEnumerator ShowRolledSubject(string text, int player)
    {

        yield return new WaitForSeconds(1.0f);
        if (players[player].GetComponent<OnlinePlayerController>().playerOver12)
        {
            Over12Question();
        }
        else
        {
            Under12Question();
        }

        if (players[player].GetComponent<OnlinePlayerController>().finalExamsStage.Value == false)
        {
            players[player].GetComponent<OnlinePlayerController>().subjectSwitchOne.SetActive(true);
            players[player].GetComponent<OnlinePlayerController>().subjectSwitchTwo.SetActive(true);
            players[player].GetComponent<OnlinePlayerController>().subjectSwitchThree.SetActive(true);
            logger.LogInformation("Subject Rolled is Shown");
        }

        yield break;
    }

    private IEnumerator MissTurn()
    {
        missedTurnText.gameObject.SetActive(true);
        yield return new WaitForSeconds(2.0f);
        missedTurnText.gameObject.SetActive(false);
        yield break;
    }

    /// <summary>
    /// Make Active Player Pulse
    /// </summary>
    private IEnumerator PulseActivePlayer()
    {
        continuePulse = false;
        for (int i = 0; i < players.Count; i++)
        {
            if (players[i].GetComponent<OnlinePlayerController>().isActivePlayer.Value)
            {
                for (float j = 0; j <= 1; j += 0.01f)
                {
                    players[i].transform.localScale = new Vector3(
                        Mathf.Lerp(transform.localScale.x, transform.localScale.x + 0.15f, Mathf.SmoothStep(0f, 1f, j)),
                        Mathf.Lerp(transform.localScale.y, transform.localScale.y + 0.15f, Mathf.SmoothStep(0f, 1f, j)),
                        Mathf.Lerp(transform.localScale.z, transform.localScale.z + 0.15f, Mathf.SmoothStep(0f, 1f, j)));
                }
                yield return new WaitForSeconds(0.2f);
            }
            else
            {
                players[i].transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
            }

            if (players[i].GetComponent<OnlinePlayerController>().isActivePlayer.Value)
            {
                for (float j = 0; j <= 1; j += 0.01f)
                {
                        players[i].transform.localScale = new Vector3(
                        Mathf.Lerp(transform.localScale.x, transform.localScale.x - 0.15f, Mathf.SmoothStep(0f, 1f, j)),
                        Mathf.Lerp(transform.localScale.y, transform.localScale.y - 0.15f, Mathf.SmoothStep(0f, 1f, j)),
                        Mathf.Lerp(transform.localScale.z, transform.localScale.z - 0.15f, Mathf.SmoothStep(0f, 1f, j)));
                }
                yield return new WaitForSeconds(0.2f);
            }
            else
            {
                players[i].transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
            }

        }
        continuePulse = true;
    }

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

        SendServerRpc("SetupEndGoal","0",0);

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

    private void SetGameObjectsToFalse()
    {
        subjectText.gameObject.SetActive(false);
        questionText.gameObject.SetActive(false);
        questionCard.SetActive(false);
        rolledText.gameObject.SetActive(false);
        missedTurnText.gameObject.SetActive(false);
        leaderboardText.gameObject.SetActive(false);
        principalCard.SetActive(false);
        principalText.gameObject.SetActive(false);
        exitPanel.SetActive(false);
        exitText.gameObject.SetActive(false);
        cancelButton.gameObject.SetActive(false);
        confirmButton.gameObject.SetActive(false);
    }

    private void PreparePlayers()
    {
        //Prepare Players for online scene
        for (int i = 0; i < FindObjectOfType<OnlineGameSetup>().playersFound.Length; i++)
        {
            FindObjectOfType<OnlineGameSetup>().playersFound[i].GetComponent<OnlinePlayerController>().NextScene();
        }
        for (int i = 0; i < FindObjectOfType<OnlineGameSetup>().playersFound.Length; i++)
        {
            FindObjectOfType<OnlineGameSetup>().playersFound[i].GetComponent<OnlinePlayerController>().SetStates();
        }

        for (int i = 0; i < players.Count; i++)
        {
            if (players[i].GetComponent<OnlinePlayerController>().IsOwner)
            {
                if (players[i].GetComponent<OnlinePlayerController>().playerId == 0)
                {
                    players[i].GetComponent<OnlinePlayerController>().isActivePlayer.Value = true;
                    players[i].GetComponent<OnlinePlayerController>().playerOver12 = FindObjectOfType<OnlineGameSetup>().pinkOver12.Value;
                    players[i].GetComponent<OnlinePlayerController>().DisableAllButtons();
                    players[i].GetComponent<OnlinePlayerController>().EnableAllButtonsAtTurnStart();
                    logger.LogInformation("Currently all are buttons are enabled for player: " + i);
                }
                else
                {
                    players[i].GetComponent<OnlinePlayerController>().isActivePlayer.Value = false;
                }

                if (players[i].GetComponent<OnlinePlayerController>().playerId == 1)
                {
                    players[i].GetComponent<OnlinePlayerController>().playerOver12 = FindObjectOfType<OnlineGameSetup>().blueOver12.Value;
                    players[i].GetComponent<OnlinePlayerController>().DisableAllButtons();
                    logger.LogInformation("Currently all are buttons are disabled for player: " + i);
                }
                else if (players[i].GetComponent<OnlinePlayerController>().playerId == 2)
                {
                    players[i].GetComponent<OnlinePlayerController>().playerOver12 = FindObjectOfType<OnlineGameSetup>().redOver12.Value;
                    players[i].GetComponent<OnlinePlayerController>().DisableAllButtons();
                    logger.LogInformation("Currently all are buttons are disabled for player: " + i);
                }
                else if (players[i].GetComponent<OnlinePlayerController>().playerId == 3)
                {
                    players[i].GetComponent<OnlinePlayerController>().playerOver12 = FindObjectOfType<OnlineGameSetup>().greenOver12.Value;
                    players[i].GetComponent<OnlinePlayerController>().DisableAllButtons();
                    logger.LogInformation("Currently all are buttons are disabled for player: " + i);
                }
            }
        }
    }

    private void SetGameWinner()
    {
        if (playersCompletedGame.Count > 0)
        {
            leaderboardText.gameObject.SetActive(true);

            switch (playersCompletedGame[0].GetComponent<OnlinePlayerController>().playerId)
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

    private void CheckIfQuestionAsked()
    {
        if (isQuestionAsked.Value)
        {
            subjectText.gameObject.SetActive(true);
            questionText.gameObject.SetActive(true);
            questionCard.SetActive(true);
            subjectText.text = subjectTextNetString.Value.ToString();
            questionText.text = questionTextNetString.Value.ToString();
            for (int j = 0; j < answerButtons.Length; j++)
            {
                answerButtons[j].SetActive(true);
            }
            answerButtons[0].GetComponentInChildren<TextMeshProUGUI>().text = answerTextOne.Value.ToString();
            answerButtons[1].GetComponentInChildren<TextMeshProUGUI>().text = answerTextTwo.Value.ToString();
            answerButtons[2].GetComponentInChildren<TextMeshProUGUI>().text = answerTextThree.Value.ToString();
        }
        else
        {
            subjectText.gameObject.SetActive(false);
            questionText.gameObject.SetActive(false);
            questionCard.SetActive(false);
            for (int j = 0; j < answerButtons.Length; j++)
            {
                answerButtons[j].SetActive(false);
            }
        }
    }

    private void CheckIfMissTurn(int playerid)
    {
        for (int k = 0; k < missTurnNumbers.Length; k++)
        {
            if (missTurnNumbers[k] == players[playerid].GetComponent<OnlinePlayerController>().currentPosition.Value)
            {
                StartCoroutine("MissTurn");
                ResetQuestionTimer();
                NextPlayerTurn();
            }
        }
    }

    private void PulseActiveButton(int activePlayer)
    {
        Transform trasformToChange = players[activePlayer].GetComponent<OnlinePlayerController>().diceRollButton.transform;

        if (players[activePlayer].GetComponent<OnlinePlayerController>().isActivePlayer.Value)
        {
            if (toBig)
            {
                if (players[activePlayer].GetComponent<OnlinePlayerController>().diceRollButton.GetComponent<Button>().enabled)
                {
                    trasformToChange = players[activePlayer].GetComponent<OnlinePlayerController>().diceRollButton.transform;
                    players[activePlayer].GetComponent<OnlinePlayerController>().diceRollButton.transform.localScale = new Vector3(
                    (trasformToChange.localScale.x + 0.225f * Time.deltaTime),
                    (trasformToChange.localScale.y + 0.225f * Time.deltaTime),
                    (trasformToChange.localScale.z + 0.225f * Time.deltaTime));
                }
                else if (players[activePlayer].GetComponent<OnlinePlayerController>().subjectRollButton.GetComponent<Button>().enabled)
                {
                    trasformToChange = players[activePlayer].GetComponent<OnlinePlayerController>().subjectRollButton.transform;
                    players[activePlayer].GetComponent<OnlinePlayerController>().subjectRollButton.transform.localScale = new Vector3(
                    (trasformToChange.localScale.x + 0.225f * Time.deltaTime),
                    (trasformToChange.localScale.y + 0.225f * Time.deltaTime),
                    (trasformToChange.localScale.z + 0.225f * Time.deltaTime));
                }
                else
                {
                    players[activePlayer].GetComponent<OnlinePlayerController>().diceRollButton.transform.localScale = new Vector3(1, 1, 1);
                    players[activePlayer].GetComponent<OnlinePlayerController>().subjectRollButton.transform.localScale = new Vector3(1, 1, 1);
                }
            }
            else
            {
                if (players[activePlayer].GetComponent<OnlinePlayerController>().diceRollButton.GetComponent<Button>().enabled)
                {
                    players[activePlayer].GetComponent<OnlinePlayerController>().diceRollButton.transform.localScale = new Vector3(1, 1, 1);
                }
                else if (players[activePlayer].GetComponent<OnlinePlayerController>().subjectRollButton.GetComponent<Button>().enabled)
                {
                    players[activePlayer].GetComponent<OnlinePlayerController>().subjectRollButton.transform.localScale = new Vector3(1, 1, 1);
                }
                else
                {
                    players[activePlayer].GetComponent<OnlinePlayerController>().diceRollButton.transform.localScale = new Vector3(1, 1, 1);
                    players[activePlayer].GetComponent<OnlinePlayerController>().subjectRollButton.transform.localScale = new Vector3(1, 1, 1);
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



    /// <summary>
    /// Make the dice roll button and subject roll button flash green for active player
    /// </summary>
    private void FlashButtonGreen(int activePlayer)
    {
        if (players[activePlayer].GetComponent<OnlinePlayerController>().isActivePlayer.Value)
        {
            if (toGreen)
            {
                currentColour += new Color(-1.5f, 0, -1.5f, 0) * Time.deltaTime;

                if (players[activePlayer].GetComponent<OnlinePlayerController>().diceRollButton.GetComponent<Button>().enabled)
                {
                    players[activePlayer].GetComponent<OnlinePlayerController>().diceRollButton.GetComponent<Image>().color = currentColour;
                }
                else
                {
                    players[activePlayer].GetComponent<OnlinePlayerController>().diceRollButton.GetComponent<Image>().color = new Color(1, 0.49f, 0, 1);
                }

                if (players[activePlayer].GetComponent<OnlinePlayerController>().subjectRollButton.GetComponent<Button>().enabled)
                {
                    players[activePlayer].GetComponent<OnlinePlayerController>().subjectRollButton.GetComponent<Image>().color = currentColour;
                }
                else
                {
                    players[activePlayer].GetComponent<OnlinePlayerController>().subjectRollButton.GetComponent<Image>().color = new Color(1, 0.49f, 0, 1);
                }
            }
            else
            {
                currentColour += new Color(1.5f, 0, 1.5f, 0) * Time.deltaTime;

                if (players[activePlayer].GetComponent<OnlinePlayerController>().diceRollButton.GetComponent<Button>().enabled)
                {
                    players[activePlayer].GetComponent<OnlinePlayerController>().diceRollButton.GetComponent<Image>().color = currentColour;
                }
                else
                {
                    players[activePlayer].GetComponent<OnlinePlayerController>().diceRollButton.GetComponent<Image>().color = new Color(1, 0.49f, 0, 1);
                }

                if (players[activePlayer].GetComponent<OnlinePlayerController>().subjectRollButton.GetComponent<Button>().enabled)
                {
                    players[activePlayer].GetComponent<OnlinePlayerController>().subjectRollButton.GetComponent<Image>().color = currentColour;
                }
                else
                {
                    players[activePlayer].GetComponent<OnlinePlayerController>().subjectRollButton.GetComponent<Image>().color = new Color(1, 0.49f, 0, 1);
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
    /// Triggers the random roll of the number dice
    /// </summary>
    public void DiceRoll()
    {
        int roll = 0;
        roll += Random.Range(1, 6);

        for (int i = 0; i < players.Count; i++)
        {
            if (players[i].GetComponent<OnlinePlayerController>().isActivePlayer.Value)
            {
                if (players[i].GetComponent<OnlinePlayerController>().finalExamsStage.Value == false)
                {
                    logger.LogInformation("Number Dice Rolled, Number rolled: " + roll);
                    SetDiceImage(roll, i);
                    SendServerRpc("rolledNumber", roll.ToString(), 0);
                    StartCoroutine(ShowRolledNum(roll.ToString(), i));
                }
                else
                {
                    roll = 1;
                    logger.LogInformation("Number Dice Rolled in exam stage, Number rolled: " + roll);
                    players[i].GetComponent<OnlinePlayerController>().diceRollButton.GetComponent<Image>().sprite = oneSprite;
                    SendServerRpc("rolledNumber", 1.ToString(), 0);
                    StartCoroutine(ShowRolledNum(roll.ToString(), i));
                }
                players[i].GetComponent<OnlinePlayerController>().DisableDiceButton();
                players[i].GetComponent<OnlinePlayerController>().EnableSubjectButton();
            }
        }
    }

    /// <summary>
    /// Set Image of Dice after it is rolled to show the number that was rolled
    /// </summary>
    /// <param name="rolledNum"></param>
    /// <param name="player"></param>
    private void SetDiceImage(int rolledNum, int player)
    {
        switch (rolledNum)
        {
            case 1:
                players[player].GetComponent<OnlinePlayerController>().diceRollButton.GetComponent<Image>().sprite = oneSprite;
                break;
            case 2:
                players[player].GetComponent<OnlinePlayerController>().diceRollButton.GetComponent<Image>().sprite = twoSprite;
                break;
            case 3:
                players[player].GetComponent<OnlinePlayerController>().diceRollButton.GetComponent<Image>().sprite = threeSprite;
                break;
            case 4:
                players[player].GetComponent<OnlinePlayerController>().diceRollButton.GetComponent<Image>().sprite = fourSprite;
                break;
            case 5:
                players[player].GetComponent<OnlinePlayerController>().diceRollButton.GetComponent<Image>().sprite = fiveSprite;
                break;
            case 6:
                players[player].GetComponent<OnlinePlayerController>().diceRollButton.GetComponent<Image>().sprite = sixSprite;
                break;
            default:
                break;
        }
    }

    public void SubjectDiceRoll()
    {
        int roll = 0;
        roll += Random.Range(0, 6);

        for (int i = 0; i < players.Count; i++)
        {
            if (players[i].GetComponent<OnlinePlayerController>().isActivePlayer.Value)
            {
                if (players[i].GetComponent<OnlinePlayerController>().finalExamsStage.Value == false)
                {
                    switch (roll)
                    {
                        case 0:
                            players[i].GetComponent<OnlinePlayerController>().subjectRollButton.GetComponent<Image>().sprite = gkSprite;
                            break;
                        case 1:
                            players[i].GetComponent<OnlinePlayerController>().subjectRollButton.GetComponent<Image>().sprite = geoSprite;
                            break;
                        case 2:
                            players[i].GetComponent<OnlinePlayerController>().subjectRollButton.GetComponent<Image>().sprite = scienceSprite;
                            break;
                        case 3:
                            players[i].GetComponent<OnlinePlayerController>().subjectRollButton.GetComponent<Image>().sprite = historySprite;
                            break;
                        case 4:
                            players[i].GetComponent<OnlinePlayerController>().subjectRollButton.GetComponent<Image>().sprite = mathSprite;
                            break;
                        case 5:
                            players[i].GetComponent<OnlinePlayerController>().subjectRollButton.GetComponent<Image>().sprite = englishSprite;
                            break;
                        default:
                            break;
                    }

                    SendServerRpc("NextSubject", FindObjectOfType<OnlineQuestions>().subjects[roll].ToString(), 0);
                    StartCoroutine(ShowRolledSubject(FindObjectOfType<OnlineQuestions>().nextSubject.ToString(), i));

                    players[i].GetComponent<OnlinePlayerController>().CheckSwitches();

                    logger.LogInformation("Subject Dice Rolled, Subject rolled: " + FindObjectOfType<OnlineQuestions>().nextSubject);
                }
                else
                {
                    switch (players[i].GetComponent<OnlinePlayerController>().currentPosition.Value)
                    {
                        case 53:
                            SendServerRpc("NextSubject", "General", 0);
                            players[i].GetComponent<OnlinePlayerController>().subjectRollButton.GetComponent<Image>().sprite = gkSprite;
                            break;
                        case 54:
                            SendServerRpc("NextSubject", "Geography", 0);
                            players[i].GetComponent<OnlinePlayerController>().subjectRollButton.GetComponent<Image>().sprite = geoSprite;
                            break;
                        case 55:
                            SendServerRpc("NextSubject", "Science", 0);
                            players[i].GetComponent<OnlinePlayerController>().subjectRollButton.GetComponent<Image>().sprite = scienceSprite;
                            break;
                        case 56:
                            SendServerRpc("NextSubject", "History", 0);
                            players[i].GetComponent<OnlinePlayerController>().subjectRollButton.GetComponent<Image>().sprite = historySprite;
                            break;
                        case 57:
                            SendServerRpc("NextSubject", "Maths", 0);
                            players[i].GetComponent<OnlinePlayerController>().subjectRollButton.GetComponent<Image>().sprite = mathSprite;
                            break;
                        case 58:
                            SendServerRpc("NextSubject", "English", 0);
                            players[i].GetComponent<OnlinePlayerController>().subjectRollButton.GetComponent<Image>().sprite = englishSprite;
                            break;
                        default:
                            break;
                    }
                    StartCoroutine(ShowRolledSubject(FindObjectOfType<OnlineQuestions>().nextSubject.Value.ToString(), i));
                }
                players[i].GetComponent<OnlinePlayerController>().DisableSubjectButton();
            }
        }
    }

    public void SwitchSubject()
    {
        for (int i = 0; i < players.Count; i++)
        {
            if (players[i].GetComponent<OnlinePlayerController>().isActivePlayer.Value)
            {
                logger.LogInformation("Subject Switch pressed");
                if (players[i].GetComponent<OnlinePlayerController>().subjectSwitchTwo.GetComponent<Button>().enabled == false)
                {
                    SendServerRpc("UseSwitch", i.ToString(), 3);
                }
                else if (players[i].GetComponent<OnlinePlayerController>().subjectSwitchOne.GetComponent<Button>().enabled == false)
                {
                    SendServerRpc("UseSwitch", i.ToString(), 2);
                }
                else
                {
                    SendServerRpc("UseSwitch", i.ToString(), 1);
                }

                logger.LogInformation("Player: " + i + " Switch buttons activated");
                players[i].GetComponent<OnlinePlayerController>().switchToGKButton.SetActive(true);
                players[i].GetComponent<OnlinePlayerController>().switchToGeographyButton.SetActive(true);
                players[i].GetComponent<OnlinePlayerController>().switchToScienceButton.SetActive(true);
                players[i].GetComponent<OnlinePlayerController>().switchToHistoryButton.SetActive(true);
                players[i].GetComponent<OnlinePlayerController>().switchToMathsButton.SetActive(true);
                players[i].GetComponent<OnlinePlayerController>().switchToEnglishButton.SetActive(true);

                SendServerRpc("IsQuestionAsked", "0", 0); //In this case the string 0 is used for false

                players[i].GetComponent<OnlinePlayerController>().CheckSwitches();
            }
        }
    }

    public void Under12Question()
    {
        logger.LogInformation("Timer Started");
        StartQuestionTimer();

        SetRandomButtonPos();
        SendServerRpc("IsQuestionAsked", "1", 1); //In this case the 1 is used for true

        GetComponent<OnlineQuestions>().SelectQuestionUnder12ServerRpc();
    }

    public void Over12Question()
    {
        logger.LogInformation("Timer Started");
        StartQuestionTimer();

        SetRandomButtonPos();
        SendServerRpc("IsQuestionAsked", "1", 1); //In this case the 1 is used for true

        GetComponent<OnlineQuestions>().SelectQuestionOver12ServerRpc();
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
        StartCoroutine("TurnButtonGreen");
    }

    private IEnumerator TurnButtonOneRed()
    {
        answerButtons[0].GetComponent<Image>().color = Color.green;
        answerButtons[1].GetComponent<Image>().color = Color.red;
        for (int j = 0; j < answerButtons.Length; j++)
        {
            answerButtons[j].GetComponent<Button>().enabled = false;
        }
        yield return new WaitForSeconds(1.0f);

        for (int j = 0; j < answerButtons.Length; j++)
        {
            answerButtons[j].GetComponent<Button>().enabled = true;
            answerButtons[j].GetComponent<Image>().color = Color.white;
        }

        for (int i = 0; i < timerComponent.Length; i++)
        {
            timerComponent[i].SetActive(false);
        }
        logger.LogInformation("Wrong Answer Chosen");
        ResetQuestionTimer();
        NextPlayerTurn();
        yield break;
    }

    private IEnumerator TurnButtonTwoRed()
    {
        answerButtons[0].GetComponent<Image>().color = Color.green;
        answerButtons[2].GetComponent<Image>().color = Color.red;
        for (int j = 0; j < answerButtons.Length; j++)
        {
            answerButtons[j].GetComponent<Button>().enabled = false;
        }
        yield return new WaitForSeconds(1.0f);

        for (int j = 0; j < answerButtons.Length; j++)
        {
            answerButtons[j].GetComponent<Button>().enabled = true;
            answerButtons[j].GetComponent<Image>().color = Color.white;
        }

        for (int i = 0; i < timerComponent.Length; i++)
        {
            timerComponent[i].SetActive(false);
        }
        logger.LogInformation("Wrong Answer Chosen");
        ResetQuestionTimer();
        NextPlayerTurn();
        yield break;
    }

    private IEnumerator TurnButtonGreen()
    {
        answerButtons[0].GetComponent<Image>().color = Color.green;
        for (int j = 0; j < answerButtons.Length; j++)
        {
            answerButtons[j].GetComponent<Button>().enabled = false;
        }
        yield return new WaitForSeconds(1.0f);

        for (int j = 0; j < answerButtons.Length; j++)
        {
            answerButtons[j].GetComponent<Button>().enabled = true;
            answerButtons[j].GetComponent<Image>().color = Color.white;
        }

        for (int i = 0; i < timerComponent.Length; i++)
        {
            timerComponent[i].SetActive(false);
        }

        for (int i = 0; i < players.Count; i++)
        {
            if (players[i].GetComponent<OnlinePlayerController>().isActivePlayer.Value)
            {
                players[i].GetComponent<OnlinePlayerController>().subjectSwitchOne.SetActive(false);
                players[i].GetComponent<OnlinePlayerController>().subjectSwitchTwo.SetActive(false);
                players[i].GetComponent<OnlinePlayerController>().subjectSwitchThree.SetActive(false);
                players[i].GetComponent<OnlinePlayerController>().EnableDiceButton();
            }
        }
        logger.LogInformation("Correct Answer Chosen");
        ResetQuestionTimer();

        yield break;
    }

    public void GoToOffice()
    {
        for (int i = 0; i < players.Count; i++)
        {
            if (players[i].GetComponent<OnlinePlayerController>().isActivePlayer.Value)
            {
                if(players[i].GetComponent<NetworkObject>().IsOwner)
                {
                    players[i].GetComponent<OnlinePlayerController>().subjectRollButton.GetComponent<Button>().enabled = false;
                    players[i].GetComponent<OnlinePlayerController>().continueToPrincipalButton.SetActive(true);
                    logger.LogInformation("Player: " + players[i] + "Go to Office");
                }
            }
        }
    }

    public void StartQuestionTimer()
    {
        for (int i = 0; i < timerComponent.Length; i++)
        {
            timerComponent[i].SetActive(true);

        }
        timerRunning = true;
    }

    public void ResetQuestionTimer()
    {
        SendServerRpc("IsQuestionAsked", "0", 0); //In this case the 0 is used for false
        timerRunning = false;
        timeForQuestion = 30;
        logger.LogInformation("Timer Reset: " + timeForQuestion);
    }

    /// <summary>
    /// Fuction which disables the current player and gets the next player in order and makes them active
    /// </summary>
    public void NextPlayerTurn()
    {
        int nextActivePlayer = 0;
        int currentActivePlayer = 0;
        for (int i = 0; i < players.Count; i++)
        {
            if (players[i].GetComponent<OnlinePlayerController>().isActivePlayer.Value)
            {
                currentActivePlayer = i;
                nextActivePlayer = i + 1;

                if (nextActivePlayer >= players.Count)
                {
                    nextActivePlayer = 0;
                }
                if (players[nextActivePlayer].GetComponent<OnlinePlayerController>().gameComplete.Value == true)
                {
                    nextActivePlayer += 1;
                }
            }
            players[i].GetComponent<OnlinePlayerController>().subjectSwitchOne.SetActive(false);
            players[i].GetComponent<OnlinePlayerController>().subjectSwitchTwo.SetActive(false);
            players[i].GetComponent<OnlinePlayerController>().subjectSwitchThree.SetActive(false);
        }
        SendServerRpc("CurrentPlayerStopTurn", currentActivePlayer.ToString(), 0);
        players[currentActivePlayer].GetComponent<OnlinePlayerController>().DisableAllButtons();
        logger.LogInformation("Currently all are buttons are disabled for: " + currentActivePlayer);
        AnalyticsManager.Instance().LogEvent("Player: " + currentActivePlayer + " Turn Over", turnsTaken, 0);
        turnsTaken++;
        AnalyticsManager.Instance().totalTurnsMade++;
        AnalyticsManager.Instance().LogEvent("Player: " + nextActivePlayer + " Turn Start", turnsTaken, 0);
        SendServerRpc("NextPlayerTurn", nextActivePlayer.ToString(), 0);
        logger.LogInformation("Currently all are buttons are enabled for: " + nextActivePlayer);
    }

    public void NextPlayerAfterOneCompleted(int lastActivePlayer)
    {
        int currentActivePlayer = lastActivePlayer;
        int nextActivePlayer = lastActivePlayer + 1;

        for (int i = 0; i < players.Count; i++)
        {
            if (nextActivePlayer > players.Count -1)
            {
                nextActivePlayer = 0;
            }
        }


        AnalyticsManager.Instance().LogEvent("Player: " + currentActivePlayer + " Turn Over", turnsTaken, 0);
        turnsTaken++;
        AnalyticsManager.Instance().totalTurnsMade++;
        AnalyticsManager.Instance().LogEvent("Player: " + nextActivePlayer + " Turn Start", turnsTaken, 0);

        SendServerRpc("NextPlayerTurn", nextActivePlayer.ToString(), 0);
        logger.LogInformation("Currently all are buttons are enabled for: " + nextActivePlayer);
    }

    public void ContinueToPrincipal()
    {
        for (int i = 0; i < players.Count; i++)
        {
            if (players[i].GetComponent<OnlinePlayerController>().isActivePlayer.Value)
            {
                if (players[i].GetComponent<NetworkObject>().IsOwner)
                {
                    players[i].GetComponent<OnlinePlayerController>().continueToPrincipalButton.SetActive(false);

                    switch (players[i].GetComponent<OnlinePlayerController>().currentPosition.Value)
                    {
                        case 1: //good
                            players[i].transform.position = officeSquares[0].transform.position;
                            SendServerRpc("PlayerMove", officeSquares[0].GetComponent<OfficePositionOnBoard>().officeBoardPosition.ToString(), i);
                            break;
                        case 11: //good
                            players[i].transform.position = officeSquares[1].transform.position;
                            SendServerRpc("PlayerMove", officeSquares[1].GetComponent<OfficePositionOnBoard>().officeBoardPosition.ToString(), i);
                            break;
                        case 16: //bad
                            players[i].transform.position = officeSquares[0].transform.position;
                            SendServerRpc("PlayerMove", officeSquares[0].GetComponent<OfficePositionOnBoard>().officeBoardPosition.ToString(), i);
                            break;
                        case 20: //good
                            players[i].transform.position = officeSquares[1].transform.position;
                            SendServerRpc("PlayerMove", officeSquares[1].GetComponent<OfficePositionOnBoard>().officeBoardPosition.ToString(), i);
                            break;
                        case 27: //good
                            players[i].transform.position = officeSquares[2].transform.position;
                            SendServerRpc("PlayerMove", officeSquares[2].GetComponent<OfficePositionOnBoard>().officeBoardPosition.ToString(), i);
                            break;
                        case 31:  //bad
                            players[i].transform.position = officeSquares[1].transform.position;
                            SendServerRpc("PlayerMove", officeSquares[1].GetComponent<OfficePositionOnBoard>().officeBoardPosition.ToString(), i);
                            break;
                        case 36:  //bad
                            players[i].transform.position = officeSquares[2].transform.position;
                            SendServerRpc("PlayerMove", officeSquares[2].GetComponent<OfficePositionOnBoard>().officeBoardPosition.ToString(), i);
                            break;
                        case 41: //good
                            players[i].transform.position = officeSquares[3].transform.position;
                            SendServerRpc("PlayerMove", officeSquares[3].GetComponent<OfficePositionOnBoard>().officeBoardPosition.ToString(), i);
                            break;
                        case 46: //bad
                            players[i].transform.position = officeSquares[2].transform.position;
                            SendServerRpc("PlayerMove", officeSquares[2].GetComponent<OfficePositionOnBoard>().officeBoardPosition.ToString(), i);
                            break;
                        default:
                            break;
                    }
                }
                GetComponent<PrincipalCards>().DrawRandomPrincipalCard();
                principalCard.SetActive(true);
                principalText.gameObject.SetActive(true);
                players[i].GetComponent<OnlinePlayerController>().acceptRuleButton.SetActive(true);
                logger.LogInformation("Accept Rule Button Enabled for player: " + i);
            }
            players[i].GetComponent<OnlinePlayerController>().continueToPrincipalButton.SetActive(false);
        }
    }

    public void AcceptRule()
    {
        logger.LogInformation("Principal Rule Accepted");
        for (int i = 0; i < players.Count; i++)
        {
            players[i].GetComponent<OnlinePlayerController>().continueToPrincipalButton.SetActive(false);
            if (players[i].GetComponent<OnlinePlayerController>().isActivePlayer.Value)
            {
                if (players[i].GetComponent<NetworkObject>().IsOwner)
                {
                    float posXStart = 16.1f;
                    float posYTop = 11.0f;
                    players[i].GetComponent<OnlinePlayerController>().SetPlayerPositioning(ref posXStart, ref posYTop);

                    if (GetComponent<PrincipalCards>().nextDirection == "forward")
                    {
                        logger.LogInformation("Rule Forward Sent");
                        SendServerRpc("AcceptRulePlus", GetComponent<PrincipalCards>().nextMove.ToString(), i);
                        players[i].transform.position = gameSquares[players[i].GetComponent<OnlinePlayerController>().currentPosition.Value].transform.position;
                        players[i].GetComponent<OnlinePlayerController>().PositionPlayerOnSquareServerRpc(posXStart, posYTop);

                        principalCard.SetActive(false);
                        principalText.gameObject.SetActive(false);
                    }
                    else if (GetComponent<PrincipalCards>().nextDirection == "back")
                    {
                        logger.LogInformation("Rule Back Sent");
                        SendServerRpc("AcceptRuleMinus", GetComponent<PrincipalCards>().nextMove.ToString(), i);
                        players[i].transform.position = gameSquares[players[i].GetComponent<OnlinePlayerController>().currentPosition.Value].transform.position;
                        players[i].GetComponent<OnlinePlayerController>().PositionPlayerOnSquareServerRpc(posXStart, posYTop);

                        principalCard.SetActive(false);
                        principalText.gameObject.SetActive(false);
                    }
                }
            }
            players[i].GetComponent<OnlinePlayerController>().acceptRuleButton.SetActive(false);
            principalCard.SetActive(false);
            principalText.gameObject.SetActive(false);
            players[i].GetComponent<OnlinePlayerController>().subjectRollButton.GetComponent<Button>().enabled = true;
        }
    }

    public void SetRandomButtonPos()
    {
        int roll = Random.Range(0, 3);
        answerButtons[0].transform.position = buttonPositions[roll];
        logger.LogInformation("Answers Randomised");
        switch (roll)
        {
            case 0:
                roll = Random.Range(1, 3);
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
                roll = Random.Range(0, 2);
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

    private void DisplayCurrentPlayerText()
    {   for(int i = 0; i < players.Count; i++)
        {
            if (players[i].GetComponent<OnlinePlayerController>().isActivePlayer.Value)
            {
                switch (i)
                {
                    case 0:
                        currentPlayerText.text = FindObjectOfType<GameSetupManager>().player1Name + "'s Turn";
                        currentPlayerText.color = FindObjectOfType<GameSetupManager>().player1Color;
                        questionCard.GetComponent<Image>().color = FindObjectOfType<GameSetupManager>().player1Color;
                        break;
                    case 1:
                        currentPlayerText.text = FindObjectOfType<GameSetupManager>().player2Name + "'s Turn";
                        currentPlayerText.color = FindObjectOfType<GameSetupManager>().player2Color;
                        questionCard.GetComponent<Image>().color = FindObjectOfType<GameSetupManager>().player2Color;
                        break;
                    case 2:
                        currentPlayerText.text = FindObjectOfType<GameSetupManager>().player3Name + "'s Turn";
                        currentPlayerText.color = FindObjectOfType<GameSetupManager>().player3Color;
                        questionCard.GetComponent<Image>().color = FindObjectOfType<GameSetupManager>().player3Color;
                        break;
                    case 3:
                        currentPlayerText.text = FindObjectOfType<GameSetupManager>().player4Name + "'s Turn";
                        currentPlayerText.color = FindObjectOfType<GameSetupManager>().player4Color;
                        questionCard.GetComponent<Image>().color = FindObjectOfType<GameSetupManager>().player4Color;
                        break;
                }
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

    /// <summary>
    /// Function used By Online GameManager and Online Players to send information to the server and update data
    /// </summary>
    /// <param name="valueType"></param>
    /// <param name="value"></param>
    /// <param name="extraInt"></param>
    [ServerRpc(RequireOwnership = false)]
    public void SendServerRpc(string valueType, string value, int extraInt)
    {
        int outInt = 0;
        int.TryParse(value, out outInt);

        switch (valueType)
        {
            case "PlayerMove":
                players[extraInt].GetComponent<OnlinePlayerController>().currentPosition.Value = outInt;
                logger.LogInformation("(ServerRpc)current Position Changed" + outInt);
                if(players[extraInt].GetComponent<OnlinePlayerController>().currentPosition.Value > 58)
                {
                    players[extraInt].GetComponent<OnlinePlayerController>().CompleteGame();
                }
                break;

            case "MoveBy3":
                players[extraInt].GetComponent<OnlinePlayerController>().currentPosition.Value = outInt;
                logger.LogInformation("(ServerRpc)current Position Changed" + outInt);
                break;

            case "CurrentPlayerStopTurn":
                players[outInt].GetComponent<OnlinePlayerController>().isActivePlayer.Value = false;
                logger.LogInformation("(ServerRpc)Next Player Turn, previous player ID: " + outInt);
                break;

            case "NextPlayerTurn":
                players[outInt].GetComponent<OnlinePlayerController>().isActivePlayer.Value = true;
                players[outInt].GetComponent<OnlinePlayerController>().isStartOfTurn.Value = true;
                logger.LogInformation("(ServerRpc)Next Player Turn, next player ID: " + outInt);

                break;

            case "Started":
                if (players.Count > 1)
                {
                    players[outInt].GetComponent<OnlinePlayerController>().isStartOfTurn.Value = false;
                    logger.LogInformation("(ServerRpc)Turn Started - player ID: " + outInt);
                }
                break;

            case "NextSubject":
                FindObjectOfType<OnlineQuestions>().nextSubject.Value = value;
                logger.LogInformation("(ServerRpc)Next Subject: " + value);
                break;

            case "GameComplete":
                players[outInt].GetComponent<OnlinePlayerController>().gameComplete.Value = true;
                logger.LogInformation("(ServerRpc)Player: " + outInt + "Completed Game");
                break;

            case "POTY":
                players[outInt].GetComponent<OnlinePlayerController>().currentPosition.Value = 53;
                logger.LogInformation("(ServerRpc)Player: " + outInt + "POTY");
                players[outInt].GetComponent<OnlinePlayerController>().finalExamsStage.Value = true;
                logger.LogInformation("(ServerRpc)Player: " + outInt + "Final Exams");
                break;

            case "FinalExams":
                players[outInt].GetComponent<OnlinePlayerController>().finalExamsStage.Value = true;
                logger.LogInformation("(ServerRpc)Player: " + outInt + "Final Exams");
                break;

            case "AcceptRulePlus":
                int newPosPlus = players[extraInt].GetComponent<OnlinePlayerController>().currentPosition.Value + outInt;
                players[extraInt].GetComponent<OnlinePlayerController>().currentPosition.Value = newPosPlus;
                if (players[extraInt].GetComponent<OnlinePlayerController>().currentPosition.Value >= 53)
                {
                    players[extraInt].GetComponent<OnlinePlayerController>().finalExamsStage.Value = true;
                }
                logger.LogInformation("(ServerRpc)Rule Accepted, Next position: " + newPosPlus);
                break;

            case "AcceptRuleMinus":
                int newPosMinus = players[extraInt].GetComponent<OnlinePlayerController>().currentPosition.Value - outInt;
                if (newPosMinus >= 0)
                {
                    players[extraInt].GetComponent<OnlinePlayerController>().currentPosition.Value = newPosMinus;
                }
                else
                {
                    players[extraInt].GetComponent<OnlinePlayerController>().currentPosition.Value = 0;
                }
                logger.LogInformation("(ServerRpc)Rule Accepted, Next position: " + newPosMinus);
                break;

            case "IsQuestionAsked":
                if (outInt == 0)
                {
                    logger.LogInformation("(ServerRpc)is Question Asked set to: false");
                    isQuestionAsked.Value = false;
                }
                else if (outInt == 1)
                {
                    logger.LogInformation("(ServerRpc)is Question Asked set to: true");
                    isQuestionAsked.Value = true;
                }
                break;

            case "UseSwitch":
                logger.LogInformation("(ServerRpc)Switch: " + extraInt + " Used for - player ID: " + outInt);
                if (extraInt == 3)
                {
                    players[outInt].GetComponent<OnlinePlayerController>().switchThreeUsed.Value = true;
                }
                else if (extraInt == 2)
                {
                    players[outInt].GetComponent<OnlinePlayerController>().switchTwoUsed.Value = true;
                }
                else if (extraInt == 1)
                {
                    players[outInt].GetComponent<OnlinePlayerController>().switchOneUsed.Value = true;
                }
                break;

            default:
                break;
        }

        for (int i = 0; i < players.Count; i++)
        {
            if (valueType == "rolledNumber")
            {
                players[i].GetComponent<OnlinePlayerController>().rolledNumber.Value = outInt;
                logger.LogInformation("(ServerRpc)player ID: " + i + " rolled " + outInt);
            }
        }
    }
}
