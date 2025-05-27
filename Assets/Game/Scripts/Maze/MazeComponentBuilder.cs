using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public static class MazeComponentBuilder
{
    public const float GRID_SIZE = 2f;
    private const float WALL_THICKNESS = 0.1f;
    private const float HALF_WALL = WALL_THICKNESS / 2f;
    private const float GRID_MINUS_WALL = GRID_SIZE - WALL_THICKNESS;
    private const float GRID_MINUS_2WALL = GRID_SIZE - (2f * WALL_THICKNESS);

    public static GameObject Build(Transform parent, Vector3 pos, Maze.CellType cellType, Material wallMaterial)
    {
        var type = cellType & Maze.CellType.Straight;
        return type switch
        {
            Maze.CellType.Straight & ~Maze.CellType.W => BuildSingleWallCell(pos, Vector3.forward, parent, wallMaterial),
            Maze.CellType.Straight & ~Maze.CellType.N => BuildSingleWallCell(pos, Vector3.right, parent, wallMaterial),
            Maze.CellType.Straight & ~Maze.CellType.E => BuildSingleWallCell(pos, Vector3.back, parent, wallMaterial),
            Maze.CellType.Straight & ~Maze.CellType.S => BuildSingleWallCell(pos, Vector3.left, parent, wallMaterial),
            Maze.CellType.N | Maze.CellType.S => BuildStraightCell(pos, Vector3.forward, parent, wallMaterial),
            Maze.CellType.E | Maze.CellType.W => BuildStraightCell(pos, Vector3.right, parent, wallMaterial),
            Maze.CellType.N | Maze.CellType.E => BuildCornerCell(pos, Vector3.forward, parent, wallMaterial),
            Maze.CellType.E | Maze.CellType.S => BuildCornerCell(pos, Vector3.right, parent, wallMaterial),
            Maze.CellType.S | Maze.CellType.W => BuildCornerCell(pos, Vector3.back, parent, wallMaterial),
            Maze.CellType.W | Maze.CellType.N => BuildCornerCell(pos, Vector3.left, parent, wallMaterial),
            Maze.CellType.N => BuildEndCell(pos, Vector3.forward, parent, wallMaterial),
            Maze.CellType.E => BuildEndCell(pos, Vector3.right, parent, wallMaterial),
            Maze.CellType.S => BuildEndCell(pos, Vector3.back, parent, wallMaterial),
            Maze.CellType.W => BuildEndCell(pos, Vector3.left, parent, wallMaterial),
            _ => BuildNoWallCell(pos, Vector3.forward, parent, wallMaterial)
        };
    }

    private static GameObject BuildCornerCell(Vector3 pos, Vector3 fwd, Transform parent, Material wallMaterial)
    {
        GameObject go = new GameObject("corner");
        go.transform.position = pos;
        go.transform.forward = fwd;
        go.transform.SetParent(parent);
        AddWall(go, wallMaterial, "west wall", GRID_MINUS_WALL, new Vector3(-1f + WALL_THICKNESS, 1f, HALF_WALL), Quaternion.Euler(0f, 270f, 0f));
        AddWall(go, wallMaterial, "south wall", GRID_MINUS_WALL, new Vector3(HALF_WALL, 1f, -1f + WALL_THICKNESS), Quaternion.Euler(0f, 180f, 0f));
        AddCorners(go, wallMaterial, true, false, false, false);

        return go;
    }

    private static GameObject BuildStraightCell(Vector3 pos, Vector3 fwd, Transform parent, Material wallMaterial)
    {
        GameObject go = new GameObject("straight");
        go.transform.position = pos;
        go.transform.forward = fwd;
        go.transform.SetParent(parent);
        AddWall(go, wallMaterial, "east wall", GRID_SIZE, new Vector3(1f - WALL_THICKNESS, 1f, 0f), Quaternion.Euler(0f, 90f, 0f));
        AddWall(go, wallMaterial, "west wall", GRID_SIZE, new Vector3(-1f + WALL_THICKNESS, 1f, 0f), Quaternion.Euler(0f, 270f, 0f));

        return go;
    }

    private static GameObject BuildSingleWallCell(Vector3 pos, Vector3 fwd, Transform parent, Material wallMaterial)
    {
        GameObject go = new GameObject("t junction");
        go.transform.position = pos;
        go.transform.forward = fwd;
        go.transform.SetParent(parent);
        AddCorners(go, wallMaterial, true, true, false, false);
        AddWall(go, wallMaterial, "west wall", GRID_SIZE, new Vector3(-1f + WALL_THICKNESS, 1f, 0f), Quaternion.Euler(0f, 270f, 0f));

        return go;
    }

    private static GameObject BuildEndCell(Vector3 pos, Vector3 fwd, Transform parent, Material wallMaterial)
    {
        GameObject go = new GameObject("end");
        go.transform.position = pos;
        go.transform.forward = fwd;
        go.transform.SetParent(parent);
        AddWall(go, wallMaterial, "south wall", GRID_MINUS_2WALL, new Vector3(0f, 1f, -1f + WALL_THICKNESS), Quaternion.Euler(0f, 180f, 0f));
        AddWall(go, wallMaterial, "east wall", GRID_MINUS_WALL, new Vector3(1f - WALL_THICKNESS, 1f, HALF_WALL), Quaternion.Euler(0f, 90f, 0f));
        AddWall(go, wallMaterial, "west wall", GRID_MINUS_WALL, new Vector3(-1f + WALL_THICKNESS, 1f, HALF_WALL), Quaternion.Euler(0f, 270f, 0f));

        return go;
    }

    private static GameObject BuildNoWallCell(Vector3 pos, Vector3 fwd, Transform parent, Material wallMaterial)
    {
        GameObject go = new GameObject("x junction");
        go.transform.position = pos;
        go.transform.forward = fwd;
        go.transform.SetParent(parent);
        AddCorners(go, wallMaterial, true, true, true, true);
        return go;
    }


    private static void AddCorner(GameObject source, Material wallMaterial, Vector3 pos1, Vector3 pos2, Quaternion r1, Quaternion r2)
    {
        GameObject quad1 = GameObject.CreatePrimitive(PrimitiveType.Quad);
        quad1.transform.SetParent(source.transform);
        quad1.transform.localPosition = pos1;
        quad1.transform.localScale = new Vector3(WALL_THICKNESS, GRID_SIZE, 1f);
        quad1.transform.localRotation = r1;
        quad1.GetComponent<MeshRenderer>().material = wallMaterial;

        GameObject quad2 = GameObject.CreatePrimitive(PrimitiveType.Quad);
        quad2.transform.SetParent(source.transform);
        quad2.transform.localPosition = pos2;
        quad2.transform.localScale = new Vector3(WALL_THICKNESS, GRID_SIZE, 1f);
        quad2.transform.localRotation = r2;
        quad2.GetComponent<MeshRenderer>().material = wallMaterial;

    }

    private static void AddCorners(GameObject source, Material wallMaterial, bool NE, bool SE, bool NW, bool SW)
    {
        if (NE) AddCorner(source, wallMaterial, new Vector3((1f - HALF_WALL), 1f, 1f - WALL_THICKNESS), new Vector3(1f - WALL_THICKNESS, 1f, (1f - HALF_WALL)), Quaternion.Euler(0f, 0f, 0f), Quaternion.Euler(0f, 90f, 0f));
        if (SE) AddCorner(source, wallMaterial, new Vector3(1f - WALL_THICKNESS, 1f, (-1f + HALF_WALL)), new Vector3((1f - HALF_WALL), 1f, -1f + WALL_THICKNESS), Quaternion.Euler(0f, 90f, 0f), Quaternion.Euler(0f, 180f, 0f));
        if (NW) AddCorner(source, wallMaterial, new Vector3((-1f + HALF_WALL), 1f, 1f - WALL_THICKNESS), new Vector3(-1f + WALL_THICKNESS, 1f, (1f - HALF_WALL)), Quaternion.Euler(0f, 0f, 0f), Quaternion.Euler(0f, 270f, 0f));
        if (SW) AddCorner(source, wallMaterial, new Vector3(-1f + WALL_THICKNESS, 1f, (-1f + HALF_WALL)), new Vector3((-1f + HALF_WALL), 1f, -1f + WALL_THICKNESS), Quaternion.Euler(0f, 270f, 0f), Quaternion.Euler(0f, 180f, 0f));
    }



    private static void AddWall(GameObject source, Material wallMaterial, string name, float len, Vector3 pos, Quaternion rotation)
    {
        GameObject wall = GameObject.CreatePrimitive(PrimitiveType.Quad);
        wall.name = name;
        wall.transform.SetParent(source.transform);
        wall.transform.localPosition = pos;
        wall.transform.localScale = new Vector3(len, GRID_SIZE, 1f);
        wall.transform.localRotation = rotation;
        wall.GetComponent<MeshRenderer>().material = wallMaterial;
    }

}