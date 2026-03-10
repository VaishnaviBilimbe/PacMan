using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LeaderboardManager : MonoBehaviour
{
    public static LeaderboardManager Instance { get; private set; }

    private string filePath;
    private LeaderboardData leaderboardData = new LeaderboardData();

    public int maxEntries = 10;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        filePath = Path.Combine(Application.persistentDataPath, "leaderboard.json");
        LoadLeaderboard();
    }

    public void AddScore(string playerName, int score)
    {
        if (string.IsNullOrEmpty(playerName)) playerName = "Anonymous";

        // Check if player already exists
        LeaderboardEntry existingEntry = leaderboardData.entries.FirstOrDefault(e => e.playerName == playerName);
        
        if (existingEntry != null)
        {
            // Update score if new score is higher
            if (score > existingEntry.score)
            {
                existingEntry.score = score;
            }
        }
        else
        {
            leaderboardData.entries.Add(new LeaderboardEntry(playerName, score));
        }

        // Sort descending and keep top X
        leaderboardData.entries = leaderboardData.entries
            .OrderByDescending(e => e.score)
            .Take(maxEntries)
            .ToList();

        SaveLeaderboard();
    }

    public List<LeaderboardEntry> GetEntries()
    {
        return leaderboardData.entries;
    }

    private void SaveLeaderboard()
    {
        string json = JsonUtility.ToJson(leaderboardData, true);
        File.WriteAllText(filePath, json);
        Debug.Log($"[LeaderboardManager] Saved to {filePath}");
    }

    private void LoadLeaderboard()
    {
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            leaderboardData = JsonUtility.FromJson<LeaderboardData>(json);

            // Deduplicate existing entries from before the fix
            var uniqueEntries = new Dictionary<string, LeaderboardEntry>();
            foreach (var entry in leaderboardData.entries)
            {
                if (uniqueEntries.TryGetValue(entry.playerName, out var existing))
                {
                    if (entry.score > existing.score)
                    {
                        existing.score = entry.score;
                    }
                }
                else
                {
                    uniqueEntries[entry.playerName] = entry;
                }
            }
            leaderboardData.entries = uniqueEntries.Values.ToList();

            Debug.Log("[LeaderboardManager] Loaded and deduplicated data.");
        }
        else
        {
            leaderboardData = new LeaderboardData();
            Debug.Log("[LeaderboardManager] No file found, starting fresh.");
        }
    }

    public void LoadLoginScene()
    {
        SceneManager.LoadScene("Login");
    }
}
