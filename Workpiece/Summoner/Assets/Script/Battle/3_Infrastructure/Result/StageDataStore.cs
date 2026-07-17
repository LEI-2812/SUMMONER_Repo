using UnityEngine;

public class StageDataStore
{
    private const string StageResultDataResourceName = "StageResultData";

    public StageResultData LoadStageResultData()
    {
        return Resources.Load<StageResultData>(StageResultDataResourceName);
    }
}
