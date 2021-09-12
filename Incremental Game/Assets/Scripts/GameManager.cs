using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance = null;

    public static GameManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<GameManager>();
            }
            return _instance;
        }
    }

    public double TotalGold { get => _totalGold; set => _totalGold = value; }
    public double MaxGold { get => maxGold; set => maxGold = value; }

    [Range(0f, 1f)]
    public float AutoCollectPercentage = 0.1f;
    public ResourceConfig[] ResourcesConfigs;

    public Transform ResourcesParent;
    public Transform BoostParent;
    public ResourceController ResourcePrefab;
    public Boost BoostPrefab;

    public Text GoldInfo;
    public Text AutoCollectInfo;

    private List<ResourceController> _activeResources = new List<ResourceController>();
    private List<Boost> boostList = new List<Boost>();
    private float _collectSecond;

    private double _totalGold;
    private double maxGold;

    public TapText tapTextPrefab;
    public Transform coinIcon;

    private List<TapText> _tapTextPool = new List<TapText>();

    public Sprite[] resourcesSprites;

    public BoostConfig[] BoostConfigs;

    // Start is called before the first frame update
    void Start()
    {
        AddAllResources();
        AddAllBoosts();
    }

    private void AddAllBoosts()
    {
        foreach (BoostConfig config in BoostConfigs)
        {
            GameObject GO = Instantiate(BoostPrefab.gameObject, BoostParent, false);
            Boost boost = GO.GetComponent<Boost>();

            boost.SetBoost(config);

            GO.gameObject.SetActive(true);

            boostList.Add(boost);
        }
    }

    private void AddAllResources()
    {
        bool showResource = true;
        foreach (ResourceConfig config in ResourcesConfigs)
        {
            GameObject GO = Instantiate(ResourcePrefab.gameObject, ResourcesParent, false);
            ResourceController resource = GO.GetComponent<ResourceController>();

            resource.SetConfig(config);

            GO.gameObject.SetActive(showResource);
            if (showResource && !resource.IsUnlocked)
            {
                showResource = false;
            }

            _activeResources.Add(resource);
        }
    }

    // Update is called once per frame
    void Update()
    {
        _collectSecond += Time.unscaledDeltaTime;
        if(_collectSecond >= 1f)
        {
            CollectPerSecond();
            _collectSecond = 0f;
        }

        
        coinIcon.transform.Rotate(0f, 0f, Time.deltaTime * -100f);
        coinIcon.transform.localScale = Vector3.LerpUnclamped(coinIcon.transform.localScale, Vector3.one, 0.15f);

        CheckResourceCost();
    }

    private void CheckResourceCost()
    {
        foreach (ResourceController resource in _activeResources)
        {
            bool isResourceBuyable = false;
            if (resource.IsUnlocked)
            {
                isResourceBuyable = TotalGold >= resource.GetUpgradeCost();
            }
            else
            {
                isResourceBuyable = TotalGold >= resource.GetUnlockCost();
            }

            resource.resourceImage.sprite = resourcesSprites[isResourceBuyable ? 1 : 0];
        }
    }

    private void CollectPerSecond()
    {
        double output = 0;
        foreach (ResourceController resource in _activeResources)
        {
            if (resource.IsUnlocked)
            {
                output += resource.GetOutput();
            }
        }

        Boost boostAuto = boostList.Find(b => b.BoostConfig.boostType == BoostType.AUTO_COLLECT_MONEY);
        if (boostAuto.IsActivated)
            output = output + output * boostAuto.GetMultiplier();

        output *= AutoCollectPercentage;
        AutoCollectInfo.text = $"Auto Collect: { output.ToString("F1") } / second";

        AddGold(output);
    }

    public void AddGold(double value)
    {
        TotalGold += value;

        if(TotalGold > MaxGold)
        {
            MaxGold = TotalGold;
        }

        GoldInfo.text = $"Gold: { TotalGold.ToString("0") }";
    }

    public void CollectByTap(Vector3 tapPosition, Transform parent)
    {
        double output = 0;
        foreach (ResourceController resource in _activeResources)
        {
            if (resource.IsUnlocked)
            {
                output += resource.GetOutput();
            }
        }

        TapText tapText = GetOrCreateTapText();
        tapText.transform.SetParent(parent, false);
        tapText.transform.position = tapPosition;

        Boost boostTap = boostList.Find(b => b.BoostConfig.boostType == BoostType.TAP_MONEY);
        if(boostTap.IsActivated)
            output = output + output * boostTap.GetMultiplier();

        tapText.text.text = $"+{ output.ToString("0") }";
        tapText.gameObject.SetActive(true);
        coinIcon.transform.localScale = Vector3.one * 1.75f;

        AddGold(output);

        AudioManager.instance.Play("Coin");
    }

    private TapText GetOrCreateTapText()
    {
        TapText tapText = _tapTextPool.Find(t => !t.gameObject.activeSelf);
        if(tapText == null)
        {
            tapText = Instantiate(tapTextPrefab).GetComponent<TapText>();
            _tapTextPool.Add(tapText);
        }

        return tapText;
    }

    public void ShowNextResource()
    {
        foreach (ResourceController resource in _activeResources)
        {
            if (!resource.gameObject.activeSelf)
            {
                resource.gameObject.SetActive(true);
                break;
            }
        }
    }
}

[System.Serializable]
public struct ResourceConfig
{
    public string Name;
    public double UnlockCost;
    public double UpgradeCost;
    public double Output;
}

[System.Serializable]
public struct BoostConfig
{
    public string Name;
    public float Duration;
    public double CostPercentageFromTotalGold;
    public double multiplier;
    public BoostType boostType;
}

public enum BoostType
{
    TAP_MONEY, AUTO_COLLECT_MONEY
}