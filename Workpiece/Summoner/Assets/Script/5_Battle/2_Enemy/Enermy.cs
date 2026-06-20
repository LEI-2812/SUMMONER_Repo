using UnityEngine;

// 역할: 적 턴 시작 시 적 공격 컨트롤러를 호출하고 턴 종료를 요청한다.
public class Enermy : MonoBehaviour
{
    [Header("컨트롤러")]
    [SerializeField] private TurnController turnController;
    [SerializeField] private EnermyAttackController enermyAttackController;

    private void Awake()
    {
        if (enermyAttackController == null)
        {
            enermyAttackController = GetComponent<EnermyAttackController>();
        }

        if (turnController == null)
        {
            turnController = FindObjectOfType<TurnController>();
        }

        if (enermyAttackController == null)
        {
            Debug.LogError("Enermy needs EnermyAttackController.");
        }

        if (turnController == null)
        {
            Debug.LogError("Enermy needs TurnController.");
        }
    }

    public void EnermyTurnStart()
    {
        Debug.Log("적 턴 시작");
        EnermyActionTake();
    }

    private void EnermyActionTake()
    {
        if (enermyAttackController == null)
        {
            Debug.LogError("Enermy cannot act without EnermyAttackController.");
            return;
        }

        Debug.Log("리스트를 가져와서 적 대응시작");
        enermyAttackController.EnermyAttackStart();

        EnermyTurnEnd();
    }

    private void EnermyTurnEnd()
    {
        if (turnController == null)
        {
            Debug.LogError("Enermy cannot end turn without TurnController.");
            return;
        }

        Debug.Log("적 턴 종료");
        turnController.EndCurrentTurn();
    }

}
