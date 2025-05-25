using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class HMSAMData
{
    public float perceivedEaseOfUse;
    public float joy;
    public float immersion;
    public float curiosity;
    public float perceivedUsefulness;
    public float behavioralIntention;
    public int totalPlayTime;
    public int levelsCompleted;
    public int totalScore;
}

public class HMSAMEvaluator : MonoBehaviour
{
    private static HMSAMEvaluator instance;
    public static HMSAMEvaluator Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<HMSAMEvaluator>();
                if (instance == null)
                {
                    GameObject go = new GameObject("HMSAMEvaluator");
                    instance = go.AddComponent<HMSAMEvaluator>();
                    DontDestroyOnLoad(go);
                }
            }
            return instance;
        }
    }

    private HMSAMData evaluationData;
    private float sessionStartTime;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeEvaluation();
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void InitializeEvaluation()
    {
        evaluationData = new HMSAMData();
        sessionStartTime = Time.time;
    }

    public void RecordGameSession(GameType gameType, int score, int levelsCompleted)
    {
        evaluationData.totalScore += score;
        evaluationData.levelsCompleted += levelsCompleted;
        evaluationData.totalPlayTime = Mathf.RoundToInt(Time.time - sessionStartTime);

        // Calculate HMSAM metrics based on performance
        CalculateHMSAMMetrics();
    }

    private void CalculateHMSAMMetrics()
    {
        // Perceived Ease of Use (based on completion rate)
        float completionRate = evaluationData.levelsCompleted > 0 ?
            (float)evaluationData.totalScore / evaluationData.levelsCompleted : 0f;
        evaluationData.perceivedEaseOfUse = Mathf.Clamp01(completionRate / 100f);

        // Joy (based on play time and engagement)
        float playTimeScore = Mathf.Clamp01(evaluationData.totalPlayTime / 300f); // 5 minutes = max joy
        evaluationData.joy = playTimeScore;

        // Immersion (based on continuous play and level progression)
        float immersionScore = evaluationData.levelsCompleted > 5 ? 0.8f :
                              evaluationData.levelsCompleted * 0.15f;
        evaluationData.immersion = Mathf.Clamp01(immersionScore);

        // Curiosity (based on different games played)
        evaluationData.curiosity = Mathf.Clamp01(0.6f + (evaluationData.levelsCompleted * 0.05f));

        // Perceived Usefulness (based on educational content completion)
        evaluationData.perceivedUsefulness = Mathf.Clamp01(0.7f + (completionRate / 200f));

        // Behavioral Intention (based on overall engagement)
        evaluationData.behavioralIntention = (evaluationData.joy + evaluationData.immersion +
                                            evaluationData.curiosity) / 3f;
    }

    public HMSAMData GetEvaluationData()
    {
        return evaluationData;
    }

    public void ResetEvaluation()
    {
        InitializeEvaluation();
    }

    public void SaveEvaluationToPlayerPrefs()
    {
        PlayerPrefs.SetFloat("HMSAM_EaseOfUse", evaluationData.perceivedEaseOfUse);
        PlayerPrefs.SetFloat("HMSAM_Joy", evaluationData.joy);
        PlayerPrefs.SetFloat("HMSAM_Immersion", evaluationData.immersion);
        PlayerPrefs.SetFloat("HMSAM_Curiosity", evaluationData.curiosity);
        PlayerPrefs.SetFloat("HMSAM_Usefulness", evaluationData.perceivedUsefulness);
        PlayerPrefs.SetFloat("HMSAM_BehavioralIntention", evaluationData.behavioralIntention);
        PlayerPrefs.SetInt("HMSAM_PlayTime", evaluationData.totalPlayTime);
        PlayerPrefs.SetInt("HMSAM_LevelsCompleted", evaluationData.levelsCompleted);
        PlayerPrefs.SetInt("HMSAM_TotalScore", evaluationData.totalScore);
        PlayerPrefs.Save();
    }
}