using UnityEngine;

public class BattleBG : MonoBehaviour
{
    private BattleBGPiece[,] pieces;

    public void Awake()
    {
        pieces = new BattleBGPiece[2, 6];
        int num = 0;
        int num2 = 0;
        BattleBGPiece[] componentsInChildren = base.transform.GetComponentsInChildren<BattleBGPiece>();
        foreach (BattleBGPiece battleBGPiece in componentsInChildren)
        {
            pieces[num2, num] = battleBGPiece;
            num++;
            if (num == 6)
            {
                num2++;
                num = 0;
            }
        }
    }

    public void StartBG(int type, float intensity, float speed, bool isBoss)
    {
        StartBG(type, intensity, speed, new Color(0.1333f, 0.694f, 0.298f), isBoss);
    }

    public void StartBG(int type, float intensity, float speed, Color color, bool isBoss)
    {
        BattleBGPiece[,] array = pieces;
        int upperBound = array.GetUpperBound(0);
        int upperBound2 = array.GetUpperBound(1);
        for (int i = array.GetLowerBound(0); i <= upperBound; i++)
        {
            for (int j = array.GetLowerBound(1); j <= upperBound2; j++)
            {
                array[i, j].StartBG(type, intensity, speed, color, isBoss);
            }
        }
    }
}
