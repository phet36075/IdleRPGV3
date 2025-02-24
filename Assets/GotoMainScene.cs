using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public void LoadTestStage1()
    {
        SceneManager.LoadScene("TestStage1");
    }
}