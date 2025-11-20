using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelResultLoseUI : LazySingleton<LevelResultLoseUI>
{
    [SerializeField] private RectTransform maskTransform;
    [SerializeField] private RectTransform backgroundTransform;

    [SerializeField] private Button tryAgainButton;
    [SerializeField] private Button closeButton;


    private void Awake()
    {
        tryAgainButton.onClick.AddListener(HandleTryAgainButtonClicked);
        closeButton.onClick.AddListener(HandleCloseButtonClicked);
    }

    public void Show()
    {
        StartCoroutine(Reveal());
    }

    private IEnumerator Reveal()
    {
        float elapsed = 0f;
        float startHeight = 0f;
        float targetHeight = backgroundTransform.sizeDelta.y;
        const float revealDuration = 1f;

        backgroundTransform.gameObject.SetActive(false);
        maskTransform.sizeDelta = new Vector2(maskTransform.sizeDelta.x, 0f);

        while (elapsed < revealDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / revealDuration);
            float currentHeight = Mathf.Lerp(startHeight, targetHeight, t);
            maskTransform.sizeDelta = new Vector2(maskTransform.sizeDelta.x, currentHeight);
            yield return null;
        }

        maskTransform.sizeDelta = new Vector2(maskTransform.sizeDelta.x, targetHeight);
        backgroundTransform.gameObject.SetActive(true);
    }

    private void HandleTryAgainButtonClicked()
    {
        SceneManager.LoadScene("LevelScene");
    }

    private void HandleCloseButtonClicked()
    {
        SceneManager.LoadScene("MainScene");
    }
}
