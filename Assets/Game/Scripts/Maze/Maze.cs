using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class Maze : MonoBehaviour
{
    private class SearchData
    {
        public int index = 0;
        public CellType A = CellType.None;
        public CellType B = CellType.None;

        public SearchData(int i, CellType a, CellType b)
        {
            index = i;
            A = a;
            B = b;
        }
    }

    [UnityEditor.MenuItem("BookWorm/GenerateMaze")]
    private static void Generate()
    {
        Shader litShader = Shader.Find("Universal Render Pipeline/Lit");

        Material wallMaterial = new Material(litShader);
        wallMaterial.color = Color.red;

        Material groundMaterial = new Material(litShader);
        groundMaterial.color = Color.white;

        Material ceilingMaterial = new Material(litShader);
        ceilingMaterial.color = Color.black;

        Generate(Random.Range(0, int.MaxValue), wallMaterial, groundMaterial, ceilingMaterial, 20, 20);
    }
    public static Maze Generate(int seed, Material wallMaterial, Material groundMaterial, Material ceilingMaterial, int width = 40, int height = 40)
    {
        GameObject mazeGO = new GameObject("Maze");
        Maze maze = mazeGO.AddComponent<Maze>();
        maze.Configure(width, height, seed, wallMaterial, groundMaterial, ceilingMaterial);
        return maze;
    }

    [System.Flags]
    public enum CellType { None = 0, N = 1, E = 2, S = 4, W = 8, Straight = 15 }
    private Vector2Int dimensions;
    public Vector2Int Dimensions => dimensions;
    private List<CellType> cells;
    public List<CellType> Cells => cells;

    private GameObject ceiling;
    private GameObject Ceiling => ceiling;
    private GameObject ground;
    private GameObject Ground => ground;
    private GameObject wallsContainer;
    private GameObject WallsContainer => wallsContainer;

    public void Configure(int width, int height, int seed, Material wallMaterial, Material groundMaterial, Material ceilingMaterial)
    {
        ground = GameObject.CreatePrimitive(PrimitiveType.Quad);
        ground.name = "Floor";
        ground.transform.SetParent(transform);
        ground.transform.localPosition = Vector3.zero;
        ground.transform.localScale = new Vector3(width, height, 1f) * MazeComponentBuilder.GRID_SIZE; //a bit confusing that y and z are swapped but its because we are working with a rotated upright quad
        ground.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
        ground.GetComponent<MeshRenderer>().material = groundMaterial;

        ceiling = GameObject.CreatePrimitive(PrimitiveType.Quad);
        ceiling.name = "Ceiling";
        ceiling.transform.SetParent(transform);
        ceiling.transform.localScale = new Vector3(width, height, 1f) * MazeComponentBuilder.GRID_SIZE; //a bit confusing that y and z are swapped but its because we are working with a rotated upright quad
        ceiling.transform.localPosition = Vector3.up * MazeComponentBuilder.GRID_SIZE;
        ceiling.transform.localRotation = Quaternion.Euler(270f, 0f, 0f);
        ceiling.GetComponent<MeshRenderer>().material = ceilingMaterial;

        wallsContainer = new GameObject("Walls");
        wallsContainer.transform.SetParent(transform);

        dimensions = new Vector2Int(width, height);
        cells = Enumerable.Range(0, dimensions.x * dimensions.y).Select(x => CellType.None).ToList();
        Build(seed);
        for (int i = 0; i < cells.Count; i++)
        {
            MazeComponentBuilder.Build(wallsContainer.transform, CoordsToWorld(IndexToCoords(i)), cells[i], wallMaterial);
        }
    }

    public Vector2Int IndexToCoords(int index)
    {
        int y = index / dimensions.x;
        int x = index - dimensions.x * y;
        return new Vector2Int(x, y);
    }

    public Vector3 CoordsToWorld(Vector2Int coordinates)
    {
        float x = MazeComponentBuilder.GRID_SIZE * coordinates.x + 1f - dimensions.x;
        float y = MazeComponentBuilder.GRID_SIZE * coordinates.y + 1f - dimensions.y;
        return new Vector3(x, 0f, y);
    }

    private void Build(int seed)
    {
        Random.InitState(seed);
        var searchData = Enumerable.Range(0, 4).Select(x => new SearchData(0, CellType.None, CellType.None)).ToList();
        var searchIndices = Enumerable.Range(0, cells.Count).Select(x => 0).ToList();
        searchIndices[0] = Random.Range(0, cells.Count);

        int firstSearchIndex = 0, lastSearchIndex = 0;
        while (firstSearchIndex <= lastSearchIndex)
        {
            int index = searchIndices[lastSearchIndex];

            int possibleRoutes = SearchCorridors(index, searchData);
            if (possibleRoutes <= 1)
            {
                lastSearchIndex -= 1;
            }
            if (possibleRoutes > 0)
            {
                SearchData coridoor = searchData[Random.Range(0, possibleRoutes)];
                cells[index] |= coridoor.A;
                cells[coridoor.index] = coridoor.B;
                searchIndices[++lastSearchIndex] = coridoor.index;
            }
        }
    }

    private int SearchCorridors(int idx, List<SearchData> searchDataList)
    {
        var coords = IndexToCoords(idx);
        int count = 0;

        void TryAddPassage(int testIndex, CellType from, CellType to)
        {
            if (cells[testIndex] == CellType.None)
            {
                searchDataList[count++] = new SearchData(testIndex, from, to);
            }
        }

        if (coords.x + 1 < dimensions.x)
        {
            TryAddPassage(idx + 1, CellType.E, CellType.W);
        }
        if (coords.x > 0)
        {
            TryAddPassage(idx - 1, CellType.W, CellType.E);
        }
        if (coords.y + 1 < dimensions.y)
        {
            TryAddPassage(idx + dimensions.x, CellType.N, CellType.S);
        }
        if (coords.y > 0)
        {
            TryAddPassage(idx - dimensions.x, CellType.S, CellType.N);
        }
        return count;
    }
}