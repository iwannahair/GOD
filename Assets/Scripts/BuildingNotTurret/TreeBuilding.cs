using NUnit.Framework;
using UnityEngine;

public class TreeBuilding : Building
{
    private Card buildingCard;
    public Card BuildingCard
    {
        set
        {
            buildingCard = value;
            SetUp();
        }
    }

    private void SetUp()
    {
        if (buildingCard == null) return;
        
    }
}
