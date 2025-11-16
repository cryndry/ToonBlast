using TMPro;
using UnityEngine;

public class MoveCountManager : LazySingleton<MoveCountManager>
{
    [SerializeField] private TextMeshProUGUI moveText;

    private int moves;
    public int Moves
    {
        get
        {
            return moves;
        }
        set
        {
            moves = value;
            moveText.text = value.ToString();
        }
    }
}
