using UnityEngine;

public class ParentSummonImage : MonoBehaviour
{
    [Header("소환수 설정")]
    [SerializeField] private Summon summon; // 배치할 소환수 프리팹

    private BattleBoardInputController plate; // 소환수를 배치할 플레이트

    private void Awake()
    {
        // 같은 오브젝트에 연결된 플레이트를 찾는다.
        plate = GetComponent<BattleBoardInputController>();

        if (summon == null)
        {
            Debug.Log("배치할 소환수가 없습니다.");
            return;
        }

        if (plate == null)
        {
            Debug.Log("소환수를 배치할 플레이트가 없습니다.");
            return;
        }

        plate.SummonPlaceOnPlate(summon, isResummon: false);
    }
}
