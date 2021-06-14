using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ProceduralGeneration : MonoBehaviour
{
    [Header("Terrain Gen")]
    [SerializeField] int width; // Width & height of our procedurally generation
    [SerializeField] int height;
    [SerializeField] float smoothness;
    [SerializeField] float seed;

    [Header("Cave Gen")]
    [Range(0, 1)]
    [SerializeField] float caveModifier;
    [Range(0, 1)]
    [SerializeField] float oreModifier;

    [Header("Tile")]
    [SerializeField] public TileBase groundTile; // This is where we put our rule tile
    [SerializeField] public Tilemap groundTileMap;
    [SerializeField] public TileBase caveTile;
    [SerializeField] public Tilemap caveTileMap;
    [SerializeField] public TileBase oreTile;
    [SerializeField] public Tilemap oreTileMap;

    int[,] map;
    private RaycastHit hit;


    // Start is called before the first frame update
    void Start()
    {
        Generation();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            Generation();
        }
    }

    public void Generation()
    {
        ClearMap();
        map = GenerateArray(width, height, true);
        map = TerrainGeneration(map);
        RenderMap(map, groundTileMap, groundTile, caveTileMap, caveTile, oreTileMap, oreTile);
    }

    public int[,] GenerateArray(int width, int height, bool empty)
    {
        int[,] map = new int[width, height];
        for (int x = 0; x < width; x++) // Will go through width of the map
        {
            for (int y = 0; y < height; y++) // WIll go through height of the map
            {
                if (empty) map[x, y] = 0;
                else map[x, y] = 1;
            }
        }
        return map;
    }

    public int[,] TerrainGeneration(int[,] map)
    {
        int perlinHeight;
        for (int x = 0; x < width; x++)
        {
            perlinHeight = Mathf.RoundToInt(Mathf.PerlinNoise(x / smoothness, seed)*height/2);
            perlinHeight += height / 2;
            for (int y = 0; y < perlinHeight; y++)
            {  
                int caveValue = Mathf.RoundToInt(Mathf.PerlinNoise(x * caveModifier + seed, y * caveModifier + seed));
                float oreSpawnValue = Random.Range(1f, 101f);

                if (caveValue == 1 && y < perlinHeight - 10) // Generate cave tile 10 tiles below surface level
                {
                    map[x, y] = 2; // 2 = cave tile
                }
                else
                {
                    if (oreSpawnValue < 5f && y < perlinHeight - 20) // Give ore a 5% chance to spawn instead of a ground tile
                    {
                        map[x, y] = 3; // 3 = ore tile
                    } else
                    {
                        map[x, y] = 1; // 1 = ground tile
                    }
                }  
            }
        }
        return map;
    }

    public void RenderMap(int[,] map, Tilemap groundTileMap, TileBase groundTile, Tilemap caveTileMap, TileBase caveTile, Tilemap oreTileMap, TileBase oreTile)
    {
        for (int x = 0; x < map.GetUpperBound(0); x++) // Will go through width of the map
        {
            for (int y = 0; y < map.GetUpperBound(1); y++) // WIll go through height of the map
            {
                if (map[x, y] == 1) groundTileMap.SetTile(new Vector3Int(x, y, 0), groundTile);
                if (map[x, y] == 2) caveTileMap.SetTile(new Vector3Int(x, y, 0), caveTile);
                if (map[x, y] == 3) oreTileMap.SetTile(new Vector3Int(x, y, 0), oreTile);
            }
        }
    }

    public void ClearMap()
    {
        groundTileMap.ClearAllTiles();
        caveTileMap.ClearAllTiles();
        oreTileMap.ClearAllTiles();
    }
}
