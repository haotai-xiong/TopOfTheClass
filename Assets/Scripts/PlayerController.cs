using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerController : MonoBehaviour
{
    [Header("Player Data")]
    public int playerId = 0;
    public int rolledNumber = 0;
    public int currentPosition = 0;
    public int turnsLeft = 3;
    public bool isActivePlayer;
    public bool gameComplete = false;
    public bool playerOver12 = true;
    public bool switchOneUsed = false;
    public bool switchTwoUsed = false;
    public bool switchThreeUsed = false;
    public bool finalExamsStage = false;
    public bool missingThisTurn;
    public bool flash = false;

    [Header("Switch Subject screen")]
    public GameObject switchSubjectScreen;

    [Header("Other UI Buttons")]
    public GameObject diceRollButton;
    public GameObject subjectRollButton;
    public GameObject acceptRuleButton;
    public GameObject acceptWildRuleButton;
    // public LayerMask defaultLayerMask;

    void Start()
    {
        SetGameObjectsToFalse();

    }

    void Update()
    {
        if (currentPosition >= 53)
        {
            finalExamsStage = true;
        }
        IsClicked();
    }

    private void SetGameObjectsToFalse()
    {
        switchSubjectScreen.SetActive(false);
        acceptRuleButton.SetActive(false);
        acceptWildRuleButton.SetActive(false);
    }

    public void MovePlayer()
    {
        float posXStart = 16.1f;
        float posYTop = 11.0f;
        Vector3 currentPos;

        switch (playerId)
        {
            case 0:
                posXStart = -13.42f;
                posYTop = -2.53f;
                break;
            case 1:
                posXStart = -12.52f;
                posYTop = -2.53f;
                break;
            case 2:
                posXStart = -11.52f;
                posYTop = -2.53f;
                break;
            case 3:
                posXStart = -10.62f;
                posYTop = -2.53f;
                break;
        }

        if(currentPosition >= 53)
        {
            finalExamsStage = true;
            currentPosition++;
            // Causes error
            currentPos = FindObjectOfType<GameManager>().gameSquares[currentPosition].transform.position;
            if (currentPosition > 58)
            {
                //WIN
                transform.position = new Vector3(posXStart, currentPos.y, currentPos.z);
                //gameComplete = true;
                ServiceLocator.EventManager.RaiseEvent(GameEvent.GameOverTrigger, 0);
            }
            else
            {
                transform.position = new Vector3(posXStart, currentPos.y, currentPos.z);//Right Of Board
            }
        }
        else 
        {
            if (currentPosition + rolledNumber > 53)
            {
                currentPosition = 53;
                currentPos = FindObjectOfType<GameManager>().gameSquares[currentPosition].transform.position;
                transform.position = new Vector3(posXStart, currentPos.y, currentPos.z); //Right Of Board
            }
            else
            {
                currentPosition = currentPosition + rolledNumber;
                currentPos = FindObjectOfType<GameManager>().gameSquares[currentPosition].transform.position;
                if(currentPosition < 4)
                {
                    transform.position = new Vector3(posXStart, currentPos.y, currentPos.z);//Right Of Board
                }
                if(currentPosition == 4)
                {
                    transform.position = new Vector3(posXStart - 0.5f, currentPos.y - playerId/2, currentPos.z);//Principal Top Right
                }
                if (currentPosition >= 5 && currentPosition < 22)//Top Of Board
                {
                    transform.position = new Vector3(currentPos.x, posYTop, currentPos.z);
                }
                if (currentPosition == 22)
                {
                    transform.position = new Vector3(-posXStart, currentPos.y + (playerId/2), currentPos.z);//Principal Top Left
                }
                if (currentPosition >= 23 && currentPosition < 34) //Left Of Board
                {
                    transform.position = new Vector3(-posXStart, currentPos.y, currentPos.z);
                }
                if (currentPosition == 34)
                {
                    transform.position = new Vector3(-posXStart, currentPos.y - playerId, currentPos.z);//Principal Bottom Left
                }
                if (currentPosition >= 35 && currentPosition < 53) //Bottom Of Board
                {
                    transform.position = new Vector3(currentPos.x, -posYTop, currentPos.z);
                }
                if (currentPosition == 53)
                {
                    transform.position = new Vector3(posXStart, currentPos.y - playerId, currentPos.z);//Principal Bottom Right
                }
            }
        }
        if(currentPosition < 0)
        {
            currentPosition = 0;
            currentPos = FindObjectOfType<GameManager>().gameSquares[currentPosition].transform.position;
        }

        transform.position = new Vector3(transform.position.x, 1.17f, transform.position.z);
    }

    private void IsClicked()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began && FindObjectOfType<GameManager>().allowSelect)
            {
                Vector3 touchPosWorld = Camera.main.ScreenToWorldPoint(new Vector3(touch.position.x, touch.position.y, 10)); 
                Vector3 direction = (touchPosWorld - Camera.main.transform.position).normalized;
                RaycastHit hit;
                if (Physics.Raycast(Camera.main.transform.position, direction, out hit))
                {
                    if (hit.transform.gameObject == gameObject && !FindObjectOfType<GameManager>().selectedPlayerId.Contains(playerId))
                    {
                        Debug.Log("Hit gameobject: " + gameObject.name);
                        FindObjectOfType<GameManager>().selectedPlayerId.Add(playerId);
                        flash = true;
                        StartCoroutine(ToggleRenderer());
                    }
                    else if (hit.transform.gameObject == gameObject && FindObjectOfType<GameManager>().selectedPlayerId.Contains(playerId))
                    {
                        Debug.Log("Hit gameobject: " + gameObject.name);
                        FindObjectOfType<GameManager>().selectedPlayerId.Remove(playerId);
                        flash = false;
                        StopCoroutine(ToggleRenderer());
                        GetComponent<MeshRenderer>().enabled = true;
                    }
                }
            }
        }
    }

    public void UseTurn()
    {
        if (turnsLeft > 0)
        {
            turnsLeft--;
        }
    }

    public bool CanTakeTurn()
    {
        return turnsLeft > 0;
    }

    public void SetActivePlayer(bool isActive)
    {
        isActivePlayer = isActive;
        if (isActivePlayer)
        {
            FindObjectOfType<SwitchQuestion>().UpdateAttemptsText(turnsLeft);
        }
    }

    public IEnumerator ToggleRenderer()
    {
        while (flash)
        {
            GetComponent<MeshRenderer>().enabled = !GetComponent<MeshRenderer>().enabled;
            yield return new WaitForSeconds(0.25f);
        }
    }
}
