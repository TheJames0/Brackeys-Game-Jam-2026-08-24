using UnityEngine;
using System.Collections.Generic;
using UnityEditor;
using System;

[CreateAssetMenu(fileName = "Turn", menuName = "CardSystem/Turn")]

public class Turn : ScriptableObject
{
    public Player playerTurn;

    public bool useTurnText = true;
    [TextArea]
    public string turnText;

    public List<Card> cardsGivenToPlayer;

    public bool useOutcomeText = true;
    [TextArea]
    public string turnOutcomeText;
}
