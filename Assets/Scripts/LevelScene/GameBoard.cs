using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class GameBoard : MonoBehaviour
{
    [SerializeField] private GameObject slotPrefab;
    [SerializeField] private Transform slotContainer;

    private int level;
    private int columnCount = 10;
    private int rowCount = 10;
    private int moveCount = 20;

    private float cellSize;
    private const float cellSizePPUScaler = 100f;
    private Vector3 startPosition;

    private TileSlot[,] slotGrid;
    private string[] gridCellTypes;

    void Start()
    {
        LoadLevelData();
        GetGridInfo(out cellSize, out startPosition);
        DrawBoard();
        Debug.Log($"Cell Size: {cellSize}, Start Position: {startPosition}");
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

            slotGrid = new TileSlot[columnCount, rowCount];
            for (int x = 0; x < columnCount; x++)
            {
                for (int y = 0; y < rowCount; y++)
                {
                    int index = y * columnCount + x;
                    bool isUsable = gridCellTypes[index] != "null";
                    slotGrid[x, y] = new TileSlot(x, y, isUsable);
                }
            }
        }
        else
        {
            Debug.LogError("Level JSON not found: " + levelFilePath);
        }
    }

    void GetGridInfo( out float cellSize, out Vector3 startPosition)
    {
        float screenHeight = Camera.main.orthographicSize * 2f;
        float screenWidth = screenHeight * Camera.main.aspect;

        float gridWidth = screenWidth * 0.8f;
        Debug.Log("Grid Width: " + gridWidth);
        Debug.Log("Screen width: " + screenWidth);
        Debug.Log("Screen height: " + screenHeight);

        cellSize = gridWidth / columnCount;
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

        if (slotGrid == null)
        {
            Debug.LogError("Grid data is not initialized!");
            return;
        }

        for (int x = 0; x < columnCount; x++)
        {
            for (int y = 0; y < rowCount; y++)
            {
                if (slotGrid[x, y].isUsable)
                {
                    Vector3 slotPosition = GetPositionOfTile(x, y);
                    GameObject slot = Instantiate(slotPrefab, slotContainer);
                    slot.transform.localPosition = slotPosition;
                    slot.transform.localScale = cellSize * cellSizePPUScaler * Vector3.one;
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
