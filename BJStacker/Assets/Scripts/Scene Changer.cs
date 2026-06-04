using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public void ChangeScene(int sceneIndex)
    {
      SceneManager.LoadScene(sceneIndex);
    }
}
