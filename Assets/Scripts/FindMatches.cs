using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

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
                                if (currentCell.GetComponent<Cell>().isRowBomb 
                                    || leftCell.GetComponent<Cell>().isRowBomb
                                    || rightCell.GetComponent<Cell>().isRowBomb)
                                {
                                    currentMatches.Union(GetRowPieces(j));
                                }

                                if (currentCell.GetComponent<Cell>().isColumnBomb)
                                {
                                    currentMatches.Union(GetColumnPieces(i));
                                }

                                if (leftCell.GetComponent<Cell>().isColumnBomb)
                                {
                                    currentMatches.Union(GetColumnPieces(i - 1));
                                }
                                
                                if (rightCell.GetComponent<Cell>().isColumnBomb)
                                {
                                    currentMatches.Union(GetColumnPieces(i + 1));
                                }
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
                                if (currentCell.GetComponent<Cell>().isColumnBomb 
                                    || upCell.GetComponent<Cell>().isColumnBomb
                                    || downCell.GetComponent<Cell>().isColumnBomb)
                                {
                                    currentMatches.Union(GetColumnPieces(i));
                                }
                                
                                if (currentCell.GetComponent<Cell>().isRowBomb)
                                {
                                    currentMatches.Union(GetRowPieces(j));
                                }
                                
                                if (upCell.GetComponent<Cell>().isRowBomb)
                                {
                                    currentMatches.Union(GetRowPieces(j + 1));
                                }
                                
                                if (downCell.GetComponent<Cell>().isRowBomb)
                                {
                                    currentMatches.Union(GetRowPieces(j - 1));
                                }
                                
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

    public void MatchPiecesOfColor(string color)
    {
        for (int i = 0; i < _board.width; i++)
        {
            for (int j = 0; j < _board.height; j++)
            {
                //Check if that piece exists
                if (_board.allCells[i,j] != null)
                {
                    //Check the tag on that cell
                    if (_board.allCells[i,j].tag == color)
                    {
                        //Set that cell to eb matched
                        _board.allCells[i, j].GetComponent<Cell>().isMatched = true;
                    }
                }
            }
        }
    }
    

    private List<GameObject> GetColumnPieces(int column)
    {
        List<GameObject> cells = new List<GameObject>();
        for (int i = 0; i < _board.height; i++)
        {
            if (_board.allCells[column,i] != null)
            {
                cells.Add(_board.allCells[column, i]);
                _board.allCells[column, i].GetComponent<Cell>().isMatched = true;
            }
        }
        return cells;
    }
    
    private List<GameObject> GetRowPieces(int row)
    {
        List<GameObject> cells = new List<GameObject>();
        for (int i = 0; i < _board.width; i++)
        {
            if (_board.allCells[i, row] != null)
            {
                cells.Add(_board.allCells[i, row]);
                _board.allCells[i, row].GetComponent<Cell>().isMatched = true;
            }
        }
        return cells;
    }

    public void CheckBombs()
    {
        //Did the player move something?
        if (_board.currentCell != null)
        {
            //Is the piece they moved matched?
            if (_board.currentCell.isMatched)
            {
                //make it unmatched
                _board.currentCell.isMatched = false;
                //Decide what kind of bomb to make
                
                /*int typeOfBomb = Random.Range(0, 100);
                if (typeOfBomb < 50)
                {
                    //Make a row bomb
                    _board.currentCell.MakeRowBomb();
                }else if (typeOfBomb >= 50)
                {
                    //Make a column bomb
                    _board.currentCell.MakeColumnBomb();
                }*/
                if ((_board.currentCell.swipeAngle > -45 && _board.currentCell.swipeAngle <= 45)
                    || (_board.currentCell.swipeAngle < -135 || _board.currentCell.swipeAngle >= 135))
                {
                    _board.currentCell.MakeRowBomb();
                }
                else
                {
                    _board.currentCell.MakeColumnBomb();
                }
                
                
                
            }
            //Is the other piece matched?
            else if (_board.currentCell.otherCell != null)
            {
                Cell otherCell = _board.currentCell.otherCell.GetComponent<Cell>();
                //Is the other Cell matched?
                if (otherCell.isMatched)
                {
                  //Make it unmatched   
                  otherCell.isMatched = false;
                  //Decide what kind of bomb to make
                  /*int typeOfBomb = Random.Range(0, 100);
                  if (typeOfBomb < 50)
                  {
                      //Make a row bomb
                      otherCell.MakeRowBomb();
                  }else if (typeOfBomb >= 50)
                  {
                      //Make a column bomb
                      otherCell.MakeColumnBomb();
                  }*/
                  if ((_board.currentCell.swipeAngle > -45 && _board.currentCell.swipeAngle <= 45)
                      || (_board.currentCell.swipeAngle < -135 || _board.currentCell.swipeAngle >= 135))
                  {
                      otherCell.MakeRowBomb();
                  }
                  else
                  {
                      otherCell.MakeColumnBomb();
                  }
                }
            }
        }
    }
}
