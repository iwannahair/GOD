using System;
using NUnit.Framework;
using UnityEngine;

public class TreeBuilding : Building
{
    private TreeSO treeCard;
    public TreeSO BuildingCard
    {
        set
        {
            treeCard = value;
            SetUp();
        }
    }

    private void Start()
    {
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
