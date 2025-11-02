using UnityEngine;

public class TileSlot
{
    public int x, y;
    public bool isUsable;
    public Piece currentPiece; 
    
    public TileSlot(int x, int y, bool isUsable)
    {
        this.x = x;
        this.y = y;
        this.isUsable = isUsable;
        this.currentPiece = null;
    }
}