using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Summoner/Stage Result Data")]
// 역할: StageResultData의 책임을 정의한다.
public class StageResultData : ScriptableObject
{
    [Header("스테이지 결과 조건")]
    [Tooltip("Clear turn condition per battle stage.")]
    [SerializeField] private List<StageResultCondition> conditions = new List<StageResultCondition>();

    public int GetClearTurn(int stage, int defaultClearTurn)
    {
        foreach (StageResultCondition condition in conditions)
        {
            if (condition != null && condition.StageMatches(stage))
            {
                return condition.GetClearTurn();
            }
        }

        return defaultClearTurn;
    }
}

[Serializable]
// 역할: StageResultData의 책임을 정의한다.
public class StageResultCondition
{
    [Min(1)]
    [Tooltip("Battle stage number that uses this clear turn condition.")]
    [SerializeField] private int stage;

    [Min(1)]
    [Tooltip("클리어와 실패 결과 판정에 사용할 턴 제한입니다.")]
    [SerializeField] private int clearTurn = 1;

    public bool StageMatches(int stage)
    {
        return this.stage == stage;
    }

    public int GetClearTurn()
    {
        return clearTurn;
    }
}
