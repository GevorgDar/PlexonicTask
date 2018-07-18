using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckCombo
{
    public List<GameObject> Check(List<GameObject> ListCheckFigure, int FigureCountByX, int FigureCountByY)
    {
        List<GameObject> FigureDelet = new List<GameObject>();


        for (int i = 0; i < ListCheckFigure.Count; i++)
        {
            int x = int.Parse(ListCheckFigure[i].name.Split('.')[0]);
            int y = int.Parse(ListCheckFigure[i].name.Split('.')[1]);

            if (BoardInfo.FigureInBoard[x][y] > 5)
            {
                continue;
            }

            List<GameObject> FigureSameTyp = new List<GameObject>();
            List<GameObject> FigureCheck = new List<GameObject>();

            FigureSameTyp.AddRange(FigureDelet);
            FigureSameTyp.Add(ListCheckFigure[i]);

            FigureCheck.AddRange(CheckHorizontalCombo(x, y, FigureCountByX, FigureSameTyp));
            FigureCheck.AddRange(CheckVerticalCombo(x, y, FigureCountByY, FigureSameTyp));
            FigureCheck.AddRange(CheckCubCombo(x, y, FigureCountByX, FigureCountByY, FigureSameTyp));

            if (FigureCheck.Count > 2)
            {
                FigureDelet.AddRange(FigureCheck);
            }

        }
        return FigureDelet;

    }

    List<GameObject> CheckHorizontalCombo(int x, int y, int FigureCountByX, List<GameObject> FigureSameTyp)
    {
        List<GameObject> NewFigureSameTyp = new List<GameObject>();
        List<GameObject> FigureSameTypHorizontal = new List<GameObject>();
        FigureSameTypHorizontal.Add(GameObject.Find(x + "." + y));

        int myFragmentNumber = BoardInfo.FigureInBoard[x][y];

        for (int X = x; X > 0; X--)
        {
            if (BoardInfo.FigureInBoard[X - 1][y] == myFragmentNumber && CheckNameInList(FigureSameTyp, (X - 1) + "." + y) == false)
            {
                GameObject figure = GameObject.Find((X - 1) + "." + y);
                FigureSameTypHorizontal.Add(figure);
            }
            else
            {
                break;
            }

        }
        for (int X = x; X < FigureCountByX - 1; X++)
        {
            if (BoardInfo.FigureInBoard[X + 1][y] == myFragmentNumber && CheckNameInList(FigureSameTyp, (X + 1) + "." + y) == false)
            {
                GameObject figure = GameObject.Find((X + 1) + "." + y);
                FigureSameTypHorizontal.Add(figure);
            }
            else
            {
                break;
            }

        }
        if (FigureSameTypHorizontal.Count > 2)
        {
            if (FigureSameTypHorizontal.Count > 3)
            {
                if (!CheckNameInDictionary(BoardInfo.BombInBoard, x + "." + y))
                {
                    BoardInfo.BombInBoard.Add(x + "." + y, 7);
                }
            }
            NewFigureSameTyp.AddRange(FigureSameTypHorizontal);
        }

        return NewFigureSameTyp;
    }
    List<GameObject> CheckVerticalCombo(int x, int y, int FigureCountByY, List<GameObject> FigureSameTyp)
    {
        List<GameObject> NewFigureSameTyp = new List<GameObject>();
        List<GameObject> FigureSameTypVertical = new List<GameObject>();
        FigureSameTypVertical.Add(GameObject.Find(x + "." + y));

        int myFragmentNumber = BoardInfo.FigureInBoard[x][y];

        for (int Y = y; Y > 0; Y--)
        {
            if (BoardInfo.FigureInBoard[x][Y - 1] == myFragmentNumber && CheckNameInList(FigureSameTyp, x + "." + (Y - 1)) == false)
            {
                GameObject figure = GameObject.Find(x + "." + (Y - 1));
                FigureSameTypVertical.Add(figure);
            }
            else
            {
                break;
            }

        }
        for (int Y = y; Y < FigureCountByY - 1; Y++)
        {
            if (BoardInfo.FigureInBoard[x][Y + 1] == myFragmentNumber && CheckNameInList(FigureSameTyp, x + "." + (Y + 1)) == false)
            {
                GameObject figure = GameObject.Find(x + "." + (Y + 1));
                FigureSameTypVertical.Add(figure);
            }
            else
            {
                break;
            }

        }
        if (FigureSameTypVertical.Count > 2)
        {
            if (FigureSameTypVertical.Count > 3)
            {
                if (!CheckNameInDictionary(BoardInfo.BombInBoard, x + "." + y))
                {
                    BoardInfo.BombInBoard.Add(x + "." + y, 8);
                }
            }
            NewFigureSameTyp.AddRange(FigureSameTypVertical);
        }

        return NewFigureSameTyp;
    }
    List<GameObject> CheckCubCombo(int x, int y, int FigureCountByX, int FigureCountByY, List<GameObject> FigureSameTyp)
    {
        List<GameObject> newFigureSameTyp = new List<GameObject>();
        List<GameObject> NewFigureSameTyp = new List<GameObject>();
        NewFigureSameTyp.Add(GameObject.Find(x + "." + y));
        // x x o
        // x x o
        // o o o 
        if (x > 0 && y < FigureCountByY - 1)
        {
            if (BoardInfo.FigureInBoard[x - 1][y] == BoardInfo.FigureInBoard[x][y] && 
                BoardInfo.FigureInBoard[x - 1][y + 1] == BoardInfo.FigureInBoard[x][y] && 
                BoardInfo.FigureInBoard[x][y + 1] == BoardInfo.FigureInBoard[x][y])
            {
                if (CheckNameInList(FigureSameTyp, (x - 1) + "." + y) == false)
                {
                    GameObject figure = GameObject.Find((x - 1) + "." + y);
                    NewFigureSameTyp.Add(figure);
                }
                if (CheckNameInList(FigureSameTyp, (x - 1) + "." + (y + 1)) == false)
                {
                    GameObject figure = GameObject.Find((x - 1) + "." + (y + 1));
                    NewFigureSameTyp.Add(figure);
                }
                if (CheckNameInList(FigureSameTyp, x + "." + (y + 1)) == false)
                {
                    GameObject figure = GameObject.Find(x + "." + (y + 1));
                    NewFigureSameTyp.Add(figure);
                }
            }
        }

        // o o o
        // o x x
        // o x x 
        if (y > 0 && x < FigureCountByX - 1)
        {
            if (BoardInfo.FigureInBoard[x + 1][y] == BoardInfo.FigureInBoard[x][y] && 
                BoardInfo.FigureInBoard[x + 1][y - 1] == BoardInfo.FigureInBoard[x][y] && 
                BoardInfo.FigureInBoard[x][y - 1] == BoardInfo.FigureInBoard[x][y])
            {
                if (CheckNameInList(FigureSameTyp, (x + 1) + "." + y) == false)
                {
                    GameObject figure = GameObject.Find((x + 1) + "." + y);
                    NewFigureSameTyp.Add(figure);
                }
                if (CheckNameInList(FigureSameTyp, (x + 1) + "." + (y - 1)) == false)
                {
                    GameObject figure = GameObject.Find((x + 1) + "." + (y - 1));
                    NewFigureSameTyp.Add(figure);
                }
                if (CheckNameInList(FigureSameTyp, x + "." + (y - 1)) == false)
                {
                    GameObject figure = GameObject.Find(x + "." + (y - 1));
                    NewFigureSameTyp.Add(figure);
                }
            }
        }

        // o o o
        // x x o
        // x x o
        if (x > 0 && y > 0)
        {
            if (BoardInfo.FigureInBoard[x - 1][y] == BoardInfo.FigureInBoard[x][y] &&
                BoardInfo.FigureInBoard[x - 1][y - 1] == BoardInfo.FigureInBoard[x][y] && 
                BoardInfo.FigureInBoard[x][y - 1] == BoardInfo.FigureInBoard[x][y])
            {
                if (CheckNameInList(FigureSameTyp, (x - 1) + "." + y) == false)
                {
                    GameObject figure = GameObject.Find((x - 1) + "." + y);
                    NewFigureSameTyp.Add(figure);
                }
                if (CheckNameInList(FigureSameTyp, (x - 1) + "." + (y - 1)) == false)
                {
                    GameObject figure = GameObject.Find((x - 1) + "." + (y - 1));
                    NewFigureSameTyp.Add(figure);
                }
                if (CheckNameInList(FigureSameTyp, x + "." + (y - 1)) == false)
                {
                    GameObject figure = GameObject.Find(x + "." + (y - 1));
                    NewFigureSameTyp.Add(figure);
                }
            }
        }

        // o x x
        // o x x
        // o o o
        if (x < FigureCountByX - 1 && y < FigureCountByY - 1)
        {
            if (BoardInfo.FigureInBoard[x][y + 1] == BoardInfo.FigureInBoard[x][y] &&
                BoardInfo.FigureInBoard[x + 1][y + 1] == BoardInfo.FigureInBoard[x][y] &&
                BoardInfo.FigureInBoard[x + 1][y] == BoardInfo.FigureInBoard[x][y])
            {
                if (CheckNameInList(FigureSameTyp, x + "." + (y + 1)) == false)
                {
                    GameObject figure = GameObject.Find(x + "." + (y + 1));
                    NewFigureSameTyp.Add(figure);
                }
                if (CheckNameInList(FigureSameTyp, (x + 1) + "." + (y + 1)) == false)
                {
                    GameObject figure = GameObject.Find((x + 1) + "." + (y + 1));
                    NewFigureSameTyp.Add(figure);
                }
                if (CheckNameInList(FigureSameTyp, (x + 1) + "." + y) == false)
                {
                    GameObject figure = GameObject.Find((x + 1) + "." + y);
                    NewFigureSameTyp.Add(figure);
                }
            }
        }
        if (NewFigureSameTyp.Count > 3)
        {
            if (!CheckNameInDictionary(BoardInfo.BombInBoard, x + "." + y))
            {
                BoardInfo.BombInBoard.Add(x + "." + y, 6);
            }
            newFigureSameTyp.AddRange(NewFigureSameTyp);
        }
        return newFigureSameTyp;
    }
    
    bool CheckNameInDictionary(Dictionary<string,int> figureList, string name)
    {
        foreach (var item in figureList)
        {
            if (item.Key == name)
            {
                return true;
            }
        }
        return false;
    }
    bool CheckNameInList(List<GameObject> figureList, string name)
    {
        foreach (var item in figureList)
        {
            if (item.name == name)
            {
                return true;
            }
        }
        return false;
    }
}