using System.Collections.Generic;
using UnityEngine;

public class PuzzleMG_ai_version : MonoBehaviour
{
    public static PuzzleMG_ai_version instance;
    public List<PuzzlePiece_dougong> puzzles = new List<PuzzlePiece_dougong>();
    public GameObject gameOverPanel;
    public bool isPlayerFinish = false;

    void Start()
    {
        instance = this;
        puzzles.Clear();
        puzzles.AddRange(GetComponentsInChildren<PuzzlePiece_dougong>());
    }

    public void Check()
    {
        foreach (var p in puzzles)
        {
            if (!p.isComplete) return;
        }

        isPlayerFinish = true;
        FindObjectOfType<CompeteManager>().CheckWinner();
    }
}