using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;
using UnityEngine.SceneManagement;

public class CoinsManager : MonoBehaviour
{
    public static CoinsManager Instance;

    int _coins;
    public int Coins
    {
        get
        {
            return _coins;
        }
        set
        {
            _coins = value;
            OnCoinsChanged?.Invoke(_coins);
            PlayerPrefs.SetInt("Coins", _coins);
            foreach (GameObject go in GameObject.FindGameObjectsWithTag("CoinsCountText"))
            {
                go.GetComponent<TMP_Text>().text = _coins.ToString();
            }
        }
    }
    public UnityEvent<int> OnCoinsChanged = new UnityEvent<int>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Coins = PlayerPrefs.GetInt("Coins", 0);
        }
        else
        {
            Destroy(gameObject);
        }
        SceneManager.sceneLoaded += (scene, mode) =>
        {
            foreach (GameObject go in GameObject.FindGameObjectsWithTag("CoinsCountText"))
            {
                go.GetComponent<TMP_Text>().text = Coins.ToString();
            }
        };
    }
}
