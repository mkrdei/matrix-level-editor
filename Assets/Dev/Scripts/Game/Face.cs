using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Face : MonoBehaviour
{
    public TextMeshPro letterText;
    public enum FaceType
    {
        front,
        back,
        right,
        left
    }
    public FaceType faceType;
    public TileType tileType;
    // Start is called before the first frame update
    void Start()
    {
        letterText.text = tileType.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
