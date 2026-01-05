using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColosseumManager : MonoBehaviour
{
    public List<ColosseumWave> waves;
    public List<Transform> spawnPoints;
    public List<Enemy> spawnedEnemies;

    int currentWaveIndex = 0;
    List<ColosseumEnemy> colosseumEnemies = new();

    public List<GameObject> enemiesPrefabs;

    private void Start()
    {
        StartCoroutine(StartNewWave());
    }

    IEnumerator StartNewWave()
    {
        if (currentWaveIndex >= waves.Count) yield break;

        yield return new WaitForSeconds(3);

        ColosseumWave wave = waves[currentWaveIndex];
        currentWaveIndex++;
        for (int i = 0; i < wave.enemiesCount; i++)
        {
            Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Count)];
            
            EnemyType enemyType = wave.enemies[Random.Range(0, wave.enemies.Count)];

            GameObject enemyPrefab = enemiesPrefabs.Find(x => x.GetComponent<Enemy>().enemyType == enemyType);
            if (enemyPrefab != null)
            {
                GameObject spawnedEnemy = Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);
                Enemy enemy = spawnedEnemy.GetComponent<Enemy>();
                ColosseumEnemy ce = new ColosseumEnemy(enemy);
                colosseumEnemies.Add(ce);
                enemy.onDeath.AddListener(() =>
                {
                    ColosseumEnemy colosseumEnemy = colosseumEnemies.Find(ce => ce.enemy == enemy);
                    if (colosseumEnemy != null)
                    {
                        colosseumEnemy.isAlive = false;
                        Debug.Log("Enemy died: " + enemy.enemyType);
                    }
                    if (CheckForEndOfWave()) StartCoroutine(StartNewWave());
                });
                enemy.onVoxelling.AddListener(() =>
                {
                    IngameCoinsManager.Instance?.SpawnCoin(enemy.transform.position);
                });
            }
        }
    }

    bool CheckForEndOfWave()
    {
        bool allDead = true;
        foreach (ColosseumEnemy enemy in colosseumEnemies)
        {
            if (enemy != null && enemy.isAlive)
            {
                allDead = false;
                break;
            }
        }
        return allDead;
    }
}

public class ColosseumEnemy
{
    public Enemy enemy;
    public bool isAlive = true;
    public ColosseumEnemy(Enemy enemy)
    {
        this.enemy = enemy;
    }
}

[System.Serializable]
public class ColosseumWave
{
    public List<EnemyType> enemies;
    public int enemiesCount;
}
