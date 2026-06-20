using System.Collections.Generic;

// 역할: 이미 결정된 플레이트 압축 이동을 실행한다.
public class PlateCompactExecutor
{
    public void CompactPlates(List<Plate> plates)
    {
        if (plates == null)
        {
            return;
        }

        int nextAvailableIndex = 0;

        for (int i = 0; i < plates.Count; i++)
        {
            Summon summon = plates[i].GetCurrentSummon();
            if (summon == null)
            {
                continue;
            }

            if (i != nextAvailableIndex)
            {
                plates[nextAvailableIndex].DirectMoveSummon(summon);
                plates[i].RemoveSummon();
            }

            nextAvailableIndex++;
        }
    }
}
