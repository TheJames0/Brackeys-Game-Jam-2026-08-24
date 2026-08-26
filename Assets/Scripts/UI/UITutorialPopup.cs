using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Collections;
using System.Text.RegularExpressions;
public class UITutorialPopup : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private TMP_Text text;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private List<AudioClip> audioClips;

    private Coroutine typingCoroutine;
    void Start()
    {
        // for testing
        Sprite testSprite = Resources.Load<Sprite>("pete");
        ShowDialog(testSprite, "Hey pal, you look a little lost at the poker table. Is this your first game fella? Don't you worry, i'm here to show you the ropes, we need to work on your poker face though see mine i have perfected it.", 0.05f);
    }
    public void ShowDialog(Sprite sprite, string text, float typeSpeed = 0.03f)
    {
        if (text == null)
        {
            Debug.LogError("Tutorial text is null.");
            return;
        }

        if (image != null && this.image != null)
        {
            image.sprite = sprite;
        }

        AnimateString(text, typeSpeed);
    }

    private void AnimateString(string value, float typeSpeed)
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }

        typingCoroutine = StartCoroutine(AnimateStringCoroutine(value, typeSpeed));
    }

    private IEnumerator AnimateStringCoroutine(string value, float typeSpeed)
    {
        if (text == null)
        {
            Debug.LogError("Tutorial TMP text is not assigned.");
            typingCoroutine = null;
            yield break;
        }

        text.text = string.Empty;
        typeSpeed = Mathf.Max(0f, typeSpeed);

        for (int i = 0; i < value.Length; i++)
        {
            char character = value[i];
            text.text += character;

            //Slight pause after end of sentence.
            float delay = Random.Range(typeSpeed * 0.75f, typeSpeed * 1.25f);
            if (Regex.IsMatch(character.ToString(), @"[.,!?;:]"))
            {
                delay += Random.Range(0.3f, 1.0f);
            }

            yield return new WaitForSeconds(delay);
        }

        typingCoroutine = null;
    }

}
