using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "Hats", menuName = "Scriptable/Hats")]
public class Hats : ScriptableObject
{
    public List<Hat> hats;
}
