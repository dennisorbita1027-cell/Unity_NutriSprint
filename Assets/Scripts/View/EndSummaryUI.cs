using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Text;

public class EndSummaryUI : MonoBehaviour
{
    [Header("Score Text")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI highScoreText;
    public TextMeshProUGUI questText;

    [Header("Food Counts")]
    public TextMeshProUGUI goFoodCountText;
    public TextMeshProUGUI fruitCountText;
    public TextMeshProUGUI veggieCountText;
    public TextMeshProUGUI meatCountText;
    public TextMeshProUGUI junkCountText;

    private const string HIGH_SCORE_KEY = "HighScore";
    private const string QUEST_COUNT_KEY = "CompletedQuestCount";

    [SerializeField]
    public static string[] achievementTips =
    {
        // 1-4
        "Ang pechay at karot ay may bitamina A na tumutulong sa mata at nagpapalakas ng katawan.",
        "Ang mansanas at saging ay may natural na asukal na nagbibigay ng enerhiya.",
        "Ang karne, manok, at isda ay may protina na nagpapalakas ng masel sa katawan.",
        "Ang kanin at tinapay ay Go food na nagbibigay lakas sa buong araw.",
        // 5-8
        "Ang broccoli at spinach ay may bitamina C at calcium para sa malakas na buto.",
        "Ang mangga ay may bitamina A at C na panlaban sa sakit.",
        "Ang itlog ay may protina na tumutulong lumakas ang katawan.",
        "Ang oatmeal ay Go food na nagbibigay enerhiya sa umaga.",
        // 9-12
        "Ang kalabasa ay may bitamina A na mabuti sa mata at balat.",
        "Ang pakwan ay may maraming tubig at bitamina C para manatiling hydrated.",
        "Ang isda ay may protina at omega-3 na nagpapalakas ng puso at masel sa katawan.",
        "Ang kamote ay Go food na may bitamina A at energy na nakakabusog.",
        // 13-16
        "Ang talong at sitaw ay may bitamina at mineral para lumakas ang katawan.",
        "Ang pinya ay may bitamina C at fiber para sa maayos na tiyan.",
        "Ang manok ay may protina na tumutulong sa paglaki ng masel sa katawan.",
        "Ang mais ay Go food na nagbibigay dagdag lakas sa katawan.",
        // 17-20
        "Ang repolyo ay may fiber at bitamina C na mabuti sa tiyan.",
        "Ang ubas ay may bitamina C at antioxidants para sa puso.",
        "Ang baboy at baka ay may protina na kailangan ng katawan.",
        "Ang whole grain na tinapay ay Go food na nagbibigay lakas sa aktibong bata.",
        // 21-24
        "Ang ampalaya ay may bitamina C at fiber para manatiling malusog.",
        "Ang saging ay may potassium at natural na asukal para sa enerhiya bago maglaro.",
        "Ang tokwa ay protina para lumakas ang katawan at masel sa katawan.",
        "Ang brown rice ay Go food na may carbohydrates para sa lakas buong araw.",
        // 25-28
        "Ang kangkong ay may bitamina A at iron para sa dugo.",
        "Ang mansanas ay may fiber at bitamina C na masustansyang meryenda.",
        "Ang hipon at isda ay may protina at omega-3 para lumaki at lumakas.",
        "Ang patatas ay Go food na may carbohydrates at bitamina B para sa enerhiya.",
        // 29-32
        "Ang karot ay may bitamina A para sa malinaw na paningin.",
        "Ang mangga ay may bitamina C na nagpapalakas ng resistensya.",
        "Ang lean meat ay may protina para sa malakas na masel sa katawan.",
        "Ang pasta ay Go food na nagbibigay lakas sa katawan.",
        // 33-36
        "Ang pechay ay may calcium at bitamina A para sa malakas na buto.",
        "Ang strawberry ay may bitamina C at antioxidants na mabuti sa balat.",
        "Ang gatas ay may protina at calcium para lumaki at maging matibay ang katawan.",
        "Ang cereal ay Go food na nagbibigay enerhiya at sigla sa umaga.",
        // 37-40
        "Ang okra ay may fiber at bitamina C para sa tiyan at kalusugan.",
        "Ang kahel ay may bitamina C na tumutulong laban sa sipon at ubo.",
        "Ang mani ay may protina at healthy fats na nagbibigay enerhiya.",
        "Ang kanin ay pangunahing Go food ng mga Pilipino na nagbibigay lakas sa araw-araw."
    };

    void Start()
    {
        gameObject.SetActive(false);
    }

    public void Show()
    {
        gameObject.SetActive(true);

        UpdateAudioState();
        ProcessScores(out int finalScore, out int currentHighScore);
        RenderSummaryTexts(finalScore, currentHighScore);
        RenderQuestTips();
    }

    private void UpdateAudioState()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.StopBGM();
            AudioManager.Instance.PlayMainMenuBGM();
        }
    }

    private void ProcessScores(out int currentScore, out int highScore)
    {
        currentScore = (GameManager.Instance != null) ? GameManager.Instance.score : 0;
        highScore = PlayerPrefs.GetInt(HIGH_SCORE_KEY, 0);

        if (currentScore > highScore)
        {
            highScore = currentScore;
            PlayerPrefs.SetInt(HIGH_SCORE_KEY, highScore);
            PlayerPrefs.Save();
        }
    }

    private void RenderSummaryTexts(int finalScore, int highScore)
    {
        scoreText.text = $"Score: {finalScore}";
        highScoreText.text = $"Highest Score: {highScore}";

        if (GameManager.Instance == null) return;

        // Fetch data through safe interface channels
        goFoodCountText.text = $"Go Food: {GameManager.Instance.GetCollectedCount(CollectibleType.GoFood)}";
        fruitCountText.text = $"Prutas: {GameManager.Instance.GetCollectedCount(CollectibleType.Prutas)}";
        veggieCountText.text = $"Gulay: {GameManager.Instance.GetCollectedCount(CollectibleType.Gulay)}";
        meatCountText.text = $"Pagkaing Karne: {GameManager.Instance.GetCollectedCount(CollectibleType.PagkaingKarne)}";
        junkCountText.text = $"Junk Food: {GameManager.Instance.GetCollectedCount(CollectibleType.JunkFood)}";
    }

    private void RenderQuestTips()
    {
        if (GameManager.Instance == null) return;

        int newQuests = GameManager.Instance.GetNewQuestsThisRound();
        int totalCompleted = PlayerPrefs.GetInt(QUEST_COUNT_KEY, 0);

        if (newQuests <= 0)
        {
            questText.text = "Mangolekta ng mas maraming masusustansiyang pagkain para makakuha ng mas maraming kaalamang pangkalusugan!";
            return;
        }

        int startTipIndex = totalCompleted - newQuests;

        StringBuilder tipContainer = new StringBuilder();

        for (int i = startTipIndex; i < totalCompleted; i++)
        {
            if (i >= achievementTips.Length) break;
            tipContainer.Append($"• {achievementTips[i]}\n\n");
        }

        questText.text = tipContainer.ToString();
    }

    //Button Event 
    public void PlayAgain()
    {
        Time.timeScale = 1f;

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.StopBGM();
            AudioManager.Instance.PlayGameplayBGM();
        }

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void MainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
    }
}