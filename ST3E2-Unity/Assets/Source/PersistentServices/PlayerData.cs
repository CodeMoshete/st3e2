using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class PlayerData
{
    private static readonly string PLAYER_DATA_DIR = Path.Combine(Application.persistentDataPath, "player");
    private const string PLAYER_DATA_FILE = "playerdata.txt";
    private readonly string PLAYER_DATA_PATH = Path.Combine(PLAYER_DATA_DIR, PLAYER_DATA_FILE);

    private Dictionary<string, int> PlayerStats;

    private static PlayerData instance;
    public static PlayerData Instance {
        get
        {
            if (instance == null)
            {
                instance = new PlayerData();
            }
            return instance;
        }
    }

    public PlayerData()
    {
        PlayerStats = new Dictionary<string, int>();
        LoadStats();
    }

    public void SetStat(string statName, int value)
    {
        if (!PlayerStats.ContainsKey(statName))
        {
            PlayerStats.Add(statName, 0);
        }

        PlayerStats[statName] = value;
        SaveStats();
    }

    public void UpdateStat(string statName, int increment)
    {
        if (!PlayerStats.ContainsKey(statName))
        {
            PlayerStats.Add(statName, 0);
        }

        PlayerStats[statName] += increment;
        SaveStats();
    }

    public int GetStat(string statName)
    {
        int val = 0;
        if (PlayerStats.ContainsKey(statName))
        {
            val = PlayerStats[statName];
        }
        return val;
    }

    private void SaveStats()
    {
        if (!Directory.Exists(PLAYER_DATA_DIR))
        {
            Directory.CreateDirectory(PLAYER_DATA_DIR);
        }

        if (!File.Exists(PLAYER_DATA_FILE))
        {
            FileStream createStream = File.Create(PLAYER_DATA_PATH);
            createStream.Close();
        }

        StreamWriter stream = new StreamWriter(PLAYER_DATA_PATH, false);
        foreach(KeyValuePair<string, int> pair in PlayerStats)
        {
            string line = pair.Key + ":" + pair.Value;
            stream.WriteLine(line);
        }
        stream.Close();
    }

    private void LoadStats()
    {
        if (Directory.Exists(PLAYER_DATA_DIR) && File.Exists(PLAYER_DATA_FILE))
        {
            PlayerStats.Clear();
            StreamReader reader = new StreamReader(PLAYER_DATA_PATH);
            string line = reader.ReadLine();
            while (line != null)
            {
                string[] splitStr = line.Split(':');
                PlayerStats.Add(splitStr[0], int.Parse(splitStr[1]));
                line = reader.ReadLine();
            }
            reader.Close();
        }
    }
}
