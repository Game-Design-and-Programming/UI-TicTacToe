using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

public class GridController : MonoBehaviour
{
    /**
     * Responsible for laying out and managaging the grid:
     * - Layout
     * - Checking for end-of-game (win or stalemate)
     **/

    public GameController controller;

    // Layout parameters & objects
    public float boardScale = 0.85f; // Percentage of Background.

    public GameObject gridLineVertPrefab;
    public GameObject gridLineHorizPrefab;
    public GameObject gridSpacePrefab;

    // Warning! If buttonTextList is declared public, it will be set(able) in
    // the Inspector -- that means that the length of the array will come from
    // the Inspector, not from here. Weird, but logical...
    Text[] buttonTextList;

    private int rows;
    private int columns;
    private int k; // Length of run to win.

    void Awake()
    {
        // XXX - should be getters.
        rows = controller.rows;
        columns = controller.columns;
        k = controller.k;

        buttonTextList = new Text[controller.rows * columns];
        LayoutGrid(rows, columns);
    }

    public void LayoutGrid(int rows, int columns)
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
        }
        else
        {
            // Odd number of lines, even number of spaces.
            // One line is in the center.
            initialX = -(vGridLines / 2 * spaceSize);
        }
        var rot = gridLineVertPrefab.transform.rotation;
        for (int i = 0; i < columns - 1; i++)
        {
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
        }
        else
        {
            // Odd number of lines, even number of spaces.
            // One line is in the center.
            initialY = (hGridLines / 2 * spaceSize);
        }
        rot = gridLineHorizPrefab.transform.rotation; for (int i = 0; i < rows - 1; i++)
        {
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
                // Debug.Log("buttonTextList.Length = " + buttonTextList.Length);
                // Debug.Log("buttonTextIndex = " + buttonTextIndex);

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

    private void SetGameControllerReferenceOnButtons()
    {
        for (int i = 0; i < buttonTextList.Length; i++)
        {
            // Debug.Log("buttonTextList[" + i + "] = " + buttonTextList[i]);
            // Debug.Log("GridSpace = " + buttonTextList[i].GetComponentInParent<GridSpace>());
            buttonTextList[i].GetComponentInParent<GridSpace>().SetGameControllerReference(controller);
        }
    }
    
    public bool CheckForWin(string playerSide)
    {
        // Check for a horizontal sequence.
        for (int r = 0; r < rows; r++)
        {
            var matches = 0;
            for (int c = 0; c < columns; c++)
            {
                var index = r * columns + c;
                if (buttonTextList[index].text == playerSide)
                {
                    if (++matches >= k) return true;
                } else matches = 0;
            }
        }

        // Check for a vertical sequence.
        for (int c = 0; c < columns; c++)
        {
            var matches = 0;
            for (int r = 0; r < rows; r++)
            {
                var index = c * rows + r;
                if (buttonTextList[index].text == playerSide)
                {
                    if (++matches >= k) return true;  
                } else matches = 0;
            }
        }

        // To Handle grids that are taller than k.
        for (int i = 0; i <= (rows - k); i++)
        {
            var offset = i * columns;

            // Check for a diagonal sequence -- upper left to lower right.
            for (int c = 0; c <= (columns - k); c++)
            {
                var matches = 0;
                for (int r = 0; r < rows; r++)
                {
                    var index = offset + c + r + r * columns;
                    Debug.Log("index = " + index + " matches = " + matches);
                    if (index < rows * columns &&
                        buttonTextList[index].text == playerSide)
                    {
                        if (++matches >= k) return true;
                    }
                    else matches = 0;
                }
            }

            // Check for a diagonal sequence -- upper right to lower left.
            for (int c = columns - 1; c >= k - 1; c--)
            {
                var matches = 0;
                for (int r = 0; r < rows; r++)
                {
                    var index = offset + c - r + r * columns;
                    Debug.Log("index = " + index + " matches = " + matches);
                    if (index < rows * columns &&
                        buttonTextList[index].text == playerSide)
                    {
                        if (++matches >= k) return true;
                    }
                    else matches = 0;
                }
            }
        }
        return false;
    }

    public void GameOver()
    {
        for (int i = 0; i < buttonTextList.Length; i++)
        {
            buttonTextList[i].GetComponentInParent<Button>().interactable = false;
        }
    }
}
