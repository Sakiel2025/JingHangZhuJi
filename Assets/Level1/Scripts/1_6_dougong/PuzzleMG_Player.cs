using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PuzzleMG_Player : MonoBehaviour
{
    public static PuzzleMG_Player instance;
    public List<PuzzlePiece> puzzles = new List<PuzzlePiece>();
    public GameObject gameOverPanel;
    public bool isPlayerFinish = false; // 玩家是否拼完

    void Start()
    {
        instance = this;
        puzzles.Clear();
        // 只找自己子节点下的玩家碎片
        puzzles.AddRange(GetComponentsInChildren<PuzzlePiece>());
    }

    public void Check()
    {
        foreach (PuzzlePiece puzzle in puzzles)
        {
            if (!puzzle.isComplete)
            {
                return;
            }
        }

        // 玩家拼完
        isPlayerFinish = true;
        Debug.Log("玩家拼完！");
        // 通知比拼管理器
        FindObjectOfType<CompeteManager>().CheckWinner();
    }
}