using System.Collections;
using UnityEngine;

public class Box : Piece
{
    [SerializeField] private BoxData boxData;
    private int finishedParticleCount = 0;


    protected override void Awake()
    {
        base.Awake();
        GoalManager.Instance.IncreaseGoalOfType(boxData.goalType);
    }

    public override void OnTap() { }

    public override bool OnBreak()
    {
        GoalManager.Instance.DecreaseGoalOfType(boxData.goalType);
        StartCoroutine(Explode());
        return true;
    }

    public override bool OnBreakPowerUp()
    {
        GoalManager.Instance.DecreaseGoalOfType(boxData.goalType);
        StartCoroutine(Explode());
        return true;
    }

    public override bool IsFallable() => false;

    protected override IEnumerator Explode()
    {
        sr.sprite = null;

        Vector3 spread = transform.localScale * 0.5f;
        Vector3 particleScale = transform.localScale * 0.6f;

        foreach (Sprite explosionSprite in boxData.explosionSprites)
        {
            PieceGenerator.Instance.GeneratePiece("particle", transform)
                .GetComponent<Particle>()
                .Initialize(
                    particleScale,
                    explosionSprite,
                    Quaternion.Euler(0, 0, Random.Range(0f, 360f))
                ).Animate(
                    Random.insideUnitCircle.normalized * spread,
                    0.7f,
                    () => finishedParticleCount++
                );
        }

        while (finishedParticleCount < boxData.explosionSprites.Length)
            yield return null;

        Destroy(gameObject);
    }
}
