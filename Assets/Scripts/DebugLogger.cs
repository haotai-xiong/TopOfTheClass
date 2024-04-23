using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;

public class DebugLogger : MonoBehaviour
{
    public void LogInformation(string log)
    {
        if(SceneManager.GetActiveScene().name == "Gameplay")
        {
            Debug.Log("Local Game: " + log);
        }
        else
        {
            if (!NetworkManager.Singleton.IsServer)
            {
                Debug.Log("This is the Client: " + log);
            }
            else if (NetworkManager.Singleton.IsServer)
            {
                Debug.Log("This is the Server/Host: " + log);
            }
        }
    }
}
