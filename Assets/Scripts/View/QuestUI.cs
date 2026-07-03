using UnityEngine;
using TMPro;
using System.Collections;

public class QuestUI : MonoBehaviour
{
    public TextMeshProUGUI questText;

    [Header("Quest Settings")]
    public CollectibleType[] questOrder;
    public float completionMessageTime = 2.5f;

    private int questIndex;
    private int targetAmount;
    private bool showingCompletion = false;
    private bool questFinishedThisStep = false;
    private int questStartCount;

    void Start()
    {
        //PlayerPrefs.DeleteAll();
        LoadProgress();
        UpdateQuestText();

    }

    void Update()
    {
        if (showingCompletion) return;

        CollectibleType currentType = questOrder[questIndex];
        if (questOrder.Length == 0) return;

        int totalCollected = GameManager.Instance.GetCollectedCount(currentType);
        int currentCount = totalCollected - questStartCount;

        questText.text = $"Kumolokta ng {currentType} ({currentCount}/{targetAmount})";

        if (currentCount >= targetAmount && !questFinishedThisStep)
        {
            questFinishedThisStep = true;
            StartCoroutine(QuestCompleted(currentType));
        }

    }

    IEnumerator QuestCompleted(CollectibleType completedType)
    {
        showingCompletion = true;
        questText.text = $"May na-unlock kang payo!";

        Debug.Log("Quest Completed");
        yield return new WaitForSeconds(completionMessageTime);
        int completed = PlayerPrefs.GetInt("CompletedQuestCount", 0);
        PlayerPrefs.SetInt("CompletedQuestCount", completed + 1);
        PlayerPrefs.Save();

        MoveToNextQuest();
        showingCompletion = false;
    }

    void MoveToNextQuest()
    {
        questIndex++;
        if (questIndex >= questOrder.Length)
        {
            questIndex = 0;
            targetAmount++;
        }
        questFinishedThisStep = false;
        SaveProgress();
        UpdateQuestText();
    }

    void UpdateQuestText()
    {
        CollectibleType currentType = questOrder[questIndex];
        questStartCount = GameManager.Instance.GetCollectedCount(currentType);
        questText.text = $"Kumolekta ng {currentType} (0/{targetAmount})";
    }


    void SaveProgress()
    {
        PlayerPrefs.SetInt("QuestIndex", questIndex);
        PlayerPrefs.SetInt("TargetAmount", targetAmount);
        PlayerPrefs.Save();
    }

    void LoadProgress()
    {
        questIndex = PlayerPrefs.GetInt("QuestIndex", 0);
        targetAmount = PlayerPrefs.GetInt("TargetAmount", 1);

    }

}
