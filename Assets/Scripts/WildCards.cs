using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PrincipalCards;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class WildCards : MonoBehaviour
{
    public List<WildCard> wildCards = new();

    public string currentRule = "";
    private bool continueVar = false;

    public struct WildCard
    {
        public string cardText;
        public string rule;
    }

    // Start is called before the first frame update
    void Start()
    {
        ServiceLocator.EventManager.Subscribe(GameEvent.MoveToSeptember, MoveToSeptemberDialogBox);
        ServiceLocator.EventManager.Subscribe(GameEvent.MoveToStart, MoveToStartDialogBox);
        ServiceLocator.EventManager.Subscribe(GameEvent.SwitchPositionWithOnePlayer, SwitchPositionWithOnePlayerDialogBox);
        ServiceLocator.EventManager.Subscribe(GameEvent.PickTwoPlayerToSwitchPosition, PickTwoPlayerToSwitchPositionDialogBox);
        ServiceLocator.EventManager.Subscribe(GameEvent.PickOnePlayerToMissTurn, PickOnePlayerToMissTurnDialogBox);
        ServiceLocator.EventManager.Subscribe(GameEvent.PickPlayersToMonth, PickPlayersToMonthDialogBox);

        WildCard wildCard1 = new()
        {
            cardText = "College Started!\n\nMove to September",
            rule = "MoveToSeptember"
        };
        wildCards.Add(wildCard1);
        wildCards.Add(wildCard1);
        wildCards.Add(wildCard1);
        WildCard wildCard2 = new()
        {
            cardText = "You Break the College rules!\n\nMove to Start Tile",
            rule = "MoveToStart"
        };
        wildCards.Add(wildCard2);
        wildCards.Add(wildCard2);
        wildCards.Add(wildCard2);
        WildCard wildCard3 = new()
        {
            cardText = "You Get the Highest Score\nIn Your Class!\n\nChoose a Player\nTo Switch Position",
            rule = "SwitchPositionWithOnePlayer"
        };
        wildCards.Add(wildCard3);
        wildCards.Add(wildCard3);
        wildCards.Add(wildCard3);
        WildCard wildCard4 = new()
        {
            cardText = "What doesn't fail you just isn't finished yet!\n\nChoose Two Players\nTo Switch Their Position",
            rule = "PickTwoPlayerToSwitchPosition"
        };
        wildCards.Add(wildCard4);
        wildCards.Add(wildCard4);
        wildCards.Add(wildCard4);
        WildCard wildCard5 = new()
        {
            cardText = "Are you gonna finish that?\n\nChoose a Player\nTo Miss The Turn!",
            rule = "PickOnePlayerToMissTurn"
        };
        wildCards.Add(wildCard5);
        wildCards.Add(wildCard5);
        wildCards.Add(wildCard5);
        WildCard wildCard6 = new()
        {
            cardText = "Watch your step!\n\nChoose One or Two Players\nTo A Certain Month!",
            rule = "PickPlayersToMonth"
        };
        wildCards.Add(wildCard6);
        wildCards.Add(wildCard6);
        wildCards.Add(wildCard6);
    }

    public void DrawRandomWildCard()
    {
        int randomCard = Random.Range(0, wildCards.Count);
        if (SceneManager.GetActiveScene().name == "Online")
        {
            GetComponent<OnlineGameManager>().principalText.text = wildCards[randomCard].cardText + "\n\n" + wildCards[randomCard].rule;
        }
        else
        {
            GetComponent<GameManager>().principalText.text = wildCards[randomCard].cardText; //+ "\n\n" + wildCards[randomCard].rule;
        }

        //Change to Switch
        if (wildCards[randomCard].rule.Contains("MoveToSeptember"))
        {
            currentRule = "MoveToSeptember";
        }
        else if (wildCards[randomCard].rule.Contains("MoveToStart"))
        {
            currentRule = "MoveToStart";
        }
        else if (wildCards[randomCard].rule.Contains("SwitchPositionWithOnePlayer"))
        {
            currentRule = "SwitchPositionWithOnePlayer";
        }
        else if (wildCards[randomCard].rule.Contains("PickTwoPlayerToSwitchPosition"))
        {
            currentRule = "PickTwoPlayerToSwitchPosition";
        }
        else if (wildCards[randomCard].rule.Contains("PickOnePlayerToMissTurn"))
        {
            currentRule = "PickOnePlayerToMissTurn";
        }
        else if (wildCards[randomCard].rule.Contains("PickPlayersToMonth"))
        {
            currentRule = "PickPlayersToMonth";
        }
    }

    public void ContinueVar()
    {
        continueVar = true;
        Destroy(GameObject.Find("DialogBox(Clone)"));
    }

    /// <summary>
    /// Move to september
    /// </summary>
    void MoveToSeptemberDialogBox(int p)
    {
        if (0 == p)
        {
            StartCoroutine(UIInteractionSystem.Instance.ShowDialog("College Started!\n\nMove to September", "Okay", MoveToSeptember));
        }
    }

    void MoveToSeptember()
    {
        for (int i = 0; i < GetComponent<GameManager>().players.Count; i++)
        {
            if (GetComponent<GameManager>().players[i].GetComponent<PlayerController>().isActivePlayer)
            {
                GetComponent<GameManager>().players[i].GetComponent<PlayerController>().currentPosition = GetComponent<GameManager>().gameSquares.FindIndex(square => square.name == "September Square");
                GetComponent<GameManager>().players[i].transform.position = FindObjectOfType<GameManager>().gameSquares[GetComponent<GameManager>().players[i].GetComponent<PlayerController>().currentPosition].transform.position;
                GetComponent<GameManager>().players[i].GetComponent<PlayerController>().subjectRollButton.GetComponent<Button>().enabled = true;
                GetComponent<GameManager>().IsMissingTurn(i);
            }
        }
        Destroy(GameObject.Find("DialogBox(Clone)(Clone)"));
    }

    /// <summary>
    /// Move to start
    /// </summary>
    void MoveToStartDialogBox(int p)
    {
        if (0 == p)
        {
            StartCoroutine(UIInteractionSystem.Instance.ShowDialog("You Break the College rules!\n\nMove to Start Tile", "Okay", MoveToStart));
        }
    }

    void MoveToStart()
    {
        for (int i = 0; i < GetComponent<GameManager>().players.Count; i++)
        {
            if (GetComponent<GameManager>().players[i].GetComponent<PlayerController>().isActivePlayer)
            {
                GetComponent<GameManager>().players[i].GetComponent<PlayerController>().currentPosition = GetComponent<GameManager>().gameSquares.FindIndex(square => square.name == "Start Square");
                GetComponent<GameManager>().players[i].transform.position = FindObjectOfType<GameManager>().gameSquares[GetComponent<GameManager>().players[i].GetComponent<PlayerController>().currentPosition].transform.position;
                GetComponent<GameManager>().players[i].GetComponent<PlayerController>().subjectRollButton.GetComponent<Button>().enabled = true;
                GetComponent<GameManager>().IsMissingTurn(i);
            }
        }
        Destroy(GameObject.Find("DialogBox(Clone)(Clone)"));
    }

    /// <summary>
    /// Switch Position With One Player
    /// </summary>
    void SwitchPositionWithOnePlayerDialogBox(int p)
    {
        if (0 == p)
        {
            StartCoroutine(UIInteractionSystem.Instance.ShowDialog("You Get the Highest Score\nIn Your Class!\n\nChoose a Player\nTo Switch Position", "Okay", SwitchPositionWithOnePlayer));
        }
    }

    void SwitchPositionWithOnePlayer()
    {
        for (int i = 0; i < GetComponent<GameManager>().players.Count; i++)
        {
            if (GetComponent<GameManager>().players[i].GetComponent<PlayerController>().isActivePlayer)
            {
                StartCoroutine(SwitchPositionWithOnePlayerCoroutine(i));
            }
        }
        Destroy(GameObject.Find("DialogBox(Clone)(Clone)"));
    }

    IEnumerator SwitchPositionWithOnePlayerCoroutine(int currentPlayerIndex)
    {
        GetComponent<GameManager>().allowSelect = true; ;
        PlayerController currentPlayer = GetComponent<GameManager>().players[currentPlayerIndex].GetComponent<PlayerController>();
        yield return new WaitUntil(() => GetComponent<GameManager>().selectedPlayerId.Count == 1);

        StartCoroutine(UIInteractionSystem.Instance.ShowDialog("You selected to switch with\nPlayer " + (GetComponent<GameManager>().selectedPlayerId[0] + 1), "Okay", ContinueVar));
        yield return new WaitUntil(() => continueVar);
        continueVar = false;

        int selectedPlayerIndex = GetComponent<GameManager>().selectedPlayerId[0];
        if (selectedPlayerIndex != -1 && selectedPlayerIndex != currentPlayerIndex)
        {
            PlayerController selectedPlayer = GetComponent<GameManager>().players[selectedPlayerIndex].GetComponent<PlayerController>();
            selectedPlayer.flash = false;
            selectedPlayer.GetComponent<MeshRenderer>().enabled = true;
            (selectedPlayer.currentPosition, currentPlayer.currentPosition) = (currentPlayer.currentPosition, selectedPlayer.currentPosition);
            currentPlayer.transform.position = GetComponent<GameManager>().gameSquares[currentPlayer.currentPosition].transform.position;
            selectedPlayer.transform.position = GetComponent<GameManager>().gameSquares[selectedPlayer.currentPosition].transform.position;

            GetComponent<GameManager>().allowSelect = false;
            GetComponent<GameManager>().selectedPlayerId = new List<int>();
            GetComponent<GameManager>().players[currentPlayerIndex].GetComponent<PlayerController>().subjectRollButton.GetComponent<Button>().enabled = true;
            GetComponent<GameManager>().IsMissingTurn(currentPlayerIndex);
        }
    }

    /// <summary>
    /// Pick two players to switch their positions
    /// </summary>
    void PickTwoPlayerToSwitchPositionDialogBox(int p)
    {
        if (0 == p)
        {
            StartCoroutine(UIInteractionSystem.Instance.ShowDialog("What doesn't fail you just isn't finished yet!\n\nChoose Two Players\nTo Switch", "Okay", PickTwoPlayerToSwitchPosition));
        }
    }

    void PickTwoPlayerToSwitchPosition()
    {
        for (int i = 0; i < GetComponent<GameManager>().players.Count; i++)
        {
            if (GetComponent<GameManager>().players[i].GetComponent<PlayerController>().isActivePlayer)
            {
                StartCoroutine(SwitchPositionWithTwoPlayerCoroutine(i));
            }
        }
        Destroy(GameObject.Find("DialogBox(Clone)(Clone)"));
    }

    IEnumerator SwitchPositionWithTwoPlayerCoroutine(int currentPlayerIndex)
    {
        GetComponent<GameManager>().allowSelect = true;
        yield return new WaitUntil(() => GetComponent<GameManager>().selectedPlayerId.Count == 2);
        yield return new WaitForSeconds(2.0f);
        yield return new WaitUntil(() => GetComponent<GameManager>().selectedPlayerId.Count == 2);

        StartCoroutine(UIInteractionSystem.Instance.ShowDialog("You selected\nPlayer " + (GetComponent<GameManager>().selectedPlayerId[0] + 1) + "\nAnd\nPlayer " + (GetComponent<GameManager>().selectedPlayerId[1] + 1), "Okay", ContinueVar));
        yield return new WaitUntil(() => continueVar);
        continueVar = false;

        int selectedPlayerIndexOne = GetComponent<GameManager>().selectedPlayerId[0];
        int selectedPlayerIndexTwo = GetComponent<GameManager>().selectedPlayerId[1];

        PlayerController selectedPlayerOne = GetComponent<GameManager>().players[selectedPlayerIndexOne].GetComponent<PlayerController>();
        PlayerController selectedPlayerTwo = GetComponent<GameManager>().players[selectedPlayerIndexTwo].GetComponent<PlayerController>();
        selectedPlayerOne.flash = false;
        selectedPlayerOne.GetComponent<MeshRenderer>().enabled = true;
        selectedPlayerTwo.flash = false;
        selectedPlayerTwo.GetComponent<MeshRenderer>().enabled = true;

        (selectedPlayerOne.currentPosition, selectedPlayerTwo.currentPosition) = (selectedPlayerTwo.currentPosition, selectedPlayerOne.currentPosition);
        selectedPlayerOne.transform.position = GetComponent<GameManager>().gameSquares[selectedPlayerOne.currentPosition].transform.position;
        selectedPlayerTwo.transform.position = GetComponent<GameManager>().gameSquares[selectedPlayerTwo.currentPosition].transform.position;

        GetComponent<GameManager>().allowSelect = false;
        GetComponent<GameManager>().selectedPlayerId = new List<int>();
        GetComponent<GameManager>().players[currentPlayerIndex].GetComponent<PlayerController>().subjectRollButton.GetComponent<Button>().enabled = true;
        GetComponent<GameManager>().IsMissingTurn(currentPlayerIndex);
    }

    /// <summary>
    /// Pick One Player To Miss a Turn
    /// </summary>
    void PickOnePlayerToMissTurnDialogBox(int p)
    {
        if (0 == p)
        {
            StartCoroutine(UIInteractionSystem.Instance.ShowDialog("Are you gonna finish that?\n\nChoose a Player\nTo Miss The Turn!", "Okay", PickOnePlayerToMissTurn));
        }
    }

    void PickOnePlayerToMissTurn()
    {
        for (int i = 0; i < GetComponent<GameManager>().players.Count; i++)
        {
            if (GetComponent<GameManager>().players[i].GetComponent<PlayerController>().isActivePlayer)
            {
                StartCoroutine(PickOnePlayerToMissTurnCoroutine(i));
            }
        }
        Destroy(GameObject.Find("DialogBox(Clone)(Clone)"));
    }

    IEnumerator PickOnePlayerToMissTurnCoroutine(int currentPlayerIndex)
    {
        GetComponent<GameManager>().allowSelect = true;
        yield return new WaitUntil(() => GetComponent<GameManager>().selectedPlayerId.Count == 1);

        StartCoroutine(UIInteractionSystem.Instance.ShowDialog("You selected\nPlayer " + (GetComponent<GameManager>().selectedPlayerId[0] + 1) + "\nTo miss a turn", "Okay", ContinueVar));
        yield return new WaitUntil(() => continueVar);
        continueVar = false;

        GetComponent<GameManager>().players[GetComponent<GameManager>().selectedPlayerId[0]].GetComponent<PlayerController>().missingThisTurn = true;
        GetComponent<GameManager>().players[GetComponent<GameManager>().selectedPlayerId[0]].GetComponent<PlayerController>().flash = false;
        GetComponent<GameManager>().players[GetComponent<GameManager>().selectedPlayerId[0]].GetComponent<MeshRenderer>().enabled = true;
        GetComponent<GameManager>().allowSelect = false;
        GetComponent<GameManager>().selectedPlayerId = new List<int>();
        GetComponent<GameManager>().players[currentPlayerIndex].GetComponent<PlayerController>().subjectRollButton.GetComponent<Button>().enabled = true;
        GetComponent<GameManager>().IsMissingTurn(currentPlayerIndex);
    }

    /// <summary>
    /// Pick 1 or 2 Players To a certain Month
    /// </summary>
    void PickPlayersToMonthDialogBox(int p)
    {
        if (0 == p)
        {
            StartCoroutine(UIInteractionSystem.Instance.ShowDialog("Watch your step!\n\nChoose One or Two Players\nTo A Certain Month!", "Okay", PickPlayersToMonth));
        }
    }

    void PickPlayersToMonth()
    {
        for (int i = 0; i < GetComponent<GameManager>().players.Count; i++)
        {
            if (GetComponent<GameManager>().players[i].GetComponent<PlayerController>().isActivePlayer)
            {
                StartCoroutine(PickPlayersToMonthCoroutine(i));
            }
        }
        Destroy(GameObject.Find("DialogBox(Clone)(Clone)"));
    }

    IEnumerator PickPlayersToMonthCoroutine(int currentPlayerIndex)
    {
        StartCoroutine(UIInteractionSystem.Instance.ShowDialogTwoButton("Move 1 or 2 Players?\n\nSelect player chess\nAfter clicking the buttons below", "One", OnePlayerMoveToMonth, "Two", TwoPlayerMoveToMonth));
        yield return new WaitUntil(() => GetComponent<GameManager>().numOfPlayerMoveToMonth == 1 || GetComponent<GameManager>().numOfPlayerMoveToMonth == 2);

        GetComponent<GameManager>().allowSelect = true;
        yield return new WaitUntil(() => GetComponent<GameManager>().selectedPlayerId.Count == GetComponent<GameManager>().numOfPlayerMoveToMonth);

        if (1 == GetComponent<GameManager>().numOfPlayerMoveToMonth)
        {
            StartCoroutine(UIInteractionSystem.Instance.ShowDialog("You selected\nPlayer " + (GetComponent<GameManager>().selectedPlayerId[0] + 1), "Okay", ContinueVar));
        }
        else if (2 == GetComponent<GameManager>().numOfPlayerMoveToMonth)
        {   
            StartCoroutine(UIInteractionSystem.Instance.ShowDialog("You selected\nPlayer " + (GetComponent<GameManager>().selectedPlayerId[0] + 1) + "\nAnd\nPlayer " + (GetComponent<GameManager>().selectedPlayerId[1] + 1), "Okay", ContinueVar));
        }
        yield return new WaitUntil(() => continueVar);
        Destroy(GameObject.Find("DialogBox(Clone)(Clone)"));
        continueVar = false;

        foreach (var temp in GetComponent<GameManager>().monthButton)
        {
            temp.SetActive(true);
        }
        yield return new WaitUntil(() => GetComponent<GameManager>().monthSelected);

        foreach (var temp in GetComponent<GameManager>().monthButton)
        {
            temp.SetActive(false);
        }

        foreach (var index in GetComponent<GameManager>().selectedPlayerId)
        {
            GetComponent<GameManager>().players[index].GetComponent<PlayerController>().currentPosition = GetComponent<GameManager>().gameSquares.FindIndex(square => square.name == GetComponent<GameManager>().selectedMonth);
            GetComponent<GameManager>().players[index].GetComponent<PlayerController>().flash = false;
            GetComponent<GameManager>().players[index].GetComponent<MeshRenderer>().enabled = true;
            GetComponent<GameManager>().players[index].transform.position = FindObjectOfType<GameManager>().gameSquares[GetComponent<GameManager>().players[index].GetComponent<PlayerController>().currentPosition].transform.position;
        }

        GetComponent<GameManager>().numOfPlayerMoveToMonth = -1;
        GetComponent<GameManager>().monthSelected = false;
        GetComponent<GameManager>().allowSelect = false;
        GetComponent<GameManager>().selectedPlayerId = new List<int>();
        GetComponent<GameManager>().players[currentPlayerIndex].GetComponent<PlayerController>().subjectRollButton.GetComponent<Button>().enabled = true;
        GetComponent<GameManager>().IsMissingTurn(currentPlayerIndex);
    }

    private void OnePlayerMoveToMonth()
    {
        GetComponent<GameManager>().numOfPlayerMoveToMonth = 1;
        Destroy(GameObject.Find("DialogBox(Clone)(Clone)"));
    }

    private void TwoPlayerMoveToMonth()
    {
        GetComponent<GameManager>().numOfPlayerMoveToMonth = 2;
        Destroy(GameObject.Find("DialogBox(Clone)(Clone)"));
    }
}
