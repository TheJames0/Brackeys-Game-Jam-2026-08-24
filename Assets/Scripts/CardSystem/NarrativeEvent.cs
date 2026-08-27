using UnityEngine;

public enum NarrativeEventType
{
    None,
    ShowText,
    ShowCard,
    HideCard,
    PlaySound
}

[System.Serializable]
public class NarrativeEvent
{
    public NarrativeEventType eventType;

    [TextArea(2, 6)]
    public string text;
    
    public Card card;

    public AudioClip audioClip;
}
