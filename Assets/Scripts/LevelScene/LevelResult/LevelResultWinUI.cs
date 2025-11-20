using System.Collections;
using UnityEngine;

public class LevelResultWinUI : LazySingleton<LevelResultWinUI>
{
    [SerializeField] private GameObject winningStarPrefab;
    [SerializeField] private RectTransform rectTransform;


    public void Show()
    {
        StartCoroutine(Reveal());
    }

    private IEnumerator Reveal()
    {
        for (int i = 0; i < 50; i++)
        {
            SpawnStar();
            yield return new WaitForSeconds(Random.Range(0.03f, 0.06f));
        }

        EventManager.InvokeLevelResultUIFinished(true);
    }

    private void SpawnStar()
    {
        float starSize = Random.Range(20f, 40f);
        Vector2 canvasSize = rectTransform.sizeDelta;
        Vector3 spawnPosition = new Vector3(
            Random.Range(-canvasSize.x / 2f, canvasSize.x / 2f),
            canvasSize.y / 4f,
            0f
        );

        GameObject star = Instantiate(winningStarPrefab, transform);
        star.transform.localScale = new Vector3(starSize, starSize, 1f);
        star.transform.localPosition = spawnPosition;
        
        StartCoroutine(AnimateStarFall(star));
    }

    private IEnumerator AnimateStarFall(GameObject star)
    {
        float speed = Random.Range(200, 500);
        float drift = Random.Range(-50, 50);

        Vector3 moveDir = new Vector3(drift, -speed, 0f);
        SpriteRenderer sr = star.GetComponent<SpriteRenderer>();

        float elapsed = 0f;
        float lifetime = 2f;
        while (elapsed < lifetime)
        {
            elapsed += Time.deltaTime;
            star.transform.localPosition += moveDir * Time.deltaTime;
            sr.color = new Color(1f, 1f, 1f, Mathf.Lerp(1f, 0.3f, elapsed / lifetime));
            yield return null;
        }

        Destroy(star);
    }
}
