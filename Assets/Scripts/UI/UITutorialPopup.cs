using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Collections;
using System.Text.RegularExpressions;
public class UITutorialPopup : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private RectTransform panel;
    [SerializeField] private TMP_Text text;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private List<AudioClip> audioClips;
    [SerializeField] private List<TMP_FontAsset> fonts;
    [SerializeField] private Coroutine typingCoroutine;
    private const float rectFactor = 1.0f;
    private readonly List<TMP_Text> bucketTexts = new List<TMP_Text>();
    private readonly List<GameObject> lineObjects = new List<GameObject>();
    private Transform textParent;
    private static readonly Regex styleBucketRegex = new Regex(
        @"<(?:(?<font>[^,<>]+),\s*)?(?<color>#[0-9a-fA-F]{6}(?:[0-9a-fA-F]{2})?)>",
        RegexOptions.Compiled);

    private class TextBucket
    {
        public string content;
        public string fontName;
        public Color color = Color.white;
        public bool hasStyle;
    }

    void Start()
    {
        // for testing
        Sprite testSprite = Resources.Load<Sprite>("pete");
        ShowDialog(testSprite, "Hey pal, you look a little lost at the poker table. Is this your first game fella? <CreamyChicken,#00ff00> Don't you worry, i'm here to show you the ropes, we need to work on your <#ff0000> poker face though <Tropi Land, #00ff00> see mine i have perfected it.", 0.05f);
    }

    private void Awake()
    {
        if (text != null)
        {
            textParent = text.transform.parent;
        }
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

        List<TextBucket> buckets = PreprocessStyleBuckets(text);
        buckets = WrapBuckets(buckets);
        CreateBucketObjects(buckets);
        if (bucketTexts.Count < buckets.Count)
        {
            buckets = buckets.GetRange(0, bucketTexts.Count);
        }
        AnimateString(buckets, typeSpeed);
    }

    private static List<TextBucket> PreprocessStyleBuckets(string value)
    {
        MatchCollection styleBuckets = styleBucketRegex.Matches(value);
        if (styleBuckets.Count == 0)
        {
            return new List<TextBucket> { new TextBucket { content = value } };
        }

        var buckets = new List<TextBucket>();
        int contentStart = 0;
        TextBucket currentBucket = new TextBucket();

        foreach (Match styleBucket in styleBuckets)
        {
            string content = value.Substring(contentStart, styleBucket.Index - contentStart);
            if (content.Length > 0)
            {
                currentBucket.content = content;
                buckets.Add(currentBucket);
            }

            if (!ColorUtility.TryParseHtmlString(styleBucket.Groups["color"].Value, out Color color))
            {
                contentStart = styleBucket.Index + styleBucket.Length;
                continue;
            }

            currentBucket = new TextBucket
            {
                fontName = styleBucket.Groups["font"].Value.Trim(),
                color = color,
                hasStyle = true
            };
            contentStart = styleBucket.Index + styleBucket.Length;
        }

        currentBucket.content = value.Substring(contentStart);
        if (currentBucket.content.Length > 0)
        {
            buckets.Add(currentBucket);
        }

        return buckets;
    }

    private void CreateBucketObjects(List<TextBucket> buckets)
    {
        if (text == null)
        {
            return;
        }

        if (textParent == null)
        {
            textParent = text.transform.parent;
        }

        // Move the template out before removing the generated rows.
        text.transform.SetParent(textParent, false);
        foreach (GameObject lineObject in lineObjects)
        {
            Destroy(lineObject);
        }
        lineObjects.Clear();
        bucketTexts.Clear();

        Vector2 panelSize = GetPanelSize();
        float availableWidth = panelSize.x;
        float availableHeight = panelSize.y;
        float lineHeight = text.GetPreferredValues("Ag", availableWidth, 0f).y;
        float y = 0f;
        GameObject currentLine = null;
        float x = 0f;

        for (int i = 0; i < buckets.Count; i++)
        {
            if (y + lineHeight > availableHeight && currentLine != null)
            {
                break;
            }

            if (currentLine == null)
            {
                currentLine = new GameObject($"Text Line {lineObjects.Count + 1}");
                RectTransform lineRect = currentLine.AddComponent<RectTransform>();
                lineRect.SetParent(textParent, false);
                lineRect.anchorMin = new Vector2(0f, 1f);
                lineRect.anchorMax = new Vector2(0f, 1f);
                lineRect.pivot = new Vector2(0f, 1f);
                lineRect.anchoredPosition = new Vector2(0f, -y);
                lineRect.sizeDelta = new Vector2(availableWidth, lineHeight);
                lineObjects.Add(currentLine);
                x = 0f;
            }

            TMP_Text bucketText;
            if (bucketTexts.Count == 0)
            {
                bucketText = text;
                bucketText.transform.SetParent(currentLine.transform, false);
            }
            else
            {
                bucketText = Instantiate(text, currentLine.transform);
            }
            bucketText.name = $"Text Bucket {i + 1}";
            bucketText.text = string.Empty;
            bucketText.textWrappingMode = TextWrappingModes.NoWrap;
            bucketText.overflowMode = TextOverflowModes.Overflow;

            TextBucket bucket = buckets[i];
            if (bucket.hasStyle)
            {
                TMP_FontAsset font = fonts == null ? null : fonts.Find(candidate =>
                    candidate != null && candidate.name == bucket.fontName);
                if (font == null && !string.IsNullOrEmpty(bucket.fontName))
                {
                    Debug.LogWarning($"No TMP font asset named '{bucket.fontName}' was assigned.");
                }
                else if (font != null)
                {
                    bucketText.font = font;
                }

                bucketText.color = bucket.color;
            }

            float bucketWidth = bucketText.GetPreferredValues(buckets[i].content, 0f, 0f).x;
            if (x > 0f && x + bucketWidth > availableWidth)
            {
                Destroy(bucketText.gameObject);
                currentLine = null;
                y += lineHeight;
                i--;
                continue;
            }

            RectTransform bucketRect = bucketText.rectTransform;
            bucketRect.anchorMin = new Vector2(0f, 0.5f);
            bucketRect.anchorMax = new Vector2(0f, 0.5f);
            bucketRect.pivot = new Vector2(0f, 0.5f);
            bucketRect.anchoredPosition = new Vector2(x, 0f);
            bucketRect.sizeDelta = new Vector2(bucketWidth, lineHeight);
            bucketTexts.Add(bucketText);
            x += bucketWidth;
        }
    }

    private List<TextBucket> WrapBuckets(List<TextBucket> buckets)
    {
        if (text == null || text.transform.parent == null)
        {
            return buckets;
        }

        float availableWidth = GetPanelSize().x;
        if (availableWidth <= 0f)
        {
            return buckets;
        }

        var wrapped = new List<TextBucket>();
        TextWrappingModes originalWrappingMode = text.textWrappingMode;
        text.textWrappingMode = TextWrappingModes.NoWrap;
        float lineWidth = 0f;
        foreach (TextBucket bucket in buckets)
        {
            TMP_FontAsset originalFont = text.font;
            if (bucket.hasStyle && !string.IsNullOrEmpty(bucket.fontName) && fonts != null)
            {
                TMP_FontAsset font = fonts.Find(candidate => candidate != null && candidate.name == bucket.fontName);
                if (font != null)
                {
                    text.font = font;
                }
            }

            string current = string.Empty;
            MatchCollection parts = Regex.Matches(bucket.content, @"\S+\s*|\s+");
            foreach (Match part in parts)
            {
                string token = part.Value;
                string candidate = current + token;
                float tokenWidth = text.GetPreferredValues(token, 0f, 0f).x;
                float candidateWidth = text.GetPreferredValues(candidate, 0f, 0f).x;
                if (current.Length > 0 && lineWidth + candidateWidth > availableWidth)
                {
                    wrapped.Add(new TextBucket
                    {
                        content = current.TrimEnd(),
                        fontName = bucket.fontName,
                        color = bucket.color,
                        hasStyle = bucket.hasStyle
                    });
                    lineWidth = 0f;
                    current = token.TrimStart();
                }
                else if (current.Length == 0 && lineWidth > 0f && lineWidth + tokenWidth > availableWidth)
                {
                    lineWidth = 0f;
                    current = token.TrimStart();
                }
                else if (current.Length == 0 && tokenWidth > availableWidth && !string.IsNullOrWhiteSpace(token))
                {
                    string remaining = token.TrimEnd();
                    while (remaining.Length > 0)
                    {
                        int splitAt = 1;
                        for (int length = 2; length <= remaining.Length; length++)
                        {
                            if (text.GetPreferredValues(remaining.Substring(0, length), 0f, 0f).x > availableWidth)
                            {
                                break;
                            }
                            splitAt = length;
                        }

                        wrapped.Add(new TextBucket
                        {
                            content = remaining.Substring(0, splitAt),
                            fontName = bucket.fontName,
                            color = bucket.color,
                            hasStyle = bucket.hasStyle
                        });
                        remaining = remaining.Substring(splitAt);
                    }

                    current = token.Substring(token.TrimEnd().Length);
                }
                else
                {
                    current = candidate;
                }
            }

            if (current.Length > 0)
            {
                wrapped.Add(new TextBucket
                {
                    content = current,
                    fontName = bucket.fontName,
                    color = bucket.color,
                    hasStyle = bucket.hasStyle
                });
                lineWidth += text.GetPreferredValues(current, 0f, 0f).x;
            }

            text.font = originalFont;
        }

        text.textWrappingMode = originalWrappingMode;

        return wrapped;
    }

    private Vector2 GetPanelSize()
    {
        RectTransform panelRect = panel;
        if (panelRect == null && image != null)
        {
            panelRect = image.rectTransform;
        }

        if (panelRect == null)
        {
            panelRect = textParent as RectTransform;
        }

        if (panelRect == null && text != null)
        {
            panelRect = text.transform.parent as RectTransform;
        }

        return panelRect == null ? Vector2.zero : panelRect.rect.size * rectFactor;
    }

    private void AnimateString(List<TextBucket> buckets, float typeSpeed)
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }

        typingCoroutine = StartCoroutine(AnimateStringCoroutine(buckets, typeSpeed));
    }

    private IEnumerator AnimateStringCoroutine(List<TextBucket> buckets, float typeSpeed)
    {
        if (text == null)
        {
            Debug.LogError("Tutorial TMP text is not assigned.");
            typingCoroutine = null;
            yield break;
        }

        typeSpeed = Mathf.Max(0f, typeSpeed);

        for (int bucketIndex = 0; bucketIndex < buckets.Count; bucketIndex++)
        {
            TextBucket bucket = buckets[bucketIndex];
            TMP_Text bucketText = bucketTexts[bucketIndex];
            for (int i = 0; i < bucket.content.Length; i++)
            {
                char character = bucket.content[i];
                bucketText.text += character;

                // Slight pause after end of sentence.
                float delay = Random.Range(typeSpeed * 0.75f, typeSpeed * 1.25f);
                if (Regex.IsMatch(character.ToString(), @"[.,!?;:]"))
                {
                    delay += Random.Range(0.3f, 1.0f);
                }

                yield return new WaitForSeconds(delay);
            }
        }

        typingCoroutine = null;
    }

}
