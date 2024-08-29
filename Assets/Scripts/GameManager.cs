using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class GameManager : ProjectBehaviour
{
    public GameObject BulletPrefab;
    public GameObject EnemyBulletPrefab;
    public GameObject BarPrefab;
    public GameObject BarNodePrefab;
    public GameObject CompleteBarPrefab;
    public GameObject CompleteBarNodePrefab;
    public GameObject PickupPrefab;
    public Color BackgroundColor1;
    public Color BackgroundColor2;
    public Color ForegroundColor;
    public TMP_Text TimeText;
    public float StartTime;
    public List<SpawnPoint> SpawnPoints = new List<SpawnPoint>();
    public List<Wall> Walls = new List<Wall>();
    public GameObject ShooterPrefab;
    public GameObject ChargerPrefab;
    public GameObject JumperPrefab;
    public GameObject TankonPrefab;
    bool first = true;
    public float EnemyInterval = 8f;
    public float nextEnemySpawn;
    public float BossInterval = 70f;
    public float nextBossSpawn;
    public List<AudioClip> HitClips;
    [SerializeField] private GameObject leaderBoardPanel;
    public bool StopTime = false;

    private void Awake()
    {
        GameManager = this;
        StartTime = 0;
        nextEnemySpawn = StartTime;
        nextBossSpawn = StartTime + 70f;
    }

    public void ShowLeaderboard()
    {
        StopTime = true;
        leaderBoardPanel.SetActive(true);
        Leaderboard.Instance.GetLeaderboard();
    }

    private void CalcEnemyInterval()
    {
        EnemyInterval = (Mathf.Pow(4f, 3f) / (StartTime * 1.3f) + 20f);
        nextEnemySpawn += EnemyInterval;
        Debug.LogWarning("E: " + nextEnemySpawn);

        if (first)
        {
            first = false;
            nextEnemySpawn = 30f;
        }
    }

    private void CalcBossInterval()
    {
        BossInterval = (Mathf.Pow(4f, 3f) / (StartTime * 0.75f) + 60f);
        nextBossSpawn += BossInterval;
        Debug.LogWarning("B: " + nextBossSpawn);
    }

    private void Update()
    {
        if (StopTime) return;
        StartTime += Time.deltaTime;
        TimeText.text = "Time: " + ((int)(StartTime)).ToString();

        if (StartTime > nextEnemySpawn)
        {
            CalcEnemyInterval();
            SpawnEnemy();
        }

        if (StartTime > nextBossSpawn)
        {
            CalcBossInterval();
            SpawnBoss();
        }
    }

    private void SpawnEnemy()
    {
        int num = Random.Range(3, 7);
        int type1 = Random.Range(0, 3);
        int type2 = Random.Range(0, 3);

        GameObject prefab1 = type1 switch
        {
            0 => ShooterPrefab,
            1 => JumperPrefab,
            2 => ChargerPrefab,
        };

        GameObject prefab2 = type2 switch
        {
            0 => ShooterPrefab,
            1 => JumperPrefab,
            2 => ChargerPrefab,
        };

        for (int i = 0; i < num; i++)
        {
            List<SpawnPoint> spawnPoints = SpawnPoints.Where(p => !p.PlayerNear()).ToList();
            int pos = Random.Range(0, spawnPoints.Count);

            int rnd = Random.Range(0, 2);

            if (rnd == 0)
            {
                Instantiate(prefab1, spawnPoints[pos].transform.position, Quaternion.identity);
            }
            else
            {
                Instantiate(prefab2, spawnPoints[pos].transform.position, Quaternion.identity);
            }

            spawnPoints[pos].PlayParticles();
        }
    }

    private void SpawnBoss()
    {
        List<SpawnPoint> spawnPoints = SpawnPoints.Where(p => !p.PlayerNear()).ToList();
        int pos = Random.Range(0, spawnPoints.Count);

        Instantiate(TankonPrefab, spawnPoints[pos].transform.position, Quaternion.identity);
        spawnPoints[pos].PlayParticles();
    }
}
