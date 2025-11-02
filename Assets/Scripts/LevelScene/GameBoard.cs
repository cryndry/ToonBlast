using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class GameBoard : MonoBehaviour
{
    [SerializeField] private GameObject slotPrefab;
    [SerializeField] private Transform slotContainer;
    [SerializeField] private Transform pieceContainer;

    private int level;
    private int columnCount = 10;
    private int rowCount = 10;
    private int moveCount = 20;

    private float cellSize;
    private const float maxCellSize = 1f;
    private const float slotSizePPUScaler = 100f;
    private const float cellSizePPUScaler = 100f / 142f;
    private Vector3 startPosition;

    private TileSlot[,] grid;
    private string[] gridCellTypes;

    void Start()
    {
        LoadLevelData();
        GetGridInfo();
        DrawBoard();
    }

    void LoadLevelData()
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
                    grid[x, y] = new TileSlot(x, y, isUsable);
                }
            }
        }
        else
        {
            Debug.LogError("Level JSON not found: " + levelFilePath);
        }
    }

    void GetGridInfo()
    {
        columnCount = 5;
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
                if (grid[x, y].isUsable)
                {
                    Vector3 slotPosition = GetPositionOfTile(x, y);
                    GameObject slot = Instantiate(slotPrefab, slotContainer);
                    slot.transform.localPosition = slotPosition;
                    slot.transform.localScale = cellSize * slotSizePPUScaler * Vector3.one;

                    int index = y * columnCount + x;
                    string cellType = gridCellTypes[index];
                    GameObject pieceGO = PieceGenerator.Instance.GeneratePiece(cellType, pieceContainer);
                    pieceGO.transform.localPosition = slotPosition;
                    pieceGO.transform.localScale = cellSize * cellSizePPUScaler * Vector3.one;

                    Piece piece = pieceGO.GetComponent<Piece>();
                    piece.GridPosition = new Vector2Int(x, y);
                    grid[x, y].currentPiece = piece;
                }
            }
        }
    }

    Vector2 GetPositionOfTile(int x, int y)
    {
        float posX = startPosition.x + (x + 0.5f) * cellSize;
        float posY = startPosition.y + (y + 0.5f) * cellSize;
        return new Vector2(posX, posY);
    }
}
