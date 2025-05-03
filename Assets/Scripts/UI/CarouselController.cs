using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CarouselController : MonoBehaviour
{
    [Header("轮播图设置")]
    [SerializeField] private Image mainImageDisplay;
    [SerializeField] private Sprite[] carouselImages;
    [SerializeField] private Button leftButton;
    [SerializeField] private Button rightButton;
    [SerializeField] private Button closeButton;
    [SerializeField] private string nextSceneName = "Gamescene";

    [Header("指示器设置")]
    [SerializeField] private Image[] indicators;
    [SerializeField] private Sprite activeIndicator;
    [SerializeField] private Sprite inactiveIndicator;

    private int currentImageIndex = 0;

    private void Start()
    {
        // 初始化按钮监听
        leftButton.onClick.AddListener(ShowPreviousImage);
        rightButton.onClick.AddListener(ShowNextImage);
        closeButton.onClick.AddListener(CloseCarouselAndLoadScene);

        // 初始化显示第一张图片
        UpdateDisplay();
    }

    private void ShowNextImage()
    {
        currentImageIndex = (currentImageIndex + 1) % carouselImages.Length;
        UpdateDisplay();
    }

    private void ShowPreviousImage()
    {
        currentImageIndex = (currentImageIndex - 1 + carouselImages.Length) % carouselImages.Length;
        UpdateDisplay();
    }

    private void UpdateDisplay()
    {
        // 更新主图片
        mainImageDisplay.sprite = carouselImages[currentImageIndex];

        // 更新指示器状态
        for (int i = 0; i < indicators.Length; i++)
        {
            indicators[i].sprite = (i == currentImageIndex) ? activeIndicator : inactiveIndicator;
        }
    }

    private void CloseCarouselAndLoadScene()
    {
        SceneManager.LoadScene(nextSceneName);
    }

    // 可以添加点击指示器直接跳转到对应图片的功能
    public void JumpToImage(int index)
    {
        if (index >= 0 && index < carouselImages.Length)
        {
            currentImageIndex = index;
            UpdateDisplay();
        }
    }
}