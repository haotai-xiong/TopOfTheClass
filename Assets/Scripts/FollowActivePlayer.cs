using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FollowActivePlayer : MonoBehaviour
{
    public bool followActivePlayer = false;

    // Update is called once per frame
    void Update()
    {
        if(SceneManager.GetActiveScene().name == "Online")
        {
            if (followActivePlayer)
            {
                transform.position = new Vector3(FindObjectOfType<OnlineGameManager>().activePlayerPosX, FindObjectOfType<OnlineGameManager>().activePlayerPosY, -14);
            }
            else
            {
                transform.position = new Vector3(0, 0, -30);
            }
        }
        else if(SceneManager.GetActiveScene().name == "Gameplay")
        {
            if (followActivePlayer)
            {
                transform.position = new Vector3(FindObjectOfType<GameManager>().activePlayerPosX, FindObjectOfType<GameManager>().activePlayerPosY, -14);
            }
            else
            {
                transform.position = new Vector3(0, 0, -30);
            }
        }

    }

    public void Zoom()
    {
        followActivePlayer ^= true;
    }
}
