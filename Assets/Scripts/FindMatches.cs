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
    
    
    
    
    
}
