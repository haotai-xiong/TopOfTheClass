using System.Collections;
using System.Net;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class AnalyticsManager : MonoBehaviour
{
    public string _version = "0.0.5";
    public int totalTurnsMade;
    public int gamesFinished;
    public float timeElapsed;

    [System.Serializable]
    public class Data
    {
        public string appName;
        public string version;
        public string category;
        public string sceneName;
        public string eventName;
        public string deviceId;
        public int participantId;

        public int turnsCompleted;
        public float timeTakenToComplete;
    }

    [System.Serializable]
    public class SessionData : Data
    {
        public int totalTurnsCompleted;
        public int gamesCompleted;
        public float timeSpentInGame;
    }

    static AnalyticsManager _instance;

    int _participantId;

    const string URL = "https://compucore.itcarlow.ie/totc/";

    private void Update()
    {
        timeElapsed += Time.deltaTime;
    }

    static public AnalyticsManager Instance()
    {
        if (_instance == null)
        {//Create Analytics Manager
            GameObject dataManager = new GameObject("AnalyticsManager");

            //add AnalyticsManager to the object
            _instance = dataManager.AddComponent<AnalyticsManager>();
            //initialize that object
            _instance.Initialise();

            //stop it from destroying itself
            DontDestroyOnLoad(dataManager);
        }

        return _instance;
    }


    /// <summary>
    /// Initialization of the class upon creation
    /// </summary>
    private void Initialise()
    {
        const string PARTICIPANT_ID = "ParticipantID";
        _participantId = 1;

        if (PlayerPrefs.HasKey(PARTICIPANT_ID))
            _participantId = PlayerPrefs.GetInt(PARTICIPANT_ID);
            _participantId++;
        
        PlayerPrefs.SetInt(PARTICIPANT_ID, _participantId);

        LogEvent("App Started",0,0);
    }



    /// <summary>
    /// Used for logging events.
    /// </summary>
    /// <param name="eventName">What happened?</param>
    public void LogEvent(string eventName, int turns, float timeToCompleteGame)
    {
        Data data = new Data()
        {
            appName = "Top Of The Class",
            category = "Event",
            sceneName = SceneManager.GetActiveScene().name,
            eventName = eventName,
            deviceId = SystemInfo.deviceUniqueIdentifier,
            participantId = _participantId,
            turnsCompleted = turns,
            timeTakenToComplete = timeToCompleteGame,
            version = _version
            
        };

        string jsonData = JsonUtility.ToJson(data);
        StartCoroutine(PostMethod(jsonData));
    }

    /// <summary>
    /// Used for logging sessions.
    /// </summary>
    /// <param name="eventName">What happened?</param>
    /// <param name="turns">number of turns taken by players in the game</param>
    /// <param name="gamesCompleted">Number of games the player completed</param>
    /// <param name="timeInApp">Total Time using the app</param>
    public void LogSession( string eventName, int turns, int gamesCompleted, float timeInApp)
    {
        SessionData data = new SessionData()
        {
            appName = "Top Of The Class",
            category = "Session",
            sceneName = SceneManager.GetActiveScene().name,
            eventName = eventName,
            deviceId = SystemInfo.deviceUniqueIdentifier,
            participantId = _participantId,
            totalTurnsCompleted = turns,
            gamesCompleted = gamesCompleted,
            timeSpentInGame = timeInApp,
            version = _version
        };

        string jsonData = JsonUtility.ToJson(data);
        StartCoroutine(PostMethod(jsonData));
    }



    private IEnumerator PostMethod(string jsonData)
    {
        if (Debug.isDebugBuild)
            print(jsonData);

        string method = "upload_data";
        using (UnityWebRequest request = UnityWebRequest.Put(URL + method, jsonData))
        {
            request.method = UnityWebRequest.kHttpVerbPOST;
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Accept", "application/json");
        
            yield return request.SendWebRequest();
        
            if (!request.isNetworkError && request.responseCode == (int)HttpStatusCode.OK)
                Debug.Log("Data succesfully sent to the server");
            else
            {
                Debug.Log("Error sending data to the server: Error " + request.responseCode);
                Debug.Log("Error: " + request.error);
            }
        }

        //print("Sent data to: " + URL + method);
    }


    public int ParticipantId { get => _participantId; }
}
