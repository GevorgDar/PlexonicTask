using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WinningInfo : MonoBehaviour {

    public Sprite Figure;
    public Text CountText;
    public Image FigureImg;

    void Start()
    {
        FigureImg.sprite = Figure;    
    }
}
