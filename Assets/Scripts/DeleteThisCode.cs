using UnityEngine;

public class DeleteThisCode : MonoBehaviour //attach to any object then click top left
{
    [SerializeField] private float speedup = 10;
    private void OnGUI()
    {
        Rect rect = new Rect(0, 0, 200, 100);
        if (GUI.Button(rect, "Click"))
        {
            Time.timeScale = speedup;
        }
        
    }
}
