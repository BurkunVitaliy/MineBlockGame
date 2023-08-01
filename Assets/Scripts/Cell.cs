using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour
{
   
   private Vector2 _firstTouchPosition, _finalTouchPosition;
   public float swipeAngle = 0;
   public int column, row;


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
         _finalTouchPosition.x - _firstTouchPosition.x * 180 / Mathf.PI);
      Debug.Log(swipeAngle);
   }

   private void MovePieces()
   {
      if (swipeAngle > - 45 && swipeAngle <= 45)
      {
         
      }
   }
   
}
