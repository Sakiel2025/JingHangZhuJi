using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PuzzleMG_AI : MonoBehaviour
{
    public static PuzzleMG_AI instance;
    public List<PuzzlePiece> puzzles = new List<PuzzlePiece>();
    public bool isAIFinish = false; // AI是否拼完

    void Start()
    {
        instance = this;
        puzzles.Clear();
        // 只找自己子节点下的AI碎片
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

        // AI拼完
        isAIFinish = true;
        Debug.Log("AI拼完！");
        // 通知比拼管理器
        FindObjectOfType<CompeteManager>().CheckWinner();
    }
}