using System;
using System.Collections;
using UnityEngine;

public class Particle : MonoBehaviour
{
    [SerializeField] private SpriteRenderer sr;


    public Particle Initialize(
        Vector3 scale,
        Sprite sprite,
        Quaternion? rotation = null)
    {
        transform.localPosition = Vector3.zero;
        transform.localScale = scale;
        transform.localRotation = rotation ?? Quaternion.identity;

        sr.sprite = sprite;
        return this;
    }

    public void Animate(
        Vector3 wayToGo,
        float lifetime = 0.5f,
        Action onComplete = null)
    {
        StartCoroutine(AnimateParticle(
            wayToGo,
            lifetime,
            onComplete
        ));
    }

    IEnumerator AnimateParticle(
        Vector3 wayToGo,
        float lifetime,
        Action onComplete)
    {
        float elapsed = 0;

        Vector3 start = transform.position;
        Vector3 end = start + wayToGo;

        Color startColor = sr.color;

        while (elapsed < lifetime)
        {
            elapsed += Time.deltaTime;
            float progress = elapsed / lifetime;

            transform.position = Vector3.Lerp(start, end, progress);
            sr.color = new Color(
                startColor.r,
                startColor.g,
                startColor.b,
                1 - progress
            );

            yield return null;
        }

        onComplete?.Invoke();
        Destroy(gameObject);
    }
}
