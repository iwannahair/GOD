using UnityEngine;
using UnityEngine.UI;

public class IndicatorButton : MonoBehaviour
{
    [SerializeField] private int imageIndex;
    [SerializeField] private CarouselController carouselController;

    private void Start()
    {
        Button button = GetComponent<Button>();
        if (button != null)
        {
            button.onClick.AddListener(OnClick);
        }
        else
        {
            Debug.LogError("指示器上没有Button组件！");
        }
    }

    private void OnClick()
    {
        carouselController.JumpToImage(imageIndex);
    }
}