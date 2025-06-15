using UnityEngine;
using UnityEngine.SceneManagement;
public class SceneManagerScript : MonoBehaviour
{
     

     public void LoadScene(string sceneName)
     {
          SceneManager.LoadScene(sceneName);
     }
     public void LoadScene(int sceneIndex)
     {
          SceneManager.LoadScene(sceneIndex);
     }
     public void LoadNextScene( )
     {
          SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
     }
     public void QuitGame()
     {
          Application.Quit();
     }
}
