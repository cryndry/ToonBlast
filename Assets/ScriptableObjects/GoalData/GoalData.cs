using UnityEngine;

[CreateAssetMenu(fileName = "GoalData", menuName = "Scriptable Objects/GoalData")]
public class GoalData : ScriptableObject
{
    public GoalType goalType;
    public Sprite goalSprite;
}
