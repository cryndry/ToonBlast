using System.Collections;
using UnityEngine;

public class Box : Piece
{
    [SerializeField] private BoxData boxData;

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
        yield return null;
        Destroy(gameObject);
        // Additional explosion effects will be added here
    }
}
