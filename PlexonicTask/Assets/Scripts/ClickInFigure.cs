using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickInFigure : MonoBehaviour
{
    public GameObject selectedPrefab;

    private void OnMouseUp()
    {
        if (BoardInfo.FigureClicked == false)
        {
            return;
        }
        if (BoardInfo.FirstFigur != gameObject.name)
        {
            if (BoardInfo.FirstFigur == "0")
            {
                int x = int.Parse(gameObject.name.Split('.')[0]);
                int y = int.Parse(gameObject.name.Split('.')[1]);

                GameObject obj = Instantiate(selectedPrefab, new Vector2(x, y), Quaternion.identity);
                BoardInfo.FirstFigur = gameObject.name;
            }
            else
            {
                BoardInfo.SecondFigure = gameObject.name;
            }
        }
        else if (gameObject.tag == "Bomb")
        {
            //int x = int.Parse(gameObject.name.Split('.')[0]);
            //int y = int.Parse(gameObject.name.Split('.')[1]);

            Destroy(GameObject.Find("Selected(Clone)"));
            BoardInfo.FirstFigur = "0";
            BoardInfo.BombName = gameObject.name;
        }
        else
        {
            BoardInfo.FirstFigur = "0";
            Destroy(GameObject.Find("Selected(Clone)"));
        }
    }
}
