using UnityEngine;

[CreateAssetMenu(fileName = "VaseData", menuName = "Scriptable Objects/VaseData")]
public class VaseData : ScriptableObject
{
    public Sprite baseSprite;
    public Sprite damagedSprite;
    public Sprite[] explosionSprites;
    public int hitPoints;
    public GoalType goalType;
}
