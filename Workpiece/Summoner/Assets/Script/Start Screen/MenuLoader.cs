using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuLoader : MonoBehaviour
{
   [SerializeField] private static bool isHUDLoaded = false;

    void Awake()
    {
        Debug.Log(isHUDLoaded);
        if (!isHUDLoaded)
        {
            SceneManager.LoadScene("Screen/HUD", LoadSceneMode.Additive);
            isHUDLoaded = true; // HUD가 로드되었음을 표시
        }
    }

}
