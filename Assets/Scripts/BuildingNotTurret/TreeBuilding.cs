using System;
using NUnit.Framework;
using UnityEngine;

// 修改继承关系，不再继承自 Building
public class TreeBuilding : MonoBehaviour
{
    [SerializeField] private TreeSO treeCard;
    // 添加原来在 Building 中的属性
    [SerializeField] protected UnityEngine.UI.Slider healthSlider;
    protected int health;
    protected int maxHealth;
    
    public TreeSO TreeCard
    {
        set
        {
            treeCard = value;
            SetUp();
        }
    }
    
    public void BuildingCard(Card card)
    { 
        treeCard = (TreeSO)card;
        GetComponent<SpriteRenderer>().sprite = treeCard.cardSprite;
        maxHealth = treeCard.healthPoints;
        health = maxHealth;
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
        if (treeCard != null)
        {
            GameManager.instance.humanSpawnSpeedUp -= treeCard.speedUpSpawnFollower;
        }
    }
    
    // 覆盖基类的 TakeDamage 方法，使其不执行任何操作
    public virtual void TakeDamage(int damage)
    {
        // 不执行任何操作，忽略所有伤害
        Debug.Log("树建筑无法被攻击");
    }
}