using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    // Size of the game grid.
    public int m = 3;   // Rows
    public int n = 3;   // Columns
    // Marks in a row to win.
    public int k = 3;

    // Layout parameters & objects
    public float boardScale = 0.85f; // Percentage of Background.
    public GameObject gridLineVertPrefab;
    public GameObject gridLineHorizPrefab;

    public Text[] buttonList;

    private string playerSide;

    void Awake()
    {
        LayoutGrid(m, n);
        ResetGame();
    }

    private void ResetGame()
    {
        playerSide = "X";
    }

    private void LayoutGrid(int r, int c)
    {
        // Find the size of the display area (Background panel).
        // XXX - Assuming the board is square.
        var bg = GameObject.Find("Background");
        var bgRectTransform = bg.GetComponent<RectTransform>();
        var bgWidth = bgRectTransform.rect.width;
        var bgHeight = bgRectTransform.rect.height;
        var boardSize = Math.Min(bgWidth, bgHeight);

        // Set the size of the board.
        var board = GameObject.Find("Board");
        var boardRectTransform = board.GetComponent<RectTransform>();
        boardRectTransform.
            SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, boardSize);
        boardRectTransform.
            SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, boardSize);
        
        // XXX - This may scale everything (board & children), thot could be
        // handy, but when does it happen? Is it dynamac?
        var scale = boardRectTransform.localScale;
        scale.x = boardScale;
        scale.y = boardScale;
        boardRectTransform.localScale = scale;

        // Debug.Log("bgRectTransform.rect.width = " + bgWidth);
        // Debug.Log("bgRectTransform.rect.height = " + bgHeight);
        // Debug.Log("size = " + size);

        // Build the grid.
        var grid = GameObject.Find("Grid");

        // Calculate the size of the grid spaces. They will always be square.
        var spaceSize = boardSize / Math.Max(r, c);

        // Vertical lines -- start on the left and move right.
        float initialX;
        if (c % 2 == 0)
        {
            // Even number of spaces, odd number of lines.
            var spaces = c - 2; // # of lines, not counting the middle.
            initialX = -(spaces / 2 * spaceSize);
        } else {
            // Odd number of spaces, even number of lines.
            var spaces = c - 2;
            initialX = -(spaces / 2 * spaceSize);
        }
        var rot = gridLineVertPrefab.transform.rotation;
        for (int i = 0; i < c - 1; i++) {
            var pos = new Vector3((initialX + i * spaceSize), 0, 0);
            Debug.Log("initialX = " + pos.x);

            var line = Instantiate(gridLineVertPrefab, grid.transform);
            line.GetComponent<RectTransform>().anchoredPosition = pos;
        }

        for (int i = 0; i < r - 1; i++) {
            Instantiate(gridLineHorizPrefab, grid.transform);
        }
        
        // Calculate the size of the grid spaces.
        // Instantiate the grid spaces.

        // Wire up buttons (grid spaces).
        SetGameControllerReferenceOnButtons();
    }

    void SetGameControllerReferenceOnButtons()
    {
        for (int i = 0; i < buttonList.Length; i++)
        {
            buttonList[i].GetComponentInParent<GridSpace>().SetGameControllerReference(this);
        }
    }

    public string GetPlayerSide()
    {
        return playerSide;
    }

    public void EndTurn()
    {
        if (buttonList[0].text == playerSide && buttonList[1].text == playerSide && buttonList[2].text == playerSide)
        {
            GameOver();
        }

        if (buttonList[3].text == playerSide && buttonList[4].text == playerSide && buttonList[5].text == playerSide)
        {
            GameOver();
        }

        if (buttonList[6].text == playerSide && buttonList[7].text == playerSide && buttonList[8].text == playerSide)
        {
            GameOver();
        }

        if (buttonList[0].text == playerSide && buttonList[3].text == playerSide && buttonList[6].text == playerSide)
        {
            GameOver();
        }

        if (buttonList[1].text == playerSide && buttonList[4].text == playerSide && buttonList[7].text == playerSide)
        {
            GameOver();
        }

        if (buttonList[2].text == playerSide && buttonList[5].text == playerSide && buttonList[8].text == playerSide)
        {
            GameOver();
        }

        if (buttonList[0].text == playerSide && buttonList[4].text == playerSide && buttonList[8].text == playerSide)
        {
            GameOver();
        }

        if (buttonList[2].text == playerSide && buttonList[4].text == playerSide && buttonList[6].text == playerSide)
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
        for (int i = 0; i < buttonList.Length; i++)
        {
            buttonList[i].GetComponentInParent<Button>().interactable = false;
        }
    }
}