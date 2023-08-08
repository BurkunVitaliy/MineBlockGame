using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public enum GameState
{
    wait,
    move
}

public class Board : MonoBehaviour
{
    public GameState currentState = GameState.move;
    public int width, height, offSet;
    public GameObject tilePrefabs, destroyEffect;
    public GameObject[,] allCells;
    public GameObject[] cells;
    private BackgroundTile[,] _allTiles;
    public Cell currentCell;
    private FindMatches _findMatches;
    

    private void Start()
    {
        _findMatches = FindObjectOfType<FindMatches>();
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
                Vector2 tempPosition = new Vector2(i, j + offSet);
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
                cell.GetComponent<Cell>().row = j;
                cell.GetComponent<Cell>().column = i;
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
            //How many elements are in the matched pieces list from findmatches?
            if (_findMatches.currentMatches.Count == 4 || _findMatches.currentMatches.Count == 7)
            {
                _findMatches.CheckBombs();
            }
            GameObject particle = Instantiate(destroyEffect,allCells[column,row].transform.position, Quaternion.identity);
            Destroy(particle, 0.5f);
            _findMatches.currentMatches.Remove(allCells[column, row]);
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
        StartCoroutine(DecreasRowCo());
    }

    private IEnumerator DecreasRowCo()
    {
        int nullCount = 0;
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allCells[i,j] == null)
                {
                    nullCount++;
                }else if (nullCount > 0)
                {
                    allCells[i, j].GetComponent<Cell>().row -= nullCount;
                    allCells[i, j] = null;
                }
            }
            nullCount = 0;
        }
        yield return new WaitForSeconds(0.4f);
        StartCoroutine(FillBoardCo());
    }

    private void RefillBoard()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allCells[i,j] == null)
                {
                    Vector2 tempPosition = new Vector2(i, j + offSet);
                    int cellToUse = Random.Range(0, cells.Length);
                    GameObject piece = Instantiate(cells[cellToUse], tempPosition, Quaternion.identity);
                    allCells[i, j] = piece;
                    piece.GetComponent<Cell>().row = j;
                    piece.GetComponent<Cell>().column = i;
                }
            }
        }
    }

    private bool MatchesOnBoard()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allCells[i,j] != null)
                {
                    if (allCells[i,j].GetComponent<Cell>().isMatched)
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    private IEnumerator FillBoardCo()
    {
        RefillBoard();
        yield return new WaitForSeconds(0.5f);

        while (MatchesOnBoard())
        {
            yield return new WaitForSeconds(0.5f);
            DestroyMatches();
        }
        _findMatches.currentMatches.Clear();
        currentCell = null;
        yield return new WaitForSeconds(0.5f);
        currentState = GameState.move;
    }
}
