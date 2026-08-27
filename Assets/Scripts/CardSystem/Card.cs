using UnityEngine;

[CreateAssetMenu(fileName = "Card", menuName = "CardSystem/Card")]

public class Card : ScriptableObject
{
    public string cardName;
    
    [Header("Visuals")]
    public GameObject cardPrefab;
}
