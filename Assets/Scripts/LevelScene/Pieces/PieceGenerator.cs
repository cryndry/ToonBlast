using UnityEngine;

public class PieceGenerator : MonoBehaviour
{
    [SerializeField] private GameObject RedPiecePrefab;
    [SerializeField] private GameObject GreenPiecePrefab;
    [SerializeField] private GameObject BluePiecePrefab;
    [SerializeField] private GameObject YellowPiecePrefab;

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
    }

    public GameObject GeneratePiece(string pieceType, Transform parent)
    {
        GameObject piecePrefab = pieceType switch
        {
            "r" => RedPiecePrefab,
            "g" => GreenPiecePrefab,
            "b" => BluePiecePrefab,
            "y" => YellowPiecePrefab,
            _ => RedPiecePrefab, // TODO: Handle invalid type after implementing all types
        };

        if (piecePrefab != null)
        {
            return Instantiate(piecePrefab, parent);
        }

        throw new System.Exception($"Piece type '{pieceType}' is not recognized.");
    }
}