using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MazeSpawner : MonoBehaviour
{
    [SerializeField] private Material groundMaterial;
    [SerializeField] private Material ceilingMaterial;
    [SerializeField] private Material wallMaterial;
    [SerializeField] private int seed;
    [SerializeField] private Maze maze;

    private void Awake()
    {
        if (!maze)
        {
            Spawn();
        }
    }

    [ContextMenu("Spawn")]
    private void Spawn()
    {
        if (maze)
        {
            Destroy(maze.gameObject);
        }
        maze = Maze.Generate(seed, wallMaterial, groundMaterial, ceilingMaterial, 20, 20);
    }
}
