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

    // Layout parameters & objects
    public float boardScale = 0.85f; // Percentage of Background.

    public GameObject gridLineVertPrefab;
    public GameObject gridLineHorizPrefab;
    public GameObject gridSpacePrefab;

    // Warning! If buttonTextList is declared public, it will be set(able) in the
    // Inspector -- that means that the length of the array will come from the Inspector,
    // not from here. Weird, but logical...
    Text[] buttonTextList;

    private string playerSide;

    void Awake()
    {
        buttonTextList = new Text[rows * columns];
        LayoutGrid();
        ResetGame();
    }

    private void ResetGame()
    {
        playerSide = "X";
    }

    private void LayoutGrid()
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
        var spaceSize = boardSize / Math.Max(rows, columns);

        // Vertical lines -- start on the left and move right.
        var vGridLines = columns - 1;
        float initialX;
        if (vGridLines % 2 == 0)
        {
            // Even number of lines, odd number of spaces.
            initialX = -spaceSize / 2 - ((vGridLines / 2 - 1) * spaceSize);
        } else {
            // Odd number of lines, even number of spaces.
            // One line is in the center.
            initialX = -(vGridLines / 2 * spaceSize);
        }
        var rot = gridLineVertPrefab.transform.rotation;
        for (int i = 0; i < columns - 1; i++) {
            var pos = new Vector3((initialX + i * spaceSize), 0, 0);

            var vLine = Instantiate(gridLineVertPrefab, grid.transform);
            vLine.GetComponent<RectTransform>().anchoredPosition = pos;
        }

        // Horizontal lines -- start at the top and move down.
        var hGridLines = rows - 1;
        float initialY;
        if (hGridLines % 2 == 0)
        {
            // Even number of lines, odd number of spaces.
            initialY = spaceSize / 2 + ((hGridLines / 2 - 1) * spaceSize);
        } else {
            // Odd number of lines, even number of spaces.
            // One line is in the center.
            initialY = (hGridLines / 2 * spaceSize);
        }
        rot = gridLineHorizPrefab.transform.rotation; for (int i = 0; i < rows - 1; i++) {
            var pos = new Vector3(0, (initialY - i * spaceSize), 0);

            var hLine = Instantiate(gridLineHorizPrefab, grid.transform);
            hLine.GetComponent<RectTransform>().anchoredPosition = pos;
        }

        // Instantiate the grid spaces.

        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < columns; c++)
            {
                var yPos = (initialY + spaceSize / 2) - spaceSize * r;
                var xPos = (initialX - spaceSize / 2) + spaceSize * c;

                var pos = new Vector3(xPos, yPos, 0);
                var gs = Instantiate(gridSpacePrefab, grid.transform);
                gs.GetComponent<RectTransform>().anchoredPosition = pos;

                var buttonTextIndex = r * columns + c;
                Debug.Log("buttonTextList.Length = " + buttonTextList.Length);
                Debug.Log("buttonTextIndex = " + buttonTextIndex);

                // Get the Text component of the Text object that is a child of the
                // GridSpace button.
                var textGameObject = gs.transform.GetChild(0).gameObject;
                buttonTextList[buttonTextIndex] = textGameObject.GetComponent<Text>();
                if (buttonTextList[buttonTextIndex] == null)
                {
                    Debug.Log("buttonTextList[" + buttonTextIndex + "] is null");
                }
            }
        }

        // Wire up buttons (grid spaces).
        SetGameControllerReferenceOnButtons();
    }

    void SetGameControllerReferenceOnButtons()
    {
        for (int i = 0; i < buttonTextList.Length; i++)
        {
            // Debug.Log("buttonTextList[" + i + "] = " + buttonTextList[i]);
            // Debug.Log("GridSpace = " + buttonTextList[i].GetComponentInParent<GridSpace>());
            buttonTextList[i].GetComponentInParent<GridSpace>().SetGameControllerReference(this);
        }
    }

    public string GetPlayerSide()
    {
        return playerSide;
    }

    public void EndTurn()
    {
        if (buttonTextList[0].text == playerSide && buttonTextList[1].text == playerSide && buttonTextList[2].text == playerSide)
        {
            GameOver();
        }

        if (buttonTextList[3].text == playerSide && buttonTextList[4].text == playerSide && buttonTextList[5].text == playerSide)
        {
            GameOver();
        }

        if (buttonTextList[6].text == playerSide && buttonTextList[7].text == playerSide && buttonTextList[8].text == playerSide)
        {
            GameOver();
        }

        if (buttonTextList[0].text == playerSide && buttonTextList[3].text == playerSide && buttonTextList[6].text == playerSide)
        {
            GameOver();
        }

        if (buttonTextList[1].text == playerSide && buttonTextList[4].text == playerSide && buttonTextList[7].text == playerSide)
        {
            GameOver();
        }

        if (buttonTextList[2].text == playerSide && buttonTextList[5].text == playerSide && buttonTextList[8].text == playerSide)
        {
            GameOver();
        }

        if (buttonTextList[0].text == playerSide && buttonTextList[4].text == playerSide && buttonTextList[8].text == playerSide)
        {
            GameOver();
        }

        if (buttonTextList[2].text == playerSide && buttonTextList[4].text == playerSide && buttonTextList[6].text == playerSide)
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
        for (int i = 0; i < buttonTextList.Length; i++)
        {
            buttonTextList[i].GetComponentInParent<Button>().interactable = false;
        }
    }
}