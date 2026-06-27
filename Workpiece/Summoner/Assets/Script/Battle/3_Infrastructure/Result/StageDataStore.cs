using UnityEngine;

public sealed class StageDataStore
{
    private const string StageResultDataResourceName = "StageResultData";

    public StageResultData LoadStageResultData()
    {
        return Resources.Load<StageResultData>(StageResultDataResourceName);
    }
}