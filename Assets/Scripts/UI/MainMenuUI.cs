using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour
{
    [Header("Panels Flow")]
    [Tooltip("Panel where user enters their name.")]
    public GameObject loginPanel;
    [Tooltip("Panel where user enters COM port and Baud rate.")]
    public GameObject settingsPanel;

    [Header("Login References")]
    public TMP_InputField nameInputField;
    public Button nextButton; // Formerly "startButton", goes to settings
    public TextMeshProUGUI warningText;

    [Header("Settings References")]
    public Button realStartGameButton; // Starts the actual GamePlay scene

    [Header("Settings")]
    public string gameSceneName = "GamePlay";

    [Header("Joystick / Serial Settings")]
    [Tooltip("Assign the JoystickInputUI component from the COM-port panel.")]
    public JoystickInputUI joystickInputUI;

    void Start()
    {
        // 1. Initial State: Determine which panel to show first
        if (SerialInputManager.Instance != null && SerialInputManager.Instance.IsConnected)
        {
            Debug.Log("Connected");
            // Already connected -> Skip settings, show Login first
            if (settingsPanel != null) settingsPanel.SetActive(false);
            if (loginPanel != null) loginPanel.SetActive(true);
        }
        else
        {
            Debug.Log("NOt Connected");
            // Not connected -> Show Settings first, hide Login
            if (loginPanel != null) loginPanel.SetActive(false);
            if (settingsPanel != null) settingsPanel.SetActive(true);
        }

        if (warningText != null)
            warningText.gameObject.SetActive(false);
            
        // 2. Add listeners to buttons instead of doing it from Inspector
        if (nextButton != null) // Now this is on the Login panel to start the game
        {
            nextButton.onClick.RemoveAllListeners();
            nextButton.onClick.AddListener(OnNextClicked);
        }

        if (realStartGameButton != null) // Now this is on the Settings panel to go to Login
        {
            realStartGameButton.onClick.RemoveAllListeners();
            realStartGameButton.onClick.AddListener(OnStartGameClicked);
        }
    }

    // Called when user clicks "Play" on the Login panel
    public void OnNextClicked()
    {
        string playerName = nameInputField != null ? nameInputField.text : "";
        
        // Validate name on click
        if (string.IsNullOrWhiteSpace(playerName))
        {
            if (warningText != null)
            {
                warningText.text = "Please enter the name..";
                warningText.gameObject.SetActive(true);
            }
                
            Debug.Log("[MainMenuUI] Name is empty. Showing warning.");
            return;
        }

        // Hide warning if successful
        if (warningText != null) warningText.gameObject.SetActive(false);
        // Save player name
        PlayerPrefs.SetString("PlayerName", playerName);
        if (GameManager.Instance != null)
        {
            GameManager.Instance.playerName = playerName;
        }

        Debug.Log($"[MainMenuUI] Name validated: {playerName}. Loading Gameplay Scene.");

        SceneManager.LoadScene(gameSceneName);
    }

    // Called when user clicks "Next" on the Settings panel
    public void OnStartGameClicked()
    {
        // Save serial / joystick settings so SerialInputManager can read them
        if (joystickInputUI != null)
        {
            string selectedPort = joystickInputUI.SelectedPort;
            int    selectedBaud = joystickInputUI.SelectedBaudRate;

            PlayerPrefs.SetString("SerialPort", selectedPort);
            PlayerPrefs.SetInt("BaudRate",      selectedBaud);

            Debug.Log($"[MainMenuUI] Serial settings saved — Port: {selectedPort}, Baud: {selectedBaud}");
        }
        else
        {
            // No joystick UI assigned; keep whatever is in PlayerPrefs already
            Debug.Log("[MainMenuUI] JoystickInputUI not assigned. Serial settings unchanged.");
        }

        PlayerPrefs.Save();

        Debug.Log($"[MainMenuUI] Opening Login screen...");
        
        // Transition: Hide Settings Panel, Show Login Panel
        if (settingsPanel != null) settingsPanel.SetActive(false);
        if (loginPanel != null) loginPanel.SetActive(true);
    }
}
