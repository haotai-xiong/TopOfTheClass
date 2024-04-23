using UnityEngine;

public class ScreenshotTaker : MonoBehaviour
{
    private static ScreenshotTaker _instance;
    private int index = 1;
    public static ScreenshotTaker Instance
    {
        
        get
        {
            if (_instance == null)
            {
                // Create a new GameObject for the ScreenshotTaker instance.
                GameObject screenshotTakerGameObject = new GameObject("ScreenshotTaker");
                _instance = screenshotTakerGameObject.AddComponent<ScreenshotTaker>();
                
                // Ensure it persists and initializes properly.
                DontDestroyOnLoad(screenshotTakerGameObject);
            }
            return _instance;
        }
    }

    void Awake()
    {
        if (_instance != null && _instance != this)
        {
            // If an instance already exists and it's not this one, destroy this one.
            Destroy(this.gameObject);
            return;
        }
        _instance = this;
        DontDestroyOnLoad(this.gameObject); // Optional: Makes this persist across scene loads.
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            
            string filename = index + ".png";
            index++;
            ScreenCapture.CaptureScreenshot(filename);
            Debug.Log("Screenshot saved to: " + filename);
        }
    }
}