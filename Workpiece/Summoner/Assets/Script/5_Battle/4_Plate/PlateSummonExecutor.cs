using UnityEngine;

// 역할: 플레이트에 이미 결정된 소환수 생성, 파괴, 이동을 실행한다.
public class PlateSummonExecutor
{
    public Summon CreateSummonInstance(
        Summon summon,
        Transform spawnTransform,
        Transform parent)
    {
        Summon summonClone = Object.Instantiate(
            summon,
            spawnTransform.localPosition,
            spawnTransform.rotation);
        summonClone.transform.SetParent(parent, false);
        return summonClone;
    }

    public void DestroySummonInstance(
        Summon summon,
        stateObserver statePanelObserver)
    {
        if (summon == null)
        {
            return;
        }

        summon.RemoveObserver(statePanelObserver);
        Object.Destroy(summon.gameObject);
    }

    public void MoveSummonToPlate(Summon summon, Transform parent)
    {
        if (summon == null)
        {
            return;
        }

        summon.transform.SetParent(parent, false);
        summon.transform.localPosition = Vector3.zero;
    }
}
