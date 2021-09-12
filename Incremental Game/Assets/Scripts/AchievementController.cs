using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AchievementController : MonoBehaviour
{
    private static AchievementController instance = null;

    public static AchievementController Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<AchievementController>();
            }
            return instance;
        }
    }

    [SerializeField] private Transform popUpTransform;
    [SerializeField] private Text popUpText;
    [SerializeField] private float popUpShowDuration = 3f;
    [SerializeField] private List<AchievementData> achievementList;

    private float popUpShowTimer;

    private void Update()
    {
        if (popUpShowTimer > 0)
        {
            popUpTransform.localScale = Vector3.LerpUnclamped(popUpTransform.localScale, Vector3.one, 0.5f);
            popUpShowTimer -= Time.unscaledDeltaTime;
        }
        else
        {
            popUpTransform.localScale = Vector2.LerpUnclamped(popUpTransform.localScale, Vector3.right, 0.5f);
        }

    }

    public void UnlockAchievement(AchievementType type, string value)
    {
        AchievementData achievementData = achievementList.Find(a => a.achievementType == type && a.value == value);

        if (achievementData != null && !achievementData.isUnlocked)
        {
            achievementData.isUnlocked = true;
            ShowAchivementPopUp(achievementData);
        }
    }

    private void ShowAchivementPopUp(AchievementData achievementData)
    {
        popUpText.text = achievementData.title;
        popUpShowTimer = popUpShowDuration;
        popUpTransform.localScale = Vector2.right;
    }

}

[Serializable]
public class AchievementData
{
    public string title;
    public AchievementType achievementType;
    public string value;
    public bool isUnlocked;
}

public enum AchievementType {
    UnlockResource
}
