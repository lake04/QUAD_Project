using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemData", menuName = "ItemScriptable/ItemData")]
[System.Serializable]
public class ItemData : ScriptableObject
{
    public string name;
    public int Id;
}
