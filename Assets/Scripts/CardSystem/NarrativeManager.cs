using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class NarrativeManager : MonoBehaviour
{
    [SerializeField] private List<TimelinePoint> timelinePoint;

    //UI 

    //[SerializeField] private Transform cardSpawnPoint;

    private int currentTimelineIndex = 0;
    private TimelinePoint currentTimelinePoint;

    private GameObject currentCard;

    //Starts by loading the first turn within the list
    private void Start()
    {
        LoadTimelinePoint(currentTimelineIndex);
    }

    //Loads a turn
    private void LoadTimelinePoint(int pointIndex)
    {
        if (pointIndex >= timelinePoint.Count)
        {
            Debug.Log("Narrative end.");
            return;
        }

        currentTimelinePoint = timelinePoint[pointIndex];

        foreach (NarrativeEvent narrativeEvent in currentTimelinePoint.events)
        {
            ExecuteEvent(narrativeEvent);
        }
    }

    private void ExecuteEvent(NarrativeEvent narrativeEvent)
    {
        switch (narrativeEvent.eventType)
        {
            case NarrativeEventType.ShowText:
                ShowText(narrativeEvent.text);
                break;

            case NarrativeEventType.ShowCard:
                ShowCard(narrativeEvent.card);
                break;

            case NarrativeEventType.HideCard:
                HideCard(narrativeEvent.card);
                break;
        }
    }

    private void ShowText(string text)
    {
        Debug.Log(text);

        //implement UI text here
    }

    private void ShowCard(Card card)
    {
        if (card == null || card.cardPrefab == null)
            return;

        if (currentCard != null)
        {
            Destroy(currentCard);
        }

        currentCard = Instantiate(card.cardPrefab);
    }

    private void HideCard(Card card)
    {
        if (currentCard != null)
        {
            return;
        }

        Destroy(currentCard);
        currentCard = null;
    }

    //Calls the next turn (however we want to do that)
    public void NextTimelinePoint()
    {
        currentTimelineIndex++;
        LoadTimelinePoint(currentTimelineIndex);
    }
}
