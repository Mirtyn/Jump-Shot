using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dan.Main;
using TMPro;
using UnityEngine.SceneManagement;

public class Leaderboard : ProjectBehaviour
{
    private string leaderboardKey = "6951926a77e92f51c0cc0b3504b3dd6830730048ad0c715edb4fc9710e4edfb2";

    [SerializeField] private TMP_InputField inputField;

    public static Leaderboard Instance { get; private set; }

    [SerializeField] private List<GameObject> entries;

    private void Awake()
    {
        Instance = this;
    }

    public void GetLeaderboard()
    {
        LeaderboardCreator.GetLeaderboard(leaderboardKey, ((msg) =>
        {
            int loopLength = (msg.Length < entries.Count) ? msg.Length : entries.Count;
            for (int i = 0; i < loopLength; i++)
            {
                GameObject entry = entries[i];
                entry.transform.GetChild(0).gameObject.GetComponent<TMP_Text>().text = msg[i].Username;
                entry.transform.GetChild(1).gameObject.GetComponent<TMP_Text>().text = ((float)msg[i].Score / 1000f).ToString();
            }
        }));
    }

    public void UploadEntry()
    {
        string userName = inputField.text;
        int score = (int)(GameManager.StartTime * 1000);

        LeaderboardCreator.UploadNewEntry(leaderboardKey, userName, score, ((msg) =>
        {
            GetLeaderboard();
        }));
    }

    public void ReStart()
    {
        ReLoae();
    }
}
