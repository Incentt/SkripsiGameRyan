using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class AdventureGame : MonoBehaviour
{
    [Header("Game Elements")]
    public Transform questionContainer;
    public GameObject answerButtonPrefab;
    public Text questionText;
    public Text scoreText;
    public Text progressText;
    public Image characterImage;

    [Header("Adventure Settings")]
    public Sprite[] characterSprites;
    public int questionsPerAdventure = 10;

    private struct Question
    {
        public string questionText;
        public string[] answers;
        public int correctAnswerIndex;
    }

    private List<Question> allQuestions;
    private Queue<Question> currentQuestions;
    private int score = 0;
    private int currentQuestionIndex = 0;
    private List<GameObject> answerButtons = new List<GameObject>();
    private GameManager gameManager;

    private void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
        InitializeQuestions();
    }

    private void InitializeQuestions()
    {
        allQuestions = new List<Question>();

        // Alphabet questions
        allQuestions.Add(new Question
        {
            questionText = "What comes after A?",
            answers = new string[] { "B", "C", "D", "Z" },
            correctAnswerIndex = 0
        });

        // Number questions
        allQuestions.Add(new Question
        {
            questionText = "What is 2 + 2?",
            answers = new string[] { "3", "4", "5", "6" },
            correctAnswerIndex = 1
        });

        // Color questions
        allQuestions.Add(new Question
        {
            questionText = "What color do you get when you mix red and yellow?",
            answers = new string[] { "Green", "Orange", "Purple", "Blue" },
            correctAnswerIndex = 1
        });

        // Shape questions
        allQuestions.Add(new Question
        {
            questionText = "How many sides does a triangle have?",
            answers = new string[] { "2", "3", "4", "5" },
            correctAnswerIndex = 1
        });

        // Animal questions
        allQuestions.Add(new Question
        {
            questionText = "Which animal says 'Moo'?",
            answers = new string[] { "Dog", "Cat", "Cow", "Bird" },
            correctAnswerIndex = 2
        });

        // Add more questions as needed...
    }

    public void StartGame()
    {
        score = 0;
        currentQuestionIndex = 0;

        // Select and shuffle questions for this adventure
        List<Question> selectedQuestions = new List<Question>(allQuestions);
        FisherYatesShuffle.Shuffle(selectedQuestions);

        currentQuestions = new Queue<Question>();
        for (int i = 0; i < Mathf.Min(questionsPerAdventure, selectedQuestions.Count); i++)
        {
            currentQuestions.Enqueue(selectedQuestions[i]);
        }

        UpdateUI();
        DisplayNextQuestion();
    }

    private void DisplayNextQuestion()
    {
        if (currentQuestions.Count == 0)
        {
            EndAdventure();
            return;
        }

        ClearAnswerButtons();

        Question currentQuestion = currentQuestions.Dequeue();
        questionText.text = currentQuestion.questionText;

        // Shuffle answers
        List<string> shuffledAnswers = new List<string>(currentQuestion.answers);
        FisherYatesShuffle.Shuffle(shuffledAnswers);

        // Find new correct answer index after shuffle
        int newCorrectIndex = -1;
        for (int i = 0; i < shuffledAnswers.Count; i++)
        {
            if (shuffledAnswers[i] == currentQuestion.answers[currentQuestion.correctAnswerIndex])
            {
                newCorrectIndex = i;
                break;
            }
        }

        // Create answer buttons
        for (int i = 0; i < shuffledAnswers.Count; i++)
        {
            GameObject buttonObj = Instantiate(answerButtonPrefab, questionContainer);
            Button button = buttonObj.GetComponent<Button>();
            Text buttonText = buttonObj.GetComponentInChildren<Text>();

            buttonText.text = shuffledAnswers[i];

            int answerIndex = i;
            button.onClick.AddListener(() => CheckAnswer(answerIndex == newCorrectIndex, button));

            answerButtons.Add(buttonObj);
        }

        currentQuestionIndex++;
        UpdateUI();
    }

    private void CheckAnswer(bool isCorrect, Button button)
    {
        if (isCorrect)
        {
            button.image.color = Color.green;
            score += 30;
            gameManager.PlaySound(gameManager.successSound);
        }
        else
        {
            button.image.color = Color.red;
            gameManager.PlaySound(gameManager.failSound);
        }

        UpdateUI();
        Invoke("DisplayNextQuestion", 1.5f);
    }

    private void EndAdventure()
    {
        questionText.text = "Adventure Complete!\nFinal Score: " + score;
        ClearAnswerButtons();

        // Show completion character
        if (characterSprites.Length > 0)
        {
            characterImage.sprite = characterSprites[Random.Range(0, characterSprites.Length)];
            characterImage.gameObject.SetActive(true);
        }
    }

    private void ClearAnswerButtons()
    {
        foreach (GameObject buttonObj in answerButtons)
        {
            if (buttonObj != null)
                Destroy(buttonObj);
        }
        answerButtons.Clear();
    }

    private void UpdateUI()
    {
        scoreText.text = "Score: " + score;
        progressText.text = "Question " + currentQuestionIndex + " / " + questionsPerAdventure;
    }

    public void BackToMenu()
    {
        gameManager.ShowGameSelection();
    }
}