using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "Hat", menuName = "Scriptable/Hat")]
public class Hat : ScriptableObject
{
    public Sprite icon;
    public GameObject prefab;
    public bool isBought = false;
    public int price;
}
