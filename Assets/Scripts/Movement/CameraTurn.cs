using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTurn : MonoBehaviour
{
    public CinemachineVirtualCamera[] playerCameras = new CinemachineVirtualCamera[4];
    public CinemachineVirtualCamera topCamera;

    public float sensitivity = 1.0f;

    private Vector2 previousTouchPosition;
    private CinemachineOrbitalTransposer[] playerTransposers = new CinemachineOrbitalTransposer[4];

    int currentPlayer = 0;

    private void Start()
    {
        for (int i = 0; i < FindObjectOfType<GameManager>().numberOfPlayers; i++)
        {
            playerTransposers[i] = playerCameras[i].GetCinemachineComponent<CinemachineOrbitalTransposer>();
        }
        StartCoroutine(ChangeCamera());
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(0)) // Check if left mouse button is held down
        {
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);

                if (touch.phase == TouchPhase.Moved)
                {
                    // Calculate the delta in touch position
                    Vector2 touchDeltaPosition = touch.deltaPosition;

                    float xDelta = touchDeltaPosition.y * sensitivity * Time.deltaTime;

                    playerTransposers[currentPlayer].m_XAxis.Value += xDelta;
                }

                if (touch.phase == TouchPhase.Began)
                {
                    previousTouchPosition = touch.position;
                }
            }
        }
    }

    public void NextPlayer()
    {
        for (int i = 0; i < FindObjectOfType<GameManager>().numberOfPlayers; i++)
        {
            if (FindObjectOfType<GameManager>().players[i].GetComponent<PlayerController>().isActivePlayer)
            {
                currentPlayer = i;
            }
        }
        StartCoroutine(ChangeCamera());
    }

    IEnumerator ChangeCamera()
    {
        topCamera.Priority = 1;
        playerCameras[0].Priority = 0;
        playerCameras[1].Priority = 0;
        playerCameras[2].Priority = 0;
        playerCameras[3].Priority = 0;
        yield return new WaitForSeconds(3);
        topCamera.Priority = 0;
        for (int i = 0; i < FindObjectOfType<GameManager>().numberOfPlayers; i++)
        {
            playerCameras[i].Priority = 0;
            if (FindObjectOfType<GameManager>().players[i].GetComponent<PlayerController>().isActivePlayer)
            {
                playerCameras[i].Priority = 1;
            }
        }
    }

}
