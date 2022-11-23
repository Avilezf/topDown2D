using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    private GameStateEnum gameState;
    public string towerConfiguration;

    public string playerConfiguration;
    public int countPlayer = 0;
    public int gameCount = 0;
    public string setOfWinPlayers = "";

    private void Awake()
    {
        Instance = this;
        UpdateGameState(GameStateEnum.start);
    }

    public void UpdateGameState(GameStateEnum gameStateEnum)
    {
        gameState = gameStateEnum;
    }

    void Update()
    {
        switch (gameState)
        {
            case GameStateEnum.start:
                BoardManager.Instance.SetupBoard();
                UpdateGameState(GameStateEnum.progress);
                break;
            case GameStateEnum.progress:
                break;
            case GameStateEnum.end:
                SceneManager.LoadScene("TerrainScene");
                GenerateFile.Instance.GenerateTxt();
                break;
        }

    }

    public enum GameStateEnum
    {
        start,
        progress, 
        end
    }
}
