using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StageController : MonoBehaviour
{

   public void stageLoader(int stage)
    {
        SceneManager.LoadScene("Stage" + stage.ToString());   
    }

    //�ӽ��ڵ�
    public void TestSceneLoad()
    {
        SceneManager.LoadScene("Fight Screen");
    }
}
