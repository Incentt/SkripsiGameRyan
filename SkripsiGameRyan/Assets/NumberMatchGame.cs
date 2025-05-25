using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class NumberMatchingGame : MonoBehaviour
{
    [Header("Game Elements")]
    public Transform numberContainer;
    public GameObject numberButtonPrefab;
    public Text targetNumberText;
    public Text scoreText;
    public Image[] countingImages;

    private int targetNumber;
    private int score = 0;
    private int level = 1;
    private List<GameObject> numberButtons = new List<GameObject>();
    private GameManager gameManager;

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
        ClearNumberButtons();

        // Generate target number (1-10)
        targetNumber = Random.Range(1, 11);
        targetNumberText.text = "Count and find: " + targetNumber;

        // Show counting images
        ShowCountingImages(targetNumber);

        // Create number options
        List<int> numbers = new List<int>();
        numbers.Add(targetNumber);

        // Add wrong numbers
        for (int i = 0; i < 5; i++)
        {
            int wrongNumber;
            do
            {
                wrongNumber = Random.Range(1, 11);
            } while (numbers.Contains(wrongNumber));

            numbers.Add(wrongNumber);
        }

        // Shuffle numbers
        FisherYatesShuffle.Shuffle(numbers);

        // Create number buttons
        foreach (int number in numbers)
        {
            GameObject buttonObj = Instantiate(numberButtonPrefab, numberContainer);
            Button button = buttonObj.GetComponent<Button>();
            Text buttonText = buttonObj.GetComponentInChildren<Text>();

            buttonText.text = number.ToString();
            button.onClick.AddListener(() => CheckAnswer(number, button));

            numberButtons.Add(buttonObj);
        }
    }

    private void ShowCountingImages(int count)
    {
        // Hide all images first
        foreach (Image img in countingImages)
        {
            img.gameObject.SetActive(false);
        }

        // Show required number of images
        for (int i = 0; i < count && i < countingImages.Length; i++)
        {
            countingImages[i].gameObject.SetActive(true);
        }
    }

    private void CheckAnswer(int selectedNumber, Button button)
    {
        if (selectedNumber == targetNumber)
        {
            // Correct answer
            button.image.color = Color.green;
            score += 20;
            gameManager.PlaySound(gameManager.successSound);

            Invoke("NextLevel", 1.5f);
        }
        else
        {
            // Wrong answer
            button.image.color = Color.red;
            gameManager.PlaySound(gameManager.failSound);

            Invoke("ResetButtonColors", 1f);
        }

        UpdateUI();
    }

    private void NextLevel()
    {
        level++;
        GenerateLevel();
        ResetButtonColors();
    }

    private void ResetButtonColors()
    {
        foreach (GameObject buttonObj in numberButtons)
        {
            if (buttonObj != null)
                buttonObj.GetComponent<Button>().image.color = Color.white;
        }
    }

    private void ClearNumberButtons()
    {
        foreach (GameObject buttonObj in numberButtons)
        {
            if (buttonObj != null)
                Destroy(buttonObj);
        }
        numberButtons.Clear();
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