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

    private List<GameObject> IsRowBomb(Cell cell1, Cell cell2, Cell cell3)
    {
        List<GameObject> currentCells = new List<GameObject>();
        if (cell1.isRowBomb)
        {
            currentMatches.Union(GetRowPieces(cell1.row));
        }
                                
        if (cell2.isRowBomb)
        {
            currentMatches.Union(GetRowPieces(cell2.row));
        }
                                
        if (cell3.isRowBomb)
        {
            currentMatches.Union(GetRowPieces(cell3.row));
        }

        return currentCells;
    }
    
    private List<GameObject> IsColumnBomb(Cell cell1, Cell cell2, Cell cell3)
    {
        List<GameObject> currentCells = new List<GameObject>();
        if (cell1.isColumnBomb)
        {
            currentMatches.Union(GetColumnPieces(cell1.column));
        }
                                
        if (cell2.isColumnBomb)
        {
            currentMatches.Union(GetColumnPieces(cell2.column));
        }
                                
        if (cell3.isColumnBomb)
        {
            currentMatches.Union(GetColumnPieces(cell3.column));
        }

        return currentCells;
    }

    private void AddToListAndMatch(GameObject cell)
    {
        if (!currentMatches.Contains(cell))
        {
            currentMatches.Add(cell);
        }
        cell.GetComponent<Cell>().isMatched = true;
    }

    private void GetNearbyPieces(GameObject cell1, GameObject cell2, GameObject cell3)
    {
        AddToListAndMatch(cell1);
        
        AddToListAndMatch(cell2);
        
        AddToListAndMatch(cell3);
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
                    Cell currentCellCell = currentCell.GetComponent<Cell>();
                    if (i > 0 && i < _board.width - 1)
                    {
                        GameObject leftCell = _board.allCells[i - 1, j];
                        GameObject rightCell = _board.allCells[i + 1, j];
                        if (leftCell != null && rightCell != null)
                        {
                            Cell rightCellCell = rightCell.GetComponent<Cell>();
                            Cell leftCellCell = leftCell.GetComponent<Cell>();
                            if (leftCell != null && rightCell != null)
                            {
                                if (leftCell.tag == currentCell.tag && rightCell.tag == currentCell.tag)
                                {
                                    currentMatches.Union(IsRowBomb(leftCellCell,currentCellCell,rightCellCell));

                                    currentMatches.Union(IsColumnBomb(leftCellCell, currentCellCell, rightCellCell));
                                
                                    GetNearbyPieces(leftCell,currentCell,rightCell);
                                }
                            }  
                        }
                    }
                    if (j > 0 && j < _board.height - 1)
                    {
                        GameObject upCell = _board.allCells[i , j +1];
                        GameObject downCell = _board.allCells[i, j -1];
                        if (upCell != null && downCell != null)
                        {
                            Cell downCellCell = downCell.GetComponent<Cell>();
                            Cell upCellCell = upCell.GetComponent<Cell>();
                            if (upCell != null && downCell != null)
                            {
                                if (upCell.tag == currentCell.tag && downCell.tag == currentCell.tag)
                                {
                                    currentMatches.Union(IsColumnBomb(upCellCell, currentCellCell,downCellCell));

                                    currentMatches.Union(IsRowBomb(upCellCell, currentCellCell,downCellCell));
                                
                                    GetNearbyPieces(upCell,currentCell,downCell);
                                }
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
