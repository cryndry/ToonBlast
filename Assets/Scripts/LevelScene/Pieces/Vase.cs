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
        Damage();
        return currentHP <= 0;
    }

    public override bool OnBreakPowerUp()
    {
        Damage();
        return currentHP <= 0;
    }

    public override bool IsFallable() => true;

    private void Damage()
    {
        currentHP -= 1;
        if (currentHP <= 0)
        {
            StartCoroutine(Explode());
        }
        else
        {
            sr.sprite = vaseData.damagedSprite;
        }
    }

    protected override IEnumerator Explode()
    {
        yield return null;
        Destroy(gameObject);
        // Additional explosion effects will be added here
    }
}
