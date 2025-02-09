using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class TransitionManager : MonoBehaviour
{
    public static TransitionManager Instance { get; private set; }

    [SerializeField] private Canvas transitionCanvas;
    [SerializeField] private Image fadeImage;
    [SerializeField] private TextMeshProUGUI loadingText;
    [SerializeField] public float fadeDuration = 1f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void StartTransition(string loadingMessage, System.Action onTransitionIn, System.Action onTransitionOut)
    {
        StartCoroutine(PerformTransition(loadingMessage, onTransitionIn, onTransitionOut));
    }

    private IEnumerator PerformTransition(string loadingMessage, System.Action onTransitionIn, System.Action onTransitionOut)
    {
        transitionCanvas.gameObject.SetActive(true);
        loadingText.text = loadingMessage;
        // Set initial fade image to fully opaque instead of transparent
        fadeImage.color = new Color(0, 0, 0, 1);

        // Remove fade in coroutine
        onTransitionIn?.Invoke();

        yield return new WaitForSeconds(1f);

        onTransitionOut?.Invoke();

        yield return StartCoroutine(FadeOut());

        transitionCanvas.gameObject.SetActive(false);
    }
    
    private IEnumerator FadeOut()
    {
        float elapsedTime = 0f;
        Color startColor = new Color(0, 0, 0, 1);
        Color endColor = new Color(0, 0, 0, 0);

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            fadeImage.color = Color.Lerp(startColor, endColor, elapsedTime / fadeDuration);
            yield return null;
        }

        fadeImage.color = endColor;
    }
}