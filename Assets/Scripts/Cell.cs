using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour
{
   [Header("Board Variables")]
   public float swipeAngle, swipeResist = 1f;
   public int column, row, targetX, targetY, previousColumn, previousRow;
   public bool isMatched;
   private Vector2 _firstTouchPosition, _finalTouchPosition, _tempPosition;
   private GameObject _otherCell;
   private Board _board;
   

   private void Start()
   {
      _board = FindObjectOfType<Board>();
      /*targetX = (int) transform.position.x;
      targetY = (int) transform.position.y;
      row = targetY;
      column = targetX;
      previousRow = row;
      previousColumn = column;*/
   }

   private void Update()
   {
      FindMatches();
      if (isMatched)
      {
         SpriteRenderer mySprite = GetComponent<SpriteRenderer>();
         mySprite.color = new Color(1f, 1f, 1, 0.2f);
      }
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
      }
      else
      {
         _tempPosition = new Vector2(transform.position.x, targetY);
         transform.position = _tempPosition;
      }
   }

   public IEnumerator CheckMoveCo()
   {
      yield return new WaitForSeconds(0.3f);
      if (_otherCell != null)
      {
         if (!isMatched && !_otherCell.GetComponent<Cell>().isMatched)
         {
            _otherCell.GetComponent<Cell>().row = row;
            _otherCell.GetComponent<Cell>().column = column;
            row = previousRow;
            column = previousColumn;
            yield return new WaitForSeconds(0.5f);
            _board.currentState = GameState.move;
         }
         else
         {
            _board.DestroyMatches();
         }

         _otherCell = null;
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
         _otherCell = _board.allCells[column + 1, row];
         previousRow = row;
         previousColumn = column;
         _otherCell.GetComponent<Cell>().column -= 1;
         column += 1;
      }else if (swipeAngle >  45 && swipeAngle <= 135 && row < _board.height -1)
      {
         //Up Swipe
         _otherCell = _board.allCells[column , row + 1];
         previousRow = row;
         previousColumn = column;
         _otherCell.GetComponent<Cell>().row -= 1;
         row += 1;
      }else if ((swipeAngle > 135 || swipeAngle <= -135) && column > 0)
      {
         //Left Swipe
         _otherCell = _board.allCells[column - 1, row];
         previousRow = row;
         previousColumn = column;
         _otherCell.GetComponent<Cell>().column += 1;
         column -= 1;
      }else if (swipeAngle < - 45 && swipeAngle >= -135 && row > 0)
      {
         //Down Swipe
         _otherCell = _board.allCells[column, row - 1];
         previousRow = row;
         previousColumn = column;
         _otherCell.GetComponent<Cell>().row += 1;
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
   
}
