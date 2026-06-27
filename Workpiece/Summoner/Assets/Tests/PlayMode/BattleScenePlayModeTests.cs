using System.Collections;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

public class BattleScenePlayModeTests
{
    private static readonly string[] FightScenePaths =
    {
        "Assets/Screen/FightScene/Fight Screen_1Stage.unity",
        "Assets/Screen/FightScene/Fight Screen_2Stage.unity",
        "Assets/Screen/FightScene/Fight Screen_3Stage.unity",
        "Assets/Screen/FightScene/Fight Screen_4Stage.unity",
        "Assets/Screen/FightScene/Fight Screen_5Stage.unity",
        "Assets/Screen/FightScene/Fight Screen_6Stage.unity",
        "Assets/Screen/FightScene/Fight Screen_7Stage.unity"
    };

    private static readonly int[] ExpectedEnemyPlateCounts = { 3, 3, 3, 3, 3, 3, 1 };
    private static readonly int[] ExpectedPlacedEnemyCounts = { 2, 2, 2, 3, 3, 3, 1 };
    private static readonly int[] ExpectedClearTurns = { 7, 10, 13, 15, 17, 20, 25 };
    private static readonly double[] ExpectedStageMultipliers = { 1, 1, 1.2, 1.2, 1.5, 1.5, 2.5 };

    [TearDown]
    public void TearDown()
    {
        SetSummonStatMultiplier(1);
    }

    [UnityTest]
    public IEnumerator FightScenes_LoadWithBattleRuntimeAndPlateBoard()
    {
        foreach (string scenePath in FightScenePaths)
        {
            yield return LoadScene(scenePath);
            yield return null;

            MonoBehaviour battleSceneRuntime = FindComponent("BattleSceneRuntime");
            MonoBehaviour plateBoardView = FindComponent("PlateBoardView");
            MonoBehaviour enemyTurnRuntime = FindComponent("EnemyTurnRuntime");

            Assert.NotNull(battleSceneRuntime, scenePath + "에 BattleSceneRuntime이 필요합니다.");
            Assert.NotNull(plateBoardView, scenePath + "에 PlateBoardView가 필요합니다.");
            Assert.NotNull(enemyTurnRuntime, scenePath + "에 EnemyTurnRuntime이 필요합니다.");
            AssertPlateListHasEntries(plateBoardView, "GetPlayerPlates", scenePath + "에 최소 하나의 플레이어 플레이트가 필요합니다.");
            AssertPlateListHasEntries(plateBoardView, "GetEnemyPlates", scenePath + "에 최소 하나의 적 플레이트가 필요합니다.");
        }
    }

    [UnityTest]
    public IEnumerator FightScenes_HaveExpectedBattlePlateCounts()
    {
        for (int i = 0; i < FightScenePaths.Length; i++)
        {
            string scenePath = FightScenePaths[i];
            yield return LoadScene(scenePath);
            yield return null;

            MonoBehaviour plateBoardView = FindComponent("PlateBoardView");
            Assert.NotNull(plateBoardView, scenePath + "에 PlateBoardView가 필요합니다.");

            Assert.AreEqual(3, GetPlateCount(plateBoardView, "GetPlayerPlates"), scenePath + "의 플레이어 플레이트 개수가 변경되었습니다.");
            Assert.AreEqual(ExpectedEnemyPlateCounts[i], GetPlateCount(plateBoardView, "GetEnemyPlates"), scenePath + "의 적 플레이트 개수가 변경되었습니다.");
        }
    }

    [UnityTest]
    public IEnumerator PlayerCommands_AreCallableAfterBattleSceneLoad()
    {
        string[] commandMethods =
        {
            "OnClickSummon",
            "OnClickRedraw",
            "OnClickNormalAttack",
            "OnClickSpecialAttack",
            "OnClickEndTurn"
        };

        foreach (string commandMethod in commandMethods)
        {
            yield return LoadScene(FightScenePaths[0]);
            yield return null;

            MonoBehaviour playerCommandController = FindComponent("PlayerCommandController");
            Assert.NotNull(playerCommandController, "Fight Screen_1Stage에 PlayerCommandController가 필요합니다.");

            InvokePublicMethod(playerCommandController, commandMethod);
            yield return null;
        }
    }

    [UnityTest]
    public IEnumerator PlayerEndTurn_ReturnsToPlayerTurnAndAdvancesRound()
    {
        yield return LoadScene(FightScenePaths[0]);
        yield return null;

        MonoBehaviour battleSceneRuntime = FindComponent("BattleSceneRuntime");
        MonoBehaviour playerCommandController = FindComponent("PlayerCommandController");
        Assert.NotNull(battleSceneRuntime, "Fight Screen_1Stage에 BattleSceneRuntime이 필요합니다.");
        Assert.NotNull(playerCommandController, "Fight Screen_1Stage에 PlayerCommandController가 필요합니다.");

        Assert.IsTrue((bool)InvokePublicMethod(battleSceneRuntime, "IsPlayerTurn"), "전투는 플레이어 턴에서 시작해야 합니다.");
        int turnCountBefore = (int)InvokePublicMethod(battleSceneRuntime, "GetTurnCount");

        InvokePublicMethod(playerCommandController, "OnClickEndTurn");
        yield return null;
        yield return null;

        Assert.IsTrue((bool)InvokePublicMethod(battleSceneRuntime, "IsPlayerTurn"), "적 턴은 완료 후 플레이어 턴으로 돌아와야 합니다.");
        Assert.AreEqual(turnCountBefore + 1, (int)InvokePublicMethod(battleSceneRuntime, "GetTurnCount"), "턴 종료 시 라운드가 1 증가해야 합니다.");
    }

    [UnityTest]
    public IEnumerator FightScenes_ApplyExpectedStageEnemyPlacement()
    {
        for (int i = 0; i < FightScenePaths.Length; i++)
        {
            string scenePath = FightScenePaths[i];
            yield return LoadScene(scenePath);
            yield return null;

            MonoBehaviour plateBoardView = FindComponent("PlateBoardView");
            Assert.NotNull(plateBoardView, scenePath + "에 PlateBoardView가 필요합니다.");

            Assert.AreEqual(
                ExpectedPlacedEnemyCounts[i],
                CountOccupiedPlates(plateBoardView, "GetEnemyPlates"),
                scenePath + "의 적 배치 개수가 변경되었습니다.");
        }
    }

    [UnityTest]
    public IEnumerator FightScenes_ResolveExpectedClearTurn()
    {
        for (int i = 0; i < FightScenePaths.Length; i++)
        {
            string scenePath = FightScenePaths[i];
            yield return LoadScene(scenePath);
            yield return null;

            MonoBehaviour battleSceneRuntime = FindComponent("BattleSceneRuntime");
            Assert.NotNull(battleSceneRuntime, scenePath + "에 BattleSceneRuntime이 필요합니다.");

            Assert.AreEqual(
                ExpectedClearTurns[i],
                (int)InvokePublicMethod(battleSceneRuntime, "GetClearTurn"),
                scenePath + "의 클리어 턴이 변경되었습니다.");
        }
    }

    [UnityTest]
    public IEnumerator FightScenes_ShowExpectedTurnProgressText()
    {
        for (int i = 0; i < FightScenePaths.Length; i++)
        {
            string scenePath = FightScenePaths[i];
            yield return LoadScene(scenePath);
            yield return null;

            object currentTurnText = FindText("CurrentTurnText");
            object clearTurnText = FindText("ClearTurnText");
            Assert.NotNull(currentTurnText, scenePath + "에 CurrentTurnText가 필요합니다.");
            Assert.NotNull(clearTurnText, scenePath + "에 ClearTurnText가 필요합니다.");

            Assert.AreEqual("Current Turn : 1", GetTextValue(currentTurnText), scenePath + "의 현재 턴 텍스트가 변경되었습니다.");
            Assert.AreEqual("Clear Turn : " + ExpectedClearTurns[i], GetTextValue(clearTurnText), scenePath + "의 클리어 턴 텍스트가 변경되었습니다.");
        }
    }

    [UnityTest]
    public IEnumerator PlayerSummonCommand_PlacesSelectedSummonOnPlayerPlate()
    {
        yield return LoadScene(FightScenePaths[0]);
        yield return null;

        MonoBehaviour playerCommandController = FindComponent("PlayerCommandController");
        MonoBehaviour summonSelectionController = FindComponent("SummonSelectionController");
        MonoBehaviour plateBoardView = FindComponent("PlateBoardView");
        Assert.NotNull(playerCommandController, "Fight Screen_1Stage에 PlayerCommandController가 필요합니다.");
        Assert.NotNull(summonSelectionController, "Fight Screen_1Stage에 SummonSelectionController가 필요합니다.");
        Assert.NotNull(plateBoardView, "Fight Screen_1Stage에 PlateBoardView가 필요합니다.");

        int occupiedBefore = CountOccupiedPlates(plateBoardView, "GetPlayerPlates");
        object firstSummon = GetFirstSummonOption(summonSelectionController);
        Assert.NotNull(firstSummon, "SummonSelectionController에 최소 하나의 소환수 선택지가 필요합니다.");

        InvokePublicMethod(playerCommandController, "OnClickSummon");
        yield return null;

        Assert.IsTrue((bool)InvokePublicMethod(summonSelectionController, "IsSummoning"), "소환수 선택이 시작되어야 합니다.");

        InvokeMethodWithSingleArgument(summonSelectionController, "OnSelectSummon", firstSummon);
        yield return null;

        Assert.AreEqual(occupiedBefore + 1, CountOccupiedPlates(plateBoardView, "GetPlayerPlates"), "선택한 소환수가 플레이어 플레이트에 배치되어야 합니다.");
        Assert.IsFalse((bool)InvokePublicMethod(summonSelectionController, "IsSummoning"), "소환수 선택이 종료되어야 합니다.");
    }

    [UnityTest]
    public IEnumerator PlayerSummonCommand_AppliesCurrentStageMultiplierToPlacedSummon()
    {
        const double multiplier = 2.0;

        yield return LoadScene(FightScenePaths[0]);
        yield return null;

        MonoBehaviour playerCommandController = FindComponent("PlayerCommandController");
        MonoBehaviour summonSelectionController = FindComponent("SummonSelectionController");
        MonoBehaviour plateBoardView = FindComponent("PlateBoardView");
        Assert.NotNull(playerCommandController, "Fight Screen_1Stage에 PlayerCommandController가 필요합니다.");
        Assert.NotNull(summonSelectionController, "Fight Screen_1Stage에 SummonSelectionController가 필요합니다.");
        Assert.NotNull(plateBoardView, "Fight Screen_1Stage에 PlateBoardView가 필요합니다.");

        SetSummonStatMultiplier(multiplier);

        MonoBehaviour selectedSummon = GetFirstSummonOption(summonSelectionController) as MonoBehaviour;
        Assert.NotNull(selectedSummon, "SummonSelectionController에 소환수 선택지가 필요합니다.");
        InvokePublicMethod(selectedSummon, "SummonInitialize");
        double baseMaxHp = (double)InvokePublicMethod(selectedSummon, "GetMaxHP");
        double baseAttackPower = (double)InvokePublicMethod(selectedSummon, "GetAttackPower");

        InvokePublicMethod(playerCommandController, "OnClickSummon");
        yield return null;
        InvokeMethodWithSingleArgument(summonSelectionController, "OnSelectSummon", selectedSummon);
        yield return null;

        MonoBehaviour playerPlate = GetFirstOccupiedPlate(plateBoardView, "GetPlayerPlates");
        MonoBehaviour placedSummon = InvokePublicMethod(playerPlate, "GetCurrentSummon") as MonoBehaviour;
        Assert.NotNull(placedSummon, "플레이어 플레이트에 소환수가 배치되어야 합니다.");
        Assert.AreEqual((int)(baseMaxHp * multiplier), (double)InvokePublicMethod(placedSummon, "GetMaxHP"), "플레이어 소환수 체력에 스테이지 배수가 적용되어야 합니다.");
        Assert.AreEqual((int)(baseAttackPower * multiplier), (double)InvokePublicMethod(placedSummon, "GetAttackPower"), "플레이어 소환수 공격력에 스테이지 배수가 적용되어야 합니다.");
    }

    [UnityTest]
    public IEnumerator FightScenes_ApplyPreparedStageMultiplierToPlayerSummon()
    {
        for (int i = 0; i < FightScenePaths.Length; i++)
        {
            int stage = i + 1;
            double multiplier = ExpectedStageMultipliers[i];

            Assert.IsTrue(PrepareFightStage(stage), "전투 스테이지 배수 준비가 성공해야 합니다: " + stage);
            yield return LoadScene(FightScenePaths[i]);
            yield return null;

            MonoBehaviour playerCommandController = FindComponent("PlayerCommandController");
            MonoBehaviour summonSelectionController = FindComponent("SummonSelectionController");
            MonoBehaviour plateBoardView = FindComponent("PlateBoardView");
            Assert.NotNull(playerCommandController, FightScenePaths[i] + "에 PlayerCommandController가 필요합니다.");
            Assert.NotNull(summonSelectionController, FightScenePaths[i] + "에 SummonSelectionController가 필요합니다.");
            Assert.NotNull(plateBoardView, FightScenePaths[i] + "에 PlateBoardView가 필요합니다.");

            MonoBehaviour selectedSummon = GetFirstSummonOption(summonSelectionController) as MonoBehaviour;
            Assert.NotNull(selectedSummon, "SummonSelectionController에 소환수 선택지가 필요합니다.");
            InvokePublicMethod(selectedSummon, "SummonInitialize");
            double baseMaxHp = (double)InvokePublicMethod(selectedSummon, "GetMaxHP");
            double baseAttackPower = (double)InvokePublicMethod(selectedSummon, "GetAttackPower");

            InvokePublicMethod(playerCommandController, "OnClickSummon");
            yield return null;
            InvokeMethodWithSingleArgument(summonSelectionController, "OnSelectSummon", selectedSummon);
            yield return null;

            MonoBehaviour playerPlate = GetFirstOccupiedPlate(plateBoardView, "GetPlayerPlates");
            MonoBehaviour placedSummon = InvokePublicMethod(playerPlate, "GetCurrentSummon") as MonoBehaviour;
            Assert.NotNull(placedSummon, "플레이어 플레이트에 소환수가 배치되어야 합니다.");
            Assert.AreEqual((int)(baseMaxHp * multiplier), (double)InvokePublicMethod(placedSummon, "GetMaxHP"), "스테이지 " + stage + " 플레이어 소환수 체력 배수가 맞아야 합니다.");
            Assert.AreEqual((int)(baseAttackPower * multiplier), (double)InvokePublicMethod(placedSummon, "GetAttackPower"), "스테이지 " + stage + " 플레이어 소환수 공격력 배수가 맞아야 합니다.");
        }
    }

    [UnityTest]
    public IEnumerator Stage7_PlayerSummonStatePanel_ShowsScaledHealthAsFullSlider()
    {
        const int stage = 7;
        const double multiplier = 2.5;

        Assert.IsTrue(PrepareFightStage(stage), "전투 스테이지 배수 준비가 성공해야 합니다: " + stage);
        yield return LoadScene(FightScenePaths[stage - 1]);
        yield return null;

        MonoBehaviour playerCommandController = FindComponent("PlayerCommandController");
        MonoBehaviour summonSelectionController = FindComponent("SummonSelectionController");
        MonoBehaviour plateBoardView = FindComponent("PlateBoardView");
        Assert.NotNull(playerCommandController, "Fight Screen_7Stage에 PlayerCommandController가 필요합니다.");
        Assert.NotNull(summonSelectionController, "Fight Screen_7Stage에 SummonSelectionController가 필요합니다.");
        Assert.NotNull(plateBoardView, "Fight Screen_7Stage에 PlateBoardView가 필요합니다.");

        MonoBehaviour selectedSummon = GetFirstSummonOption(summonSelectionController) as MonoBehaviour;
        Assert.NotNull(selectedSummon, "SummonSelectionController에 소환수 선택지가 필요합니다.");
        InvokePublicMethod(selectedSummon, "SummonInitialize");
        double baseMaxHp = (double)InvokePublicMethod(selectedSummon, "GetMaxHP");

        InvokePublicMethod(playerCommandController, "OnClickSummon");
        yield return null;
        InvokeMethodWithSingleArgument(summonSelectionController, "OnSelectSummon", selectedSummon);
        yield return null;

        MonoBehaviour playerPlate = GetFirstOccupiedPlate(plateBoardView, "GetPlayerPlates");
        MonoBehaviour placedSummon = InvokePublicMethod(playerPlate, "GetCurrentSummon") as MonoBehaviour;
        Assert.NotNull(placedSummon, "플레이어 플레이트에 소환수가 배치되어야 합니다.");
        Assert.AreEqual((int)(baseMaxHp * multiplier), (double)InvokePublicMethod(placedSummon, "GetMaxHP"), "7스테이지 플레이어 소환수 체력 배수가 맞아야 합니다.");

        InvokeMethodWithSingleArgument(playerPlate, "OnPointerClick", null);
        yield return null;

        MonoBehaviour statePanelView = FindComponent("SummonStatePanelView");
        Assert.NotNull(statePanelView, "SummonStatePanelView가 필요합니다.");
        object hpSlider = GetPrivateFieldValue(statePanelView, "HPSlider");
        Assert.NotNull(hpSlider, "SummonStatePanelView에 HPSlider가 필요합니다.");

        float sliderValue = (float)hpSlider.GetType().GetProperty("value").GetValue(hpSlider, null);
        Assert.AreEqual(1f, sliderValue, 0.001f, "배수 적용 직후 상태 패널 HP 슬라이더는 가득 차 있어야 합니다.");
    }

    [UnityTest]
    public IEnumerator PlayerNormalAttackCommand_ExecutesWithoutRuntimeError()
    {
        yield return LoadScene(FightScenePaths[0]);
        yield return null;

        MonoBehaviour playerCommandController = FindComponent("PlayerCommandController");
        MonoBehaviour summonSelectionController = FindComponent("SummonSelectionController");
        MonoBehaviour plateBoardView = FindComponent("PlateBoardView");
        Assert.NotNull(playerCommandController, "Fight Screen_1Stage에 PlayerCommandController가 필요합니다.");
        Assert.NotNull(summonSelectionController, "Fight Screen_1Stage에 SummonSelectionController가 필요합니다.");
        Assert.NotNull(plateBoardView, "Fight Screen_1Stage에 PlateBoardView가 필요합니다.");

        InvokePublicMethod(playerCommandController, "OnClickSummon");
        yield return null;
        InvokeMethodWithSingleArgument(summonSelectionController, "OnSelectSummon", GetFirstSummonOption(summonSelectionController));
        yield return null;

        MonoBehaviour playerPlate = GetFirstOccupiedPlate(plateBoardView, "GetPlayerPlates");
        MonoBehaviour enemyPlate = GetFirstOccupiedPlate(plateBoardView, "GetEnemyPlates");
        MonoBehaviour attackingSummon = InvokePublicMethod(playerPlate, "GetCurrentSummon") as MonoBehaviour;
        MonoBehaviour targetSummon = InvokePublicMethod(enemyPlate, "GetCurrentSummon") as MonoBehaviour;
        Assert.NotNull(attackingSummon, "플레이어 공격에는 공격 소환수가 필요합니다.");
        Assert.NotNull(targetSummon, "플레이어 공격에는 대상 소환수가 필요합니다.");

        double targetHpBefore = (double)InvokePublicMethod(targetSummon, "GetNowHP");

        InvokeMethodWithSingleArgument(playerPlate, "OnPointerClick", null);
        yield return null;
        InvokePublicMethod(playerCommandController, "OnClickNormalAttack");
        yield return null;

        MonoBehaviour targetSummonAfter = InvokePublicMethod(enemyPlate, "GetCurrentSummon") as MonoBehaviour;
        bool attackerSpentAction = !(bool)InvokePublicMethod(attackingSummon, "GetIsAttack");

        if (targetSummonAfter == null)
        {
            Assert.IsTrue(attackerSpentAction, "일반 공격으로 대상을 제거하면 공격 행동을 소비해야 합니다.");
            yield break;
        }

        double targetHpAfter = (double)InvokePublicMethod(targetSummonAfter, "GetNowHP");
        Assert.Less(targetHpAfter, targetHpBefore, "일반 공격은 대상에게 피해를 줘야 합니다.");
        Assert.IsTrue(attackerSpentAction, "일반 공격은 공격 행동을 소비해야 합니다.");
    }

    [UnityTest]
    public IEnumerator PlayerSpecialAttackCommand_ExecutesWithoutRuntimeError()
    {
        yield return LoadScene(FightScenePaths[0]);
        yield return null;

        MonoBehaviour playerCommandController = FindComponent("PlayerCommandController");
        MonoBehaviour summonSelectionController = FindComponent("SummonSelectionController");
        MonoBehaviour plateBoardView = FindComponent("PlateBoardView");
        Assert.NotNull(playerCommandController, "Fight Screen_1Stage에 PlayerCommandController가 필요합니다.");
        Assert.NotNull(summonSelectionController, "Fight Screen_1Stage에 SummonSelectionController가 필요합니다.");
        Assert.NotNull(plateBoardView, "Fight Screen_1Stage에 PlateBoardView가 필요합니다.");

        InvokePublicMethod(playerCommandController, "OnClickSummon");
        yield return null;
        InvokeMethodWithSingleArgument(summonSelectionController, "OnSelectSummon", GetFirstSummonOption(summonSelectionController));
        yield return null;

        MonoBehaviour playerPlate = GetFirstOccupiedPlate(plateBoardView, "GetPlayerPlates");
        MonoBehaviour enemyPlate = GetFirstOccupiedPlate(plateBoardView, "GetEnemyPlates");
        MonoBehaviour attackingSummon = InvokePublicMethod(playerPlate, "GetCurrentSummon") as MonoBehaviour;
        MonoBehaviour targetSummon = InvokePublicMethod(enemyPlate, "GetCurrentSummon") as MonoBehaviour;
        Assert.NotNull(attackingSummon, "플레이어 특수 공격에는 공격 소환수가 필요합니다.");
        Assert.NotNull(targetSummon, "플레이어 특수 공격에는 대상 소환수가 필요합니다.");

        double targetHpBefore = (double)InvokePublicMethod(targetSummon, "GetNowHP");

        InvokeMethodWithSingleArgument(playerPlate, "OnPointerClick", null);
        yield return null;
        InvokePublicMethod(playerCommandController, "OnClickSpecialAttack");
        yield return null;

        MonoBehaviour targetSummonAfter = InvokePublicMethod(enemyPlate, "GetCurrentSummon") as MonoBehaviour;
        bool attackerSpentAction = !(bool)InvokePublicMethod(attackingSummon, "GetIsAttack");

        if (targetSummonAfter == null)
        {
            Assert.IsTrue(attackerSpentAction, "특수 공격으로 대상을 제거하면 공격 행동을 소비해야 합니다.");
            yield break;
        }

        double targetHpAfter = (double)InvokePublicMethod(targetSummonAfter, "GetNowHP");
        Assert.Less(targetHpAfter, targetHpBefore, "특수 공격은 대상에게 피해를 줘야 합니다.");
        Assert.IsTrue(attackerSpentAction, "특수 공격은 공격 행동을 소비해야 합니다.");
    }

    [UnityTest]
    public IEnumerator PlayerTargetedSpecialAttack_SelectsTargetPlateAndExecutes()
    {
        yield return LoadScene(FightScenePaths[0]);
        yield return null;

        MonoBehaviour playerCommandController = FindComponent("PlayerCommandController");
        MonoBehaviour summonSelectionController = FindComponent("SummonSelectionController");
        MonoBehaviour plateBoardView = FindComponent("PlateBoardView");
        Assert.NotNull(playerCommandController, "Fight Screen_1Stage에 PlayerCommandController가 필요합니다.");
        Assert.NotNull(summonSelectionController, "Fight Screen_1Stage에 SummonSelectionController가 필요합니다.");
        Assert.NotNull(plateBoardView, "Fight Screen_1Stage에 PlateBoardView가 필요합니다.");

        object targetedSummon = GetFirstSummonOptionWithFirstSpecialStrategyAndStatus(
            summonSelectionController,
            "TargetedAttackStrategy",
            "None");
        Assert.NotNull(targetedSummon, "SummonSelectionController에 피해를 주는 대상 지정 특수 공격 소환수 선택지가 필요합니다.");

        InvokePublicMethod(playerCommandController, "OnClickSummon");
        yield return null;
        InvokeMethodWithSingleArgument(summonSelectionController, "OnSelectSummon", targetedSummon);
        yield return null;

        MonoBehaviour playerPlate = GetFirstOccupiedPlate(plateBoardView, "GetPlayerPlates");
        MonoBehaviour enemyPlate = GetFirstOccupiedPlate(plateBoardView, "GetEnemyPlates");
        MonoBehaviour attackingSummon = InvokePublicMethod(playerPlate, "GetCurrentSummon") as MonoBehaviour;
        MonoBehaviour targetSummon = InvokePublicMethod(enemyPlate, "GetCurrentSummon") as MonoBehaviour;
        Assert.NotNull(attackingSummon, "대상 지정 특수 공격에는 공격 소환수가 필요합니다.");
        Assert.NotNull(targetSummon, "대상 지정 특수 공격에는 대상 소환수가 필요합니다.");

        double targetHpBefore = (double)InvokePublicMethod(targetSummon, "GetNowHP");

        InvokeMethodWithSingleArgument(playerPlate, "OnPointerClick", null);
        yield return null;
        InvokePublicMethod(playerCommandController, "OnClickSpecialAttack");
        yield return null;
        InvokeMethodWithSingleArgument(enemyPlate, "OnPointerClick", null);
        yield return null;

        MonoBehaviour targetSummonAfter = InvokePublicMethod(enemyPlate, "GetCurrentSummon") as MonoBehaviour;
        bool attackerSpentAction = !(bool)InvokePublicMethod(attackingSummon, "GetIsAttack");

        if (targetSummonAfter == null)
        {
            Assert.IsTrue(attackerSpentAction, "대상 지정 특수 공격으로 대상을 제거하면 공격 행동을 소비해야 합니다.");
            yield break;
        }

        double targetHpAfter = (double)InvokePublicMethod(targetSummonAfter, "GetNowHP");
        Assert.Less(targetHpAfter, targetHpBefore, "대상 지정 특수 공격은 선택한 대상에게 피해를 줘야 합니다.");
        Assert.IsTrue(attackerSpentAction, "대상 지정 특수 공격은 공격 행동을 소비해야 합니다.");
    }

    [UnityTest]
    public IEnumerator BattleResultController_StartsClearAndFailResultsSafely()
    {
        yield return LoadScene(FightScenePaths[0]);
        yield return null;

        MonoBehaviour clearResultController = FindComponent("BattleResultController");
        Assert.NotNull(clearResultController, "Fight Screen_1Stage에 BattleResultController가 필요합니다.");
        object clearStarted = InvokePublicMethodWithArguments(
            clearResultController,
            "TryStartClearResult",
            true,
            999,
            1);
        Assert.IsTrue((bool)clearStarted, "클리어 턴 안에 적 플레이트가 비면 클리어 결과가 시작되어야 합니다.");

        yield return LoadScene(FightScenePaths[0]);
        yield return null;

        MonoBehaviour failResultController = FindComponent("BattleResultController");
        Assert.NotNull(failResultController, "Fight Screen_1Stage에 BattleResultController가 필요합니다.");
        InvokePublicMethodWithArguments(
            failResultController,
            "TryStartFailResult",
            1,
            999);
        yield return null;
    }

    [UnityTest]
    public IEnumerator SummonDeath_ClearsOccupiedBattlePlate()
    {
        yield return LoadScene(FightScenePaths[0]);
        yield return null;

        MonoBehaviour plateBoardView = FindComponent("PlateBoardView");
        Assert.NotNull(plateBoardView, "Fight Screen_1Stage에 PlateBoardView가 필요합니다.");

        MonoBehaviour enemyPlate = GetFirstOccupiedPlate(plateBoardView, "GetEnemyPlates");
        MonoBehaviour enemySummon = InvokePublicMethod(enemyPlate, "GetCurrentSummon") as MonoBehaviour;
        Assert.NotNull(enemySummon, "사망 테스트 전에 적 플레이트에 소환수가 있어야 합니다.");

        InvokePublicMethodWithArguments(enemySummon, "TakeDamage", 999999d);
        yield return null;

        Assert.IsNull(InvokePublicMethod(enemyPlate, "GetCurrentSummon"), "처치된 소환수는 전투 플레이트에서 제거되어야 합니다.");
    }

    private static IEnumerator LoadScene(string scenePath)
    {
        AsyncOperation loadOperation = SceneManager.LoadSceneAsync(scenePath, LoadSceneMode.Single);
        Assert.NotNull(loadOperation, "씬 로드를 시작하지 못했습니다: " + scenePath);

        while (!loadOperation.isDone)
        {
            yield return null;
        }
    }

    private static MonoBehaviour FindComponent(string typeName)
    {
        foreach (MonoBehaviour component in Object.FindObjectsOfType<MonoBehaviour>())
        {
            if (component != null && component.GetType().Name == typeName)
            {
                return component;
            }
        }

        return null;
    }

    private static object FindText(string objectName)
    {
        GameObject textObject = GameObject.Find(objectName);
        if (textObject == null)
        {
            return null;
        }

        foreach (MonoBehaviour component in textObject.GetComponents<MonoBehaviour>())
        {
            if (component != null && component.GetType().Name == "TextMeshProUGUI")
            {
                return component;
            }
        }

        return null;
    }

    private static string GetTextValue(object textComponent)
    {
        PropertyInfo textProperty = textComponent.GetType().GetProperty("text");
        Assert.NotNull(textProperty, textComponent.GetType().Name + "에 text 프로퍼티가 필요합니다.");
        return (string)textProperty.GetValue(textComponent, null);
    }

    private static void AssertPlateListHasEntries(MonoBehaviour plateBoardView, string methodName, string message)
    {
        MethodInfo getPlates = plateBoardView.GetType().GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public);
        Assert.NotNull(getPlates, "PlateBoardView에 " + methodName + " 메서드가 필요합니다.");

        object plates = getPlates.Invoke(plateBoardView, null);
        Assert.NotNull(plates, message);

        ICollection plateCollection = plates as ICollection;
        Assert.NotNull(plateCollection, message);
        Assert.Greater(plateCollection.Count, 0, message);
    }

    private static int GetPlateCount(MonoBehaviour plateBoardView, string methodName)
    {
        MethodInfo getPlates = plateBoardView.GetType().GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public);
        Assert.NotNull(getPlates, "PlateBoardView에 " + methodName + " 메서드가 필요합니다.");

        object plates = getPlates.Invoke(plateBoardView, null);
        Assert.NotNull(plates, "PlateBoardView " + methodName + "이 null을 반환했습니다.");

        ICollection plateCollection = plates as ICollection;
        Assert.NotNull(plateCollection, "PlateBoardView " + methodName + "은 컬렉션을 반환해야 합니다.");
        return plateCollection.Count;
    }

    private static int CountOccupiedPlates(MonoBehaviour plateBoardView, string methodName)
    {
        int occupiedCount = 0;
        foreach (object plate in GetPlateCollection(plateBoardView, methodName))
        {
            MonoBehaviour plateComponent = plate as MonoBehaviour;
            Assert.NotNull(plateComponent, "플레이트 항목은 MonoBehaviour여야 합니다.");

            object summon = InvokePublicMethod(plateComponent, "GetCurrentSummon");
            if (summon != null)
            {
                occupiedCount++;
            }
        }

        return occupiedCount;
    }

    private static ICollection GetPlateCollection(MonoBehaviour plateBoardView, string methodName)
    {
        MethodInfo getPlates = plateBoardView.GetType().GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public);
        Assert.NotNull(getPlates, "PlateBoardView에 " + methodName + " 메서드가 필요합니다.");

        object plates = getPlates.Invoke(plateBoardView, null);
        Assert.NotNull(plates, "PlateBoardView " + methodName + "이 null을 반환했습니다.");

        ICollection plateCollection = plates as ICollection;
        Assert.NotNull(plateCollection, "PlateBoardView " + methodName + "은 컬렉션을 반환해야 합니다.");
        return plateCollection;
    }

    private static MonoBehaviour GetFirstOccupiedPlate(MonoBehaviour plateBoardView, string methodName)
    {
        foreach (object plate in GetPlateCollection(plateBoardView, methodName))
        {
            MonoBehaviour plateComponent = plate as MonoBehaviour;
            Assert.NotNull(plateComponent, "플레이트 항목은 MonoBehaviour여야 합니다.");

            if (InvokePublicMethod(plateComponent, "GetCurrentSummon") != null)
            {
                return plateComponent;
            }
        }

        Assert.Fail(methodName + "에 최소 하나의 점유된 플레이트가 필요합니다.");
        return null;
    }

    private static object InvokePublicMethod(MonoBehaviour component, string methodName)
    {
        MethodInfo method = component.GetType().GetMethod(
            methodName,
            BindingFlags.Instance | BindingFlags.Public,
            null,
            System.Type.EmptyTypes,
            null);
        Assert.NotNull(method, component.GetType().Name + "에 " + methodName + " 메서드가 필요합니다.");

        return method.Invoke(component, null);
    }

    private static void InvokeMethodWithSingleArgument(MonoBehaviour component, string methodName, object argument)
    {
        foreach (MethodInfo method in component.GetType().GetMethods(BindingFlags.Instance | BindingFlags.Public))
        {
            if (method.Name != methodName || method.GetParameters().Length != 1)
            {
                continue;
            }

            method.Invoke(component, new[] { argument });
            return;
        }

        Assert.Fail(component.GetType().Name + "에 " + methodName + " 인자 1개짜리 오버로드가 필요합니다.");
    }

    private static object InvokePublicMethodWithArguments(MonoBehaviour component, string methodName, params object[] arguments)
    {
        foreach (MethodInfo method in component.GetType().GetMethods(BindingFlags.Instance | BindingFlags.Public))
        {
            if (method.Name != methodName || method.GetParameters().Length != arguments.Length)
            {
                continue;
            }

            return method.Invoke(component, arguments);
        }

        Assert.Fail(component.GetType().Name + "에 " + methodName + " 인자 " + arguments.Length + "개짜리 오버로드가 필요합니다.");
        return null;
    }

    private static object GetFirstSummonOption(MonoBehaviour summonSelectionController)
    {
        FieldInfo summonsField = summonSelectionController.GetType().GetField(
            "summons",
            BindingFlags.Instance | BindingFlags.NonPublic);
        Assert.NotNull(summonsField, "SummonSelectionController에 summons 필드가 필요합니다.");

        IList summons = summonsField.GetValue(summonSelectionController) as IList;
        Assert.NotNull(summons, "SummonSelectionController의 summons 필드는 리스트여야 합니다.");
        Assert.Greater(summons.Count, 0, "SummonSelectionController의 summons 리스트는 비어 있으면 안 됩니다.");
        return summons[0];
    }

    private static object GetFirstSummonOptionWithFirstSpecialStrategyAndStatus(
        MonoBehaviour summonSelectionController,
        string strategyTypeName,
        string statusTypeName)
    {
        foreach (object summon in GetSummonOptions(summonSelectionController))
        {
            MonoBehaviour summonComponent = summon as MonoBehaviour;
            if (summonComponent == null)
            {
                continue;
            }

            InvokePublicMethod(summonComponent, "SummonInitialize");
            object specialAttacks = InvokePublicMethod(summonComponent, "GetSpecialAttackStrategy");
            IEnumerable attackEnumerable = specialAttacks as IEnumerable;
            if (attackEnumerable == null)
            {
                continue;
            }

            foreach (object attackData in attackEnumerable)
            {
                if (attackData == null)
                {
                    continue;
                }

                PropertyInfo strategyProperty = attackData.GetType().GetProperty("Strategy");
                object strategy = strategyProperty == null ? null : strategyProperty.GetValue(attackData, null);
                object statusType = InvokeParameterlessMethod(attackData, "GetStatusType");
                if (strategy != null
                    && strategy.GetType().Name == strategyTypeName
                    && statusType != null
                    && statusType.ToString() == statusTypeName)
                {
                    return summon;
                }

                break;
            }
        }

        return null;
    }

    private static object InvokeParameterlessMethod(object target, string methodName)
    {
        MethodInfo method = target.GetType().GetMethod(
            methodName,
            BindingFlags.Instance | BindingFlags.Public,
            null,
            System.Type.EmptyTypes,
            null);
        Assert.NotNull(method, target.GetType().Name + "에 " + methodName + " 메서드가 필요합니다.");

        return method.Invoke(target, null);
    }

    private static object GetPrivateFieldValue(object target, string fieldName)
    {
        FieldInfo field = target.GetType().GetField(
            fieldName,
            BindingFlags.Instance | BindingFlags.NonPublic);
        Assert.NotNull(field, target.GetType().Name + "에 " + fieldName + " 필드가 필요합니다.");

        return field.GetValue(target);
    }

    private static IList GetSummonOptions(MonoBehaviour summonSelectionController)
    {
        FieldInfo summonsField = summonSelectionController.GetType().GetField(
            "summons",
            BindingFlags.Instance | BindingFlags.NonPublic);
        Assert.NotNull(summonsField, "SummonSelectionController에 summons 필드가 필요합니다.");

        IList summons = summonsField.GetValue(summonSelectionController) as IList;
        Assert.NotNull(summons, "SummonSelectionController의 summons 필드는 리스트여야 합니다.");
        return summons;
    }

    private static bool PrepareFightStage(int stage)
    {
        System.Type transitionUseCaseType = FindType("StageTransitionUseCase");
        Assert.NotNull(transitionUseCaseType, "StageTransitionUseCase 타입을 찾을 수 있어야 합니다.");

        object transitionUseCase = System.Activator.CreateInstance(transitionUseCaseType);
        MethodInfo method = transitionUseCaseType.GetMethod(
            "TryPrepareFightStage",
            BindingFlags.Instance | BindingFlags.Public);
        Assert.NotNull(method, "StageTransitionUseCase.TryPrepareFightStage 메서드가 필요합니다.");

        return (bool)method.Invoke(transitionUseCase, new object[] { stage });
    }

    private static void SetSummonStatMultiplier(double multiplier)
    {
        System.Type summonType = FindType("Summon");
        Assert.NotNull(summonType, "Summon 타입을 찾을 수 있어야 합니다.");

        MethodInfo method = summonType.GetMethod(
            "StatMultiplierSet",
            BindingFlags.Static | BindingFlags.Public);
        Assert.NotNull(method, "Summon.StatMultiplierSet 메서드가 필요합니다.");

        method.Invoke(null, new object[] { multiplier });
    }

    private static System.Type FindType(string typeName)
    {
        foreach (System.Reflection.Assembly assembly in System.AppDomain.CurrentDomain.GetAssemblies())
        {
            System.Type type = assembly.GetType(typeName);
            if (type != null)
            {
                return type;
            }
        }

        return null;
    }
}
