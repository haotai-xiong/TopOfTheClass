using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using UnityEngine.UI;
using System.IO;
using TMPro;
using UnityEditor;
using Unity.VisualScripting;

public class SceneLoader : MonoBehaviour
{
    Dictionary<string, AsyncOperation> loadedScenes = new Dictionary<string, AsyncOperation>();
    List<string> tipsList = new List<string>();
    float loadTime = 5.0f;
    string gameplaySceneName = "Gameplay";
  //  string mainMenuSceneName = "Main Menu";
    Image image;
    TextMeshProUGUI tipsText;
    bool realMode = false;
    bool isDoneLoading = false;

    public static SceneLoader instance;
    // public GameObject backgroundMusic;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void Start()
    {
        image = GameObject.Find("Loading Bar").GetComponent<Image>();
        tipsText = GameObject.Find("Tips Text").GetComponent<TextMeshProUGUI>();
        LoadTipsFromTextFile();
        LoadScenesAsync();
        if (realMode)
        {

        }
        else
        {
            StartCoroutine(FillProgressBarTimed());
        }
        StartCoroutine(RotateThroughTips());
        print(loadedScenes);
    }

    static public SceneLoader Instance()
    {
        if (instance == null)
        {
            GameObject sceneLoader = new GameObject("SceneLoader");

            //add AnalyticsManager to the object
            instance = sceneLoader.AddComponent<SceneLoader>();

            DontDestroyOnLoad(sceneLoader);
        }

        return instance;
    }
    public void LoadScenesAsync()
    {
        //StartCoroutine(LoadSceneAsync(mainMenuSceneName));
        StartCoroutine(LoadSceneAsync(gameplaySceneName));
    }

    private IEnumerator LoadSceneAsync(string sceneName)
    {
        if (loadedScenes.ContainsKey(sceneName))
        {
            Debug.LogWarning($"Scene '{sceneName}' is already loaded.");
            yield break;
        }

        if (SceneManager.GetSceneByName(sceneName) == null)
        {
            Debug.LogError($"Scene '{sceneName}' does not exist.");
            yield break;
        }

        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneName);
        asyncOperation.allowSceneActivation = false;

        while (!asyncOperation.isDone)
        {
            float progress = asyncOperation.progress / 0.9f;
            print(sceneName + ":" + progress);
            if (sceneName == gameplaySceneName)
            {
                SetProgressValue(progress);

                if (progress >= 0.9999f && realMode)
                {
                    SceneManager.LoadScene("Main Menu");
                }

            }

            if (asyncOperation.progress >= 0.9f)
            {
                loadedScenes[sceneName] = asyncOperation;
            }

            yield return null;
        }
    }

    public void ShowScene(string sceneName)
    {
        try
        {
            AsyncOperation asyncOperation = loadedScenes[sceneName];
            asyncOperation.allowSceneActivation = true;
            isDoneLoading = true;
        }
        catch (KeyNotFoundException e)
        {
            Debug.Log(sceneName + " scene not found");
        }
    }

    void LoadTipsFromTextFile()
    {
        string fileName = "tips";
        TextAsset textAsset = Resources.Load<TextAsset>(fileName);

        if (textAsset != null)
        {
            string[] fileLines = textAsset.text.Split('\n');

            foreach (string line in fileLines)
            {
                tipsList.Add(line.Trim());
            }

            Debug.Log("Loaded Content:");
            foreach (string line in tipsList)
            {
                Debug.Log(line);
            }
        }
        else
        {
            Debug.LogError("File not found in Resources: " + fileName);
        }
    }

    IEnumerator FillProgressBarTimed()
    {
        float progress;

        if (image == null)
        {
            Debug.LogError("Image to animate is not assigned.");
            yield break;
        }

        float targetFillAmount = 1f;

        float duration = loadTime;
        float elapsedTime = 0f;

        float startFillAmount = 0.0f;

        while (elapsedTime < duration)
        {
            progress = Mathf.Lerp(startFillAmount, targetFillAmount, elapsedTime / duration);
            SetProgressValue(progress);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        image.fillAmount = targetFillAmount;
        //ShowScene(mainMenuSceneName);
        SceneManager.LoadScene("Main Menu");
    }

    void SetProgressValue(float progress)
    {
        if (image == null)
        {
            Debug.LogError("Image to animate is not assigned.");
        }
        image.fillAmount = progress;
    }

    IEnumerator RotateThroughTips()
    {
        while (!isDoneLoading)
        {
            if (tipsList.Count > 0)
            {
                int randomIndex = Random.Range(0, tipsList.Count);
                tipsText.text = tipsList[randomIndex];
            }
            yield return new WaitForSeconds(3.0f);
        }
    }


}
