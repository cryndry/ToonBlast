using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GoalItemUI : MonoBehaviour
{
    [SerializeField] private GoalData goalData;
    [SerializeField] private Image goalImage;
    [SerializeField] private TextMeshProUGUI goalCountText;

    public GoalType GoalType => goalData.goalType;


    public void Initialize(GoalData data, int initialCount)
    {
        goalData = data;
        goalImage.sprite = goalData.goalSprite;
        UpdateGoalCount(initialCount);
    }

    public void UpdateGoalCount(int count)
    {
        goalCountText.text = count.ToString();
    }
}