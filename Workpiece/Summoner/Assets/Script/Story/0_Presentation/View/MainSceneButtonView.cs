using UnityEngine;
using UnityEngine.EventSystems;

// 역할: MainSceneButtonView의 책임을 정의한다.
public class MainSceneButtonView : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        GameSceneUseCase.LoadStartScreen();
    }
}
