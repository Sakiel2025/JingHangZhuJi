using UnityEngine;

public class PuzzleResetFinal : MonoBehaviour
{
    // 把你【所有可拖拽的拼图】全部拖进这里！
    public GameObject[] allPuzzlePieces;

    public void ResetAll()
    {
        foreach (GameObject piece in allPuzzlePieces)
        {
            piece.GetComponent<PuzzlePiecePos>().ResetMe();
        }
    }
}