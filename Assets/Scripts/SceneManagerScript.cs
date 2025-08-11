using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 场景管理器脚本 - 负责场景切换和游戏重启
/// </summary>
public class SceneManagerScript : MonoBehaviour
{
    /// <summary>
    /// 加载下一个场景
    /// </summary>
    public void LoadNextScene()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextSceneIndex = (currentSceneIndex + 1) % SceneManager.sceneCountInBuildSettings;
        SceneManager.LoadScene(nextSceneIndex);
    }

    /// <summary>
    /// 重新加载当前场景（重启游戏）
    /// </summary>
    public void RestartGame()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex);
    }

    /// <summary>
    /// 退出游戏
    /// </summary>
    public void QuitGame()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }
}