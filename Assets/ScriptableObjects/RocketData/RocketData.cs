using UnityEngine;

[CreateAssetMenu(fileName = "RocketData", menuName = "Scriptable Objects/RocketData")]
public class RocketData : ScriptableObject
{
    public bool isVertical;

    public Sprite baseSprite;
    public Sprite headSprite;
    public Sprite tailSprite;
}
