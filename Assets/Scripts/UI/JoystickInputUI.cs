using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO.Ports;

/// <summary>
/// Attached to the settings panel in the Main Menu.
/// Handles manual input of COM port and dropdown for baud rate.
/// </summary>
public class JoystickInputUI : MonoBehaviour
{
    [Header("UI References")]
    [Tooltip("Input field for the user to manually enter COM port (e.g. COM3, COM5). Leave empty or 'None' for no serial.")]
    public TMP_InputField comPortTxt;

    [Tooltip("Dropdown for baud rate selection.")]
    public TMP_Dropdown baudRateDropdown;

    [Tooltip("(Optional) Status label showing connection state.")]
    public TextMeshProUGUI statusLabel;

    // ─── Common baud rates offered to the user ────────────────────────────
    private static readonly int[] BaudRates = { 9600, 19200, 38400, 57600, 115200 };

    // ─── Public read-back properties (used by MainMenuUI) ─────────────────
    public string SelectedPort
    {
        get
        {
            if (comPortTxt == null) return "None";
            string val = comPortTxt.text.Trim();
            if (string.IsNullOrEmpty(val)) val = "None";
            return val;
        }
    }

    public int SelectedBaudRate
    {
        get
        {
            if (baudRateDropdown == null) return 9600;
            int idx = baudRateDropdown.value;
            return (idx >= 0 && idx < BaudRates.Length) ? BaudRates[idx] : 9600;
        }
    }

    // ─────────────────────────────────────────────────────────────────────
    void Start()
    {
        PopulateBaudRateDropdown();
        RestoreSavedSelections();
        
        if (comPortTxt != null)
        {
            comPortTxt.onValueChanged.AddListener(_ => UpdateStatusLabel());
        }
        if (baudRateDropdown != null)
        {
            baudRateDropdown.onValueChanged.AddListener(_ => UpdateStatusLabel());
        }

        UpdateStatusLabel();
    }

    private void PopulateBaudRateDropdown()
    {
        if (baudRateDropdown == null) return;

        baudRateDropdown.ClearOptions();

        var options = new System.Collections.Generic.List<string>();
        foreach (int rate in BaudRates)
            options.Add(rate.ToString());

        baudRateDropdown.AddOptions(options);
    }

    // ─── Persist / restore selections ─────────────────────────────────────

    private void RestoreSavedSelections()
    {
        // Restore COM port
        string savedPort = PlayerPrefs.GetString("SerialPort", "");
        if (comPortTxt != null && savedPort != "None")
        {
            comPortTxt.text = savedPort;
        }

        // Restore baud rate
        int savedBaud = PlayerPrefs.GetInt("BaudRate", 9600);
        if (baudRateDropdown != null)
        {
            for (int i = 0; i < BaudRates.Length; i++)
            {
                if (BaudRates[i] == savedBaud)
                {
                    baudRateDropdown.value = i;
                    break;
                }
            }
        }
    }

    private void UpdateStatusLabel()
    {
        if (statusLabel == null) return;

        if (SelectedPort == "None" || string.IsNullOrEmpty(SelectedPort))
        {
            statusLabel.text = "Serial: Disabled (keyboard only)";
            statusLabel.color = Color.gray;
        }
        else
        {
            statusLabel.text = $"Serial: {SelectedPort} @ {SelectedBaudRate} baud";
            statusLabel.color = Color.cyan;
        }
    }
}
