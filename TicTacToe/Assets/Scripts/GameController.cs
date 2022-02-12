using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// XXX - singleton, no? enforce that.

public class GameController : MonoBehaviour
{
    // Size of the game grid.
    public int rows = 3;   // Rows
    public int columns = 3;   // Columns
    // Marks in a row to win.
    public int k = 3;

    public GridController grid;

    private string playerSide;

    void Awake()
    {
        ResetGame();
    }

    private void ResetGame()
    {
        playerSide = "X";
    }

    public string GetPlayerSide()
    {
        return playerSide;
    }

    public void EndTurn()
    {
        if (grid.CheckForWin(playerSide))
        {
            GameOver();
        }

        ChangeSides();
    }

    void ChangeSides()
    {
        playerSide = (playerSide == "X") ? "O" : "X";
    }

    void GameOver()
    {
        grid.GameOver();
    }
}