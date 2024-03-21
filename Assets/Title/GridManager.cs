using UnityEngine;

public class GridManager : MonoBehaviour
{
    public GameObject hexagonPrefab;
    public int width = 10;
    public int height = 10;
    private Hexagon[,] grid;

    private int oldX, oldY;


    public float spriteSizeModifier;

    public float horizontalModifier, verticalModifier, gridOffset;

    public Vector2 gridStartPosition = new Vector2(0, 0); 

    public float hexagonWidth = 1.0f;

    public bool regenerateGrid;

    public int startingPulseChance = 200;

    [SerializeField] private bool gridEnabled = true;

    void Start()
    {
        if (gridEnabled)
        {
            GenerateGrid();
        }
    }

    private void Update()
    {
        if (regenerateGrid)
        {
            GenerateGrid();
            regenerateGrid = false;
        }
    }

    void GenerateGrid()
    {
        ClearGrid();
        grid = new Hexagon[width, height];
        CreateGrid();
        AssignNeighbors();
        oldX = width;
        oldY = height;
    }
    void CreateGrid()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector2 position = CalculateHexagonPosition(x, y) + gridStartPosition;
                GameObject hexObj = Instantiate(hexagonPrefab, position, Quaternion.identity, this.transform);
                Hexagon hexScript = hexObj.GetComponent<Hexagon>();

                hexObj.transform.localScale *= spriteSizeModifier;


                if (Random.Range(0, startingPulseChance) < 1) 
                {
                    bool pulseDirection = Random.Range(0, 2) == 0;
                    hexScript.isPulsing = true;
                    if (pulseDirection)
                    {
                        hexScript.SignalRight = true;
                    }
                    else
                    {
                        hexScript.SignalLeft = true;
                    }
                }

                grid[x, y] = hexScript;
            }
        }
    }


    void ClearGrid()
    {
        if (grid != null)
        {
            for (int x = 0; x < oldX; x++)
            {
                for (int y = 0; y < oldY; y++)
                {
                    if (grid[x, y] != null)
                        Destroy(grid[x, y].gameObject);
                }
            }
        }
    }


    void AssignNeighbors()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Hexagon currentHex = grid[x, y];
                if (x > 0) currentHex.leftNeighbor = grid[x - 1, y];
                if (x < width - 1) currentHex.rightNeighbor = grid[x + 1, y];
            }
        }
    }

    Vector2 CalculateHexagonPosition(int x, int y)
    {
        float horizontalSpacing = hexagonWidth * horizontalModifier;
        float verticalSpacing = hexagonWidth * verticalModifier;

        float xOffset = x * horizontalSpacing + (y % 2 == 0 ? 0 : gridOffset * hexagonWidth);
        float yOffset = y * verticalSpacing;

        return new Vector2(xOffset, yOffset);
    }
}
