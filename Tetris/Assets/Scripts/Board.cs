using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI; 
using System.Collections.Generic;

[DefaultExecutionOrder(-1)]
public class Board : MonoBehaviour
{
    public Tilemap tilemap { get; private set; }
    public Piece activePiece { get; private set; }
    public TetrominoData[] tetrominoes; 
    public Vector2Int boardSize = new Vector2Int(10, 20);
    public Vector3Int spawnPosition = new Vector3Int(-1, 8, 0);

    private ScoreManager scoreManager;
    private TetrominoData nextTetromino;

    public GameObject nextPiecePanel; 
    public GameObject nextPieceTile; 

    public RectInt Bounds
    {
        get
        {
            Vector2Int position = new Vector2Int(-boardSize.x / 2, -boardSize.y / 2);
            return new RectInt(position, boardSize);
        }
    }

    private void Awake()
    {
        tilemap = GetComponentInChildren<Tilemap>();
        activePiece = GetComponentInChildren<Piece>();
        scoreManager = FindObjectOfType<ScoreManager>();

       
        for (int i = 0; i < tetrominoes.Length; i++)
        {
            tetrominoes[i].Initialize();
        }

        nextTetromino = GetRandomTetromino();
    }

    private void Start()
    {
        SpawnPiece();
        DisplayNextPiece(); 
    }

    public void SpawnPiece()
    {
        TetrominoData data = nextTetromino;
        nextTetromino = GetRandomTetromino(); 
        activePiece.Initialize(this, spawnPosition, data);

        if (IsValidPosition(activePiece, spawnPosition))
        {
            Set(activePiece);
            DisplayNextPiece(); 
        }
        else
        {
            GameOver();
        }
    }

    private TetrominoData GetRandomTetromino()
    {
        int random = Random.Range(0, tetrominoes.Length);
        return tetrominoes[random];
    }

    public void GameOver()
    {
        tilemap.ClearAllTiles();

        if (scoreManager != null)
        {
            scoreManager.ResetScore();
        }

        Debug.Log("Game Over!");
    }

    public void Set(Piece piece)
    {
        for (int i = 0; i < piece.cells.Length; i++)
        {
            Vector3Int tilePosition = piece.cells[i] + piece.position;
            tilemap.SetTile(tilePosition, piece.data.tile);
        }
    }

    public void Clear(Piece piece)
    {
        for (int i = 0; i < piece.cells.Length; i++)
        {
            Vector3Int tilePosition = piece.cells[i] + piece.position;
            tilemap.SetTile(tilePosition, null);
        }
    }

    public bool IsValidPosition(Piece piece, Vector3Int position)
    {
        RectInt bounds = Bounds;

        for (int i = 0; i < piece.cells.Length; i++)
        {
            Vector3Int tilePosition = piece.cells[i] + position;

            if (!bounds.Contains((Vector2Int)tilePosition) || tilemap.HasTile(tilePosition))
            {
                return false;
            }
        }

        return true;
    }

    private void DisplayNextPiece()
    {
        
        foreach (Transform child in nextPiecePanel.transform)
        {
            Destroy(child.gameObject);
        }

       
        Vector2Int[] cells = nextTetromino.cells; 
        foreach (var cell in cells)
        {
            
            GameObject tileObject = new GameObject("Tile");

            
            RectTransform rectTransform = tileObject.AddComponent<RectTransform>();
            rectTransform.SetParent(nextPiecePanel.transform, false); 
            rectTransform.sizeDelta = new Vector2(30, 30); 
            rectTransform.localPosition = new Vector3(cell.x * 30, cell.y * 30, 0); 

            
            Image tileImage = tileObject.AddComponent<Image>();
            tileImage.sprite = nextTetromino.tile.sprite; 
            tileImage.rectTransform.sizeDelta = new Vector2(30, 30); 
        }
    }

    public void ClearLines()
    {
        RectInt bounds = Bounds;
        int row = bounds.yMin;

        
        while (row < bounds.yMax)
        {
            if (IsLineFull(row))
            {
                LineClear(row); 
                scoreManager.AddScore(10); 
                Debug.Log("Satýr silindi, 10 puan eklendi! Toplam Puan: " + scoreManager.CurrentScore);
            }
            else
            {
                row++;
            }
        }
    }

    public bool IsLineFull(int row)
    {
        RectInt bounds = Bounds;

        
        for (int col = bounds.xMin; col < bounds.xMax; col++)
        {
            Vector3Int position = new Vector3Int(col, row, 0);

           
            if (!tilemap.HasTile(position))
            {
                return false;
            }
        }

       
        return true;
    }

    public void LineClear(int row)
    {
        RectInt bounds = Bounds;

        
        for (int col = bounds.xMin; col < bounds.xMax; col++)
        {
            Vector3Int position = new Vector3Int(col, row, 0);
            tilemap.SetTile(position, null);
        }

        
        while (row < bounds.yMax)
        {
            for (int col = bounds.xMin; col < bounds.xMax; col++)
            {
                Vector3Int position = new Vector3Int(col, row + 1, 0);
                TileBase above = tilemap.GetTile(position);

                position = new Vector3Int(col, row, 0);
                tilemap.SetTile(position, above);
            }

            row++;
        }
    }
}
