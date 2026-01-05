using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IngameCoinsManager : MonoBehaviour
{
    public static IngameCoinsManager Instance;

    public GameObject coinPrefab;

    private void Awake()
    {
        Instance = this;
    }
    public void SpawnCoin(Vector3 position)
    {
        Debug.Log("Spawning coin at: " + position);
        Instantiate(coinPrefab, position, Quaternion.identity);
    }
}
