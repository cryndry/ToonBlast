using UnityEngine;

[CreateAssetMenu(fileName = "BoxData", menuName = "Scriptable Objects/BoxData")]
public class BoxData : ScriptableObject
{
    public Sprite baseSprite;
    public Sprite[] explosionSprites;
}
