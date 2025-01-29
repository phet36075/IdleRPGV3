using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class GotoMainStage : MonoBehaviour
{
    public string stageName;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GoMainStage()
    {
        SceneManager.LoadScene(stageName);
    }
}
