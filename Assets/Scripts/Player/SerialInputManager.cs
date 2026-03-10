using UnityEngine;
using System.IO.Ports;
using System.Threading;
using System;

/// <summary>
/// Persistent singleton that reads microcontroller serial input (U / D / R / L bytes)
/// and exposes the latest direction for PlayerController to consume.
///
/// Configure via PlayerPrefs keys "SerialPort" and "BaudRate" (written by MainMenuUI
/// before the gameplay scene loads). If "SerialPort" is empty or "None", serial stays
/// disconnected and keyboard-only play is used.
/// </summary>
public class SerialInputManager : MonoBehaviour
{
    // ─── Singleton ───────────────────────────────────────────────────────────
    public static SerialInputManager Instance { get; private set; }

    // ─── Serial Settings (read from PlayerPrefs at Start) ────────────────────
    private string portName  = "";
    private int    baudRate  = 9600;

    // ─── Internal State ──────────────────────────────────────────────────────
    private SerialPort serialPort;
    private Thread     readThread;
    private volatile bool isRunning = false;

    // Thread-safe latest direction received (guarded by lock)
    private Vector3 pendingDirection = Vector3.zero;
    private readonly object dirLock  = new object();

    // ─── Status (read-only for debugging / HUD) ──────────────────────────────
    public bool IsConnected => serialPort != null && serialPort.IsOpen;

    // ─────────────────────────────────────────────────────────────────────────
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        portName = PlayerPrefs.GetString("SerialPort", "None");
        baudRate = PlayerPrefs.GetInt("BaudRate", 9600);

        if (string.IsNullOrEmpty(portName) || portName == "None")
        {
            Debug.Log("[SerialInputManager] No COM port selected. Serial input disabled.");
            return;
        }

        OpenPort();
    }

    void Start()
    {
        // Initialization moved to Awake to avoid race conditions with MainMenuUI checking IsConnected in its Start()
    }

    // ─── Public API ──────────────────────────────────────────────────────────

    /// <summary>
    /// Returns the latest direction received from the microcontroller and
    /// resets it to zero so each direction is only consumed once.
    /// </summary>
    public Vector3 ConsumePendingDirection()
    {
        lock (dirLock)
        {
            Vector3 dir = pendingDirection;
            pendingDirection = Vector3.zero;
            return dir;
        }
    }

    /// <summary>
    /// Re-open the serial port with new settings (called from UI at runtime).
    /// </summary>
    public void Connect(string port, int baud)
    {
        ClosePort();
        portName = port;
        baudRate = baud;

        if (!string.IsNullOrEmpty(portName) && portName != "None")
            OpenPort();
    }

    public void Disconnect()
    {
        ClosePort();
    }

    // ─── Port Lifecycle ──────────────────────────────────────────────────────

    private void OpenPort()
    {
        try
        {
            Debug.Log($"Port Name:{portName},Baud Rate: {baudRate} ");
            serialPort = new SerialPort(portName, baudRate)
            {
                ReadTimeout  = 100,
                WriteTimeout = 100,
                DtrEnable    = true
            };
            serialPort.Open();
            isRunning  = true;
            readThread = new Thread(ReadLoop) { IsBackground = true };
            readThread.Start();
            Debug.Log($"[SerialInputManager] Connected to {portName} @ {baudRate} baud.");
        }
        catch (System.Exception ex)
        {
            Debug.LogWarning($"[SerialInputManager] Could not open {portName}: {ex.Message}");
            serialPort = null;
        }
    }

    private void ClosePort()
    {
        isRunning = false;

        if (readThread != null && readThread.IsAlive)
        {
            readThread.Join(500);
            readThread = null;
        }

        if (serialPort != null)
        {
            if (serialPort.IsOpen)
                serialPort.Close();
            serialPort.Dispose();
            serialPort = null;
        }
    }

    // ─── Background Read Loop ────────────────────────────────────────────────

    private void ReadLoop()
    {
        while (isRunning && serialPort != null && serialPort.IsOpen)
        {
            try
            {
                int byteRead = serialPort.ReadByte();
                if (byteRead < 0) continue;

                char ch = (char)byteRead;
                Vector3 dir = CharToDirection(ch);

                if (dir != Vector3.zero)
                {
                    lock (dirLock)
                    {
                        pendingDirection = dir;
                    }
                }
            }
            catch (TimeoutException)
            {
                // Normal — no data yet, loop again
            }
            catch (System.Exception ex)
            {
                if (isRunning)
                    Debug.LogWarning($"[SerialInputManager] Read error: {ex.Message}");
                break;
            }
        }
    }

    // ─── Input Mapping ───────────────────────────────────────────────────────

    private static Vector3 CharToDirection(char ch)
    {
        switch (ch)
        {
            case 'U': case 'u': return Vector3.forward;
            case 'D': case 'd': return Vector3.back;
            case 'R': case 'r': return Vector3.right;
            case 'L': case 'l': return Vector3.left;
            default:            return Vector3.zero;
        }
    }

    // ─── Cleanup ─────────────────────────────────────────────────────────────

    void OnDestroy()
    {
        ClosePort();
    }

    void OnApplicationQuit()
    {
        ClosePort();
    }
}
