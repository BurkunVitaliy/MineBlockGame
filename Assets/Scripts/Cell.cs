using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour
{
   
   public float swipeAngle = 0;
   public int column, row, targetX, targetY;
   public bool isMatched;
   private Vector2 _firstTouchPosition, _finalTouchPosition, tempPosition;
   private GameObject _otherCell;
   private Board _board;
   

   private void Start()
   {
      _board = FindObjectOfType<Board>();
      targetX = (int) transform.position.x;
      targetY = (int) transform.position.y;
      row = targetY;
      column = targetX;
   }

   private void Update()
   {
      FindMatches();
      if (isMatched)
      {
         SpriteRenderer mySprite = GetComponent<SpriteRenderer>();
         mySprite.color = new Color(1f, 1f, 1, .2f);
      }
      targetX = column;
      targetY = row;
      if (Mathf.Abs(targetX - transform.position.x)> .1)
      {
         tempPosition = new Vector2(targetX, transform.position.y);
         transform.position = Vector2.Lerp(transform.position, tempPosition, .4f);
      }
      else
      {
         tempPosition = new Vector2(targetX, transform.position.y);
         transform.position = tempPosition;
         _board.allCells[column, row] = gameObject;
      }
      if (Mathf.Abs(targetY - transform.position.y)> .1)
      {
         tempPosition = new Vector2(transform.position.x, targetY);
         transform.position = Vector2.Lerp(transform.position, tempPosition, .4f);
      }
      else
      {
         tempPosition = new Vector2(transform.position.x, targetY);
         transform.position = tempPosition;
         _board.allCells[column, row] = gameObject;
      }
   }


   private void OnMouseDown()
   {
      _firstTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
   }

   private void OnMouseUp()
   {
      _finalTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
      CalculateAngle();
   }

   private void CalculateAngle()
   {
      swipeAngle = Mathf.Atan2(_finalTouchPosition.y - _firstTouchPosition.y,
         _finalTouchPosition.x - _firstTouchPosition.x) * 180 / Mathf.PI;
      Debug.Log(swipeAngle);
      MovePieces();
   }

   private void MovePieces()
   {
      if (swipeAngle > - 45 && swipeAngle <= 45 && column < _board.width)
      {
         //Right Swipe
         _otherCell = _board.allCells[column + 1, row];
         _otherCell.GetComponent<Cell>().column -= 1;
         column += 1;
      }else if (swipeAngle >  45 && swipeAngle <= 135 && row < _board.height)
      {
         //Up Swipe
         _otherCell = _board.allCells[column , row + 1];
         _otherCell.GetComponent<Cell>().row -= 1;
         row += 1;
      }else if ((swipeAngle > 135 || swipeAngle <= -135) && column > 0)
      {
         //Left Swipe
         _otherCell = _board.allCells[column - 1, row];
         _otherCell.GetComponent<Cell>().column += 1;
         column -= 1;
      }else if (swipeAngle < - 45 && swipeAngle >= -135 && row > 0)
      {
         //Down Swipe
         _otherCell = _board.allCells[column, row - 1];
         _otherCell.GetComponent<Cell>().row += 1;
         row -= 1;
      }
   }

   private void FindMatches()
   {
      if (column > 0 && column < _board.width -1)
      {
         GameObject leftCell1 = _board.allCells[column - 1, row];
         GameObject rightCell1 = _board.allCells[column + 1, row];
         if (leftCell1. tag == gameObject.tag && rightCell1.tag == gameObject.tag)
         {
            leftCell1.GetComponent<Cell>().isMatched = true;
            rightCell1.GetComponent<Cell>().isMatched = true;
            isMatched = true;
         }
      }
      if (row > 0 && row < _board.height -1)
      {
         GameObject upCell1 = _board.allCells[column, row + 1];
         GameObject downCell1 = _board.allCells[column, row - 1];
         if (upCell1. tag == gameObject.tag && downCell1.tag == gameObject.tag)
         {
            upCell1.GetComponent<Cell>().isMatched = true;
            downCell1.GetComponent<Cell>().isMatched = true;
            isMatched = true;
         }
      }
   }
   
}
