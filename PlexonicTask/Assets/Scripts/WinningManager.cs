using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WinningManager : MonoBehaviour
{
    [Range(10, 15)]
    public int[] FigureValues;
    public Sprite[] SpriteFigure = new Sprite[3];
    [Range(10, 50)]
    public int MoveCount;
    public Text MoveCountTxt;

    public GameObject[] prefabCountAndFigure = new GameObject[3];
    private void Awake()
    {
        MoveCountTxt.text = MoveCount.ToString();
        for (int i = 0; i < FigureValues.Length; i++)
        {
            GameObject prefab = Instantiate(prefabCountAndFigure[i], new Vector2(transform.position.x - ((i + 1) * 35), transform.position.y), Quaternion.identity);
            prefab.transform.SetParent(gameObject.transform);
            prefab.GetComponent<WinningInfo>().Figure = SpriteFigure[i];
            prefab.GetComponent<WinningInfo>().CountText.text = FigureValues[i].ToString();

            BoardInfo.FiguresForWin.Add(prefab);
        }
    }
}
