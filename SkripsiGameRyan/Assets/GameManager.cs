// ===== MAIN GAME MANAGER =====
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject mainMenuPanel;
    public GameObject gameSelectionPanel;
    public Button[] gameButtons;

    [Header("Game Panels")]
    public GameObject alphabetGamePanel;
    public GameObject colorShapeGamePanel;
    public GameObject numberGamePanel;
    public GameObject animalSoundGamePanel;
    public GameObject adventureGamePanel;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip buttonClickSound;
    public AudioClip successSound;
    public AudioClip failSound;

    private void Start()
    {
        ShowMainMenu();
        InitializeGameButtons();
    }

    private void InitializeGameButtons()
    {
        if (gameButtons.Length >= 5)
        {
            gameButtons[0].onClick.AddListener(() => StartGame(GameType.AlphabetPuzzle));
            gameButtons[1].onClick.AddListener(() => StartGame(GameType.ColorShape));
            gameButtons[2].onClick.AddListener(() => StartGame(GameType.NumberMatching));
            gameButtons[3].onClick.AddListener(() => StartGame(GameType.AnimalSound));
            gameButtons[4].onClick.AddListener(() => StartGame(GameType.Adventure));
        }
    }

    public void ShowMainMenu()
    {
        HideAllPanels();
        mainMenuPanel.SetActive(true);
    }

    public void ShowGameSelection()
    {
        HideAllPanels();
        gameSelectionPanel.SetActive(true);
        PlaySound(buttonClickSound);
    }

    private void HideAllPanels()
    {
        mainMenuPanel.SetActive(false);
        gameSelectionPanel.SetActive(false);
        alphabetGamePanel.SetActive(false);
        colorShapeGamePanel.SetActive(false);
        numberGamePanel.SetActive(false);
        animalSoundGamePanel.SetActive(false);
        adventureGamePanel.SetActive(false);
    }

    public void StartGame(GameType gameType)
    {
        HideAllPanels();
        PlaySound(buttonClickSound);

        switch (gameType)
        {
            case GameType.AlphabetPuzzle:
                alphabetGamePanel.SetActive(true);
                alphabetGamePanel.GetComponent<AlphabetMatchGame>().StartGame();
                break;
            case GameType.ColorShape:
                colorShapeGamePanel.SetActive(true);
                colorShapeGamePanel.GetComponent<ColorShapeGame>().StartGame();
                break;
            case GameType.NumberMatching:
                numberGamePanel.SetActive(true);
                numberGamePanel.GetComponent<NumberMatchingGame>().StartGame();
                break;
            case GameType.AnimalSound:
                animalSoundGamePanel.SetActive(true);
                animalSoundGamePanel.GetComponent<AnimalSoundGame>().StartGame();
                break;
            case GameType.Adventure:
                adventureGamePanel.SetActive(true);
                adventureGamePanel.GetComponent<AdventureGame>().StartGame();
                break;
        }
    }

    public void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}

public enum GameType
{
    AlphabetPuzzle,
    ColorShape,
    NumberMatching,
    AnimalSound,
    Adventure
}