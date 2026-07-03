using UnityEngine;
using TMPro;
using System;

public class AchievementsUI : MonoBehaviour
{
    [Header("Stats Text")]
    public TMP_Text veggieText;
    public TMP_Text fruitText;
    public TMP_Text meatText;
    public TMP_Text goFoodText;
    public TMP_Text junkText;
    public TMP_Text timeText;
    public TMP_Text highScoreText;

    [Header("Tips")]
    public Transform tipsContent;
    public TMP_Text tipsText;
    public string[] allTips;

    [Header("Fruit Medals")]
    public GameObject fruitBronze;
    public GameObject fruitSilver;
    public GameObject fruitGold;
    public TMP_Text fruitMedalText;

    [Header("Veggie Medals")]
    public GameObject veggieBronze;
    public GameObject veggieSilver;
    public GameObject veggieGold;
    public TMP_Text veggieMedalText;

    [Header("Meat Medals")]
    public GameObject meatBronze;
    public GameObject meatSilver;
    public GameObject meatGold;
    public TMP_Text meatMedalText;

    [Header("Go Food Medals")]
    public GameObject goBronze;
    public GameObject goSilver;
    public GameObject goGold;
    public TMP_Text goMedalText;

    private const string SAVE_VEGGIE = "TotalVeggie";
    private const string SAVE_FRUIT = "TotalFruit";
    private const string SAVE_MEAT = "TotalMeat";
    private const string SAVE_GO_FOOD = "TotalGoFood";
    private const string SAVE_JUNK = "TotalJunk";
    private const string SAVE_TIME = "TotalTimeSurvived";
    private const string SAVE_HIGH_SCORE = "HighScore";
    private const string QUEST_COUNT_KEY = "CompletedQuestCount";

    private struct MedalAssets
    {
        public GameObject bronze;
        public GameObject silver;
        public GameObject gold;
        public TMP_Text label;
        public string bronzeTitle;
        public string silverTitle;
        public string goldTitle;

        public MedalAssets(GameObject b, GameObject s, GameObject g, TMP_Text l, string bt, string st, string gt)
        {
            bronze = b; silver = s; gold = g; label = l;
            bronzeTitle = bt; silverTitle = st; goldTitle = gt;
        }
    }

    void OnEnable()
    {
        CacheTips();
        LoadStatsAndAchievements();
        DisplayActiveTips();
    }

    private void CacheTips()
    {
        allTips = (EndSummaryUI.achievementTips != null) ? EndSummaryUI.achievementTips : new string[0];
    }

    private void LoadStatsAndAchievements()
    {
        //Fetch values
        int countVeggie = PlayerPrefs.GetInt(SAVE_VEGGIE, 0);
        int countFruit = PlayerPrefs.GetInt(SAVE_FRUIT, 0);
        int countMeat = PlayerPrefs.GetInt(SAVE_MEAT, 0);
        int countGo = PlayerPrefs.GetInt(SAVE_GO_FOOD, 0);
        int countJunk = PlayerPrefs.GetInt(SAVE_JUNK, 0);
        int highScore = PlayerPrefs.GetInt(SAVE_HIGH_SCORE, 0);
        float totalSeconds = PlayerPrefs.GetFloat(SAVE_TIME, 0f);

        //Render Text Layouts
        veggieText.text = $"Gulay: {countVeggie}";
        fruitText.text = $"Prutas: {countFruit}";
        meatText.text = $"Pagkaing Karne: {countMeat}";
        goFoodText.text = $"Go Foods: {countGo}";
        junkText.text = $"Junk Foods: {countJunk}";
        highScoreText.text = $"Highest Score: {highScore}";

        TimeSpan timeSpan = TimeSpan.FromSeconds(totalSeconds);
        timeText.text = $"Time Survived: {timeSpan.Hours:D2}:{timeSpan.Minutes:D2}:{timeSpan.Seconds:D2}";

        //Process Medal Systems
        UpdateMedalState(countFruit, new MedalAssets(fruitBronze, fruitSilver, fruitGold, fruitMedalText, "Fruit Friend", "Fruit Explorer", "Fruit Hero"));
        UpdateMedalState(countVeggie, new MedalAssets(veggieBronze, veggieSilver, veggieGold, veggieMedalText, "Veggie Buddy", "Veggie Warrior", "Veggie Champion"));
        UpdateMedalState(countMeat, new MedalAssets(meatBronze, meatSilver, meatGold, meatMedalText, "Protein Rookie", "Protein Pro", "Protein Power Hero"));
        UpdateMedalState(countGo, new MedalAssets(goBronze, goSilver, goGold, goMedalText, "Energy Starter", "Energy Booster", "Energy Champion"));
    }

    private void DisplayActiveTips()
    {
        tipsText.text = string.Empty;

        int completedQuests = PlayerPrefs.GetInt(QUEST_COUNT_KEY, 0);
        int displayLimit = Mathf.Min(completedQuests, allTips.Length);

        if (displayLimit > 0)
        {
            var stringBuilder = new System.Text.StringBuilder();
            for (int i = 0; i < displayLimit; i++)
            {
                stringBuilder.Append($"• {allTips[i]}\n\n");
            }
            tipsText.text = stringBuilder.ToString();
        }

        Debug.Log($"Showing {displayLimit} tips. Completed quests: {completedQuests}");
    }

    private void UpdateMedalState(int scoreTotal, MedalAssets assets)
    {
        //Disable all images cleanly upfront
        if (assets.bronze != null) assets.bronze.SetActive(false);
        if (assets.silver != null) assets.silver.SetActive(false);
        if (assets.gold != null) assets.gold.SetActive(false);

        if (scoreTotal >= 50)
        {
            if (assets.gold != null) assets.gold.SetActive(true);
            assets.label.text = $"({scoreTotal}) {assets.goldTitle}";
        }
        else if (scoreTotal >= 25)
        {
            if (assets.silver != null) assets.silver.SetActive(true);
            assets.label.text = $"({scoreTotal}/50) {assets.silverTitle}";
        }
        else if (scoreTotal >= 10)
        {
            if (assets.bronze != null) assets.bronze.SetActive(true);
            assets.label.text = $"({scoreTotal}/25) {assets.bronzeTitle}";
        }
        else
        {
            assets.label.text = "Mangolekta para sa medalya!";
        }
    }
}