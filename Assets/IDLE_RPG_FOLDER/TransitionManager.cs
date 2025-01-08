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
        fadeImage.color = new Color(0, 0, 0, 0); // ตั้งค่าให้ fadeImage โปร่งใสเมื่อเริ่มต้น
        yield return StartCoroutine(FadeIn());

        onTransitionIn?.Invoke();

        yield return new WaitForSeconds(0.5f); // เวลาขั้นต่ำสำหรับการแสดงข้อความ

        onTransitionOut?.Invoke();

        yield return StartCoroutine(FadeOut());

        transitionCanvas.gameObject.SetActive(false);
    }

    private IEnumerator FadeIn()
    {
        yield return Fade(0, 1);
    }
    
    private IEnumerator FadeOut()
    {
        yield return Fade(1, 0);
    }

    private IEnumerator Fade(float startAlpha, float endAlpha)
    {
        float elapsedTime = 0f;
        Color startColor = new Color(0, 0, 0, startAlpha);
        Color endColor = new Color(0, 0, 0, endAlpha);

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            fadeImage.color = Color.Lerp(startColor, endColor, elapsedTime / fadeDuration);
            yield return null;
        }

        fadeImage.color = endColor;
    }
}