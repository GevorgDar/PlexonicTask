using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreateLvl : MonoBehaviour
{

    public GameObject[] FigureObjects;
    public GameObject[] BombObjects;
    public GameObject[] WinOrLostView;
    public GameObject TiledObject1;
    public GameObject TiledObject2;

    [Range(7, 10)]
    public int HorizontalFigureCount;
    [Range(7, 10)]
    public int VerticalFigureCount;

    [Range(3, 5)]
    public int FigureCounts;
    List<int> FigurNumberForWin = new List<int>();
    bool figureMovedDown = false;
    bool disableInteraction = false;

    List<Vector2> figuresNextPositions;
    List<GameObject> figuresWhitchWillMove;

    bool swapTwoFigures = false;
    List<GameObject> figuresWhichWillSwap;
    List<Vector2> swapPositionsForFigures;

    void Start()
    {
        int emptyFieldCount = 3;

        bool isEmptyField = false;
        int[,] emptyFieldsPositions = new int[2, emptyFieldCount];

        for (int i = 0; i < emptyFieldCount; i++)
        {
            int x = UnityEngine.Random.Range((HorizontalFigureCount / 3) * i, (HorizontalFigureCount / 3) * (i + 1));
            int y = UnityEngine.Random.Range(1, VerticalFigureCount);

            if (x == 0)
            {
                x = 1;
            }
            if (i == 2 && emptyFieldsPositions[0, 1] - emptyFieldsPositions[0, 0] == 1)
            {
                x = UnityEngine.Random.Range(emptyFieldsPositions[0, 1] + 3, (HorizontalFigureCount / 3) * (i + 1));
            }
            emptyFieldsPositions[0, i] = x;
            emptyFieldsPositions[1, i] = y;

        }

        foreach (var item in BoardInfo.FiguresForWin)
        {
            for (int i = 0; i < FigureObjects.Length; i++)
            {
                if (item.GetComponent<WinningInfo>().Figure.name == FigureObjects[i].name)
                {
                    FigurNumberForWin.Add(i + 1);
                }
            }
        }
        BoardInfo.FirstFigur = "0";
        BoardInfo.SecondFigure = "0";

        for (int x = 0; x < HorizontalFigureCount; x++)
        {

            for (int y = 0; y < VerticalFigureCount; y++)
            {
                for (int i = 0; i < emptyFieldCount; i++)
                {
                    if (emptyFieldsPositions[0, i] == x && emptyFieldsPositions[1, i] == y)
                    {
                        isEmptyField = true;
                        break;
                    }
                }
                if (!isEmptyField)
                {
                    if ((x + y) % 2 == 0)
                    {
                        Instantiate(TiledObject1, new Vector2(x, y), Quaternion.identity);
                    }
                    else
                    {
                        Instantiate(TiledObject2, new Vector2(x, y), Quaternion.identity);
                    }
                }
                isEmptyField = false;

            }

        }
        gameObject.transform.position = new Vector3(HorizontalFigureCount / 2, VerticalFigureCount / 2, gameObject.transform.position.z);
        if (HorizontalFigureCount > 8 || VerticalFigureCount > 8)
        {
            gameObject.GetComponent<Camera>().orthographicSize = 6;
        }
        else
        {
            gameObject.GetComponent<Camera>().orthographicSize = 5;
        }
        for (int x = 0; x < HorizontalFigureCount; x++)
        {
            BoardInfo.FigureInBoard.Add(new List<int>());
            for (int y = 0; y < VerticalFigureCount; y++)
            {
                for (int i = 0; i < emptyFieldCount; i++)
                {
                    if (emptyFieldsPositions[0, i] == x && emptyFieldsPositions[1, i] == y)
                    {
                        isEmptyField = true;
                        BoardInfo.FigureInBoard[x].Add(-1);
                        break;
                    }
                }
                if (!isEmptyField)
                {
                    BoardInfo.FigureInBoard[x].Add(0);
                }
                isEmptyField = false;

            }

        }
        StartCoroutine(FigureDefaultSize(new List<GameObject>()));
    }
    void FixedUpdate()
    {
        if (disableInteraction)
        {
            BoardInfo.FigureInBoard.Clear();
            BoardInfo.FiguresForWin.Clear();
            BoardInfo.FirstFigur = "0";
            BoardInfo.SecondFigure = "0";
            BoardInfo.BombInBoard.Clear();
            BoardInfo.BombName = "";

            BoardInfo.FigureClicked = false;
        }
        if (BoardInfo.FirstFigur != "0" && BoardInfo.SecondFigure != "0")
        {

            int x1 = int.Parse(BoardInfo.FirstFigur.Split('.')[0]);
            int y1 = int.Parse(BoardInfo.FirstFigur.Split('.')[1]);
            int x2 = int.Parse(BoardInfo.SecondFigure.Split('.')[0]);
            int y2 = int.Parse(BoardInfo.SecondFigure.Split('.')[1]);


            if ((x1 != x2 || Mathf.Abs(y1 - y2) != 1) && (y1 != y2 || Mathf.Abs(x1 - x2) != 1))
            {
                GameObject selectedFigure = GameObject.Find("Selected(Clone)");
                selectedFigure.transform.position = new Vector2(x2, y2);

                BoardInfo.FirstFigur = BoardInfo.SecondFigure;
                BoardInfo.SecondFigure = "0";
            }
            else if (BoardInfo.FigureInBoard[x1][y1] == BoardInfo.FigureInBoard[x2][y2] && BoardInfo.FigureInBoard[x1][y1] < 6)
            {
                Destroy(GameObject.Find("Selected(Clone)"));

                StartCoroutine(SwapFigures(GameObject.Find(BoardInfo.FirstFigur), GameObject.Find(BoardInfo.SecondFigure), false, false));

                BoardInfo.FirstFigur = "0";
                BoardInfo.SecondFigure = "0";
            }
            else if (x1 == x2 && Mathf.Abs(y1 - y2) == 1 || y1 == y2 && Mathf.Abs(x1 - x2) == 1)
            {
                GameObject figure1 = GameObject.Find(BoardInfo.FirstFigur);
                GameObject figure2 = GameObject.Find(BoardInfo.SecondFigure);

                if (figure1.tag == "Bomb")
                {
                    Destroy(GameObject.Find("Selected(Clone)"));
                    StartCoroutine(SwapFigures(figure1, figure2, true, true));

                    BoardInfo.FirstFigur = "0";
                    BoardInfo.SecondFigure = "0";
                }
                else if (figure2.tag == "Bomb")
                {
                    Destroy(GameObject.Find("Selected(Clone)"));
                    StartCoroutine(SwapFigures(figure1, figure2, true, true));

                    BoardInfo.FirstFigur = "0";
                    BoardInfo.SecondFigure = "0";
                }
                else
                {
                    Destroy(GameObject.Find("Selected(Clone)"));
                    StartCoroutine(SwapFigures(figure1, figure2, true, false));

                    BoardInfo.FirstFigur = "0";
                    BoardInfo.SecondFigure = "0";
                }
            }
        }
        if (figureMovedDown)
        {
            for (int i = 0; i < figuresNextPositions.Count; i++)
            {
                figuresWhitchWillMove[i].transform.position = new Vector2(Mathf.MoveTowards(figuresWhitchWillMove[i].transform.position.x, figuresNextPositions[i].x, 4 * Time.deltaTime),
                    Mathf.MoveTowards(figuresWhitchWillMove[i].transform.position.y, figuresNextPositions[i].y, 4 * Time.deltaTime));
            }
        }
        if (swapTwoFigures)
        {
            figuresWhichWillSwap[0].transform.position = new Vector2(Mathf.MoveTowards(figuresWhichWillSwap[0].transform.position.x, swapPositionsForFigures[0].x, 4 * Time.deltaTime),
                Mathf.MoveTowards(figuresWhichWillSwap[0].transform.position.y, swapPositionsForFigures[0].y, 4 * Time.deltaTime));
            figuresWhichWillSwap[1].transform.position = new Vector2(Mathf.MoveTowards(figuresWhichWillSwap[1].transform.position.x, swapPositionsForFigures[1].x, 4 * Time.deltaTime),
                Mathf.MoveTowards(figuresWhichWillSwap[1].transform.position.y, swapPositionsForFigures[1].y, 4 * Time.deltaTime));
        }
        if (BoardInfo.BombName != "")
        {
            GameObject.Find("MoveCount").GetComponent<Text>().text = (int.Parse(GameObject.Find("MoveCount").GetComponent<Text>().text) - 1).ToString();
            StartCoroutine(FigureDefaultSize(BombBoom(BoardInfo.BombName)));
            BoardInfo.BombName = "";
        }
    }

    IEnumerator SwapFigures(GameObject figure1, GameObject figure2, bool checkCombo, bool swapBoomb)
    {
        figuresWhichWillSwap = new List<GameObject>();
        swapPositionsForFigures = new List<Vector2>();

        figuresWhichWillSwap.Add(figure1);
        figuresWhichWillSwap.Add(figure2);
        swapPositionsForFigures.Add(figure2.transform.position);
        swapPositionsForFigures.Add(figure1.transform.position);
        swapTwoFigures = true;

        yield return new WaitForSeconds(0.25f);

        SwapFigurePositions(figure1, figure2);

        if (swapBoomb)
        {
            if (figure1.tag == "Bomb")
            {
                BoardInfo.BombName = figure1.name;
            }
            else
            {
                BoardInfo.BombName = figure2.name;
            }
        }

        if (checkCombo && !swapBoomb)
        {
            if (CheckMatchs(figure1, figure2) < 3)
            {
                checkCombo = !checkCombo;
            }
        }

        if (!checkCombo && !swapBoomb)
        {
            figuresWhichWillSwap = new List<GameObject>();
            swapPositionsForFigures = new List<Vector2>();

            figuresWhichWillSwap.Add(figure1);
            figuresWhichWillSwap.Add(figure2);
            swapPositionsForFigures.Add(figure2.transform.position);
            swapPositionsForFigures.Add(figure1.transform.position);

            SwapFigurePositions(figure1, figure2);

            yield return new WaitForSeconds(0.25f);
        }
        swapTwoFigures = false;
    }
    IEnumerator FigureDefaultSize(List<GameObject> figuresWhichWillBeRemoved)
    {
        BoardInfo.FigureClicked = false;
        CheckCombo checkCombo = new CheckCombo();
        for (;;)
        {
            yield return new WaitForSeconds(0.3f);

            //Jnjume Combo exac Figruner@ ev hanum Haxtanaki hashvic
            RemoveFigures(figuresWhichWillBeRemoved);
            CreateBomb();

            List<List<string>> emptyFiguresWhichWillMoveDown = GetEmptyFiguresWhichWillMoveDown();
            List<List<string>> emptyFiguresWhichWillMoveRighteOrLefth = GetEmptyFiguresWhichWillMoveRighteOrLefth();
            for (int i = 0; i < emptyFiguresWhichWillMoveDown.Count; i++)
            {
                figuresWhitchWillMove = MoveFigureToDown();
                figuresWhitchWillMove.AddRange(CreateEmptyFiguresWhichWillMoveDown(emptyFiguresWhichWillMoveDown[i]));

                figureMovedDown = true;
                yield return new WaitForSeconds(0.25f);
                figureMovedDown = false;

            }
            for (int i = 0; i < emptyFiguresWhichWillMoveRighteOrLefth.Count; i++)
            {
                figuresWhitchWillMove = MoveFigureToDown();
                figuresWhitchWillMove.AddRange(CreateEmptyFiguresWhichWillMoveRighteOrLefth(emptyFiguresWhichWillMoveRighteOrLefth[i]));
                figureMovedDown = true;
                yield return new WaitForSeconds(0.25f);
                figureMovedDown = false;
            }

            List<GameObject> allFigurInBoard = new List<GameObject>();
            for (int y = 0; y < VerticalFigureCount; y++)
            {
                for (int x = 0; x < HorizontalFigureCount; x++)
                {
                    if (BoardInfo.FigureInBoard[x][y] != -1)
                    {
                        allFigurInBoard.Add(GameObject.Find(x + "." + y));
                    }
                }
            }
            figuresWhichWillBeRemoved = checkCombo.Check(allFigurInBoard, HorizontalFigureCount, VerticalFigureCount);

            if (figuresWhichWillBeRemoved.Count >= 3)
            {
                foreach (var item in figuresWhichWillBeRemoved)
                {
                    item.GetComponent<Animator>().enabled = true;
                }
                continue;
            }
            else
            {
                break;
            }

        }
        if (WinOrLostView[0].activeSelf)
        {
            disableInteraction = true;
        }
        int moveCount = (int.Parse(GameObject.Find("MoveCount").GetComponent<Text>().text));
        if (moveCount == 0 && !WinOrLostView[0].activeSelf)
        {
            WinOrLostView[1].SetActive(true);
            disableInteraction = true;
        }
        BoardInfo.FigureClicked = true;
    }

    void SwapFigurePositions(GameObject figure1, GameObject figure2)
    {
        int x1 = int.Parse(figure1.name.Split('.')[0]);
        int y1 = int.Parse(figure1.name.Split('.')[1]);
        int x2 = int.Parse(figure2.name.Split('.')[0]);
        int y2 = int.Parse(figure2.name.Split('.')[1]);

        int numberFigure2;
        string nameOfFigure2;

        numberFigure2 = BoardInfo.FigureInBoard[x2][y2];
        BoardInfo.FigureInBoard[x2][y2] = BoardInfo.FigureInBoard[x1][y1];
        BoardInfo.FigureInBoard[x1][y1] = numberFigure2;

        nameOfFigure2 = figure2.name;
        figure2.name = figure1.name;
        figure1.name = nameOfFigure2;

    }

    int CheckMatchs(GameObject figure1, GameObject figure2)
    {
        List<GameObject> figuresList = new List<GameObject>();
        CheckCombo checkCombo = new CheckCombo();
        figuresList.Add(figure1);
        figuresList.Add(figure2);

        List<GameObject> FigureDelet = checkCombo.Check(figuresList, HorizontalFigureCount, VerticalFigureCount);

        if (FigureDelet.Count >= 3)
        {
            foreach (var item in FigureDelet)
            {
                item.GetComponent<Animator>().enabled = true;
            }
            GameObject.Find("MoveCount").GetComponent<Text>().text = (int.Parse(GameObject.Find("MoveCount").GetComponent<Text>().text) - 1).ToString();
            StartCoroutine(FigureDefaultSize(FigureDelet));
        }
        return FigureDelet.Count;
    }
    void CreateBomb()
    {
        GameObject matrix = GameObject.Find("Matrix");


        foreach (var item in BoardInfo.BombInBoard)
        {
            int x = int.Parse(item.Key.Split('.')[0]);
            int y = int.Parse(item.Key.Split('.')[1]);

            GameObject newBomb = Instantiate(BombObjects[(BoardInfo.FigureInBoard[x][y] - 6)], new Vector2(x, y), Quaternion.identity);

            newBomb.transform.parent = matrix.transform;
            newBomb.name = x + "." + y;
        }
        BoardInfo.BombInBoard.Clear();
    }

    List<GameObject> MoveFigureToDown()
    {
        figuresNextPositions = new List<Vector2>();

        List<GameObject> FigureToMove = new List<GameObject>();
        for (int y = 1; y < VerticalFigureCount; y++)
        {

            for (int x = 0; x < HorizontalFigureCount; x++)
            {

                if (BoardInfo.FigureInBoard[x][y] != 0 && BoardInfo.FigureInBoard[x][y] != -1 && BoardInfo.FigureInBoard[x][y - 1] != -1)
                {
                    // x x x 
                    // v x v
                    // x o x
                    if (BoardInfo.FigureInBoard[x][y - 1] == 0)
                    {

                        GameObject figure = GameObject.Find(x + "." + y);
                        BoardInfo.FigureInBoard[x][y - 1] = BoardInfo.FigureInBoard[x][y];
                        BoardInfo.FigureInBoard[x][y] = 0;
                        figure.name = x + "." + (y - 1);

                        FigureToMove.Add(figure);
                        figuresNextPositions.Add(new Vector2(x, y - 1));
                        continue;
                    }
                    // x x x 
                    // x v v
                    // x o x  
                    if (x < HorizontalFigureCount - 1 && BoardInfo.FigureInBoard[x + 1][y] == -1 && BoardInfo.FigureInBoard[x + 1][y - 1] == 0)
                    {
                        bool shiftRight = true;
                        for (int i = 0; i < VerticalFigureCount; i++)
                        {
                            if (BoardInfo.FigureInBoard[x][i] == -1)
                            {
                                shiftRight = false;
                                break;
                            }
                        }
                        if (shiftRight)
                        {
                            GameObject figure = GameObject.Find(x + "." + y);
                            float X = figure.transform.position.x + 1;
                            float Y = figure.transform.position.y - 1;

                            FigureToMove.Add(figure);
                            figuresNextPositions.Add(new Vector2(X, Y));

                            BoardInfo.FigureInBoard[x + 1][y - 1] = BoardInfo.FigureInBoard[x][y];
                            BoardInfo.FigureInBoard[x][y] = 0;
                            figure.name = (x + 1) + "." + (y - 1);
                        }
                        continue;

                    }
                    // x x x 
                    // v v x
                    // x o x
                    if (x > 0 && BoardInfo.FigureInBoard[x - 1][y] == -1 && BoardInfo.FigureInBoard[x - 1][y - 1] == 0)
                    {
                        bool shiftLeft = false;
                        for (int i = 0; i < VerticalFigureCount; i++)
                        {
                            if (x == 1)
                            {
                                shiftLeft = true;
                                break;
                            }
                            if (BoardInfo.FigureInBoard[x - 2][i] == -1)
                            {
                                shiftLeft = true;
                                break;
                            }
                        }
                        if (shiftLeft)
                        {
                            GameObject figure = GameObject.Find(x + "." + y);
                            float X = figure.transform.position.x - 1;
                            float Y = figure.transform.position.y - 1;

                            FigureToMove.Add(figure);
                            figuresNextPositions.Add(new Vector2(X, Y));

                            BoardInfo.FigureInBoard[x - 1][y - 1] = BoardInfo.FigureInBoard[x][y];
                            BoardInfo.FigureInBoard[x][y] = 0;
                            figure.name = (x - 1) + "." + (y - 1);
                        }
                        continue;

                    }
                }
            }

        }
        return FigureToMove;

    }
    List<GameObject> BombBoom(string name)
    {
        GameObject.Find(name).GetComponent<Animator>().enabled = true;

        int x = int.Parse(name.Split('.')[0]);
        int y = int.Parse(name.Split('.')[1]);

        int bombIndex = BoardInfo.FigureInBoard[x][y] - 6;
        List<GameObject> figureDenied = new List<GameObject>();
        GameObject figure = new GameObject();
        switch (bombIndex)
        {
            case 0:
                int X = x;
                if (X > 0)
                {
                    X = x - 1;
                }

                for (; X <= (x < HorizontalFigureCount - 1 ? x + 1 : x); X++)
                {
                    int Y = y;
                    if (Y > 0)
                    {
                        Y = y - 1;
                    }
                    for (; Y <= (y < VerticalFigureCount - 1 ? y + 1 : y); Y++)
                    {
                        if (BoardInfo.FigureInBoard[X][Y] != -1)
                        {

                            if (BoardInfo.FigureInBoard[X][Y] > 5 && GameObject.Find(X + "." + Y).GetComponent<Animator>().enabled == false)
                            {
                                figureDenied.AddRange(BombBoom(X + "." + Y));
                                continue;
                            }
                            figure = GameObject.Find(X + "." + Y);
                            figure.GetComponent<Animator>().enabled = true;
                            figureDenied.Add(figure);
                        }
                    }
                }
                return figureDenied;
            case 1:
                for (int i = 0; i < VerticalFigureCount; i++)
                {
                    if (BoardInfo.FigureInBoard[x][i] != -1)
                    {
                        if (BoardInfo.FigureInBoard[x][i] > 5 && GameObject.Find(x + "." + i).GetComponent<Animator>().enabled == false)
                        {

                            figureDenied.AddRange(BombBoom(x + "." + i));
                            continue;
                        }
                        figure = GameObject.Find(x + "." + i);
                        figure.GetComponent<Animator>().enabled = true;
                        figureDenied.Add(figure);
                    }
                }
                return figureDenied;
            case 2:
                for (int i = 0; i < HorizontalFigureCount; i++)
                {
                    if (BoardInfo.FigureInBoard[i][y] != -1)
                    {
                        if (BoardInfo.FigureInBoard[i][y] > 5 && GameObject.Find(i + "." + y).GetComponent<Animator>().enabled == false)
                        {

                            figureDenied.AddRange(BombBoom(i + "." + y));
                            continue;
                        }
                        figure = GameObject.Find(i + "." + y);
                        figure.GetComponent<Animator>().enabled = true;
                        figureDenied.Add(figure);
                    }
                }
                return figureDenied;

            default:
                return figureDenied;
        }
    }

    List<GameObject> CreateEmptyFiguresWhichWillMoveDown(List<string> oneListEmptyFigureInBoard)
    {
        GameObject matrix = GameObject.Find("Matrix");
        List<GameObject> newFigureOneHorizontalLine = new List<GameObject>();
        for (int i = 0; i < oneListEmptyFigureInBoard.Count; i++)
        {
            int x = int.Parse(oneListEmptyFigureInBoard[i].Split('.')[0]);

            BoardInfo.FigureInBoard[x][VerticalFigureCount - 1] = (int)UnityEngine.Random.Range(1f, FigureCounts + 1);
            GameObject newFigure = Instantiate(FigureObjects[BoardInfo.FigureInBoard[x][VerticalFigureCount - 1] - 1], new Vector2(x, VerticalFigureCount), Quaternion.identity);

            newFigure.transform.parent = matrix.transform;
            newFigure.name = x + "." + (VerticalFigureCount - 1);

            newFigureOneHorizontalLine.Add(newFigure);
            figuresNextPositions.Add(new Vector2(x, VerticalFigureCount - 1));
        }

        return newFigureOneHorizontalLine;
    }
    List<GameObject> CreateEmptyFiguresWhichWillMoveRighteOrLefth(List<string> oneListEmptyFigureInBoard)
    {
        GameObject matrix = GameObject.Find("Matrix");
        List<GameObject> newFigureOneHorizontalLine = new List<GameObject>();

        for (int i = 0; i < oneListEmptyFigureInBoard.Count; i++)
        {
            int x = int.Parse(oneListEmptyFigureInBoard[i].Split('.')[0]);

            {
                bool shiftLeft = true;
                if (x == 0)
                {
                    shiftLeft = false;
                }
                if (x > 0 && shiftLeft)
                {
                    for (int Y = 0; Y < VerticalFigureCount; Y++)
                    {
                        if (BoardInfo.FigureInBoard[x - 1][Y] == -1)
                        {
                            shiftLeft = false;
                            break;
                        }
                    }
                }
                if (shiftLeft)
                {
                    BoardInfo.FigureInBoard[x - 1][VerticalFigureCount - 1] = (int)UnityEngine.Random.Range(1f, FigureCounts + 1);
                    GameObject newFigure = Instantiate(FigureObjects[BoardInfo.FigureInBoard[x - 1][VerticalFigureCount - 1] - 1],
                        new Vector2(x - 1, VerticalFigureCount), Quaternion.identity);

                    newFigure.transform.parent = matrix.transform;
                    newFigure.name = (x - 1) + "." + (VerticalFigureCount - 1);

                    newFigureOneHorizontalLine.Add(newFigure);
                    figuresNextPositions.Add(new Vector2(x - 1, VerticalFigureCount - 1));
                }
                else
                {
                    BoardInfo.FigureInBoard[x + 1][VerticalFigureCount - 1] = (int)UnityEngine.Random.Range(1f, FigureCounts + 1);
                    GameObject newFigure = Instantiate(FigureObjects[BoardInfo.FigureInBoard[x + 1][VerticalFigureCount - 1] - 1],
                        new Vector2(x + 1, VerticalFigureCount), Quaternion.identity);

                    newFigure.transform.parent = matrix.transform;
                    newFigure.name = (x + 1) + "." + (VerticalFigureCount - 1);

                    newFigureOneHorizontalLine.Add(newFigure);
                    figuresNextPositions.Add(new Vector2(x + 1, VerticalFigureCount - 1));
                }
            }

        }
        return newFigureOneHorizontalLine;
    }

    List<List<string>> GetEmptyFiguresWhichWillMoveDown()
    {
        bool shiftDown = true;
        List<List<string>> figureListWhichWillMoveDown = new List<List<string>>();
        for (int y = 0; y < VerticalFigureCount; y++)
        {
            List<string> horizontalEmptyFigureList = new List<string>();
            for (int x = 0; x < HorizontalFigureCount; x++)
            {
                if (BoardInfo.FigureInBoard[x][y] == 0)
                {
                    for (int Y = 0; Y < VerticalFigureCount; Y++)
                    {
                        if (BoardInfo.FigureInBoard[x][Y] == -1)
                        {
                            if (y < Y)
                            {
                                shiftDown = false;
                                break;
                            }
                        }
                    }

                    if (shiftDown)
                    {
                        horizontalEmptyFigureList.Add(x + "." + y);
                    }
                    shiftDown = true;
                }
            }
            if (horizontalEmptyFigureList.Count > 0)
            {
                figureListWhichWillMoveDown.Add(horizontalEmptyFigureList);
            }
        }

        return figureListWhichWillMoveDown;
    }
    List<List<string>> GetEmptyFiguresWhichWillMoveRighteOrLefth()
    {
        List<List<string>> figureListWhichWillMoveRightOrLeft = new List<List<string>>();
        for (int y = 0; y < VerticalFigureCount; y++)
        {
            List<string> horizontalEmptyFigureList = new List<string>();
            for (int x = 0; x < HorizontalFigureCount; x++)
            {
                if (BoardInfo.FigureInBoard[x][y] == 0)
                {
                    for (int Y = 0; Y < VerticalFigureCount; Y++)
                    {
                        if (BoardInfo.FigureInBoard[x][Y] == -1)
                        {
                            if (y < Y)
                            {
                                horizontalEmptyFigureList.Add(x + "." + y);
                                break;
                            }
                        }
                    }
                }
            }
            if (horizontalEmptyFigureList.Count > 0)
            {
                figureListWhichWillMoveRightOrLeft.Add(horizontalEmptyFigureList);
            }
        }
        return figureListWhichWillMoveRightOrLeft;
    }

    void RemoveFigures(List<GameObject> figureList)
    {
        int x, y;

        for (int i = 0; i < figureList.Count; i++)
        {
            x = int.Parse(figureList[i].name.Split('.')[0]);
            y = int.Parse(figureList[i].name.Split('.')[1]);

            for (int k = 0; k < FigurNumberForWin.Count; k++)
            {
                if (FigurNumberForWin[k] == BoardInfo.FigureInBoard[x][y])
                {
                    if (int.Parse(BoardInfo.FiguresForWin[k].GetComponent<WinningInfo>().CountText.text) > 0)
                    {
                        BoardInfo.FiguresForWin[k].GetComponent<WinningInfo>().CountText.text =
                            (int.Parse(BoardInfo.FiguresForWin[k].GetComponent<WinningInfo>().CountText.text) - 1) + "";
                    }
                    int count = 0;
                    for (int j = 0; j < BoardInfo.FiguresForWin.Count; j++)
                    {
                        if (int.Parse(BoardInfo.FiguresForWin[j].GetComponent<WinningInfo>().CountText.text) == 0)
                        {
                            count++;
                        }
                    }
                    if (count == BoardInfo.FiguresForWin.Count)
                    {
                        if (!WinOrLostView[0].activeInHierarchy)
                        {
                            WinOrLostView[0].SetActive(true);
                        }
                    }
                }
            }
            BoardInfo.FigureInBoard[x][y] = 0;

            foreach (var item in BoardInfo.BombInBoard)
            {
                if (item.Key == x + "." + y)
                {
                    BoardInfo.FigureInBoard[x][y] = item.Value;
                }
            }
            Destroy(figureList[i]);
        }

    }
}