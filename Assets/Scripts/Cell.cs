using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour
{
   [Header("Board Variables")]
   public int column;
   public int row;
   public int targetX;
   public int targetY;
   public int previousColumn;
   public int previousRow;
   public bool isMatched;
   
   public GameObject otherCell;
   private Vector2 _firstTouchPosition, _finalTouchPosition, _tempPosition;
   private Board _board;
   private FindMatches _findMatches;
   
   [Header("Swipe Stuff")]
   public float swipeAngle;
   public float swipeResist = 1f;

   [Header("PowerUp Stuff")] 
   public bool isColorBomb;
   public bool isColumnBomb;
   public bool isRowBomb;
   public bool isBomb;
   public GameObject bomb;
   public GameObject rowArrow;
   public GameObject columnArrow;
   public GameObject colorBomb;

   private void Start()
   {

      isColumnBomb = false;
      isRowBomb = false;
      isColorBomb = false;
      isBomb = false;
      
      _board = FindObjectOfType<Board>();
      _findMatches = FindObjectOfType<FindMatches>();
      /*targetX = (int) transform.position.x;
      targetY = (int) transform.position.y;
      row = targetY;
      column = targetX;
      previousRow = row;
      previousColumn = column;*/
   }
   
   //This is for testing and Debug only.
   private void OnMouseOver()
   {
      if (Input.GetMouseButtonDown(1))
      {
         isBomb = true;
         GameObject color = Instantiate(bomb, transform.position, Quaternion.identity);
         color.transform.parent = transform;
      }
   }

   private void Update()
   {
      /*if (isMatched)
      {
         SpriteRenderer mySprite = GetComponent<SpriteRenderer>();
         mySprite.color = new Color(1f, 1f, 1, 0.2f);
      }*/
      targetX = column;
      targetY = row;
      if (Mathf.Abs(targetX - transform.position.x)> 0.1)
      {
         _tempPosition = new Vector2(targetX, transform.position.y);
         transform.position = Vector2.Lerp(transform.position, _tempPosition, 0.4f);
         if (_board.allCells[column, row] != gameObject)
         {
            _board.allCells[column, row] = gameObject;
         }
         _findMatches.FindAllMatches();
      }
      else
      {
         _tempPosition = new Vector2(targetX, transform.position.y);
         transform.position = _tempPosition;
      }
      if (Mathf.Abs(targetY - transform.position.y)> 0.1)
      {
         _tempPosition = new Vector2(transform.position.x, targetY);
         transform.position = Vector2.Lerp(transform.position, _tempPosition, 0.4f);
         if (_board.allCells[column, row] != gameObject)
         {
            _board.allCells[column, row] = gameObject;
         }
         _findMatches.FindAllMatches();

      }
      else
      {
         _tempPosition = new Vector2(transform.position.x, targetY);
         transform.position = _tempPosition;
      }
   }

   public IEnumerator CheckMoveCo()
   {
      if (isColorBomb)
      {
         //This piece is a color bomb, and the other piece is the color to destroy
         _findMatches.MatchPiecesOfColor(otherCell.tag);
         isMatched = true;
      }else if (otherCell.GetComponent<Cell>().isColorBomb)
      {
         //the other piece is a color bomb, and this piece has the color to destroy
         _findMatches.MatchPiecesOfColor(this.gameObject.tag);
         otherCell.GetComponent<Cell>().isMatched = true;
      }
      yield return new WaitForSeconds(0.5f);
      if (otherCell != null)
      {
         if (!isMatched && !otherCell.GetComponent<Cell>().isMatched)
         {
            otherCell.GetComponent<Cell>().row = row;
            otherCell.GetComponent<Cell>().column = column;
            row = previousRow;
            column = previousColumn;
            yield return new WaitForSeconds(0.5f);
            _board.currentCell = null;
            _board.currentState = GameState.move;
         }
         else
         {
            _board.DestroyMatches();
         }
         //_otherCell = null;
      }
   }


   private void OnMouseDown()
   {
      if (_board.currentState == GameState.move)
      {
         _firstTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
      }
   }

   private void OnMouseUp()
   {
      if (_board.currentState == GameState.move)
      {
         _finalTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
         CalculateAngle();
      }
   }

   private void CalculateAngle()
   {
      if (Mathf.Abs(_finalTouchPosition.y - _firstTouchPosition.y) > swipeResist || Mathf.Abs(_finalTouchPosition.x - _firstTouchPosition.x) > swipeResist)
      {
         swipeAngle = Mathf.Atan2(_finalTouchPosition.y - _firstTouchPosition.y,
            _finalTouchPosition.x - _firstTouchPosition.x) * 180 / Mathf.PI;
         MovePieces();
         _board.currentState = GameState.wait;
         _board.currentCell = this;
      }
      else
      {
         _board.currentState = GameState.move;
      }
   }

   private void MovePieces()
   {
      if (swipeAngle > - 45 && swipeAngle <= 45 && column < _board.width -1)
      {
         //Right Swipe
         otherCell = _board.allCells[column + 1, row];
         previousRow = row;
         previousColumn = column;
         otherCell.GetComponent<Cell>().column -= 1;
         column += 1;
      }else if (swipeAngle >  45 && swipeAngle <= 135 && row < _board.height -1)
      {
         //Up Swipe
         otherCell = _board.allCells[column , row + 1];
         previousRow = row;
         previousColumn = column;
         otherCell.GetComponent<Cell>().row -= 1;
         row += 1;
      }else if ((swipeAngle > 135 || swipeAngle <= -135) && column > 0)
      {
         //Left Swipe
         otherCell = _board.allCells[column - 1, row];
         previousRow = row;
         previousColumn = column;
         otherCell.GetComponent<Cell>().column += 1;
         column -= 1;
      }else if (swipeAngle < - 45 && swipeAngle >= -135 && row > 0)
      {
         //Down Swipe
         otherCell = _board.allCells[column, row - 1];
         previousRow = row;
         previousColumn = column;
         otherCell.GetComponent<Cell>().row += 1;
         row -= 1;
      }

      StartCoroutine(CheckMoveCo());
   }

   private void FindMatches()
   {
      if (column > 0 && column < _board.width -1)
      {
         GameObject leftCell1 = _board.allCells[column - 1, row];
         GameObject rightCell1 = _board.allCells[column + 1, row];
         if (leftCell1 != null && rightCell1 != null)
         {
            if (leftCell1. tag == gameObject.tag && rightCell1.tag == gameObject.tag)
            {
               leftCell1.GetComponent<Cell>().isMatched = true;
               rightCell1.GetComponent<Cell>().isMatched = true;
               isMatched = true;
            }
         }
      }
      if (row > 0 && row < _board.height -1)
      {
         GameObject upCell1 = _board.allCells[column, row + 1];
         GameObject downCell1 = _board.allCells[column, row - 1];
         if (upCell1 != null && downCell1 != null)
         { 
            if (upCell1. tag == gameObject.tag && downCell1.tag == gameObject.tag)
            {
               upCell1.GetComponent<Cell>().isMatched = true;
               downCell1.GetComponent<Cell>().isMatched = true;
               isMatched = true;
            }
         }
      }
   }

   public void MakeRowBomb()
   {
      isRowBomb = true;
      GameObject arrow = Instantiate(rowArrow, transform.position, Quaternion.identity);
      arrow.transform.parent = transform;
   }

   public void MakeColumnBomb()
   {
      isColumnBomb = true;
      GameObject arrow = Instantiate(columnArrow, transform.position, Quaternion.identity);
      arrow.transform.parent = transform;
   }
}
