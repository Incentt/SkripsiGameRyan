using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

public class AlphabetMatchGame : MonoBehaviour
{
    [Header("Game Elements")]
    public Transform letterContainer;
    public GameObject letterButtonPrefab;
    public Text targetLetterText;
    public Text scoreText;
    public Button nextLevelButton;

    [Header("Game Settings")]
    public int lettersPerLevel = 6;
    public Color correctColor = Color.green;
    public Color incorrectColor = Color.red;

    private string[] alphabet = {"A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M",
                                "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z"};
    private string currentTargetLetter;
    private int score = 0;
    private int level = 1;
    private List<GameObject> letterButtons = new List<GameObject>();
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
        nextLevelButton.onClick.AddListener(() => NextLevel());
    }

    private void GenerateLevel()
    {
        ClearLetterButtons();

        // Select random letters for this level
        string[] selectedLetters = alphabet.OrderBy(x => System.Guid.NewGuid()).Take(lettersPerLevel).ToArray();
        FisherYatesShuffle.Shuffle(selectedLetters);

        // Choose target letter from selected letters
        currentTargetLetter = selectedLetters[Random.Range(0, selectedLetters.Length)];
        targetLetterText.text = "Find: " + currentTargetLetter;

        // Create letter buttons
        for (int i = 0; i < selectedLetters.Length; i++)
        {
            GameObject buttonObj = Instantiate(letterButtonPrefab, letterContainer);
            Button button = buttonObj.GetComponent<Button>();
            Text buttonText = buttonObj.GetComponentInChildren<Text>();

            string letter = selectedLetters[i];
            buttonText.text = letter;

            button.onClick.AddListener(() => CheckAnswer(letter, button));
            letterButtons.Add(buttonObj);
        }
    }

    private void CheckAnswer(string selectedLetter, Button button)
    {
        if (selectedLetter == currentTargetLetter)
        {
            // Correct answer
            button.image.color = correctColor;
            score += 10;
            gameManager.PlaySound(gameManager.successSound);

            Invoke("NextLevel", 1f);
        }
        else
        {
            // Wrong answer
            button.image.color = incorrectColor;
            gameManager.PlaySound(gameManager.failSound);

            // Reset color after 1 second
            Invoke("ResetButtonColors", 1f);
        }

        UpdateUI();
    }

    private void NextLevel()
    {
        level++;
        lettersPerLevel = Mathf.Min(lettersPerLevel + 1, 10); // Max 10 letters
        GenerateLevel();
        ResetButtonColors();
    }

    private void ResetButtonColors()
    {
        foreach (GameObject buttonObj in letterButtons)
        {
            if (buttonObj != null)
                buttonObj.GetComponent<Button>().image.color = Color.white;
        }
    }

    private void ClearLetterButtons()
    {
        foreach (GameObject buttonObj in letterButtons)
        {
            if (buttonObj != null)
                Destroy(buttonObj);
        }
        letterButtons.Clear();
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