using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using Unity.Netcode;

public class OnlinePlayerController : NetworkBehaviour
{
    [Header("Player Data")]
    public OnlinePlayerController instance;
    public int playerId = 0;
    public bool playerOver12 = true;
    public NetworkVariable<bool> gameComplete = new NetworkVariable<bool>(false);
    public NetworkVariable<bool> finalExamsStage = new NetworkVariable<bool>(false);
    public NetworkVariable<bool> isStartOfTurn = new NetworkVariable<bool>(false);
    public NetworkVariable<bool> isActivePlayer = new NetworkVariable<bool>(false);
    public NetworkVariable<int> rolledNumber = new NetworkVariable<int>(0);
    public NetworkVariable<int> currentPosition = new NetworkVariable<int>(0);
    public NetworkVariable<bool> switchOneUsed = new NetworkVariable<bool>(false);
    public NetworkVariable<bool> switchTwoUsed = new NetworkVariable<bool>(false);
    public NetworkVariable<bool> switchThreeUsed = new NetworkVariable<bool>(false);

    [Header("Subject Switch Buttons")]
    public GameObject subjectSwitchOne;
    public GameObject subjectSwitchTwo;
    public GameObject subjectSwitchThree;

    [Header("Subject Choice Buttons")]
    public GameObject switchToGKButton;
    public GameObject switchToGeographyButton;
    public GameObject switchToScienceButton;
    public GameObject switchToHistoryButton;
    public GameObject switchToMathsButton;
    public GameObject switchToEnglishButton;

    [Header("Other UI Buttons")]
    public GameObject diceRollButton;
    public GameObject subjectRollButton;
    public GameObject acceptRuleButton;
    public GameObject continueToPrincipalButton;

    void Start()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }

        NextScene();
    }

    void Update()
    {
        if (SceneManager.GetActiveScene().name == "Online")//Ensures the updates are not executed while in Lobby
        { 
            //If player is starting new turn
            if(isStartOfTurn.Value)
            {
                EnableAllButtonsAtTurnStart();
            }
            if(subjectRollButton.GetComponent<Button>().enabled && isStartOfTurn.Value)
            {
                if(IsOwner)//Always check if is owner after player switch as server would only apply change to itself without considering the client due to update queue
                {
                    FindObjectOfType<OnlineGameManager>().logger.LogInformation("Should Stop");
                    FindObjectOfType<OnlineGameManager>().SendServerRpc("Started", playerId.ToString(), 0);
                }
            }
        }
    }

    public void EnableDiceButton()
    {
        diceRollButton.GetComponent<Button>().enabled = true;
    }
    public void DisableDiceButton()
    {
        diceRollButton.GetComponent<Button>().enabled = false;
    }
    public void EnableSubjectButton()
    {
        subjectRollButton.GetComponent<Button>().enabled = true;
    }
    public void DisableSubjectButton()
    {
        subjectRollButton.GetComponent<Button>().enabled = false;
    }

    /// <summary>
    /// Enables all buttons for a player except the dice roll as it should not be enabled at beginning of player turn
    /// </summary>
    public void EnableAllButtonsAtTurnStart()
    {
        FindObjectOfType<OnlineGameManager>().logger.LogInformation("Buttons for: " + playerId + "Enabled");

        subjectRollButton.GetComponent<Button>().enabled = true;
        subjectSwitchOne.GetComponent<Button>().enabled = true;
        subjectSwitchTwo.GetComponent<Button>().enabled = true;
        subjectSwitchThree.GetComponent<Button>().enabled = true;
        switchToGKButton.GetComponent<Button>().enabled = true;
        switchToGeographyButton.GetComponent<Button>().enabled = true;
        switchToScienceButton.GetComponent<Button>().enabled = true;
        switchToHistoryButton.GetComponent<Button>().enabled = true;
        switchToMathsButton.GetComponent<Button>().enabled = true;
        switchToEnglishButton.GetComponent<Button>().enabled = true;
        acceptRuleButton.GetComponent<Button>().enabled = true;
        continueToPrincipalButton.GetComponent<Button>().enabled = true;
    }

    /// <summary>
    /// Disables all buttons for player
    /// </summary>
    public void DisableAllButtons()
    {
        FindObjectOfType<OnlineGameManager>().logger.LogInformation("Buttons for: " + playerId + "Disabled");

        diceRollButton.GetComponent<Button>().enabled = false;
        subjectRollButton.GetComponent<Button>().enabled = false;
        subjectSwitchOne.GetComponent<Button>().enabled = false;
        subjectSwitchTwo.GetComponent<Button>().enabled = false;
        subjectSwitchThree.GetComponent<Button>().enabled = false;
        switchToGKButton.GetComponent<Button>().enabled = false;
        switchToGeographyButton.GetComponent<Button>().enabled = false;
        switchToScienceButton.GetComponent<Button>().enabled = false;
        switchToHistoryButton.GetComponent<Button>().enabled = false;
        switchToMathsButton.GetComponent<Button>().enabled = false;
        switchToEnglishButton.GetComponent<Button>().enabled = false;
        acceptRuleButton.GetComponent<Button>().enabled = false;
        continueToPrincipalButton.GetComponent<Button>().enabled = false;
    }

    /// <summary>
    /// Assigns buttons when scene is loaded
    /// </summary>
    public void NextScene()
    {
        if (SceneManager.GetActiveScene().name == "Online")
        {
            subjectSwitchOne = GameObject.FindGameObjectWithTag("SwitchOne");
            subjectSwitchTwo = GameObject.FindGameObjectWithTag("SwitchTwo");
            subjectSwitchThree = GameObject.FindGameObjectWithTag("SwitchThree");
            switchToGKButton = GameObject.FindGameObjectWithTag("General");
            switchToGeographyButton = GameObject.FindGameObjectWithTag("Geography");
            switchToScienceButton = GameObject.FindGameObjectWithTag("Science");
            switchToHistoryButton = GameObject.FindGameObjectWithTag("History");
            switchToMathsButton = GameObject.FindGameObjectWithTag("Maths");
            switchToEnglishButton = GameObject.FindGameObjectWithTag("English");
            acceptRuleButton = GameObject.FindGameObjectWithTag("AcceptRule");
            continueToPrincipalButton = GameObject.FindGameObjectWithTag("ContinueButton");

            AssignButtons();
            SetStartPosition();
        }
    }

    /// <summary>
    /// Set initial Player positions
    /// </summary>
    public void SetStartPosition()
    {
        FindObjectOfType<OnlineGameManager>().logger.LogInformation("Player start position set for playerID: " + playerId);
        float posXStart = 16.1f;
        float posYStart = 3.29f;

        //Setup positioning depending on player ID
        switch (playerId)
        {
            case 0:
                break;
            case 1:
                posXStart = 17;
                break;
            case 2:
                posXStart = 18;
                break;
            case 3:
                posXStart = 18.9f;
                break;
        }
        transform.position = new Vector3(posXStart, posYStart, 0);
    }

    /// <summary>
    /// Set states at start
    /// </summary>
    public void SetStates()
    {
        if (SceneManager.GetActiveScene().name == "Online")
        {
            subjectSwitchOne.SetActive(false);
            subjectSwitchTwo.SetActive(false);
            subjectSwitchThree.SetActive(false);
            switchToGKButton.SetActive(false);    
            switchToGeographyButton.SetActive(false);
            switchToScienceButton.SetActive(false);
            switchToHistoryButton.SetActive(false);
            switchToMathsButton.SetActive(false);
            switchToEnglishButton.SetActive(false);
            acceptRuleButton.SetActive(false);
            continueToPrincipalButton.SetActive(false);
        }
    }

    /// <summary>
    /// Check if Switches are used
    /// </summary>
    public void CheckSwitches()
    {
        if (switchOneUsed.Value)
        {
            subjectSwitchOne.GetComponent<Button>().enabled = false;
            subjectSwitchOne.GetComponentInChildren<TextMeshProUGUI>().text = "USED";
            subjectSwitchOne.GetComponentInChildren<TextMeshProUGUI>().transform.rotation = Quaternion.Euler(0, 0, 20);
            subjectSwitchOne.GetComponentInChildren<TextMeshProUGUI>().alignment = TextAlignmentOptions.Center;
        }
        else if (switchOneUsed.Value == false)
        {
            subjectSwitchOne.GetComponent<Button>().enabled = true;
            subjectSwitchOne.GetComponentInChildren<TextMeshProUGUI>().text = "Switch \nSubject";
            subjectSwitchOne.GetComponentInChildren<TextMeshProUGUI>().transform.rotation = Quaternion.Euler(0, 0, 0);
            subjectSwitchOne.GetComponentInChildren<TextMeshProUGUI>().alignment = TextAlignmentOptions.Top;
        }

        if (switchTwoUsed.Value)
        {
            subjectSwitchTwo.GetComponent<Button>().enabled = false;
            subjectSwitchTwo.GetComponentInChildren<TextMeshProUGUI>().text = "USED";
            subjectSwitchTwo.GetComponentInChildren<TextMeshProUGUI>().transform.rotation = Quaternion.Euler(0, 0, 20);
            subjectSwitchTwo.GetComponentInChildren<TextMeshProUGUI>().alignment = TextAlignmentOptions.Center;
        }
        else if (switchTwoUsed.Value == false)
        {
            subjectSwitchTwo.GetComponent<Button>().enabled = true;
            subjectSwitchTwo.GetComponentInChildren<TextMeshProUGUI>().text = "Switch \nSubject";
            subjectSwitchTwo.GetComponentInChildren<TextMeshProUGUI>().transform.rotation = Quaternion.Euler(0, 0, 0);
            subjectSwitchTwo.GetComponentInChildren<TextMeshProUGUI>().alignment = TextAlignmentOptions.Top;
        }

        if (switchThreeUsed.Value)
        {
            subjectSwitchThree.GetComponent<Button>().enabled = false;
            subjectSwitchThree.GetComponentInChildren<TextMeshProUGUI>().text = "USED";
            subjectSwitchThree.GetComponentInChildren<TextMeshProUGUI>().transform.rotation = Quaternion.Euler(0, 0, 20);
            subjectSwitchThree.GetComponentInChildren<TextMeshProUGUI>().alignment = TextAlignmentOptions.Center;
        }
        else if (switchThreeUsed.Value == false)
        {
            subjectSwitchThree.GetComponent<Button>().enabled = true;
            subjectSwitchThree.GetComponentInChildren<TextMeshProUGUI>().text = "Switch \nSubject";
            subjectSwitchThree.GetComponentInChildren<TextMeshProUGUI>().transform.rotation = Quaternion.Euler(0, 0, 0);
            subjectSwitchThree.GetComponentInChildren<TextMeshProUGUI>().alignment = TextAlignmentOptions.Top;
        }
    }

    /// <summary>
    /// Assign position to each player to avoid stacking of player sprites
    /// </summary>
    /// <param name="posXStart"></param>
    /// <param name="posYTop"></param>
    public void SetPlayerPositioning(ref float posXStart, ref float posYTop)
    {
        switch (playerId)
        {
            case 0:
                posXStart = 16.1f;
                posYTop = 11.0f;
                break;
            case 1:
                posXStart = 17;
                posYTop = 12;
                break;
            case 2:
                posXStart = 18;
                posYTop = 13;
                break;
            case 3:
                posXStart = 18.9f;
                posYTop = 14;
                break;
        }
    }

    /// <summary>
    /// Moving the player after the number dice is rolled
    /// </summary>
    public void MovePlayer()
    {
        FindObjectOfType<OnlineGameManager>().logger.LogInformation("Player being moved playerID: " + playerId);
        float posXStart = 16.1f;
        float posYTop = 11.0f;
        SetPlayerPositioning(ref posXStart, ref posYTop);
        Vector3 currentPos = FindObjectOfType<OnlineGameManager>().gameSquares[currentPosition.Value].transform.position;
        
        //If player is in final exams stage
        if (currentPosition.Value >= 53)
        {
            FindObjectOfType<OnlineGameManager>().SendServerRpc("FinalExams", playerId.ToString(), playerId);
            //currentPosition.Value++;
            int newPos = currentPosition.Value + 1;
            currentPos = FindObjectOfType<OnlineGameManager>().gameSquares[newPos].transform.position;

            FindObjectOfType<OnlineGameManager>().logger.LogInformation("Final Exams stage, new Position(CurrentPos): " + currentPos);
            FindObjectOfType<OnlineGameManager>().SendServerRpc("PlayerMove", newPos.ToString(), playerId);
            PositionPlayerOnSquareServerRpc(posXStart, posYTop);

        }
        else
        {
            if (currentPosition.Value + rolledNumber.Value > 53) //If next square is within final Exams
            {
                int newPos = 53;
                currentPos = FindObjectOfType<OnlineGameManager>().gameSquares[newPos].transform.position;
                FindObjectOfType<OnlineGameManager>().SendServerRpc("PlayerMove", newPos.ToString(), playerId);
                transform.position = new Vector3(posXStart, currentPos.y, currentPos.z); //Right Of Board
            }
            else //If next position is not part of final exams
            {
                int newPos = currentPosition.Value + rolledNumber.Value;
                FindObjectOfType<OnlineGameManager>().SendServerRpc("PlayerMove", newPos.ToString(),playerId);
                FindObjectOfType<OnlineGameManager>().logger.LogInformation("Player moved to: " + currentPosition.Value + "NewPosVariable=" + newPos);
                PositionPlayerOnSquareServerRpc(posXStart, posYTop);
            }
        }

        if (currentPosition.Value < 0)
        {
            currentPosition.Value = 0;
            currentPos = FindObjectOfType<GameManager>().gameSquares[currentPosition.Value].transform.position;
            transform.position = new Vector3(posXStart, currentPos.y, currentPos.z); //Right Of Board
        }
    }

    /// <summary>
    /// Check if player has completed the game(Checked by server everytime a player moves)
    /// </summary>
    public void CompleteGame()
    {
        float posXStart = 16.1f;
        float posYTop = 11.0f;
        SetPlayerPositioning(ref posXStart, ref posYTop);

        if (currentPosition.Value > 58)
        {
            //WIN
            FindObjectOfType<OnlineGameManager>().logger.LogInformation("Game Complete");
            FindObjectOfType<OnlineGameManager>().SendServerRpc("GameComplete", playerId.ToString(), playerId);
            PositionPlayerOnSquareServerRpc(posXStart, posYTop);
        }
    }

    /// <summary>
    /// When Final exams are not active. Assign position to player
    /// </summary>
    /// <param name="X"></param>
    /// <param name="Y"></param>
    [ServerRpc(RequireOwnership = false)]
    public void PositionPlayerOnSquareServerRpc(float X, float Y) 
    {
        FindObjectOfType<OnlineGameManager>().logger.LogInformation("Player transform moved from" + transform.position);
        Vector3 currentPos = FindObjectOfType<OnlineGameManager>().gameSquares[currentPosition.Value].transform.position;

        float posXStart = 16.1f;
        float posYTop = 11.0f;
        SetPlayerPositioning(ref posXStart, ref posYTop);

        if (currentPosition.Value < 4)
        {
            transform.position = new Vector3(X, currentPos.y, currentPos.z);//Right Of Board
        }
        if (currentPosition.Value == 4)
        {
            transform.position = new Vector3(posXStart - 0.5f, currentPos.y - playerId / 2, currentPos.z);//Principal Top Right
        }
        if (currentPosition.Value >= 5 && currentPosition.Value < 22)//Top Of Board
        {
            transform.position = new Vector3(currentPos.x, Y, currentPos.z);
        }
        if (currentPosition.Value == 22)
        {
            transform.position = new Vector3(-posXStart, currentPos.y + (playerId / 2), currentPos.z);//Principal Top Left
        }
        if (currentPosition.Value >= 23 && currentPosition.Value < 34) //Left Of Board
        {
            transform.position = new Vector3(-X, currentPos.y, currentPos.z);
        }
        if (currentPosition.Value == 34)
        {
            transform.position = currentPos;
        }
        if (currentPosition.Value >= 35 && currentPosition.Value < 53) //Bottom Of Board
        {
            transform.position = new Vector3(currentPos.x, -Y, currentPos.z);
        }
        if (currentPosition.Value == 53)
        {
            transform.position = currentPos;
        }
        if (currentPosition.Value > 53 && currentPosition.Value < 59)
        {
            transform.position = new Vector3(posXStart, currentPos.y, currentPos.z);
        }
        if (currentPosition.Value == 59)//Win
        {
            transform.position = new Vector3(posXStart, currentPos.y, currentPos.z);
        }
        FindObjectOfType<OnlineGameManager>().logger.LogInformation("Player transform moved to" + transform.position);
    }

    /// <summary>
    /// Assign the correct buttons from array to the corresponding player
    /// </summary>
    public void AssignButtons()
    {
        diceRollButton = FindObjectOfType<OnlineGameManager>().diceRollButtons[playerId];
        subjectRollButton = FindObjectOfType<OnlineGameManager>().subjectRollButtons[playerId];
        FindObjectOfType<OnlineGameManager>().logger.LogInformation("DiceRoll Button Assigned to playerID: " + playerId);
        FindObjectOfType<OnlineGameManager>().logger.LogInformation("SubjectRoll Button Assigned to playerID: " + playerId);
    }
}
