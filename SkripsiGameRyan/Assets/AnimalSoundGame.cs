using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class AnimalSoundGame : MonoBehaviour
{
    [Header("Game Elements")]
    public Transform animalContainer;
    public GameObject animalButtonPrefab;
    public Button playSoundButton;
    public Text instructionText;
    public Text scoreText;

    [Header("Animal Data")]
    public AudioClip[] animalSounds;
    public Sprite[] animalImages;
    public string[] animalNames;

    private int currentAnimalIndex;
    private int score = 0;
    private int level = 1;
    private List<GameObject> animalButtons = new List<GameObject>();
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
        ClearAnimalButtons();

        // Select random animal
        currentAnimalIndex = Random.Range(0, animalSounds.Length);
        instructionText.text = "Listen and find the animal!";

        // Create animal options
        List<int> animalIndices = new List<int>();
        animalIndices.Add(currentAnimalIndex);

        // Add wrong animals
        for (int i = 0; i < 5; i++)
        {
            int wrongIndex;
            do
            {
                wrongIndex = Random.Range(0, animalImages.Length);
            } while (animalIndices.Contains(wrongIndex));

            animalIndices.Add(wrongIndex);
        }

        // Shuffle animals
        FisherYatesShuffle.Shuffle(animalIndices);

        // Create animal buttons
        foreach (int animalIndex in animalIndices)
        {
            GameObject buttonObj = Instantiate(animalButtonPrefab, animalContainer);
            Button button = buttonObj.GetComponent<Button>();
            Image animalImage = buttonObj.GetComponent<Image>();

            animalImage.sprite = animalImages[animalIndex];
            button.onClick.AddListener(() => CheckAnswer(animalIndex, button));

            animalButtons.Add(buttonObj);
        }

        // Setup play sound button
        playSoundButton.onClick.RemoveAllListeners();
        playSoundButton.onClick.AddListener(PlayCurrentAnimalSound);

        // Auto play sound at start
        Invoke("PlayCurrentAnimalSound", 0.5f);
    }

    private void PlayCurrentAnimalSound()
    {
        if (currentAnimalIndex < animalSounds.Length && animalSounds[currentAnimalIndex] != null)
        {
            gameManager.audioSource.PlayOneShot(animalSounds[currentAnimalIndex]);
        }
    }

    private void CheckAnswer(int selectedAnimalIndex, Button button)
    {
        if (selectedAnimalIndex == currentAnimalIndex)
        {
            // Correct answer
            button.transform.localScale = Vector3.one * 1.3f;
            score += 25;
            gameManager.PlaySound(gameManager.successSound);

            Invoke("NextLevel", 2f);
        }
        else
        {
            // Wrong answer
            button.image.color = Color.gray;
            gameManager.PlaySound(gameManager.failSound);

            Invoke("ResetAnimalButtons", 1f);
        }

        UpdateUI();
    }

    private void NextLevel()
    {
        level++;
        GenerateLevel();
    }

    private void ResetAnimalButtons()
    {
        foreach (GameObject buttonObj in animalButtons)
        {
            if (buttonObj != null)
            {
                buttonObj.transform.localScale = Vector3.one;
                buttonObj.GetComponent<Image>().color = Color.white;
            }
        }
    }

    private void ClearAnimalButtons()
    {
        foreach (GameObject buttonObj in animalButtons)
        {
            if (buttonObj != null)
                Destroy(buttonObj);
        }
        animalButtons.Clear();
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