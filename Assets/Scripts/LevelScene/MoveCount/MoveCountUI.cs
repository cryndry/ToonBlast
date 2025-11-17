using TMPro;
using UnityEngine;

public class MoveCountUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI moveCountText;

    private void OnEnable()
    {
        EventManager.OnMoveCountChanged += UpdateMoveCountUI;
    }

    private void OnDisable()
    {
        EventManager.OnMoveCountChanged -= UpdateMoveCountUI;
    }

    private void UpdateMoveCountUI(int newMoveCount)
    {
        moveCountText.text = newMoveCount.ToString();
    }
}
