using System.Collections;
using UnityEngine;

public class Vase : Piece
{
    [SerializeField] private VaseData vaseData;
    private int currentHP;

    private void Awake()
    {
        currentHP = vaseData.hitPoints;
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
            GameBoard.Instance.AddFallCount(GridPosition.x);
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
        yield return null;
        Destroy(gameObject);
        // Additional explosion effects will be added here
    }
}
