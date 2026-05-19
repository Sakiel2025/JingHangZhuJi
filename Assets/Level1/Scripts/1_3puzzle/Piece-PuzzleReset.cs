using UnityEngine;

public class PuzzlePiecePos : MonoBehaviour
{
    Vector3 startPos;

    void Start()
    {
        // 游戏一开始，自己记住自己的出生位置
        startPos = transform.position;
    }

    // 重置自己
    public void ResetMe()
    {
        transform.position = startPos;
        GetComponent<PuzzlePiece>().isComplete = false;
    }
}