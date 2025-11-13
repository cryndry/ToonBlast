using UnityEngine;

public class TileSlot
{
    public Vector2Int position;
    public bool isUsable;
    public Piece currentPiece;
    public Piece futurePiece;

    public TileSlot(Vector2Int position, bool isUsable)
    {
        this.position = position;
        this.isUsable = isUsable;
    }
}