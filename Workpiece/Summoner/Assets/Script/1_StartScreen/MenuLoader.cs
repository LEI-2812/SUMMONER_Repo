using UnityEngine;

// 역할: 시작 화면에서 메뉴 씬으로 이동하는 흐름을 담당한다.
public class MenuLoader : MonoBehaviour
{
    // Start is called before the first frame update
    void Awake()
    {
        StartGameFlow.LoadStartScreenHud();
    }

}
