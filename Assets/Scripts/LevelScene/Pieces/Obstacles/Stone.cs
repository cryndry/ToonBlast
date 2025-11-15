using System.Collections;
using UnityEngine;

public class Stone : Piece
{
    [SerializeField] private StoneData stoneData;

    public override void OnTap() { }

    public override bool OnBreak()
    {
        return false;
    }

    public override bool OnBreakPowerUp()
    {
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
