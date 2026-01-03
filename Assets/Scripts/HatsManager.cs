using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class HatsManager : MonoBehaviour
{
    public static HatsManager Instance;

    public GameObject hatPanel;
    public Hats hats;
    public GameObject hatPanelPrefab;
    public Transform hatsParent;
    private void Awake()
    {
        Instance = this;
    }
    public void FillHats()
    {
        foreach (Transform child in hatsParent) Destroy(child.gameObject);
        for (int i = 0; i < hats.hats.Count; i++)
        {
            Hat hat = hats.hats[i];

            GameObject newHat = Instantiate(hatPanelPrefab, hatsParent);
            newHat.GetComponent<HatButton>().icon.sprite = hat.icon;
            newHat.GetComponent<HatButton>().hat = hat;
        }
    }
    public void SetPlayerHat(Hat hat)
    {
        FindAnyObjectByType<PlayerController>().CurrentHat = hat;
    }
}
