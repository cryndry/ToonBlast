using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class GameBoard : MonoBehaviour
{
    private GameBoard() { }
    public static GameBoard Instance { get; private set; }

    [SerializeField] private GameObject slotPrefab;
    [SerializeField] private Transform slotContainer;
    [SerializeField] private Transform pieceContainer;

    private int level;
    private int columnCount = 10;
    private int rowCount = 10;
    private int moveCount = 20;

    private float cellSize;
    private const float maxCellSize = 1f;
    private const float slotSizePPUScaler = 100f / 1f; // 1f is slot sprite's size in pixels, 100f is pixels per unit
    private const float cellSizePPUScaler = 100f / 142f; // 142f is piece sprite's size in pixels, 100f is pixels per unit
    private Vector3 startPosition;

    private TileSlot[,] grid;
    private string[] gridCellTypes;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
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
            gridCellTypes = levelData.grid.ToArray();
            moveCount = levelData.move_count;

            grid = new TileSlot[columnCount, rowCount];
            for (int x = 0; x < columnCount; x++)
            {
                for (int y = 0; y < rowCount; y++)
                {
                    int index = y * columnCount + x;
                    bool isUsable = gridCellTypes[index] != "null";
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

        float suggestedCellSize = screenWidth * 0.8f / columnCount;
        cellSize = Mathf.Min(suggestedCellSize, maxCellSize);

        float gridWidth = cellSize * columnCount;
        float gridHeight = cellSize * rowCount;

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
                    slotGO.transform.localScale = cellSize * slotSizePPUScaler * Vector3.one;

                    int index = y * columnCount + x;
                    string cellType = gridCellTypes[index];
                    GameObject pieceGO = PieceGenerator.Instance.GeneratePiece(cellType, pieceContainer);
                    pieceGO.transform.localPosition = slotPosition;
                    pieceGO.transform.localScale = cellSize * cellSizePPUScaler * Vector3.one;

                    Piece piece = pieceGO.GetComponent<Piece>();
                    piece.GridPosition = slot.position;
                    slot.currentPiece = piece;
                }
            }
        }
    }

    private Vector2 GetPositionOfTile(int x, int y)
    {
        float posX = startPosition.x + (x + 0.5f) * cellSize;
        float posY = startPosition.y + (y + 0.5f) * cellSize;
        return new Vector2(posX, posY);
    }

    private void ShowRocketHints()
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

                foreach (TileSlot neighbor in GetAdjacentCellPositions(current))
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

    private List<TileSlot> GetAdjacentCellPositions(TileSlot slot, bool includeDiagonals = false)
    {
        List<TileSlot> neighbors = new List<TileSlot>();
        Vector2Int cellPosition = slot.position;

        if (cellPosition.x > 0) neighbors.Add(grid[cellPosition.x - 1, cellPosition.y]);
        if (cellPosition.x < columnCount - 1) neighbors.Add(grid[cellPosition.x + 1, cellPosition.y]);
        if (cellPosition.y > 0) neighbors.Add(grid[cellPosition.x, cellPosition.y - 1]);
        if (cellPosition.y < rowCount - 1) neighbors.Add(grid[cellPosition.x, cellPosition.y + 1]);

        if (includeDiagonals)
        {
            if (cellPosition.x > 0 && cellPosition.y > 0) neighbors.Add(grid[cellPosition.x - 1, cellPosition.y - 1]);
            if (cellPosition.x > 0 && cellPosition.y < rowCount - 1) neighbors.Add(grid[cellPosition.x - 1, cellPosition.y + 1]);
            if (cellPosition.x < columnCount - 1 && cellPosition.y > 0) neighbors.Add(grid[cellPosition.x + 1, cellPosition.y - 1]);
            if (cellPosition.x < columnCount - 1 && cellPosition.y < rowCount - 1) neighbors.Add(grid[cellPosition.x + 1, cellPosition.y + 1]);
        }

        return neighbors;
    }

    private void UpdateGrid()
    {
        for (int x = 0; x < columnCount; x++)
        {
            for (int y = 0; y < rowCount; y++)
            {
                TileSlot slot = grid[x, y];
                if (!slot.isUsable) continue;

                Piece piece = slot.currentPiece;
                if (piece != null) continue;

                // Logic to make pieces fall into empty slots would go here
                for (int checkY = y + 1; checkY < rowCount; checkY++)
                {
                    TileSlot checkSlot = grid[x, checkY];
                    if (checkSlot.isUsable && checkSlot.currentPiece != null && checkSlot.currentPiece.IsFallable())
                    {
                        Piece fallingPiece = checkSlot.currentPiece;
                        checkSlot.currentPiece = null;
                        slot.currentPiece = fallingPiece;
                        fallingPiece.GridPosition = slot.position;
                        fallingPiece.transform.localPosition = GetPositionOfTile(x, y);
                        break;
                    }
                }
            }
        }
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

            foreach (ColoredPiece match in matchingPieces)
            {
                TileSlot matchSlot = grid[match.GridPosition.x, match.GridPosition.y];
                match.Status = ColoredPieceStatus.Exploding;
                matchSlot.currentPiece = null;
            }

            if (matchingPieces.Count >= 4)
            {
                // spawn a rocket
            }
        }

        UpdateGrid();
        ShowRocketHints();
    }
}
