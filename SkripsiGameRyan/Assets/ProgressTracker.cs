using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ProgressTracker : MonoBehaviour
{
    [Header("Progress UI")]
    public GameObject progressPanel;
    public Text totalScoreText;
    public Text totalPlayTimeText;
    public Text levelsCompletedText;
    public Slider[] gameProgressSliders;
    public Text[] gameProgressTexts;

    [Header("Achievement System")]
    public GameObject achievementPrefab;
    public Transform achievementContainer;
    public AudioClip achievementSound;

    private Dictionary<GameType, int> gameScores = new Dictionary<GameType, int>();
    private Dictionary<GameType, int> gameLevels = new Dictionary<GameType, int>();
    private List<string> unlockedAchievements = new List<string>();
    private GameManager gameManager;

    private void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
        InitializeProgress();
    }

    private void InitializeProgress()
    {
        // Initialize game progress tracking
        foreach (GameType gameType in System.Enum.GetValues(typeof(GameType)))
        {
            gameScores[gameType] = PlayerPrefs.GetInt("Score_" + gameType.ToString(), 0);
            gameLevels[gameType] = PlayerPrefs.GetInt("Levels_" + gameType.ToString(), 0);
        }

        LoadAchievements();
    }

    public void UpdateGameProgress(GameType gameType, int score, int levelsCompleted)
    {
        gameScores[gameType] += score;
        gameLevels[gameType] += levelsCompleted;

        // Save progress
        PlayerPrefs.SetInt("Score_" + gameType.ToString(), gameScores[gameType]);
        PlayerPrefs.SetInt("Levels_" + gameType.ToString(), gameLevels[gameType]);
        PlayerPrefs.Save();

        // Check for achievements
        CheckAchievements(gameType);

        // Update HMSAM evaluation
        HMSAMEvaluator.Instance.RecordGameSession(gameType, score, levelsCompleted);
    }

    public void ShowProgressPanel()
    {
        progressPanel.SetActive(true);
        UpdateProgressDisplay();
    }

    public void HideProgressPanel()
    {
        progressPanel.SetActive(false);
    }

    private void UpdateProgressDisplay()
    {
        int totalScore = 0;
        int totalLevels = 0;

        foreach (var score in gameScores.Values)
            totalScore += score;

        foreach (var levels in gameLevels.Values)
            totalLevels += levels;

        totalScoreText.text = "Total Score: " + totalScore;
        totalPlayTimeText.text = "Play Time: " + HMSAMEvaluator.Instance.GetEvaluationData().totalPlayTime + "s";
        levelsCompletedText.text = "Levels Completed: " + totalLevels;

        // Update individual game progress
        int gameIndex = 0;
        foreach (GameType gameType in System.Enum.GetValues(typeof(GameType)))
        {
            if (gameIndex < gameProgressSliders.Length)
            {
                float progress = gameLevels[gameType] / 20f; // Max 20 levels per game
                gameProgressSliders[gameIndex].value = Mathf.Clamp01(progress);
                gameProgressTexts[gameIndex].text = gameType.ToString() + ": " +
                                                   gameLevels[gameType] + " levels";
                gameIndex++;
            }
        }
    }

    private void CheckAchievements(GameType gameType)
    {
        // Achievement: First Steps
        if (gameLevels[gameType] >= 1 && !unlockedAchievements.Contains("FirstSteps_" + gameType))
        {
            UnlockAchievement("FirstSteps_" + gameType, "First Steps in " + gameType + "!");
        }

        // Achievement: Level Master
        if (gameLevels[gameType] >= 10 && !unlockedAchievements.Contains("LevelMaster_" + gameType))
        {
            UnlockAchievement("LevelMaster_" + gameType, "Level Master in " + gameType + "!");
        }

        // Achievement: High Scorer
        if (gameScores[gameType] >= 500 && !unlockedAchievements.Contains("HighScorer_" + gameType))
        {
            UnlockAchievement("HighScorer_" + gameType, "High Scorer in " + gameType + "!");
        }

        // Achievement: Completionist
        int totalLevels = 0;
        foreach (var levels in gameLevels.Values)
            totalLevels += levels;

        if (totalLevels >= 50 && !unlockedAchievements.Contains("Completionist"))
        {
            UnlockAchievement("Completionist", "Completionist - 50 levels total!");
        }
    }

    private void UnlockAchievement(string achievementId, string achievementText)
    {
        unlockedAchievements.Add(achievementId);

        // Create achievement notification
        GameObject achievementObj = Instantiate(achievementPrefab, achievementContainer);
        Text achievementTextComponent = achievementObj.GetComponentInChildren<Text>();
        achievementTextComponent.text = "Achievement Unlocked!\n" + achievementText;

        // Play achievement sound
        if (gameManager != null && achievementSound != null)
        {
            gameManager.PlaySound(achievementSound);
        }

        // Auto-hide achievement after 3 seconds
        Destroy(achievementObj, 3f);

        // Save achievement
        SaveAchievements();
    }

    private void SaveAchievements()
    {
        string achievementString = string.Join(",", unlockedAchievements.ToArray());
        PlayerPrefs.SetString("Achievements", achievementString);
        PlayerPrefs.Save();
    }

    private void LoadAchievements()
    {
        string achievementString = PlayerPrefs.GetString("Achievements", "");
        if (!string.IsNullOrEmpty(achievementString))
        {
            unlockedAchievements = new List<string>(achievementString.Split(','));
        }
    }
}