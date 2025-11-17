using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class GameBoard : LazySingleton<GameBoard>
{
    [SerializeField] private GameObject slotPrefab;
    [SerializeField] private Transform slotContainer;
    [SerializeField] private Transform pieceContainer;

    private int level;
    private int columnCount = 10;
    private int rowCount = 10;

    public float pieceSize;
    private const float maxPieceSize = 1f;
    private const float slotSizePPUScaler = 100f / 1f; // 1f is slot sprite's size in pixels, 100f is pixels per unit
    private const float pieceSizePPUScaler = 100f / 142f; // 142f is piece sprite's size in pixels, 100f is pixels per unit
    private Vector3 startPosition;

    private TileSlot[,] grid;
    private string[] gridPieceTypes;
    private Queue<Piece>[] willFallQueues;

    public int activeRocketCount = 0;
    public bool IsInteractable
    {
        get
        {
            return (
                activeRocketCount == 0
                && MoveCountManager.Instance.HasMovesLeft()
                && !GoalManager.Instance.AreGoalsCompleted()
            );
        }
    }

    private void Start()
    {
        LoadLevelData();
        GetGridInfo();
        DrawBoard();
        ShowRocketHints();
    }

    private void LoadLevelData()
    {
        level = PlayerPrefs.GetInt("CurrentLevel", 1);
        string levelFileName = $"level_{level:D2}.json";
        string levelFilePath = Path.Combine(Application.dataPath, "Levels", levelFileName);

        if (File.Exists(levelFilePath))
        {
            string jsonContent = File.ReadAllText(levelFilePath);
            LevelData levelData = JsonUtility.FromJson<LevelData>(jsonContent);

            columnCount = levelData.grid_width;
            rowCount = levelData.grid_height;
            gridPieceTypes = levelData.grid.ToArray();
            MoveCountManager.Instance.SetInitialMoves(levelData.move_count);

            willFallQueues = new Queue<Piece>[columnCount];
            grid = new TileSlot[columnCount, rowCount];
            for (int x = 0; x < columnCount; x++)
            {
                willFallQueues[x] = new Queue<Piece>();
                for (int y = 0; y < rowCount; y++)
                {
                    int index = y * columnCount + x;
                    bool isUsable = gridPieceTypes[index] != "null";
                    Vector2Int slotPosition = new Vector2Int(x, y);
                    grid[x, y] = new TileSlot(slotPosition, isUsable);
                }
            }
        }
        else
        {
            Debug.LogError("Level JSON not found: " + levelFilePath);
        }
    }

    private void GetGridInfo()
    {
        float screenHeight = Camera.main.orthographicSize * 2f;
        float screenWidth = screenHeight * Camera.main.aspect;

        float suggestedPieceSize = screenWidth * 0.8f / columnCount;
        pieceSize = Mathf.Min(suggestedPieceSize, maxPieceSize);

        float gridWidth = pieceSize * columnCount;
        float gridHeight = pieceSize * rowCount;

        GameObject canvas = GameObject.Find("MainCanvas");
        CanvasScaler scaler = canvas.GetComponent<CanvasScaler>();
        float canvasHeight = scaler.referenceResolution.y;

        RectTransform canvasHeader = canvas.transform.Find("Header").GetComponent<RectTransform>();
        float canvasHeaderHeight = canvasHeader.sizeDelta.y;

        float headerHeightWorld = screenHeight * canvasHeaderHeight / canvasHeight;

        startPosition = new Vector3(
            gridWidth / -2f,
            (gridHeight + headerHeightWorld) / -2f,
            -1
        );
    }

    public void DrawBoard()
    {
        if (grid == null)
        {
            Debug.LogError("Grid data is not initialized!");
            return;
        }

        for (int x = 0; x < columnCount; x++)
        {
            for (int y = 0; y < rowCount; y++)
            {
                TileSlot slot = grid[x, y];
                if (slot.isUsable)
                {
                    Vector3 slotPosition = GetPositionOfTile(x, y);
                    GameObject slotGO = Instantiate(slotPrefab, slotContainer);
                    slotGO.transform.localPosition = slotPosition;
                    slotGO.transform.localScale = pieceSize * slotSizePPUScaler * Vector3.one;

                    int index = y * columnCount + x;
                    string pieceType = gridPieceTypes[index];
                    GameObject pieceGO = PieceGenerator.Instance.GeneratePiece(pieceType, pieceContainer);
                    pieceGO.transform.localPosition = slotPosition;
                    pieceGO.transform.localScale = pieceSize * pieceSizePPUScaler * Vector3.one;

                    Piece piece = pieceGO.GetComponent<Piece>();
                    piece.GridPosition = slot.position;
                    slot.currentPiece = piece;
                    slot.isReserved = true;
                }
            }
        }

        gridPieceTypes = null;
    }

    public Vector2Int GetGridSize()
    {
        return new Vector2Int(columnCount, rowCount);
    }

    public Vector2 GetPositionOfTile(int x, int y)
    {
        float posX = startPosition.x + (x + 0.5f) * pieceSize;
        float posY = startPosition.y + (y + 0.5f) * pieceSize;
        return new Vector2(posX, posY);
    }

    public Vector2Int GetGridIndexFromPosition(Vector2 worldPos)
    {
        int x = Mathf.FloorToInt((worldPos.x - startPosition.x) / pieceSize - 0.5f);
        int y = Mathf.FloorToInt((worldPos.y - startPosition.y) / pieceSize - 0.5f);

        return new Vector2Int(x, y);
    }

    public void ShowRocketHints()
    {
        for (int x = 0; x < columnCount; x++)
        {
            for (int y = 0; y < rowCount; y++)
            {
                if (grid[x, y].currentPiece is ColoredPiece coloredPiece)
                {
                    coloredPiece.Status = ColoredPieceStatus.Normal;
                }
            }
        }

        for (int x = 0; x < columnCount; x++)
        {
            for (int y = 0; y < rowCount; y++)
            {
                TileSlot slot = grid[x, y];
                if (slot.isUsable && slot.currentPiece is ColoredPiece coloredPiece && coloredPiece.Status == ColoredPieceStatus.Normal)
                {
                    List<ColoredPiece> matchingPieces = FindMatchingColoredPieces(slot);
                    if (matchingPieces.Count >= 4)
                    {
                        foreach (ColoredPiece match in matchingPieces)
                        {
                            match.Status = ColoredPieceStatus.Rocketable;
                        }
                    }
                }
            }
        }
    }

    private List<ColoredPiece> FindMatchingColoredPieces(TileSlot startSlot)
    {
        if (startSlot.currentPiece is not ColoredPiece startPiece) return new List<ColoredPiece>();

        List<ColoredPiece> group = new List<ColoredPiece>();
        Queue<TileSlot> queue = new Queue<TileSlot>();
        HashSet<TileSlot> visited = new HashSet<TileSlot>();

        queue.Enqueue(startSlot);
        visited.Add(startSlot);

        ColoredPieceColor targetColor = startPiece.Color;

        while (queue.Count > 0)
        {
            TileSlot current = queue.Dequeue();
            Piece piece = current.currentPiece;

            if (piece != null && piece is ColoredPiece coloredPiece && coloredPiece.Color == targetColor)
            {
                group.Add(coloredPiece);

                foreach (TileSlot neighbor in GetAdjacentPiecePositions(current))
                {
                    if (!visited.Contains(neighbor))
                    {
                        queue.Enqueue(neighbor);
                        visited.Add(neighbor);
                    }
                }
            }
        }

        return group;
    }

    private List<TileSlot> GetAdjacentPiecePositions(TileSlot slot, bool includeDiagonals = false)
    {
        List<TileSlot> neighbors = new List<TileSlot>();
        Vector2Int piecePosition = slot.position;

        if (piecePosition.x > 0) neighbors.Add(grid[piecePosition.x - 1, piecePosition.y]);
        if (piecePosition.x < columnCount - 1) neighbors.Add(grid[piecePosition.x + 1, piecePosition.y]);
        if (piecePosition.y > 0) neighbors.Add(grid[piecePosition.x, piecePosition.y - 1]);
        if (piecePosition.y < rowCount - 1) neighbors.Add(grid[piecePosition.x, piecePosition.y + 1]);

        if (includeDiagonals)
        {
            if (piecePosition.x > 0 && piecePosition.y > 0) neighbors.Add(grid[piecePosition.x - 1, piecePosition.y - 1]);
            if (piecePosition.x > 0 && piecePosition.y < rowCount - 1) neighbors.Add(grid[piecePosition.x - 1, piecePosition.y + 1]);
            if (piecePosition.x < columnCount - 1 && piecePosition.y > 0) neighbors.Add(grid[piecePosition.x + 1, piecePosition.y - 1]);
            if (piecePosition.x < columnCount - 1 && piecePosition.y < rowCount - 1) neighbors.Add(grid[piecePosition.x + 1, piecePosition.y + 1]);
        }

        return neighbors;
    }

    public void UpdateGrid()
    {
        for (int x = 0; x < columnCount; x++)
        {
            for (int y = 0; y < rowCount; y++)
            {
                TileSlot slot = grid[x, y];
                if (!slot.isUsable || slot.isReserved) continue;

                Piece piece = slot.currentPiece;
                if (piece != null) continue;

                // Logic to make pieces fall into empty slots
                for (int checkY = y + 1; checkY < rowCount; checkY++)
                {
                    TileSlot checkSlot = grid[x, checkY];
                    if (checkSlot.isUsable && checkSlot.currentPiece != null)
                    {
                        if (checkSlot.currentPiece.IsFallable())
                        {
                            Piece fallingPiece = checkSlot.currentPiece;
                            ClearSlotPiece(checkSlot.position);
                            slot.isReserved = true;
                            fallingPiece.MoveToPosition(GetPositionOfTile(x, y), extraSpeedFactor: checkY - y);
                        }

                        break;
                    }
                }
            }

            List<int> slotsWithoutFuturePieceIndices = new List<int>();
            for (int y = 0; y < rowCount; y++)
            {
                TileSlot slot = grid[x, y];
                if (!slot.isUsable) continue;

                if (slot.currentPiece != null && !slot.currentPiece.IsFallable())
                {
                    slotsWithoutFuturePieceIndices.Clear();
                    continue;
                }

                if (!slot.isReserved)
                {
                    slotsWithoutFuturePieceIndices.Add(y);
                }
            }

            for (int i = 0; i < slotsWithoutFuturePieceIndices.Count; i++)
            {
                GameObject queuedPieceGO = PieceGenerator.Instance.GeneratePiece("rand", pieceContainer);
                queuedPieceGO.transform.localScale = pieceSize * pieceSizePPUScaler * Vector3.one;

                Piece queuedPiece = queuedPieceGO.GetComponent<Piece>();
                willFallQueues[x].Enqueue(queuedPiece);

                Vector3 initialPosition = GetPositionOfTile(x, rowCount + willFallQueues[x].Count - 1);
                queuedPieceGO.transform.localPosition = initialPosition;

                Vector2Int queuedPieceTargetGridPosition = new Vector2Int(x, slotsWithoutFuturePieceIndices[i]);
                Vector3 queuedPieceTargetPosition = GetPositionOfTile(queuedPieceTargetGridPosition.x, queuedPieceTargetGridPosition.y);
                queuedPiece.MoveToPosition(queuedPieceTargetPosition, extraSpeedFactor: slotsWithoutFuturePieceIndices.Count);

                grid[queuedPieceTargetGridPosition.x, queuedPieceTargetGridPosition.y].isReserved = true;
            }
        }

        if (GoalManager.Instance.AreGoalsCompleted())
        {
            EventManager.InvokeLevelCompleted(true);
        }
        else if (!MoveCountManager.Instance.HasMovesLeft())
        {
            EventManager.InvokeLevelCompleted(false);
        }
    }

    private bool IsSlotUsable(Vector2Int gridPosition)
    {
        if (gridPosition.x < 0 || gridPosition.x >= columnCount ||
            gridPosition.y < 0 || gridPosition.y >= rowCount)
        {
            // Debug.LogError("Grid position out of bounds: " + gridPosition);
            return false;
        }

        return grid[gridPosition.x, gridPosition.y].isUsable;
    }

    public Piece GetSlotPiece(Vector2Int gridPosition)
    {
        bool isSlotUsable = IsSlotUsable(gridPosition);
        if (!isSlotUsable) return null;

        return grid[gridPosition.x, gridPosition.y].currentPiece;
    }

    public bool SetSlotPiece(Vector2Int gridPosition, Piece piece)
    {
        if (piece == null) return false;

        if (!IsSlotUsable(gridPosition)) return false;

        TileSlot slot = grid[gridPosition.x, gridPosition.y];
        slot.currentPiece = piece;
        piece.GridPosition = gridPosition;

        if (willFallQueues[gridPosition.x].TryPeek(out Piece nextPiece) && nextPiece == piece)
        {
            willFallQueues[gridPosition.x].Dequeue();
        }

        ShowRocketHints();

        return true;
    }

    public void ClearSlotPiece(Vector2Int gridPosition)
    {
        if (!IsSlotUsable(gridPosition)) return;

        TileSlot slotToEmpty = grid[gridPosition.x, gridPosition.y];

        slotToEmpty.currentPiece = null;
        slotToEmpty.isReserved = false;
    }

    public void ResolveMatch(Vector2Int gridPosition)
    {
        TileSlot slot = grid[gridPosition.x, gridPosition.y];
        Piece piece = slot.currentPiece;

        if (piece == null) return;

        if (piece is ColoredPiece)
        {
            List<ColoredPiece> matchingPieces = FindMatchingColoredPieces(slot);

            if (matchingPieces.Count < 2) return;

            HashSet<Piece> willBreakPieces = new HashSet<Piece>();
            foreach (ColoredPiece match in matchingPieces)
            {
                TileSlot matchSlot = grid[match.GridPosition.x, match.GridPosition.y];
                match.Status = ColoredPieceStatus.Exploding;
                ClearSlotPiece(matchSlot.position);

                List<TileSlot> neighbors = GetAdjacentPiecePositions(matchSlot);
                foreach (TileSlot neighbor in neighbors)
                {
                    if (neighbor.currentPiece != null)
                    {
                        willBreakPieces.Add(neighbor.currentPiece);
                    }
                }
            }

            foreach (Piece breakPiece in willBreakPieces)
            {
                bool isBroken = breakPiece.OnBreak();
                if (isBroken)
                {
                    ClearSlotPiece(breakPiece.GridPosition);
                }
            }

            if (matchingPieces.Count >= 4)
            {
                GameObject rocketGO = PieceGenerator.Instance.GeneratePiece("randrocket", pieceContainer);
                rocketGO.transform.localScale = pieceSize * pieceSizePPUScaler * Vector3.one;
                rocketGO.transform.localPosition = GetPositionOfTile(gridPosition.x, gridPosition.y);

                Rocket rocket = rocketGO.GetComponent<Rocket>();
                rocket.GridPosition = gridPosition;
                slot.currentPiece = rocket;
                slot.isReserved = true;
            }
        }

        if (activeRocketCount <= 0)
        {
            UpdateGrid();
            ShowRocketHints();
        }
    }
}
