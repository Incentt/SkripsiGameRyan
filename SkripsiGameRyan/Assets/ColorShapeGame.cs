using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ColorShapeGame : MonoBehaviour
{
    [Header("Game Elements")]
    public Transform shapeContainer;
    public GameObject shapePrefab;
    public Text instructionText;
    public Text scoreText;

    [Header("Shapes and Colors")]
    public Sprite[] shapeSprites; // Ensure these match shapeNames order and count
    public Color[] gameColors;    // Red, Blue, Green, Yellow

    private struct ShapeData
    {
        public int shapeIndex;
        public int colorIndex;
        public string shapeName;
        public string colorName;
    }

    private ShapeData targetShape;
    private int score = 0;
    private int level = 1;
    private List<GameObject> shapeObjects = new List<GameObject>();
    private GameManager gameManager;

    // Make sure these names match the order of shapeSprites!
    private string[] shapeNames = {
        "Circle", "Triangle", "Square", "Rectangle", "Oval",
        "Plus", "Pentagon", "Hexagon", "Diamond", "Crescent",
        "Star", "Heart", "Arrow", "Rhombus", "Semi-Circle"
    };

    private string[] colorNames = { "Red", "Blue", "Green", "Yellow" };

    private void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
    }

    public void StartGame()
    {
        score = 0;
        level = 1;
        UpdateUI();
        GenerateLevel();
    }

    private void GenerateLevel()
    {
        ClearShapes();

        // Generate correct answer
        int shapeIdx = Random.Range(0, shapeSprites.Length);
        int colorIdx = Random.Range(0, gameColors.Length);

        targetShape = new ShapeData
        {
            shapeIndex = shapeIdx,
            colorIndex = colorIdx,
            shapeName = shapeNames[shapeIdx],
            colorName = colorNames[colorIdx]
        };

        instructionText.text = "Find: " + targetShape.colorName + " " + targetShape.shapeName;

        List<ShapeData> shapes = new List<ShapeData> { targetShape };

        // Generate 5 incorrect options
        for (int i = 0; i < 5; i++)
        {
            int wrongShapeIdx = Random.Range(0, shapeSprites.Length);
            int wrongColorIdx = Random.Range(0, gameColors.Length);

            ShapeData wrongShape = new ShapeData
            {
                shapeIndex = wrongShapeIdx,
                colorIndex = wrongColorIdx,
                shapeName = shapeNames[wrongShapeIdx],
                colorName = colorNames[wrongColorIdx]
            };

            shapes.Add(wrongShape);
        }

        FisherYatesShuffle.Shuffle(shapes);

        // Instantiate buttons
        foreach (ShapeData shape in shapes)
        {
            GameObject shapeObj = Instantiate(shapePrefab, shapeContainer);
            Image shapeImage = shapeObj.GetComponent<Image>();
            Button shapeButton = shapeObj.GetComponent<Button>();

            shapeImage.sprite = shapeSprites[shape.shapeIndex];
            shapeImage.color = gameColors[shape.colorIndex];

            ShapeData capturedShape = shape; // Closure-safe
            shapeButton.onClick.AddListener(() => CheckAnswer(capturedShape, shapeButton));

            shapeObjects.Add(shapeObj);
        }
    }

    private void CheckAnswer(ShapeData selectedShape, Button button)
    {
        if (selectedShape.shapeIndex == targetShape.shapeIndex &&
            selectedShape.colorIndex == targetShape.colorIndex)
        {
            button.transform.localScale = Vector3.one * 1.2f;
            score += 15;
            gameManager.PlaySound(gameManager.successSound);
            Invoke(nameof(NextLevel), 1.5f);
        }
        else
        {
            button.image.color = Color.gray;
            gameManager.PlaySound(gameManager.failSound);
            Invoke(nameof(NextLevel), 1.5f); // Skip ResetShapes and just continue
        }

        UpdateUI();
    }

    private void NextLevel()
    {
        level++;
        GenerateLevel();
    }

    private void ClearShapes()
    {
        foreach (GameObject shape in shapeObjects)
        {
            if (shape != null)
                Destroy(shape);
        }
        shapeObjects.Clear();
    }

    private void UpdateUI()
    {
        scoreText.text = "Score: " + score + " | Level: " + level;
    }

    public void BackToMenu()
    {
        gameManager.ShowGameSelection();
    }
}
