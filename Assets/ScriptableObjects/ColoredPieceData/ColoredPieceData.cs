using UnityEngine;

[CreateAssetMenu(fileName = "ColoredPieceData", menuName = "Scriptable Objects/ColoredPieceData")]
public class ColoredPieceData : ScriptableObject
{
    public ColoredPieceColor color;
    public Sprite baseSprite;
    public Sprite rocketSprite;
    public Sprite explosionSprite;
}
