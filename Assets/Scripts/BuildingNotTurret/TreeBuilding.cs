using System;
using NUnit.Framework;
using UnityEngine;

public class TreeBuilding : Building
{
    [SerializeField] private TreeSO treeCard;
    public TreeSO TreeCard
    {
        set
        {
            treeCard = value;
            SetUp();
        }
    }
    private void BuildingCard(Card card)
    { 
        treeCard = (TreeSO)card; 
    }
    private void Start()
    {
        LoadCardData += BuildingCard;
        if (treeCard == null) return;
        SetUp();
    }

    private void SetUp()
    {
        if (treeCard == null) return;
        GameManager.instance.humanSpawnSpeedUp += treeCard.speedUpSpawnFollower;
    }

    private void OnDestroy()
    {
        GameManager.instance.humanSpawnSpeedUp -= treeCard.speedUpSpawnFollower;
    }
}
