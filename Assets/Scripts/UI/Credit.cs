using UnityEngine;

public class Credit : MonoBehaviour
{
    [SerializeField] private GameObject creditPanel, buttonPanel;

    public void OpenCreditPanel()
    {
        if (!creditPanel || !buttonPanel) return;
        creditPanel.SetActive(true);
        buttonPanel.SetActive(false);
    }

    public void CloseCreditPanel()
    {
        if (!creditPanel || !buttonPanel) return;
        creditPanel.SetActive(false);
        buttonPanel.SetActive(true);
    }
}
