using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private readonly Dictionary<CollectibleType, int> collectedCounts = new Dictionary<CollectibleType, int>();

    [Header("Score Settings")]
    public float timeSurvived = 0f;
    public int score = 0;
    public float scorePerSecond = 1f;
    private float scoreFloat = 0f;
    private int questCountAtRoundStart;

    public int goodFoodScore = 2;
    public int junkFoodPenalty = 10;

    [Header("Score Modifier UI")]
    public TextMeshProUGUI scoreModifierText;
    public float scoreModifierDuration = 0.8f;
    public float floatSpeed = 30f;

    private Vector3 scoreModifierStartPos;

    [Header("Tutorial UI")]
    public GameObject tutorialPanel;
    public bool isTutorialActive = true;

    [HideInInspector]
    public bool isGameOver = false;

    [Header("End Game UI")]
    public EndSummaryUI endSummaryUI;

    //Save Data Keys
    private const string SAVE_VEGGIE = "TotalVeggie";
    private const string SAVE_FRUIT = "TotalFruit";
    private const string SAVE_MEAT = "TotalMeat";
    private const string SAVE_GO_FOOD = "TotalGoFood";
    private const string SAVE_JUNK = "TotalJunk";
    private const string SAVE_TIME = "TotalTimeSurvived";
    private const string QUEST_COUNT_KEY = "CompletedQuestCount";

    void Awake()
    {
        if (InitializeSingleton())
        {
            InitializeCollectibleDictionary();
        }
    }

    void Start()
    {
        InitializeScoreModifierUI();
    }

    void Update()
    {
        if (isGameOver) return;

        if (isTutorialActive)
        {
            HandleTutorialInput();
            return;
        }

        UpdateGameProgress();
    }

    void OnEnable() => SceneManager.sceneLoaded += OnSceneLoaded;
    void OnDisable() => SceneManager.sceneLoaded -= OnSceneLoaded;

    //Scene Transitions
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        const int GAME_SCENE_INDEX = 1;
        if (scene.buildIndex != GAME_SCENE_INDEX) return;

        ResetRunState();
        MarkRoundStart();

        FindAndSetupSceneUI();

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayGameplayBGM();
        }
    }

    private bool InitializeSingleton()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            return true;
        }

        Destroy(gameObject);
        return false;
    }

    private void InitializeCollectibleDictionary()
    {
        collectedCounts.Clear();
        foreach (CollectibleType type in System.Enum.GetValues(typeof(CollectibleType)))
        {
            collectedCounts[type] = 0;
        }
    }

    private void InitializeScoreModifierUI()
    {
        if (scoreModifierText != null)
        {
            scoreModifierStartPos = scoreModifierText.rectTransform.localPosition;
            scoreModifierText.gameObject.SetActive(false);
        }
    }

    private void HandleTutorialInput()
    {
        if (Input.anyKeyDown)
        {
            StartGame();
        }
    }

    private void UpdateGameProgress()
    {
        timeSurvived += Time.deltaTime;
        scoreFloat += scorePerSecond * Time.deltaTime;
        score = Mathf.FloorToInt(scoreFloat);

        AnimateScoreModifier();
    }

    private void AnimateScoreModifier()
    {
        if (scoreModifierText != null && scoreModifierText.gameObject.activeSelf)
        {
            scoreModifierText.rectTransform.Translate(Vector3.up * floatSpeed * Time.deltaTime);
        }
    }

    private void FindAndSetupSceneUI()
    {
        tutorialPanel = GameObject.Find("TutorialPanel");
        if (tutorialPanel != null) ResetTutorial();
        else Debug.LogWarning("TutorialPanel not found in scene!");

        if (scoreModifierText == null)
        {
            scoreModifierText = GameObject.Find("ScoreModifierText")?.GetComponent<TextMeshProUGUI>();
            InitializeScoreModifierUI();

            if (scoreModifierText == null)
            {
                Debug.LogError("ScoreModifierText not found in scene!");
            }
        }

        endSummaryUI = FindAnyObjectByType<EndSummaryUI>();
        if (endSummaryUI == null)
        {
            Debug.LogError("EndSummaryUI missing in Game Scene!");
        }
    }

    //Gameplay States
    public void StartGame()
    {
        if (tutorialPanel != null) tutorialPanel.SetActive(false);
        Time.timeScale = 1f;
        isTutorialActive = false;
    }

    public void ResetTutorial()
    {
        if (tutorialPanel != null) tutorialPanel.SetActive(true);
        Time.timeScale = 0f;
        isTutorialActive = true;
    }

    public void EndGame()
    {
        SaveTotals();
        Time.timeScale = 0f;

        if (endSummaryUI == null)
        {
            endSummaryUI = FindAnyObjectByType<EndSummaryUI>();
        }

        if (endSummaryUI != null)
        {
            endSummaryUI.Show();
        }
        else
        {
            Debug.LogError("EndSummaryUI not found in scene!");
        }
    }

    //Scoring Logic
    public void RecordCollectible(CollectibleType type)
    {
        collectedCounts[type]++;

        if (type == CollectibleType.JunkFood)
        {
            ReduceScore();
            ShowScoreModifier(-junkFoodPenalty);
        }
        else
        {
            AddScore();
            ShowScoreModifier(goodFoodScore);
        }

        Debug.Log($"Collected {type} | Score: {score}");
    }

    public void AddScore()
    {
        scoreFloat += goodFoodScore;
        score = Mathf.FloorToInt(scoreFloat);
    }

    public void ReduceScore()
    {
        scoreFloat -= junkFoodPenalty;
        if (scoreFloat < 0f) scoreFloat = 0f;
        score = Mathf.FloorToInt(scoreFloat);
    }

    public int GetScore() => score;
    public int GetCollectedCount(CollectibleType type) => collectedCounts.TryGetValue(type, out int val) ? val : 0;

    public void SaveTotals()
    {
        PlayerPrefs.SetInt(SAVE_VEGGIE, GetCollectedCount(CollectibleType.Gulay) + PlayerPrefs.GetInt(SAVE_VEGGIE, 0));
        PlayerPrefs.SetInt(SAVE_FRUIT, GetCollectedCount(CollectibleType.Prutas) + PlayerPrefs.GetInt(SAVE_FRUIT, 0));
        PlayerPrefs.SetInt(SAVE_MEAT, GetCollectedCount(CollectibleType.PagkaingKarne) + PlayerPrefs.GetInt(SAVE_MEAT, 0));
        PlayerPrefs.SetInt(SAVE_GO_FOOD, GetCollectedCount(CollectibleType.GoFood) + PlayerPrefs.GetInt(SAVE_GO_FOOD, 0));
        PlayerPrefs.SetInt(SAVE_JUNK, GetCollectedCount(CollectibleType.JunkFood) + PlayerPrefs.GetInt(SAVE_JUNK, 0));
        PlayerPrefs.SetFloat(SAVE_TIME, timeSurvived + PlayerPrefs.GetFloat(SAVE_TIME, 0f));
        PlayerPrefs.Save();
    }

    public int GetNewQuestsThisRound()
    {
        int now = PlayerPrefs.GetInt(QUEST_COUNT_KEY, 0);
        return now - questCountAtRoundStart;
    }

    public void MarkRoundStart()
    {
        questCountAtRoundStart = PlayerPrefs.GetInt(QUEST_COUNT_KEY, 0);
    }

    public void ResetRunState()
    {
        timeSurvived = 0f;
        score = 0;
        scoreFloat = 0f;
        isGameOver = false;
        InitializeCollectibleDictionary();
    }

    public void ShowScoreModifier(int amount)
    {
        if (scoreModifierText == null) return;

        CancelInvoke(nameof(HideScoreModifier));
        scoreModifierText.rectTransform.localPosition = scoreModifierStartPos;
        scoreModifierText.gameObject.SetActive(true);

        if (amount >= 0)
        {
            scoreModifierText.text = $"+{amount}";
            scoreModifierText.color = Color.green;
        }
        else
        {
            scoreModifierText.text = amount.ToString();
            scoreModifierText.color = Color.red;
        }

        Invoke(nameof(HideScoreModifier), scoreModifierDuration);
    }

    private void HideScoreModifier()
    {
        if (scoreModifierText != null)
        {
            scoreModifierText.gameObject.SetActive(false);
        }
    }
    //!!!
    public void ClearAllPlayerProfileData()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();

        Debug.Log("Game Architecture: PlayerPrefs history successfully wiped clean.");

        ResetRunState();
        MarkRoundStart();
    }
}