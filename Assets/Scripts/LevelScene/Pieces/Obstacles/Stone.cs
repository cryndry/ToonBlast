using System.Collections;
using UnityEngine;

public class Stone : Piece
{
    [SerializeField] private StoneData stoneData;

    protected override void Awake()
    {
        base.Awake();
        GoalManager.Instance.IncreaseGoalOfType(stoneData.goalType);
    }

    public override void OnTap() { }

    public override bool OnBreak()
    {
        return false;
    }

    public override bool OnBreakPowerUp()
    {
        GoalManager.Instance.DecreaseGoalOfType(stoneData.goalType);
        StartCoroutine(Explode());
        return true;
    }

    public override bool IsFallable() => false;

    protected override IEnumerator Explode()
    {
        yield return null;
        Destroy(gameObject);
        // Additional explosion effects will be added here
    }
}
