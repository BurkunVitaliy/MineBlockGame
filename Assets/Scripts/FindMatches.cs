using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FindMatches : MonoBehaviour
{
    private Board _board;
    public List<GameObject> currentMatches = new List<GameObject>();

    private void Start()
    {
        _board = FindObjectOfType<Board>();
    }

    public void FindAllMatches()
    {
        StartCoroutine(FindAllMatchesCo());
    }

    private IEnumerator FindAllMatchesCo()
    {
        yield return new WaitForSeconds(0.2f);
        for (int i = 0; i < _board.width; i++)
        {
            for (int j = 0; j < _board.height; j++)
            {
                GameObject currentCell = _board.allCells[i, j];
                if (currentCell != null)
                {
                    if (i > 0 && i < _board.width - 1)
                    {
                        GameObject leftCell = _board.allCells[i - 1, j];
                        GameObject rightCell = _board.allCells[i + 1, j];
                        if (leftCell != null && rightCell != null)
                        {
                            if (leftCell.tag == currentCell.tag && rightCell.tag == currentCell.tag)
                            {
                                if (!currentMatches.Contains(leftCell))
                                {
                                    currentMatches.Add(leftCell);
                                }
                                leftCell.GetComponent<Cell>().isMatched = true;
                                if (!currentMatches.Contains(rightCell))
                                {
                                    currentMatches.Add(rightCell);
                                }
                                rightCell.GetComponent<Cell>().isMatched = true;
                                if (!currentMatches.Contains(currentCell))
                                {
                                    currentMatches.Add(currentCell);
                                }
                                currentCell.GetComponent<Cell>().isMatched = true;
                            }
                        }
                    }
                    if (j > 0 && j < _board.height - 1)
                    {
                        GameObject upCell = _board.allCells[i , j +1];
                        GameObject downCell = _board.allCells[i, j -1];
                        if (upCell != null && downCell != null)
                        {
                            if (upCell.tag == currentCell.tag && downCell.tag == currentCell.tag)
                            {
                                if (!currentMatches.Contains(upCell))
                                {
                                    currentMatches.Add(upCell);
                                }
                                upCell.GetComponent<Cell>().isMatched = true;
                                if (!currentMatches.Contains(downCell))
                                {
                                    currentMatches.Add(downCell);
                                }
                                downCell.GetComponent<Cell>().isMatched = true;
                                if (!currentMatches.Contains(currentCell))
                                {
                                    currentMatches.Add(currentCell);
                                }
                                currentCell.GetComponent<Cell>().isMatched = true;
                            }
                        }
                    }
                }
            }
        }
    }
    
    
    
}
