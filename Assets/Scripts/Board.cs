using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Board : MonoBehaviour
{
    public int width, height;
    public GameObject tilePrefabs;
    public GameObject[,] allCells;
    public GameObject[] cells;
    private BackgroundTile[,] _allTiles;
    

    private void Start()
    {
        _allTiles = new BackgroundTile[width, height];
        allCells = new GameObject[width, height];
        SetUp();
    }

    private void SetUp()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                Vector2 tempPosition = new Vector2(i, j);
                GameObject backgroundTile = Instantiate(tilePrefabs,tempPosition,Quaternion.identity);
                backgroundTile.transform.parent = transform;
                backgroundTile.name = "(" + i + ", " + j + " )";
                int cellToUse = Random.Range(0, cells.Length);
                GameObject cell = Instantiate(cells[cellToUse], tempPosition, Quaternion.identity);
                cell.transform.parent = transform;
                cell.name = "(" + i + ", " + j + " )";
                allCells[i, j] = cell;
            }
        }
    }
}
