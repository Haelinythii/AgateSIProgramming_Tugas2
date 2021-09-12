using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResourceController : MonoBehaviour
{
    public Text ResourceDescription;
    public Text ResourceUpgradeCost;
    public Text ResourceUnlockCost;

    public Button resourceButton;
    public Image resourceImage;

    private ResourceConfig _config;
    private int _level = 1;

    private bool isUnlocked;

    public bool IsUnlocked { get => isUnlocked; private set => isUnlocked = value; }

    private void Start()
    {
        resourceButton.onClick.AddListener(() => {
            if (IsUnlocked)
            {
                UpgradeLevel();
            }
            else
            {
                UnlockResource();
            }
        });
    }

    private void UnlockResource()
    {
        double unlockCost = GetUnlockCost();
        if(GameManager.Instance.TotalGold < unlockCost)
        {
            return;
        }

        GameManager.Instance.AddGold(-unlockCost);
        SetUnlocked(true);
        GameManager.Instance.ShowNextResource();

        AchievementController.Instance.UnlockAchievement(AchievementType.UnlockResource, _config.Name);
    }

    public void UpgradeLevel()
    {
        double upgradeCost = GetUpgradeCost();
        if(GameManager.Instance.TotalGold < upgradeCost)
        {
            return;
        }

        GameManager.Instance.AddGold(-upgradeCost);
        _level++;

        ResourceUpgradeCost.text = $"Upgrade Cost\n{ GetUpgradeCost() }";
        ResourceDescription.text = $"{ _config.Name } Lv. { _level }\n+{ GetOutput().ToString("0") }";
    }

    private void SetUnlocked(bool v)
    {
        IsUnlocked = v;
        resourceImage.color = IsUnlocked ? Color.white : Color.grey;
        ResourceUnlockCost.gameObject.SetActive(!v);
        ResourceUpgradeCost.gameObject.SetActive(v);
    }

    public void SetConfig(ResourceConfig config)
    {
        _config = config;
        
        ResourceDescription.text = $"{ _config.Name } Lv. { _level }\n+{ GetOutput().ToString("0") }";
        ResourceUnlockCost.text = $"Unlock Cost\n{ _config.UnlockCost }";
        ResourceUpgradeCost.text = $"Upgrade Cost\n{ GetUpgradeCost() }";

        SetUnlocked(_config.UnlockCost == 0);
    }

    public double GetOutput()
    {
        return _config.Output * _level;
    }

    public double GetUpgradeCost()
    {
        return _config.UpgradeCost * Mathf.Pow(_level, 3);
    }

    public double GetUnlockCost()
    {
        return _config.UnlockCost;
    }
}
