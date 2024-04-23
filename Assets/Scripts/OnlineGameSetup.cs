using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Netcode;
using UnityEngine.UI;
using Unity.Collections;

public class OnlineGameSetup : NetworkBehaviour
{
    [Header("Information")]
    [SerializeField] private Relay relay;
    public OnlineGameSetup instance;
    public GameObject[] playersFound;
    public Color[] playerColours = new Color[4] { Color.magenta, Color.cyan, Color.red, Color.green };
    public int numberOfPlayers = 4;

    [Header("UI")]
    [SerializeField] private GameObject panel;
    [SerializeField] private GameObject hostButton;
    [SerializeField] private GameObject joinButton;
    [SerializeField] private GameObject playButton;
    [SerializeField] TextMeshProUGUI joinCodeText;
    [SerializeField] TextMeshProUGUI enterCodeText;
    public TMP_InputField inputCode;

    [Header("Pink Player")]
    public NetworkVariable<bool> pinkOver12 = new NetworkVariable<bool>(true);
    private NetworkVariable<FixedString64Bytes> pinkReadyString = new NetworkVariable<FixedString64Bytes>("Not Ready");
    [SerializeField] TextMeshProUGUI pinkReadyText;
    [SerializeField] TextMeshProUGUI pinkOverText;
    [SerializeField] TextMeshProUGUI pinkUnderText;
    [SerializeField] GameObject pinkReady;
    [SerializeField] GameObject pinkAgeButton;
    [SerializeField] GameObject pinkOverImage;
    [SerializeField] GameObject pinkUnderImage;
    [SerializeField] GameObject pinkplayerCircle;

    [Header("Blue Player")]
    public NetworkVariable<bool> blueOver12 = new NetworkVariable<bool>(true);
    private NetworkVariable<FixedString64Bytes> blueReadyString = new NetworkVariable<FixedString64Bytes>("Not Ready");
    [SerializeField] TextMeshProUGUI blueReadyText;
    [SerializeField] TextMeshProUGUI blueOverText;
    [SerializeField] TextMeshProUGUI blueUnderText;
    [SerializeField] GameObject blueReady;
    [SerializeField] GameObject blueAgeButton;
    [SerializeField] GameObject blueOverImage;
    [SerializeField] GameObject blueUnderImage;
    [SerializeField] GameObject blueplayerCircle;

    [Header("Red Player")]
    public NetworkVariable<bool> redOver12 = new NetworkVariable<bool>(true);
    private NetworkVariable<FixedString64Bytes> redReadyString = new NetworkVariable<FixedString64Bytes>("Not Ready");
    [SerializeField] TextMeshProUGUI redReadyText;
    [SerializeField] TextMeshProUGUI redOverText;
    [SerializeField] TextMeshProUGUI redUnderText;
    [SerializeField] GameObject redReady;
    [SerializeField] GameObject redAgeButton;
    [SerializeField] GameObject redOverImage;
    [SerializeField] GameObject redUnderImage;
    [SerializeField] GameObject redplayerCircle;

    [Header("Green Player")]
    public NetworkVariable<bool> greenOver12 = new NetworkVariable<bool>(true);
    private NetworkVariable<FixedString64Bytes> greenReadyString = new NetworkVariable<FixedString64Bytes>("Not Ready");
    [SerializeField] TextMeshProUGUI greenReadyText;
    [SerializeField] TextMeshProUGUI greenOverText;
    [SerializeField] TextMeshProUGUI greenUnderText;
    [SerializeField] GameObject greenReady;
    [SerializeField] GameObject greenAgeButton;
    [SerializeField] GameObject greenOverImage;
    [SerializeField] GameObject greenUnderImage;
    [SerializeField] GameObject greenplayerCircle;

    // Start is called before the first frame update
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

        panel.SetActive(true);
        joinButton.SetActive(true);
        hostButton.SetActive(true);
        playButton.SetActive(false);

        blueplayerCircle.SetActive(false);
        pinkplayerCircle.SetActive(false);
        redplayerCircle.SetActive(false);
        greenplayerCircle.SetActive(false);

        joinCodeText.color = Color.yellow;
    }

    // Update is called once per frame
    void Update()
    {
        playersFound = GameObject.FindGameObjectsWithTag("PlayerPiece");
        numberOfPlayers = playersFound.Length;

        if (SceneManager.GetActiveScene().name == "Lobby")
        {
            for (int i = 0; i < playersFound.Length; i++)
            {
                playersFound[i].GetComponent<OnlinePlayerController>().playerId = i;
                playersFound[i].GetComponent<SpriteRenderer>().color = playerColours[i];
                if (playersFound[i].GetComponent<OnlinePlayerController>().playerId == 0 && IsOwner)
                {
                    playButton.SetActive(true);
                }
                switch (i)
                {
                    case 0:
                        playersFound[0].GetComponent<OnlinePlayerController>().playerOver12 = FindObjectOfType<OnlineGameSetup>().pinkOver12.Value;
                        break;
                    case 1:
                        playersFound[1].GetComponent<OnlinePlayerController>().playerOver12 = FindObjectOfType<OnlineGameSetup>().blueOver12.Value;
                        break;
                    case 2:
                        playersFound[2].GetComponent<OnlinePlayerController>().playerOver12 = FindObjectOfType<OnlineGameSetup>().redOver12.Value;
                        break;
                    case 3:
                        playersFound[3].GetComponent<OnlinePlayerController>().playerOver12 = FindObjectOfType<OnlineGameSetup>().greenOver12.Value;
                        break;
                    default:
                        break;
                }

                ActivateButtons();
                SetOnScreenPosition(i);
                EnableButtons(i);
            }

            //Display Age in Lobby UI
            CheckPinkAge();
            CheckBlueAge();
            CheckRedAge();
            CheckGreenAge();
            CheckPlayersReadyStatus();

            joinCodeText.text ="Join Code: " + relay.joinCode;
        }
    }

    private void CheckPlayersReadyStatus()
    {
        pinkReadyText.text = pinkReadyString.Value.ToString();
        blueReadyText.text = blueReadyString.Value.ToString();
        redReadyText.text = redReadyString.Value.ToString();
        greenReadyText.text = greenReadyString.Value.ToString();
    }

    private void CheckPinkAge()
    {
        if (pinkOver12.Value)
        {
            pinkOverText.gameObject.SetActive(true);
            pinkOverImage.GetComponent<Image>().color = Color.magenta;

            pinkUnderText.gameObject.SetActive(false);
            pinkUnderImage.GetComponent<Image>().color = new Color(0.132f, 0.132f, 0.132f, 1);
        }
        else
        {
            pinkOverText.gameObject.SetActive(false);
            pinkOverImage.GetComponent<Image>().color = new Color(0.132f, 0.132f, 0.132f, 1);

            pinkUnderText.gameObject.SetActive(true);
            pinkUnderImage.GetComponent<Image>().color = Color.magenta;
        }
    }

    private void CheckBlueAge()
    {
        if (blueOver12.Value)
        {
            blueOverText.gameObject.SetActive(true);
            blueOverImage.GetComponent<Image>().color = Color.cyan;

            blueUnderText.gameObject.SetActive(false);
            blueUnderImage.GetComponent<Image>().color = new Color(0.132f, 0.132f, 0.132f, 1);
        }
        else
        {
            blueOverText.gameObject.SetActive(false);
            blueOverImage.GetComponent<Image>().color = new Color(0.132f, 0.132f, 0.132f, 1);

            blueUnderText.gameObject.SetActive(true);
            blueUnderImage.GetComponent<Image>().color = Color.cyan;
        }
    }

    private void CheckRedAge()
    {
        if (redOver12.Value)
        {
            redOverText.gameObject.SetActive(true);
            redOverImage.GetComponent<Image>().color = Color.red;

            redUnderText.gameObject.SetActive(false);
            redUnderImage.GetComponent<Image>().color = new Color(0.132f, 0.132f, 0.132f, 1);
        }
        else
        {
            redOverText.gameObject.SetActive(false);
            redOverImage.GetComponent<Image>().color = new Color(0.132f, 0.132f, 0.132f, 1);

            redUnderText.gameObject.SetActive(true);
            redUnderImage.GetComponent<Image>().color = Color.red;
        }
    }

    private void CheckGreenAge()
    {
        if (greenOver12.Value)
        {
            greenOverText.gameObject.SetActive(true);
            greenOverImage.GetComponent<Image>().color = Color.green;

            greenUnderText.gameObject.SetActive(false);
            greenUnderImage.GetComponent<Image>().color = new Color(0.132f, 0.132f, 0.132f, 1);
        }
        else
        {
            greenOverText.gameObject.SetActive(false);
            greenOverImage.GetComponent<Image>().color = new Color(0.132f, 0.132f, 0.132f, 1);

            greenUnderText.gameObject.SetActive(true);
            greenUnderImage.GetComponent<Image>().color = Color.green;
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void TogglePinkServerRpc()
    {
        pinkOver12.Value ^= true; 
    }

    [ServerRpc(RequireOwnership = false)]
    public void ToggleBlueServerRpc()
    {
        blueOver12.Value ^= true;
    }

    [ServerRpc(RequireOwnership = false)]
    public void ToggleRedServerRpc()
    {
        redOver12.Value ^= true;
    }

    [ServerRpc(RequireOwnership = false)]
    public void ToggleGreenServerRpc()
    {
        greenOver12.Value ^= true;
    }

    [ServerRpc(RequireOwnership = false)]
    public void ReadyPinkServerRpc()
    {
        if (pinkReadyText.text == "Ready")
        {
            pinkReadyString.Value = "Not Ready";
        }
        else if (pinkReadyText.text == "Not Ready")
        {
            pinkReadyString.Value = "Ready";
        }
        Debug.Log("PinkReadyChange");
    }

    [ServerRpc(RequireOwnership = false)]
    public void ReadyBlueServerRpc()
    {
        if (blueReadyText.text == "Ready")
        {
            blueReadyString.Value = "Not Ready";
        }
        else if (blueReadyText.text == "Not Ready")
        {
            blueReadyString.Value = "Ready";
        }
        Debug.Log("BlueReadyChange");
    }

    [ServerRpc(RequireOwnership = false)]
    public void ReadyRedServerRpc()
    {
        if (redReadyText.text == "Ready")
        {
            redReadyString.Value = "Not Ready";
        }
        else if (redReadyText.text == "Not Ready")
        {
            redReadyString.Value = "Ready";
        }
        Debug.Log("RedReadyChange");
    }

    [ServerRpc(RequireOwnership = false)]
    public void ReadyGreenServerRpc()
    {
        if (greenReadyText.text == "Ready")
        {
            greenReadyString.Value = "Not Ready";
        }
        else if (greenReadyText.text == "Not Ready")
        {
            greenReadyString.Value = "Ready";
        }
        Debug.Log("GreenReadyChange");
    }

    public void Host()
    {
        relay.CreateRelay();
        panel.SetActive(false);
        joinButton.SetActive(false);
        hostButton.SetActive(false);
        enterCodeText.gameObject.SetActive(false);
        inputCode.gameObject.SetActive(false);
    }

    public void Join()
    {
        relay.ProcessJoinCode();
        panel.SetActive(false);
        joinButton.SetActive(false);
        hostButton.SetActive(false);
        enterCodeText.gameObject.SetActive(false);
        inputCode.gameObject.SetActive(false);
    }

    public void PlayOnlineMode()
    {
        NetworkManager.Singleton.SceneManager.LoadScene("Online",LoadSceneMode.Single);
    }

    private void SetOnScreenPosition(int player)
    {
        if (playersFound[player].GetComponent<OnlinePlayerController>().playerId == 0)
        {
            playersFound[player].transform.position = new Vector3(-8, 3, 0);
        }
        else if (playersFound[player].GetComponent<OnlinePlayerController>().playerId == 1)
        {
            playersFound[player].transform.position = new Vector3(-2.5f, 3, 0);
        }
        else if (playersFound[player].GetComponent<OnlinePlayerController>().playerId == 2)
        {
            playersFound[player].transform.position = new Vector3(2.5f, 3, 0);
        }
        else if (playersFound[player].GetComponent<OnlinePlayerController>().playerId == 3)
        {
            playersFound[player].transform.position = new Vector3(8, 3, 0);
        }
    }

    private void ActivateButtons()
    {
        if (numberOfPlayers == 1)
        {
            blueReady.SetActive(false);
            redReady.SetActive(false);
            greenReady.SetActive(false);

            blueAgeButton.SetActive(false);
            redAgeButton.SetActive(false);
            greenAgeButton.SetActive(false);
        }
        else if (numberOfPlayers == 2)
        {
            blueReady.SetActive(true);
            redReady.SetActive(false);
            greenReady.SetActive(false);

            blueAgeButton.SetActive(true);
            redAgeButton.SetActive(false);
            greenAgeButton.SetActive(false);
        }
        else if (numberOfPlayers == 3)
        {
            blueReady.SetActive(true);
            redReady.SetActive(true);
            greenReady.SetActive(false);

            blueAgeButton.SetActive(true);
            redAgeButton.SetActive(true);
            greenAgeButton.SetActive(false);
        }
        else if (numberOfPlayers == 4)
        {
            blueReady.SetActive(true);
            redReady.SetActive(true);
            greenReady.SetActive(true);

            blueAgeButton.SetActive(true);
            redAgeButton.SetActive(true);
            greenAgeButton.SetActive(true);
        }
    }

    private void EnableButtons(int player)
    {
        if (playersFound[player].GetComponent<NetworkObject>().IsOwner)
        {
            if (playersFound[player].GetComponent<OnlinePlayerController>().playerId == 0)
            {
                pinkAgeButton.GetComponent<Button>().enabled = true;
                blueAgeButton.GetComponent<Button>().enabled = false;
                redAgeButton.GetComponent<Button>().enabled = false;
                greenAgeButton.GetComponent<Button>().enabled = false;

                pinkReady.GetComponent<Button>().enabled = true;
                blueReady.GetComponent<Button>().enabled = false;
                redReady.GetComponent<Button>().enabled = false;
                greenReady.GetComponent<Button>().enabled = false;

                pinkplayerCircle.SetActive(true);
            }
            else if (playersFound[player].GetComponent<OnlinePlayerController>().playerId == 1)
            {
                pinkAgeButton.GetComponent<Button>().enabled = false;
                blueAgeButton.GetComponent<Button>().enabled = true;
                redAgeButton.GetComponent<Button>().enabled = false;
                greenAgeButton.GetComponent<Button>().enabled = false;

                pinkReady.GetComponent<Button>().enabled = false;
                blueReady.GetComponent<Button>().enabled = true;
                redReady.GetComponent<Button>().enabled = false;
                greenReady.GetComponent<Button>().enabled = false;

                blueplayerCircle.SetActive(true);
            }
            else if (playersFound[player].GetComponent<OnlinePlayerController>().playerId == 2)
            {
                pinkAgeButton.GetComponent<Button>().enabled = false;
                blueAgeButton.GetComponent<Button>().enabled = false;
                redAgeButton.GetComponent<Button>().enabled = true;
                greenAgeButton.GetComponent<Button>().enabled = false;

                pinkReady.GetComponent<Button>().enabled = false;
                blueReady.GetComponent<Button>().enabled = false;
                redReady.GetComponent<Button>().enabled = true;
                greenReady.GetComponent<Button>().enabled = false;

                redplayerCircle.SetActive(true);
            }
            else if (playersFound[player].GetComponent<OnlinePlayerController>().playerId == 3)
            {
                pinkAgeButton.GetComponent<Button>().enabled = false;
                blueAgeButton.GetComponent<Button>().enabled = false;
                redAgeButton.GetComponent<Button>().enabled = false;
                greenAgeButton.GetComponent<Button>().enabled = true;

                pinkReady.GetComponent<Button>().enabled = false;
                blueReady.GetComponent<Button>().enabled = false;
                redReady.GetComponent<Button>().enabled = false;
                greenReady.GetComponent<Button>().enabled = true;

                greenplayerCircle.SetActive(true);
            }
        }
    }
}
