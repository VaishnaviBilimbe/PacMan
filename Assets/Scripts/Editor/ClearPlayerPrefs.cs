using System.IO;
using UnityEditor;
using UnityEditor.EditorTools;
using UnityEngine;

public class ClearPlayerPrefs : MonoBehaviour
{
    [MenuItem("Tools/Clear Player Prefs")]
    private static void ClearPlayerPref()
    {
        // Clear PlayerPrefs
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();

        // Path to leaderboard file
        string filePath = Path.Combine(Application.persistentDataPath, "leaderboard.json");

        // Delete JSON file if it exists
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
            Debug.Log("Leaderboard JSON file deleted.");
        }
        else
        {
            Debug.Log("Leaderboard JSON file not found.");
        }

        Debug.Log("Player Prefs cleared.");
    }
}
