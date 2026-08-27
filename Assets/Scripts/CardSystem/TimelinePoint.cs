using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TimelinePoint", menuName = "CardSystem/TimelinePoint")]

public class TimelinePoint : ScriptableObject
{
    public Player playerTurn;

    public List<NarrativeEvent> events;
}
