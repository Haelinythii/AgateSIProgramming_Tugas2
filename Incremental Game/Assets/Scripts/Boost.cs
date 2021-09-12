using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Boost : MonoBehaviour
{
    public Text boostDescription;
    public Text boostPricing;
    public Text boostTimer;

    public Button boostButton;
    private bool isActivated;

    private BoostConfig boostConfig;
    

    private float durationTimer = 5f;

    public bool IsActivated { get => isActivated; private set => isActivated = value; }
    public BoostConfig BoostConfig { get => boostConfig; set => boostConfig = value; }

    private void Start()
    {
        boostButton.onClick.AddListener(() => {
            BuyBoost();
        });
    }

    private void Update()
    {
        if(durationTimer > 0f)
        {
            durationTimer -= Time.unscaledDeltaTime;
            boostTimer.text = "Boost Timer\n" + durationTimer.ToString("0");
        }
        else if(durationTimer < 0f)
        {
            IsActivated = false;
            boostTimer.gameObject.SetActive(false);
            boostPricing.gameObject.SetActive(true);
            boostButton.interactable = true;
        }
    }

    private void BuyBoost()
    {
        AudioManager.instance.Play("MenuSelect");
        if (GameManager.Instance.TotalGold < GameManager.Instance.MaxGold * GetCostPercentageFromTotalGold())
        {
            return;
        }

        GameManager.Instance.AddGold(-(GameManager.Instance.MaxGold * GetCostPercentageFromTotalGold()));
        IsActivated = true;
        durationTimer = BoostConfig.Duration;
        boostTimer.gameObject.SetActive(true);
        boostPricing.gameObject.SetActive(false);
        boostButton.interactable = false;

        Debug.Log("buy boost = " + BoostConfig.Name);
    }

    public void SetBoost(BoostConfig config)
    {
        BoostConfig = config;
        durationTimer = config.Duration;

        boostDescription.text = $"{ BoostConfig.Name } \n+{ (GetMultiplier() * 100f).ToString("0") }% For {GetDuration()}s";
        boostPricing.text = $"Boost Cost\n{ GetCostPercentageFromTotalGold() * 100f}% Max Gold";
    }

    public double GetMultiplier()
    {
        return BoostConfig.multiplier;
    }

    public double GetDuration()
    {
        return BoostConfig.Duration;
    }

    public double GetCostPercentageFromTotalGold()
    {
        return BoostConfig.CostPercentageFromTotalGold;
    }
}
