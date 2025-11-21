using System.Collections;
using UnityEngine;

public class Vase : Piece
{
    [SerializeField] private VaseData vaseData;
    private int currentHP;
    private int finishedParticleCount = 0;


    protected override void Awake()
    {
        base.Awake();
        currentHP = vaseData.hitPoints;
        GoalManager.Instance.IncreaseGoalOfType(vaseData.goalType);
    }

    public override void OnTap() { }

    public override bool OnBreak()
    {
        bool isBroken = Damage();
        return isBroken;
    }

    public override bool OnBreakPowerUp()
    {
        bool isBroken = Damage();
        return isBroken;
    }

    public override bool IsFallable() => true;

    private bool Damage()
    {
        currentHP--;
        bool isBroken = currentHP <= 0;

        if (isBroken)
        {
            GoalManager.Instance.DecreaseGoalOfType(vaseData.goalType);
            StartCoroutine(Explode());
        }
        else
        {
            sr.sprite = vaseData.damagedSprite;
        }

        return isBroken;
    }

    protected override IEnumerator Explode()
    {
        sr.sprite = null;

        Vector3 spread = transform.localScale * 0.5f;
        Vector3 particleScale = transform.localScale * 0.6f;

        foreach (Sprite explosionSprite in vaseData.explosionSprites)
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

        while (finishedParticleCount < vaseData.explosionSprites.Length)
            yield return null;

        Destroy(gameObject);
    }
}
