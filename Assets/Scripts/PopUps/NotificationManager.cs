using System.Collections;
using TMPro;
using UnityEngine;

public class NotificationManager : MonoBehaviour
{
    public TextMeshProUGUI notificationText;

    private void Start()
    {
        if (notificationText == null)
        {
            Debug.LogError("Notification Text is not assigned.");
        }
    }

    public void ShowNotification(string message)
    {
        notificationText.text = message;
        StartCoroutine(FadeOutNotification());
    }

    private IEnumerator FadeOutNotification()
    {
        // Make the text fully visible
        notificationText.alpha = 1f;

        // Wait for 4 seconds
        yield return new WaitForSeconds(4);

        // Fade out the text over 1 second
        float fadeDuration = 1f;
        float fadeSpeed = 1f / fadeDuration;

        while (notificationText.alpha > 0)
        {
            notificationText.alpha -= Time.deltaTime * fadeSpeed;
            yield return null;
        }

        // Ensure the text is completely invisible
        notificationText.alpha = 0;
    }
}
