using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class BoardInfo
{
    public static string FirstFigur = "0";
    public static string SecondFigure = "0";
    //Check
    public static List<List<int>> FigureInBoard = new List<List<int>>();
    public static List<GameObject> FiguresForWin = new List<GameObject>();

    public static Dictionary<string, int> BombInBoard = new Dictionary<string, int>();
    public static string BombName = "";

    public static bool FigureClicked = true;
}