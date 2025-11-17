public class MoveCountManager : LazySingleton<MoveCountManager>
{
    private int moves;
    private int Moves
    {
        get
        {
            return moves;
        }
        set
        {
            moves = value;
            EventManager.InvokeMoveCountChanged(moves);
        }
    }

    public void SetInitialMoves(int initialCount)
    {
        Moves = initialCount;
    }

    public void UseMove()
    {
        Moves--;
    }

    public bool HasMovesLeft()
    {
        return Moves > 0;
    }
}
