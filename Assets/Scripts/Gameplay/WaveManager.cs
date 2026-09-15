using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EnemyObject
{
    public string enemyName;
    public GameObject enemyPrefab;
    [Range(0f, 100f)]public float spawnChance;
    public float spawnChanceIncrease;
    //This just stores accumulated weight for each enemy
    [HideInInspector]public double _weight;
}

public class WaveManager : MonoBehaviour
{
    private int waveNumber = 0;
    public int WaveNumber => waveNumber;
    private int score = 0;
    public int Score => score;

    public int enemiesRemaining = 0;
    private int enemiesToSpawn = 0;
    private int enemiesKilled = 0;
    private int difficulty = 0;
    private float spawnInterval = 3f;
    private bool isSpawning = false, gameStarted = false;

    [SerializeField] private List<EnemyObject> enemy;

    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private ParticleSystem[] spawnParticles;
    [SerializeField] private float dangerRadius = 5f;
    private System.Random rand = new System.Random();
    private double accumulatedWeight;
    [SerializeField] private Animator waveAnimator;

    [Header("Difficulty Settings")]
    [SerializeField] float spawnIntervalReduction = 0.1f;
    [SerializeField] float damageMult = 0.15f;
    [SerializeField] float healthMult = 15f;
    [SerializeField] GameObject shopPanel;
    [SerializeField] MoneyManager moneyManager;

    void Awake()
    {
        gameStarted = false;
    }

    void Update()
    {
        //Starting the waves if all enemies dead and no more enemies to spawn
        if (enemiesRemaining <= 0 && enemiesToSpawn <= 0 && !isSpawning && gameStarted)
        {
            Debug.Log($"Wave {waveNumber + 1} Starting");
            StartCoroutine(StartNextWave());
        }
    }

    IEnumerator SpawnWave()
    {
        while (enemiesToSpawn > 0)
        {
            int spawnIndex = ChooseEnemyIndex();
            StartCoroutine(SpawnEnemy(enemy[spawnIndex].enemyPrefab));
            enemiesToSpawn--;
            enemiesRemaining++;
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    int ChooseEnemyIndex()
    {
        //Uh go look at some tutorial for accumulated weights and probability I am kinda clueless here too
        double r = rand.NextDouble() * accumulatedWeight;
        for (int i = 0; i < enemy.Count; i++)
        {
            if (r < enemy[i]._weight)
            {
                return i;
            }
        }
        return 0;
    }

    IEnumerator SpawnEnemy(GameObject enemyPrefab)
    {
        //Get spawn play animation boom we have enemy
        int spawnLocationIndex = UnityEngine.Random.Range(0, spawnPoints.Length);
        if (enemyPrefab == null || spawnPoints == null || spawnPoints.Length == 0) yield break;
        Transform spawnPoint = spawnPoints[spawnLocationIndex];
        spawnParticles[spawnLocationIndex].Play();
        yield return new WaitForSeconds(1.7f);
        GameObject go = Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);

        // Apply difficulty-based adjustments to the spawned instance (not the prefab that shi broke the game)
        var enemyComp = go.GetComponent<Enemy>();
        if (enemyComp != null)
        {
            enemyComp.damageMultiplier = 1f + difficulty * damageMult;
        }

        var weaponHandlerComp = go.GetComponent<EnemyWeaponHandler>();
        if (weaponHandlerComp != null)
        {
            weaponHandlerComp.UpdateDamage();
        }

        var healthComp = go.GetComponent<Health>();
        if (healthComp != null)
        {
            float newMaxHealth = healthComp.MaxHealth + difficulty * healthMult;
            healthComp.UpdateMaxHealth(newMaxHealth);
        }
    }

    private void OnDrawGizmos()
    {
        if (spawnPoints == null) return;
        Gizmos.color = Color.red;
        foreach (Transform spawnPoint in spawnPoints)
        {
            if (spawnPoint != null)
                Gizmos.DrawWireSphere(spawnPoint.position, dangerRadius);
        }
    }

    IEnumerator StartNextWave()
    {
        //Start next wave update wave stuff and difficulty and spawn shop or smthg
        isSpawning = true;
        CalculateWeight();
        yield return new WaitForSeconds(1f);
        if (shopPanel != null && waveNumber % 3 == 0 && waveNumber != 0)
        {
            StartCoroutine(OpenShop());
            moneyManager.AddMoney(50 * (difficulty + 1));
            moneyManager.UpdateMoneyText();
        }
        waveNumber++;
        yield return new WaitForSeconds(2f);
        waveAnimator.SetTrigger("StartWave");
        yield return new WaitForSeconds(0.5f);
        if (waveNumber % 3 == 0)
        {
            difficulty++;
            Debug.Log($"Difficulty Increased! {difficulty}");
            UpdateDifficulty();
        }
        enemiesToSpawn = 3 + Mathf.FloorToInt(0.18f * Mathf.Pow(waveNumber, 2.3f));
        yield return StartCoroutine(SpawnWave());
        isSpawning = false;
    }

    //Logic for enemy selection and difficulty scaling
    void CalculateWeight()
    {
        accumulatedWeight = 0f;
        foreach(EnemyObject e in enemy)
        {
            accumulatedWeight += e.spawnChance;
            e._weight = accumulatedWeight;
        }
    }

    void UpdateDifficulty()
    {
        spawnInterval = Mathf.Max(0.75f, spawnInterval - spawnIntervalReduction);
        if (enemy == null) return;
        foreach (EnemyObject e in enemy)
            e.spawnChance = Mathf.Min(90f, e.spawnChance + e.spawnChanceIncrease * difficulty);
    }

    public void NotifyDeath()
    {
        enemiesKilled++;
        enemiesRemaining = Mathf.Max(0, enemiesRemaining - 1);
        Debug.Log($"NotifyDeath called. enemiesRemaining={enemiesRemaining}", this);
    }

    //Some UI and other related functions
    public IEnumerator OpenShop()
    {
        if (shopPanel != null)
        {
            shopPanel.SetActive(true);
            shopPanel.GetComponent<Animator>().SetTrigger("Open");
            yield return new WaitForSeconds(1f);
            Time.timeScale = 0f; //Pausing the game
        }
    }

    public void CloseShop()
    {
        if (shopPanel != null)
        {
            shopPanel.GetComponent<Animator>().SetTrigger("Close");
            Invoke("DisableShopUI", 1f);
            Time.timeScale = 1f; //Resuming the game
        }
    }

    void DisableShopUI()
    {
        shopPanel.SetActive(false);
    }

    public void StartGame()
    {
        gameStarted = true;
        waveNumber = 0;
        enemiesRemaining = 0;
        enemiesToSpawn = 0;
        difficulty = 0;
        spawnInterval = 3f;
        isSpawning = false;
        CalculateWeight();
    }

    public void CalculateScore()
    {
        score = enemiesKilled * 10 + waveNumber * 25 * (int)Mathf.Pow(difficulty, 2f);
    }
}