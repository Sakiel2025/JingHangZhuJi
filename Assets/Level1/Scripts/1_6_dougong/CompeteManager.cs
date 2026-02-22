using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CompeteManager : MonoBehaviour
{
    [Header("UI")]
    public TextMeshProUGUI timeText;
    public TextMeshProUGUI winLoseText;
    public Button startButton;

    [Header("两套拼图")]
    public PuzzleMG_ai_version playerPuzzle;
    public List<PuzzlePiece_dougong> aiPuzzles;

    [Header("AI参数")]
    public float totalTime = 480f;
    public float aiMoveSpeed = 8f;
    public float aiThinkTime = 2f;

    private float remainingTime;
    private int aiIndex = 0;
    private bool isCompeting;
    private bool isGameOver;

    void Start()
    {
        remainingTime = totalTime;
        winLoseText.enabled = false;
        UpdateTimeText();
        startButton.onClick.AddListener(StartCompete);
    }

    void Update()
    {
        if (!isCompeting || isGameOver) return;

        remainingTime -= Time.deltaTime;
        UpdateTimeText();

        if (remainingTime <= 0)
        {
            GameOver(false);
            return;
        }

        UpdateAI();
    }

    void StartCompete()
    {
        isCompeting = true;
        startButton.gameObject.SetActive(false);
    }

    void UpdateTimeText()
    {
        int m = Mathf.FloorToInt(remainingTime / 60);
        int s = Mathf.FloorToInt(remainingTime % 60);
        timeText.text = $"{m:00}:{s:00}";
    }

    void UpdateAI()
    {
        if (aiIndex >= aiPuzzles.Count) return;

        var piece = aiPuzzles[aiIndex];
        if (piece.isComplete)
        {
            aiIndex++;
            StartCoroutine(Delay());
            return;
        }

        piece.MoveToTarget(aiMoveSpeed);
        CheckAIAllDone();
    }

    void CheckAIAllDone()
    {
        foreach (var p in aiPuzzles)
        {
            if (!p.isComplete) return;
        }
        GameOver(false);
    }

    IEnumerator Delay()
    {
        yield return new WaitForSeconds(aiThinkTime);
    }

    public void CheckWinner()
    {
        if (playerPuzzle.isPlayerFinish && !isGameOver)
        {
            GameOver(true);
        }
    }

    void GameOver(bool win)
    {
        isGameOver = true;
        isCompeting = false;

        if (win)
        {
            winLoseText.text = "胜利！你赢了西洋工匠！";
            winLoseText.color = Color.yellow;
        }
        else
        {
            winLoseText.text = "失败！西洋工匠更快！";
            winLoseText.color = Color.red;
        }

        winLoseText.enabled = true;
        playerPuzzle.gameOverPanel.SetActive(true);
    }
}