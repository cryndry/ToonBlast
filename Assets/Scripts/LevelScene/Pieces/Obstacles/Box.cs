using System.Collections;
using UnityEngine;

public class Box : Piece
{
    [SerializeField] private BoxData boxData;

    public override void OnTap() { }

    public override bool OnBreak()
    {
        StartCoroutine(Explode());
        return true;
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
