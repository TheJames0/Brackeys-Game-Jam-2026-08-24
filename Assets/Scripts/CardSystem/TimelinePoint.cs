using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Turn", menuName = "CardSystem/Turn")]

public class TimelinePoint : ScriptableObject
{
    public Player playerTurn;

    public List<NarrativeEvent> events;
}
