using UnityEngine;

public class PieceGenerator : MonoBehaviour
{
    [SerializeField] private GameObject RedPiecePrefab;
    [SerializeField] private GameObject GreenPiecePrefab;
    [SerializeField] private GameObject BluePiecePrefab;
    [SerializeField] private GameObject YellowPiecePrefab;
    private GameObject[] coloredPiecePrefabs;
    [SerializeField] private GameObject BoxPrefab;
    [SerializeField] private GameObject StonePrefab;
    [SerializeField] private GameObject VasePrefab;


    public static PieceGenerator Instance;
    private PieceGenerator() { }


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        coloredPiecePrefabs = new GameObject[]
        {
            RedPiecePrefab,
            GreenPiecePrefab,
            BluePiecePrefab,
            YellowPiecePrefab
        };
    }

    public GameObject GeneratePiece(string pieceType, Transform parent)
    {
        GameObject piecePrefab = pieceType switch
        {
            "r" => RedPiecePrefab,
            "g" => GreenPiecePrefab,
            "b" => BluePiecePrefab,
            "y" => YellowPiecePrefab,
            "rand" => coloredPiecePrefabs[Random.Range(0, 4)],
            "bo" => BoxPrefab,
            "s" => StonePrefab,
            "v" => VasePrefab,
            _ => RedPiecePrefab, // TODO: Handle invalid type after implementing all types
        };

        if (piecePrefab != null)
        {
            return Instantiate(piecePrefab, parent);
        }

        throw new System.Exception($"Piece type '{pieceType}' is not recognized.");
    }
}