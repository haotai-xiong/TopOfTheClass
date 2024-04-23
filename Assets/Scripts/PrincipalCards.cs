using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using static Questions;
using UnityEngine.SceneManagement;

public class PrincipalCards : MonoBehaviour
{
    public List<string> loadedPrincipalCards;

    public List<PrincipalCard> principalCards = new List<PrincipalCard>();

    public int nextMove = 0;
    public string nextDirection = "";
    public string type = "";

    public struct PrincipalCard
    {
        public string cardText;
        public string rule;
    }

    // Start is called before the first frame update
    void Start()
    {
        var loadedPrincipal = Resources.Load("PrincipalOffice");
        loadedPrincipalCards = loadedPrincipal.ToString().Split(new[] { '\n' }).ToList<string>();
        SplitCards();
    }

    public void SplitCards()
    {
        Debug.Log(loadedPrincipalCards);
        PrincipalCard card = new PrincipalCard();
        card.cardText = "";
        card.rule = "";

        for (int i = 0; i < loadedPrincipalCards.Count -1; i++)
        {
            var data = loadedPrincipalCards[i].Split(",");
            for (int j = 1; j < 3; j++)
            {
                //Debug.Log(j.ToString() + data[j].ToString());
                if (j == 1)
                {
                    card.cardText = data[j].ToString();
                }
                else if (j == 2)
                {
                    card.rule = data[j].ToString();
                }
            }
            principalCards.Add(card);
        }
    }

    public void DrawRandomPrincipalCard()
    {
        int randomCard = Random.Range(1, principalCards.Count + GetComponent<WildCards>().wildCards.Count);
        // randomCard = principalCards.Count + 16; // test

        if (randomCard < principalCards.Count)
        {
            type = "principal";
        }
        else if (randomCard >= principalCards.Count)
        {
            type = "wild";
        }

        if(SceneManager.GetActiveScene().name == "Online")
        {
            if ("principal" == type)
            {
                GetComponent<OnlineGameManager>().principalText.text = principalCards[randomCard].cardText + "\n\n" + principalCards[randomCard].rule;
            }
            else if ("wild" == type)
            {
                if (GetComponent<WildCards>().wildCards[randomCard - principalCards.Count].rule == "MoveToSeptember")
                {
                    ServiceLocator.EventManager.RaiseEvent(GameEvent.MoveToSeptember, 0);
                }
                else if (GetComponent<WildCards>().wildCards[randomCard - principalCards.Count].rule == "MoveToStart")
                {
                    ServiceLocator.EventManager.RaiseEvent(GameEvent.MoveToStart, 0);
                }
                else if (GetComponent<WildCards>().wildCards[randomCard - principalCards.Count].rule == "PickPlayersToMonth")
                {
                    ServiceLocator.EventManager.RaiseEvent(GameEvent.PickPlayersToMonth, 0);
                }
                else if (GetComponent<OnlineGameManager>().players.Count > 1)
                {
                    if (GetComponent<WildCards>().wildCards[randomCard - principalCards.Count].rule == "SwitchPositionWithOnePlayer")
                    {
                        ServiceLocator.EventManager.RaiseEvent(GameEvent.SwitchPositionWithOnePlayer, 0);
                    }
                    else if (GetComponent<WildCards>().wildCards[randomCard - principalCards.Count].rule == "PickTwoPlayerToSwitchPosition")
                    {
                        ServiceLocator.EventManager.RaiseEvent(GameEvent.PickTwoPlayerToSwitchPosition, 0);
                    }
                    else if (GetComponent<WildCards>().wildCards[randomCard - principalCards.Count].rule == "PickOnePlayerToMissTurn")
                    {
                        ServiceLocator.EventManager.RaiseEvent(GameEvent.PickOnePlayerToMissTurn, 0);
                    }
                }
                else if (GetComponent<OnlineGameManager>().players.Count == 1)
                {
                    DrawRandomPrincipalCard();
                }
            }
        }
        else
        {
            if ("principal" == type)
            {
                GetComponent<GameManager>().principalText.text = principalCards[randomCard].cardText + "\n\n" + principalCards[randomCard].rule;
            }
            else if ("wild" == type)
            {
                if (GetComponent<WildCards>().wildCards[randomCard - principalCards.Count].rule == "MoveToSeptember")
                {
                    ServiceLocator.EventManager.RaiseEvent(GameEvent.MoveToSeptember, 0);
                }
                else if (GetComponent<WildCards>().wildCards[randomCard - principalCards.Count].rule == "MoveToStart")
                {
                    ServiceLocator.EventManager.RaiseEvent(GameEvent.MoveToStart, 0);
                }
                else if (GetComponent<WildCards>().wildCards[randomCard - principalCards.Count].rule == "PickPlayersToMonth")
                {
                    ServiceLocator.EventManager.RaiseEvent(GameEvent.PickPlayersToMonth, 0);
                }
                else if (GetComponent<GameManager>().numberOfPlayers > 1)
                {
                    if (GetComponent<WildCards>().wildCards[randomCard - principalCards.Count].rule == "SwitchPositionWithOnePlayer")
                    {
                        ServiceLocator.EventManager.RaiseEvent(GameEvent.SwitchPositionWithOnePlayer, 0);
                    }
                    else if (GetComponent<WildCards>().wildCards[randomCard - principalCards.Count].rule == "PickTwoPlayerToSwitchPosition")
                    {
                        ServiceLocator.EventManager.RaiseEvent(GameEvent.PickTwoPlayerToSwitchPosition, 0);
                    }
                    else if (GetComponent<WildCards>().wildCards[randomCard - principalCards.Count].rule == "PickOnePlayerToMissTurn")
                    {
                        ServiceLocator.EventManager.RaiseEvent(GameEvent.PickOnePlayerToMissTurn, 0);
                    }
                }
                else if (GetComponent<GameManager>().numberOfPlayers == 1)
                {
                    DrawRandomPrincipalCard();
                }
            }
        }

        if ("principal" == type)
        {
            if (randomCard >= principalCards.Count)
            {
                randomCard = Random.Range(1, principalCards.Count);
            }

            if (principalCards[randomCard].rule.Contains("forward"))
            {
                nextDirection = "forward";
            }
            else if (principalCards[randomCard].rule.Contains("back"))
            {
                nextDirection = "back";
            }

            //Change to Switch
            if (principalCards[randomCard].rule.Contains("1"))
            {
                nextMove = 1;
            }
            else if (principalCards[randomCard].rule.Contains("2"))
            {
                nextMove = 2;
            }
            else if (principalCards[randomCard].rule.Contains("3"))
            {
                nextMove = 3;
            }
            else if (principalCards[randomCard].rule.Contains("4"))
            {
                nextMove = 4;
            }
            else if (principalCards[randomCard].rule.Contains("5"))
            {
                nextMove = 5;
            }
            else if (principalCards[randomCard].rule.Contains("6"))
            {
                nextMove = 6;
            }
        }
    }
}
