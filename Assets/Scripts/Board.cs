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
                int maxIterations = 0;

                while (MatchesAt(i,j, cells[cellToUse]) && maxIterations < 100)
                {
                    cellToUse = Random.Range(0, cells.Length);
                    maxIterations++;
                    Debug.Log(maxIterations);
                }

                maxIterations = 0;
                GameObject cell = Instantiate(cells[cellToUse], tempPosition, Quaternion.identity);
                cell.transform.parent = transform;
                cell.name = "(" + i + ", " + j + " )";
                allCells[i, j] = cell;
            }
        }
    }

    private bool MatchesAt(int column, int row, GameObject piece)
    {
        if (column > 1 && row > 1)
        {
            if (allCells[column -1,row].tag == piece.tag && allCells[column -2, row])
            {
                return true;
            }
            if (allCells[column ,row-1].tag == piece.tag && allCells[column, row -2])
            {
                return true;
            }
        }
        else if (column <= 1  || row <= 1)
        {
            if (row > 1)
            {
                if (allCells[column, row -1].tag == piece.tag && allCells[column, row -2 ].tag == piece.tag)
                {
                    return true;
                }
            }
            if (column > 1)
            {
                if (allCells[column-1, row ].tag == piece.tag && allCells[column -2 , row].tag == piece.tag)
                {
                    return true;
                }
            }
        }
        
        return false;
    }

    private void DestroyMatchesAt(int column , int row)
    {
        if (allCells[column, row].GetComponent<Cell>().isMatched)
        {
            Destroy(allCells[column,row]);
            allCells[column, row] = null;
        }
    }

    public void DestroyMatches()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allCells[i,j] != null)
                {
                    DestroyMatchesAt(i,j);
                }
            }
        }
    }
}
