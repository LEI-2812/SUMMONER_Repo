using UnityEngine;
using UnityEngine.EventSystems;

// 역할: 스토리 화면의 메인 버튼 클릭 표시와 입력을 담당한다.
public class MainSceneButtonView : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        MenuNavigationFlow.ReturnToStartScreen();
    }
}
