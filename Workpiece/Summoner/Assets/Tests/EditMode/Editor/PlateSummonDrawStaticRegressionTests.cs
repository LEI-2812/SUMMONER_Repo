using System.IO;
using NUnit.Framework;

namespace Summoner.EditModeTests
{
    public class PlateSummonDrawStaticRegressionTests
    {
        [Test]
        public void StartSummon_SetsDrawStateBeforeBranching()
        {
            string text = File.ReadAllText("Assets/Script/Battle/SummonPick/SummonController.cs");
            int methodIndex = text.IndexOf("public void StartSummon(int plateIndex, bool isRedraw)");
            int drawStateIndex = text.IndexOf("isSummoning = true;", methodIndex);
            int branchIndex = text.IndexOf("if (isRedraw)", methodIndex);

            Assert.GreaterOrEqual(methodIndex, 0, "StartSummon method should exist.");
            Assert.Greater(drawStateIndex, methodIndex, "StartSummon should enter draw state.");
            Assert.Greater(branchIndex, methodIndex, "StartSummon should still branch by redraw flag.");
            Assert.Less(drawStateIndex, branchIndex, "Draw state should be set before normal Draw/Redraw branching.");
        }

        [Test]
        public void SummonController_UsesDrawNamesForInternalSelectionFlow()
        {
            string text = File.ReadAllText("Assets/Script/Battle/SummonPick/SummonController.cs");
            int startIndex = text.IndexOf("public void StartSummon(int plateIndex, bool isResummon)");

            Assert.AreEqual(-1, startIndex, "StartSummon should use Draw naming for its redraw flag.");
            StringAssert.Contains("public void StartSummon(int plateIndex, bool isRedraw)", text);
            StringAssert.Contains("OpenDrawOptions()", text);
            StringAssert.Contains("OpenRedrawOptions()", text);
            StringAssert.Contains("private void OpenDrawOptions()", text);
            StringAssert.Contains("private void OpenRedrawOptions()", text);
            StringAssert.Contains("private IEnumerator DrawSelection(int plateIndex)", text);
            StringAssert.Contains("private IEnumerator RedrawSelection(int plateIndex)", text);
            StringAssert.Contains("StartCoroutine(DrawSelection(plateIndex))", text);
            StringAssert.Contains("StartCoroutine(RedrawSelection(plateIndex))", text);
            StringAssert.Contains("private List<Summon> CreateDrawOptions()", text);
            StringAssert.Contains("private Summon SelectDrawOptionByRank()", text);
            StringAssert.Contains("private Summon SelectRandomSummonByRank(SummonRank rank)", text);
            StringAssert.Contains("summon.GetDrawRank() == rank", text);
            Assert.IsFalse(text.Contains("summon.GetSummonRank() == rank"), "Draw option selection should read rank from SummonData without initializing summon state.");
            Assert.IsFalse(text.Contains("TakeSummonSelection"), "Internal draw coroutine should not use old Take naming.");
            Assert.IsFalse(text.Contains("ReSummonSelection"), "Internal redraw coroutine should not use old ReSummon selection naming.");
            Assert.IsFalse(text.Contains("ReSummonPanelOpenAndHighlight"), "Internal redraw plate selection should use Redraw naming.");
            Assert.IsFalse(text.Contains("ResummonSelectStart"), "Internal redraw option selection should use Redraw naming.");
            Assert.IsFalse(text.Contains("randomReTakeSummon"), "Internal redraw option creation should use Redraw naming.");
            Assert.IsFalse(text.Contains("randomTakeSummon"), "Internal draw option creation should use Draw naming.");
            Assert.IsFalse(text.Contains("SummonRandomly"), "Draw option creation should use Draw naming.");
            Assert.IsFalse(text.Contains("GetSummonByRank"), "Ranked draw option selection should use explicit random selection naming.");
        }

        [Test]
        public void SummonController_UsesDrawNamesForSerializedFieldsSafely()
        {
            string text = File.ReadAllText("Assets/Script/Battle/SummonPick/SummonController.cs");

            StringAssert.Contains("using UnityEngine.Serialization;", text);
            StringAssert.Contains("[FormerlySerializedAs(\"takeSummonPanel\")]", text);
            StringAssert.Contains("[FormerlySerializedAs(\"selectSummonPanels\")]", text);
            StringAssert.Contains("[FormerlySerializedAs(\"reTakeSummonPanel\")]", text);
            StringAssert.Contains("[FormerlySerializedAs(\"ReselectSummonPanels\")]", text);
            StringAssert.Contains("[SerializeField] private GameObject drawPanel;", text);
            StringAssert.Contains("[SerializeField] private List<DrawOptionPanelView> drawOptionPanels;", text);
            StringAssert.Contains("[SerializeField] private GameObject redrawPanel;", text);
            StringAssert.Contains("[SerializeField] private List<DrawOptionPanelView> redrawOptionPanels;", text);
            StringAssert.Contains("drawPanel.SetActive(true)", text);
            StringAssert.Contains("redrawPanel.SetActive(true)", text);
            StringAssert.Contains("drawPanel.SetActive(false)", text);
            StringAssert.Contains("redrawPanel.SetActive(false)", text);
        }

        [Test]
        public void DrawOptionPanelView_ClassNameMatchesFileNameAndKeepsGuid()
        {
            string text = File.ReadAllText("Assets/Script/Battle/SummonPick/DrawOptionPanelView.cs");
            string metaText = File.ReadAllText("Assets/Script/Battle/SummonPick/DrawOptionPanelView.cs.meta");

            StringAssert.Contains("public class DrawOptionPanelView : MonoBehaviour", text);
            StringAssert.Contains("guid: ae497cba18bfab944a51f1a09ba60431", metaText);
            Assert.IsFalse(File.Exists("Assets/Script/Battle/SummonPick/PickSummonPanelView.cs"), "Old PickSummonPanelView file should be renamed.");
            Assert.IsFalse(text.Contains("PickSummonPanelView"), "Draw option panel view should not keep the old Pick name.");
        }

        [Test]
        public void DrawOptionPanelView_ReportsSelectionWithoutSummonControllerSingleton()
        {
            string viewText = File.ReadAllText("Assets/Script/Battle/SummonPick/DrawOptionPanelView.cs");
            string controllerText = File.ReadAllText("Assets/Script/Battle/SummonPick/SummonController.cs");

            StringAssert.Contains("using System;", viewText);
            StringAssert.Contains("private Action<Summon> selectSummon;", viewText);
            StringAssert.Contains("public void SetSelectionHandler(Action<Summon> selectSummon)", viewText);
            StringAssert.Contains("selectSummon?.Invoke(assignedSummon);", viewText);
            Assert.IsFalse(viewText.Contains("SummonController.Instance"), "DrawOptionPanelView should report selection through an injected handler.");
            StringAssert.Contains("ConnectOptionPanels(drawOptionPanels)", controllerText);
            StringAssert.Contains("ConnectOptionPanels(redrawOptionPanels)", controllerText);
            StringAssert.Contains("optionPanel.SetSelectionHandler(OnSelectSummon)", controllerText);
            Assert.IsFalse(controllerText.Contains("public static SummonController Instance"), "SummonController should not keep an unused global Instance.");
            Assert.IsFalse(controllerText.Contains("Instance = this"), "SummonController should not keep singleton assignment after panel selection uses handlers.");
        }

        [Test]
        public void SummonController_DelegatesRedrawPlateSelection()
        {
            string controllerText = File.ReadAllText("Assets/Script/Battle/SummonPick/SummonController.cs");
            string selectionText = File.ReadAllText("Assets/Script/Battle/SummonPick/PlateSelectionController.cs");

            StringAssert.Contains("private PlateSelectionController plateSelectionController;", controllerText);
            StringAssert.Contains("plateSelectionController.CanStartRedrawSelection()", controllerText);
            StringAssert.Contains("plateSelectionController.ShowRedrawSelectablePlates(darkBackground)", controllerText);
            StringAssert.Contains("plateSelectionController.TryGetPlayerPlateIndex(plate, out selectedPlateIndex)", controllerText);
            StringAssert.Contains("public class PlateSelectionController", selectionText);
        }

        [Test]
        public void Player_UsesRedrawButtonHandlerName()
        {
            string text = File.ReadAllText("Assets/Script/Battle/Unit/Player.cs");

            StringAssert.Contains("public void OnRedrawButtonClick()", text);
            StringAssert.Contains("summonController.StartRedraw()", text);
            Assert.IsFalse(text.Contains("OnReSummonBtnClick"), "Player redraw handler should use Draw naming.");
        }

        [Test]
        public void Player_DelegatesSpecialAttackTargetTypeToBattleController()
        {
            string playerText = File.ReadAllText("Assets/Script/Battle/Unit/Player.cs");
            string battleText = File.ReadAllText("Assets/Script/Battle/Flow/BattleController.cs");

            StringAssert.Contains("battleController.HasCurrentSpecialAttackInfo()", playerText);
            StringAssert.Contains("battleController.GetCurrentSpecialAttackInfoIndex()", playerText);
            StringAssert.Contains("battleController.DoesCurrentSpecialAttackTargetPlayerPlate()", playerText);
            Assert.IsFalse(playerText.Contains("PlayerTargetSpecialAttackCheck"), "Player should not keep its own target status type check.");
            Assert.IsFalse(playerText.Contains("StatusType.Heal"), "Player should not decide benefit target status types directly.");
            Assert.IsFalse(playerText.Contains("StatusType.Upgrade"), "Player should not decide benefit target status types directly.");
            Assert.IsFalse(playerText.Contains("StatusType.Shield"), "Player should not decide benefit target status types directly.");

            StringAssert.Contains("public bool HasCurrentSpecialAttackInfo()", battleText);
            StringAssert.Contains("public SpecialAttackInfo GetCurrentSpecialAttackInfo()", battleText);
            StringAssert.Contains("public int GetCurrentSpecialAttackInfoIndex()", battleText);
            StringAssert.Contains("public bool DoesCurrentSpecialAttackTargetPlayerPlate()", battleText);
        }

        [Test]
        public void Player_UsesSingleTargetPlateSelectionWaitFlow()
        {
            string playerText = File.ReadAllText("Assets/Script/Battle/Unit/Player.cs");

            StringAssert.Contains("return WaitForTargetPlateSelection(", playerText);
            StringAssert.Contains("private IEnumerator WaitForTargetPlateSelection(", playerText);
            StringAssert.Contains("private bool MousePositionInsidePlates(List<Plate> targetPlates)", playerText);
            StringAssert.Contains("plateController.GetEnermyPlates(),", playerText);
            StringAssert.Contains("plateController.GetPlayerPlates(),", playerText);
            StringAssert.Contains("plateController.DownTransparencyForWhoPlate(downTransparencyForPlayerPlate);", playerText);
            Assert.AreEqual(1, CountOccurrences(playerText, "foreach (var plate in targetPlates)"), "Player should keep one shared mouse-position plate loop.");
        }

        [Test]
        public void FightScenes_UseRedrawButtonHandlerInUnityEvents()
        {
            string[] scenePaths =
            {
                "Assets/Screen/FightScene/Fight Screen_1Stage.unity",
                "Assets/Screen/FightScene/Fight Screen_2Stage.unity",
                "Assets/Screen/FightScene/Fight Screen_3Stage.unity",
                "Assets/Screen/FightScene/Fight Screen_4Stage.unity",
                "Assets/Screen/FightScene/Fight Screen_5Stage.unity",
                "Assets/Screen/FightScene/Fight Screen_6Stage.unity",
                "Assets/Screen/FightScene/Fight Screen_7Stage.unity"
            };

            foreach (string scenePath in scenePaths)
            {
                string text = File.ReadAllText(scenePath);

                StringAssert.Contains("m_MethodName: OnRedrawButtonClick", text);
                Assert.IsFalse(text.Contains("m_MethodName: OnReSummonBtnClick"), scenePath + " should not use old resummon handler.");
            }
        }

        [Test]
        public void Plate_SetCurrentSummonSynchronizesOccupancy()
        {
            string text = File.ReadAllText("Assets/Script/Battle/Unit/Plate.cs");
            int methodIndex = text.IndexOf("public void SetCurrentSummon(Summon currentSummon)");
            int assignIndex = text.IndexOf("this.currentSummon = currentSummon;", methodIndex);
            int occupancyIndex = text.IndexOf("isInSummon = currentSummon != null;", methodIndex);

            Assert.GreaterOrEqual(methodIndex, 0, "SetCurrentSummon method should exist.");
            Assert.Greater(assignIndex, methodIndex, "SetCurrentSummon should assign the summon reference.");
            Assert.Greater(occupancyIndex, assignIndex, "SetCurrentSummon should synchronize occupancy after assigning the summon.");
        }

        [Test]
        public void Plate_RemoveSummonAlwaysClearsSlotState()
        {
            string text = File.ReadAllText("Assets/Script/Battle/Unit/Plate.cs");
            int methodIndex = text.IndexOf("public void RemoveSummon()");
            int clearSummonIndex = text.IndexOf("SetCurrentSummon(null);", methodIndex);
            int directMoveIndex = text.IndexOf("public void DirectMoveSummon", methodIndex);
            string methodBody = text.Substring(methodIndex, directMoveIndex - methodIndex);

            Assert.GreaterOrEqual(methodIndex, 0, "RemoveSummon method should exist.");
            Assert.Greater(clearSummonIndex, methodIndex, "RemoveSummon should clear slot state through SetCurrentSummon.");
            Assert.IsFalse(methodBody.Contains("if (isInSummon)"), "RemoveSummon should always clear slot state, even if occupancy was already inconsistent.");
        }

        [Test]
        public void TargetedAttackStrategy_GuardsInvalidTargetIndex()
        {
            string strategyText = File.ReadAllText("Assets/Script/Battle/Attack/TargetedAttackStrategy.cs");
            string selectorText = File.ReadAllText("Assets/Script/Battle/Attack/IAttackTargetSelector.cs");
            int strategyMethodIndex = strategyText.IndexOf("public void Attack(Summon attacker, List<Plate> targetPlates, int selectedPlateIndex, int specialAttackArrayIndex)");
            int selectorIndex = selectorText.IndexOf("class SelectedPlateAttackTargetSelector");
            int nullGuardIndex = selectorText.IndexOf("targetPlates == null", selectorIndex);
            int negativeGuardIndex = selectorText.IndexOf("selectedPlateIndex < 0", selectorIndex);
            int upperGuardIndex = selectorText.IndexOf("selectedPlateIndex >= targetPlates.Count", selectorIndex);
            int accessIndex = selectorText.IndexOf("targetPlates[selectedPlateIndex]", selectorIndex);
            int targetPlateNullGuardIndex = selectorText.IndexOf("targetPlate == null", selectorIndex);
            int summonAccessIndex = selectorText.IndexOf("targetPlate.GetCurrentSummon()", selectorIndex);

            Assert.GreaterOrEqual(strategyMethodIndex, 0, "TargetedAttackStrategy.Attack method should exist.");
            Assert.GreaterOrEqual(selectorIndex, 0, "SelectedPlateAttackTargetSelector should own selected target lookup.");
            Assert.Greater(nullGuardIndex, selectorIndex, "Selected target lookup should guard null target plate lists.");
            Assert.Greater(negativeGuardIndex, selectorIndex, "Selected target lookup should guard negative target indexes.");
            Assert.Greater(upperGuardIndex, selectorIndex, "Selected target lookup should guard out-of-range target indexes.");
            Assert.Greater(accessIndex, selectorIndex, "Selected target lookup should access target plates after validation.");
            Assert.Greater(targetPlateNullGuardIndex, accessIndex, "Selected target lookup should guard null target plates.");
            Assert.Greater(summonAccessIndex, targetPlateNullGuardIndex, "Selected target lookup should access summons after target plate validation.");
            Assert.Less(nullGuardIndex, accessIndex, "Null guard should run before target plate access.");
            Assert.Less(negativeGuardIndex, accessIndex, "Negative index guard should run before target plate access.");
            Assert.Less(upperGuardIndex, accessIndex, "Upper bound guard should run before target plate access.");
        }

        [Test]
        public void AreaAndClosestAttackStrategies_GuardNullTargetPlates()
        {
            string areaText = File.ReadAllText("Assets/Script/Battle/Attack/AttackAllEnemiesStrategy.cs");
            string selectorText = File.ReadAllText("Assets/Script/Battle/Attack/IAttackTargetSelector.cs");
            int areaMethodIndex = areaText.IndexOf("public void Attack(Summon attacker, List<Plate> targetPlates, int selectedPlateIndex, int specialAttackArrayIndex)");
            int areaSelectorIndex = selectorText.IndexOf("class AllEnemiesAttackTargetSelector");
            int areaNullGuardIndex = selectorText.IndexOf("targetPlates == null", areaSelectorIndex);
            int areaLoopIndex = selectorText.IndexOf("foreach (Plate plate in targetPlates)", areaSelectorIndex);
            int areaPlateGuardIndex = selectorText.IndexOf("plate == null", areaSelectorIndex);
            int areaSummonAccessIndex = selectorText.IndexOf("plate.GetCurrentSummon()", areaSelectorIndex);

            Assert.GreaterOrEqual(areaMethodIndex, 0, "AttackAllEnemiesStrategy.Attack method should exist.");
            Assert.GreaterOrEqual(areaSelectorIndex, 0, "AllEnemiesAttackTargetSelector should own area target lookup.");
            Assert.Greater(areaNullGuardIndex, areaSelectorIndex, "Area target lookup should guard null target plate lists.");
            Assert.Less(areaNullGuardIndex, areaLoopIndex, "Area target lookup should guard before iterating target plates.");
            Assert.Greater(areaPlateGuardIndex, areaLoopIndex, "Area target lookup should skip null plates.");
            Assert.Less(areaPlateGuardIndex, areaSummonAccessIndex, "Area target lookup should guard null plates before summon access.");

            string closestText = File.ReadAllText("Assets/Script/Battle/Attack/ClosestEnemyAttackStrategy.cs");
            int closestMethodIndex = closestText.IndexOf("public void Attack(Summon attacker, List<Plate> targetPlates, int selectedPlateIndex, int specialAttackArrayIndex)");
            int closestNullGuardIndex = closestText.IndexOf("targetPlates == null", closestMethodIndex);
            int closestSelectorCallIndex = closestText.IndexOf("targetSelector.SelectTargets(attacker, targetPlates, selectedPlateIndex)", closestMethodIndex);
            int closestSelectorIndex = selectorText.IndexOf("class ClosestEnemyAttackTargetSelector");
            int helperNullGuardIndex = selectorText.IndexOf("targetPlates == null", closestSelectorIndex);
            int helperPlateGuardIndex = selectorText.IndexOf("targetPlates[i] == null", closestSelectorIndex);
            int helperSummonAccessIndex = selectorText.IndexOf("targetPlates[i].GetCurrentSummon()", closestSelectorIndex);

            Assert.GreaterOrEqual(closestMethodIndex, 0, "ClosestEnemyAttackStrategy.Attack method should exist.");
            Assert.Greater(closestNullGuardIndex, closestMethodIndex, "ClosestEnemyAttackStrategy should guard null target plate lists.");
            Assert.Less(closestNullGuardIndex, closestSelectorCallIndex, "ClosestEnemyAttackStrategy should guard before resolving closest target.");
            Assert.GreaterOrEqual(closestSelectorIndex, 0, "ClosestEnemyAttackTargetSelector should own closest target lookup.");
            Assert.Greater(helperNullGuardIndex, closestSelectorIndex, "Closest target lookup should also guard null lists.");
            Assert.Greater(helperPlateGuardIndex, helperNullGuardIndex, "ClosestEnemyAttackStrategy should skip null plates.");
            Assert.Less(helperPlateGuardIndex, helperSummonAccessIndex, "ClosestEnemyAttackStrategy should guard null plates before summon access.");
        }

        [Test]
        public void AttackStrategies_DelegateCooldownState()
        {
            string cooldownText = File.ReadAllText("Assets/Script/Battle/Attack/AttackCooldownState.cs");
            string targetedText = File.ReadAllText("Assets/Script/Battle/Attack/TargetedAttackStrategy.cs");
            string closestText = File.ReadAllText("Assets/Script/Battle/Attack/ClosestEnemyAttackStrategy.cs");
            string areaText = File.ReadAllText("Assets/Script/Battle/Attack/AttackAllEnemiesStrategy.cs");

            StringAssert.Contains("public class AttackCooldownState", cooldownText);
            StringAssert.Contains("private readonly int cooldownDuration;", cooldownText);
            StringAssert.Contains("private int currentCooldown;", cooldownText);
            StringAssert.Contains("public void ApplyCooldown()", cooldownText);
            StringAssert.Contains("public void ReduceCooldown()", cooldownText);

            AssertAttackStrategyDelegatesCooldown(targetedText, "TargetedAttackStrategy");
            AssertAttackStrategyDelegatesCooldown(closestText, "ClosestEnemyAttackStrategy");
            AssertAttackStrategyDelegatesCooldown(areaText, "AttackAllEnemiesStrategy");
        }

        [Test]
        public void AttackStrategies_DoNotStoreEffectStatusTime()
        {
            string targetedText = File.ReadAllText("Assets/Script/Battle/Attack/TargetedAttackStrategy.cs");
            string areaText = File.ReadAllText("Assets/Script/Battle/Attack/AttackAllEnemiesStrategy.cs");
            string targetedEffectText = File.ReadAllText("Assets/Script/Battle/Attack/TargetedAttackEffectInstanceCreate.cs");
            string areaEffectText = File.ReadAllText("Assets/Script/Battle/Attack/AllEnemiesAttackEffectInstanceCreate.cs");

            Assert.IsFalse(targetedText.Contains("private int statusTime;"), "TargetedAttackStrategy should not store status duration after effect creation.");
            Assert.IsFalse(areaText.Contains("private int statusTime;"), "AttackAllEnemiesStrategy should not store status duration after effect creation.");
            StringAssert.Contains("TargetedAttackEffectInstanceCreate.Create(statusType, statusTime, damage)", targetedText);
            StringAssert.Contains("AllEnemiesAttackEffectInstanceCreate.Create(statusType, statusTime)", areaText);
            StringAssert.Contains("private int statusTime;", targetedEffectText);
            StringAssert.Contains("private int statusTime;", areaEffectText);
        }

        [Test]
        public void Summon_GuardsNullSpecialAttackArrays()
        {
            string text = File.ReadAllText("Assets/Script/Summons/Summon.cs");

            StringAssert.Contains("if (specialAttackStrategies == null)", text);
            StringAssert.Contains("return availableSpecialAttacks.ToArray();", text);
            StringAssert.Contains("public int GetSpecialAttackCount() => specialAttackStrategies == null ? 0 : specialAttackStrategies.Length;", text);
        }

        [Test]
        public void PlateView_OwnsBasicShowHideOperations()
        {
            string text = File.ReadAllText("Assets/Script/Battle/View/PlateView.cs");

            StringAssert.Contains("public void HidePlates(List<Plate> plates)", text);
            StringAssert.Contains("public void ShowPlates(List<Plate> plates)", text);
            StringAssert.Contains("SetPlatesActive", text);
            StringAssert.Contains("plates == null", text);
            StringAssert.Contains("plate == null", text);
            StringAssert.Contains("plate.gameObject.SetActive(active)", text);
        }

        [Test]
        public void PlateController_DelegatesBasicShowHideToPlateView()
        {
            string text = File.ReadAllText("Assets/Script/Battle/PlateControl/PlateController.cs");

            StringAssert.Contains("[RequireComponent(typeof(PlateView))]", text);
            StringAssert.Contains("PlateView plateView", text);
            StringAssert.Contains("EnsurePlateView()", text);
            StringAssert.Contains("plateView.HidePlates(enermyPlates)", text);
            StringAssert.Contains("plateView.ShowPlates(enermyPlates)", text);
            StringAssert.Contains("plateView.HidePlates(playerPlates)", text);
            StringAssert.Contains("plateView.ShowPlates(playerPlates)", text);
            StringAssert.Contains("plateView.HidePlates(plates)", text);
            StringAssert.Contains("plateView.ShowPlates(plates)", text);
        }

        [Test]
        public void PlateView_OwnsHighlightAndTransparencyOperations()
        {
            string text = File.ReadAllText("Assets/Script/Battle/View/PlateView.cs");

            StringAssert.Contains("public void DownTransparencyForOccupiedPlates(List<Plate> plates)", text);
            StringAssert.Contains("public void HighlightOccupiedPlates(List<Plate> plates)", text);
            StringAssert.Contains("public void ResetOccupiedPlateHighlight(List<Plate> plates)", text);
            StringAssert.Contains("SetOccupiedPlateTransparency(plates, 0.5f)", text);
            StringAssert.Contains("plate.Highlight()", text);
            StringAssert.Contains("plate.Unhighlight()", text);
            StringAssert.Contains("plate.SetSummonImageTransparency(0.5f)", text);
            StringAssert.Contains("plate.SetSummonImageTransparency(1.0f)", text);
            StringAssert.Contains("private bool HasSummon(Plate plate)", text);
            StringAssert.Contains("plate != null && plate.GetCurrentSummon() != null", text);
        }

        [Test]
        public void PlateController_DelegatesHighlightAndTransparencyToPlateView()
        {
            string text = File.ReadAllText("Assets/Script/Battle/PlateControl/PlateController.cs");

            StringAssert.Contains("plateView.DownTransparencyForOccupiedPlates(isPlayer ? playerPlates : enermyPlates)", text);
            StringAssert.Contains("plateView.HighlightOccupiedPlates(playerPlates)", text);
            StringAssert.Contains("plateView.ResetOccupiedPlateHighlight(playerPlates)", text);
            StringAssert.Contains("plateView.HighlightOccupiedPlates(enermyPlates)", text);
            StringAssert.Contains("plateView.ResetOccupiedPlateHighlight(enermyPlates)", text);
            StringAssert.Contains("HideEnemyPlates()", text);
            StringAssert.Contains("ShowEnemyPlates()", text);
            StringAssert.Contains("HidePlayerPlates()", text);
            StringAssert.Contains("ShowPlayerPlates()", text);
        }

        [Test]
        public void PlateController_ClosestEnemyPlateQueryUsesEnemyPlates()
        {
            string text = File.ReadAllText("Assets/Script/Battle/PlateControl/PlateController.cs");
            int methodIndex = text.IndexOf("public int GetClosestEnermyPlateIndexExcept(Summon attackingSummon)");
            int nextMethodIndex = text.IndexOf("public int GetPlayerSummonCount()", methodIndex);

            Assert.GreaterOrEqual(methodIndex, 0, "GetClosestEnermyPlateIndexExcept method should exist.");
            Assert.Greater(nextMethodIndex, methodIndex, "GetClosestEnermyPlateIndexExcept should be followed by GetPlayerSummonCount.");

            string methodBody = text.Substring(methodIndex, nextMethodIndex - methodIndex);
            StringAssert.Contains("return FindClosestOccupiedPlateIndex(enermyPlates, attackingSummon);", methodBody);
            StringAssert.Contains("targetPlates[i].GetCurrentSummon()", text);
            Assert.IsFalse(methodBody.Contains("playerPlates[i].GetCurrentSummon()"), "Enemy plate query should not read player plates.");
        }

        [Test]
        public void PlayerAttackPrediction_DefaultNormalAttackTargetsEnemyPlateIndex()
        {
            string text = File.ReadAllText("Assets/Script/Battle/Prediction/PlayerAttackPrediction.cs");

            StringAssert.Contains("int attackIndex = plateController.GetClosestEnermyPlateIndexExcept(summon);", text);
            StringAssert.Contains("enermyPlates, //타겟 플레이트", text);
            Assert.IsFalse(text.Contains("int attackIndex = plateController.GetClosestPlayerPlateIndex();"), "Player attack prediction should not target a player plate index.");
        }

        [Test]
        public void Plate_DelegatesAttackTargetDecisionToPlateController()
        {
            string plateText = File.ReadAllText("Assets/Script/Battle/Unit/Plate.cs");
            string controllerText = File.ReadAllText("Assets/Script/Battle/PlateControl/PlateController.cs");

            StringAssert.Contains("private bool TrySelectAttackTargetPlate()", plateText);
            StringAssert.Contains("private bool TryGetAttackTargetPlate(out int plateIndex, out string plateName)", plateText);
            StringAssert.Contains("TryGetAttackTargetPlate(out _, out _)", plateText);
            StringAssert.Contains("plateController.TryGetAttackTargetPlate(this, targetsPlayerPlate, out plateIndex, out plateName)", plateText);
            StringAssert.Contains("Debug.Log($\"{plateName}의 플레이트 {plateIndex}가 선택되었습니다.\");", plateText);
            Assert.IsFalse(plateText.Contains("plateController.CanSelectAttackTargetPlate(this, AttackingSummonTargetsPlayerPlate())"), "Plate should use one attack target lookup path for hover and click.");

            StringAssert.Contains("public bool CanSelectAttackTargetPlate(Plate plate, bool targetsPlayerPlate)", controllerText);
            StringAssert.Contains("public int GetAttackTargetPlateIndex(Plate plate, bool targetsPlayerPlate)", controllerText);
            StringAssert.Contains("public string GetAttackTargetPlateName(bool targetsPlayerPlate)", controllerText);
            StringAssert.Contains("public bool TryGetAttackTargetPlate(", controllerText);
            StringAssert.Contains("out int plateIndex", controllerText);
            StringAssert.Contains("out string plateName", controllerText);
            StringAssert.Contains("? ContainsPlayerPlate(plate)", controllerText);
            StringAssert.Contains(": ContainsEnermyPlate(plate)", controllerText);
        }

        [Test]
        public void Plate_DelegatesCurrentSpecialAttackTargetTypeToBattleController()
        {
            string plateText = File.ReadAllText("Assets/Script/Battle/Unit/Plate.cs");
            string battleText = File.ReadAllText("Assets/Script/Battle/Flow/BattleController.cs");

            StringAssert.Contains("battleController.DoesCurrentSpecialAttackTargetPlayerPlate()", plateText);
            Assert.IsFalse(plateText.Contains("StatusType.Heal"), "Plate should not decide benefit target status types directly.");
            Assert.IsFalse(plateText.Contains("StatusType.Upgrade"), "Plate should not decide benefit target status types directly.");
            Assert.IsFalse(plateText.Contains("StatusType.Shield"), "Plate should not decide benefit target status types directly.");

            StringAssert.Contains("public bool DoesCurrentSpecialAttackTargetPlayerPlate()", battleText);
            StringAssert.Contains("public SpecialAttackInfo GetCurrentSpecialAttackInfo()", battleText);
            StringAssert.Contains("private bool DoesAttackStrategyTargetPlayerPlate(IAttackStrategy attackStrategy)", battleText);
            StringAssert.Contains("attackStrategy.BenefitEffectCheck()", battleText);
            Assert.IsFalse(battleText.Contains("StatusType.Heal"), "BattleController should not decide benefit target status types directly.");
            Assert.IsFalse(battleText.Contains("StatusType.Upgrade"), "BattleController should not decide benefit target status types directly.");
            Assert.IsFalse(battleText.Contains("StatusType.Shield"), "BattleController should not decide benefit target status types directly.");
        }

        [Test]
        public void BattleController_UsesSharedSpecialAttackTargetPlateResolution()
        {
            string battleText = File.ReadAllText("Assets/Script/Battle/Flow/BattleController.cs");

            StringAssert.Contains("private List<Plate> GetSpecialAttackTargetPlates(IAttackStrategy attackStrategy, bool isPlayer)", battleText);
            StringAssert.Contains("GetSpecialAttackTargetPlates(targetedAttack, isPlayer)", battleText);
            StringAssert.Contains("GetSpecialAttackTargetPlates(allAttackstrategy, isPlayer)", battleText);
            StringAssert.Contains("GetSpecialAttackTargetPlates(closestAttack, isPlayer)", battleText);
            StringAssert.Contains("bool targetsOwnPlates = attackStrategy.BenefitEffectCheck();", battleText);
        }

        [Test]
        public void EnermyAlgorithm_UsesSharedNormalAttackExecution()
        {
            string algorithmText = File.ReadAllText("Assets/Script/Battle/EnemyAction/EnermyAlgorithm.cs");

            StringAssert.Contains("private void EnemyNormalAttackExecute(Summon attacker, int targetPlateIndex)", algorithmText);
            StringAssert.Contains("private void EnemyHeavyNormalAttackExecute(Summon attacker)", algorithmText);
            Assert.AreEqual(2, CountOccurrences(algorithmText, "EnemyNormalAttackExecute(attacker, targetPlateIndex);"));
            Assert.AreEqual(1, CountOccurrences(algorithmText, "attacker.SetAttackPower(attacker.GetHeavyAttackPower());"));
            StringAssert.Contains("attacker.NormalAttack(plateController.GetPlayerPlates(), targetPlateIndex);", algorithmText);
            StringAssert.Contains("attacker.SetAttackPower(originPower);", algorithmText);
        }

        [Test]
        public void EnermyAlgorithm_UsesSharedSpecialAttackExecutionLog()
        {
            string algorithmText = File.ReadAllText("Assets/Script/Battle/EnemyAction/EnermyAlgorithm.cs");

            StringAssert.Contains("private void EnemySpecialAttackExecute(Summon attacker, int targetPlateIndex, int specialAttackIndex, string logMessage)", algorithmText);
            StringAssert.Contains("battleController.SpecialAttackExecute(attacker, targetPlateIndex, specialAttackIndex);", algorithmText);
            StringAssert.Contains("Debug.Log(logMessage);", algorithmText);
            Assert.LessOrEqual(CountOccurrences(algorithmText, "battleController.SpecialAttackExecute(attacker,"), 1);
            StringAssert.Contains("EnemySpecialAttackExecute(attacker, targetPlateIndex, i,", algorithmText);
            StringAssert.Contains("EnemySpecialAttackExecute(attacker, attackingEnermyPlateIndex, i,", algorithmText);
            StringAssert.Contains("EnemySpecialAttackExecute(attacker, playerPrediction.GetAttackSummonPlateIndex(), i,", algorithmText);
            StringAssert.Contains("private bool CanUseSpecialAttack(Summon attacker, IAttackStrategy attackStrategy)", algorithmText);
            StringAssert.Contains("!attacker.IsSpecialAttackCool(attackStrategy)", algorithmText);
            Assert.LessOrEqual(CountOccurrences(algorithmText, "attacker.IsSpecialAttackCool("), 1);
        }

        [Test]
        public void EnermyAlgorithm_UsesStoredStatusDamageForPrediction()
        {
            string algorithmText = File.ReadAllText("Assets/Script/Battle/EnemyAction/EnermyAlgorithm.cs");
            string summonText = File.ReadAllText("Assets/Script/Summons/Summon.cs");
            int methodStart = algorithmText.IndexOf("private Summon ApplyEnermyStatus(Summon clonedSummon)");
            int methodEnd = algorithmText.IndexOf("public PlateController GetPlateController()", methodStart);

            Assert.GreaterOrEqual(methodStart, 0, "Enemy prediction should keep a status apply method.");
            Assert.Greater(methodEnd, methodStart, "Enemy prediction status method boundary should be visible before public accessors.");

            string activeStatusPredictionText = algorithmText.Substring(methodStart, methodEnd - methodStart);

            StringAssert.Contains("public IReadOnlyList<StatusEffect> GetActiveStatusEffects()", summonText);
            StringAssert.Contains("foreach (StatusEffect statusEffect in clonedSummon.GetActiveStatusEffects())", activeStatusPredictionText);
            StringAssert.Contains("IsDamagePredictionStatus(statusEffect)", activeStatusPredictionText);
            StringAssert.Contains("statusEffect.damagePerTurn", activeStatusPredictionText);
            StringAssert.Contains("statusEffect.statusType == StatusType.Poison", activeStatusPredictionText);
            StringAssert.Contains("statusEffect.statusType == StatusType.Burn", activeStatusPredictionText);
            StringAssert.Contains("statusEffect.statusType == StatusType.LifeDrain", activeStatusPredictionText);
            Assert.IsFalse(activeStatusPredictionText.Contains("GetMaxHP() * 0.1"), "Enemy prediction should use stored poison damage, not recompute a fixed max HP ratio.");
            Assert.IsFalse(activeStatusPredictionText.Contains("GetMaxHP() * 0.2"), "Enemy prediction should use stored burn/life drain damage, not recompute a fixed max HP ratio.");
            Assert.IsFalse(algorithmText.Contains("private int GetLowestMonsterIndex("), "Enemy prediction should not keep empty unused helper methods.");
            Assert.IsFalse(algorithmText.Contains("//private List<Plate> GetApplyStatusEnermyPlates()"), "Enemy prediction should not keep outdated commented implementations.");
            Assert.IsFalse(algorithmText.Contains("//private Summon ApplyEnermyStatus(Summon enermySummon)"), "Enemy prediction should not keep outdated commented status damage implementations.");
        }

        [Test]
        public void EnermyAlgorithm_RestoresAdjustedEnemyPlatesAfterPrediction()
        {
            string algorithmText = File.ReadAllText("Assets/Script/Battle/EnemyAction/EnermyAlgorithm.cs");
            int predictionMethodStart = algorithmText.IndexOf("public List<AttackPrediction> GetPlayerAttackPredictionsList()");
            int predictionMethodEnd = algorithmText.IndexOf("public List<Plate> CheckPlayerPlateState()", predictionMethodStart);
            int applyMethodStart = algorithmText.IndexOf("private List<Plate> GetApplyStatusEnermyPlates(List<Summon> originEnermySummons)");
            int applyMethodEnd = algorithmText.IndexOf("private void RestoreEnermyPlates", applyMethodStart);

            Assert.GreaterOrEqual(predictionMethodStart, 0, "Enemy prediction entry method should exist.");
            Assert.Greater(predictionMethodEnd, predictionMethodStart, "Enemy prediction entry method boundary should be visible.");
            Assert.GreaterOrEqual(applyMethodStart, 0, "Enemy status apply method should accept an origin summon list.");
            Assert.Greater(applyMethodEnd, applyMethodStart, "Enemy status apply method boundary should be visible.");

            string predictionMethodText = algorithmText.Substring(predictionMethodStart, predictionMethodEnd - predictionMethodStart);
            string applyMethodText = algorithmText.Substring(applyMethodStart, applyMethodEnd - applyMethodStart);

            StringAssert.Contains("List<Summon> originEnermySummons = new List<Summon>();", predictionMethodText);
            StringAssert.Contains("GetApplyStatusEnermyPlates(originEnermySummons)", predictionMethodText);
            StringAssert.Contains("try", predictionMethodText);
            StringAssert.Contains("playerAttackPrediction.GetPlayerAttackPredictionList(playerPlates, applyEnermyPlates)", predictionMethodText);
            StringAssert.Contains("finally", predictionMethodText);
            StringAssert.Contains("RestoreEnermyPlates(applyEnermyPlates, originEnermySummons);", predictionMethodText);
            StringAssert.Contains("originEnermySummons.Add(originSummon);", applyMethodText);
            StringAssert.Contains("plate.SetCurrentSummon(adjustedSummon);", applyMethodText);
            Assert.IsFalse(applyMethodText.Contains("plate.SetCurrentSummon(originSummon);"), "Adjusted enemy plates should stay adjusted until prediction finishes.");
            StringAssert.Contains("private void RestoreEnermyPlates(List<Plate> enermyPlates, List<Summon> originSummons)", algorithmText);
            StringAssert.Contains("enermyPlates[i].SetCurrentSummon(originSummons[i]);", algorithmText);
        }

        [Test]
        public void ClosestEnemyAttack_UsesCurrentAttackPowerForBuffedDamage()
        {
            string effectText = File.ReadAllText("Assets/Script/Battle/Attack/ClosestEnemyAttackEffectInstanceCreate.cs");
            int effectMethodIndex = effectText.IndexOf("public void AttackEffectApply(Summon attacker, Summon target, int specialAttackArrayIndex)");
            int takeDamageIndex = effectText.IndexOf("target.TakeDamage(attacker.GetAttackPower());", effectMethodIndex);
            int specialDamageIndex = effectText.IndexOf("GetSpecialDamage()", effectMethodIndex);

            Assert.GreaterOrEqual(effectMethodIndex, 0, "Closest enemy attack effect should have an apply method.");
            Assert.Greater(takeDamageIndex, effectMethodIndex, "Closest enemy attack should use current attack power so buffs and curses affect damage.");
            Assert.AreEqual(-1, specialDamageIndex, "Closest enemy attack should not use fixed strategy damage for runtime damage.");
            StringAssert.Contains("버프/저주/강공격 전환이 반영된 현재 공격력을 사용한다.", effectText);
        }

        [Test]
        public void CatSpecialAttack_KeepsHeavyAttackOnCurrentAttackPowerPath()
        {
            string catText = File.ReadAllText("Assets/Script/Summons/Cat.cs");
            int methodIndex = catText.IndexOf("private void CatSpecialAttackWithHeavyAttackPowerExecute");
            int originIndex = catText.IndexOf("double originAttackPower = GetAttackPower();", methodIndex);
            int heavyPowerIndex = catText.IndexOf("SetAttackPower(GetHeavyAttackPower());", methodIndex);
            int attackIndex = catText.IndexOf("specialAttack.Attack(this, enemyPlates, selectedPlateIndex, specialAttackArrayIndex);", methodIndex);
            int restoreIndex = catText.IndexOf("SetAttackPower(originAttackPower);", methodIndex);

            Assert.GreaterOrEqual(methodIndex, 0, "Cat should keep a dedicated heavy attack execution path.");
            Assert.Greater(originIndex, methodIndex, "Cat should save current attack power before heavy attack conversion.");
            Assert.Greater(heavyPowerIndex, originIndex, "Cat should place heavy attack power on the current attack power path.");
            Assert.Greater(attackIndex, heavyPowerIndex, "Cat should attack after heavy attack conversion.");
            Assert.Greater(restoreIndex, attackIndex, "Cat should restore current attack power after the attack.");
            StringAssert.Contains("강공격도 버프/저주가 반영되는 현재 공격력 경로로 실행한다.", catText);
        }

        private static int CountOccurrences(string text, string value)
        {
            int count = 0;
            int index = 0;

            while ((index = text.IndexOf(value, index)) >= 0)
            {
                count++;
                index += value.Length;
            }

            return count;
        }

        private static void AssertAttackStrategyDelegatesCooldown(string text, string strategyName)
        {
            StringAssert.Contains("private AttackCooldownState cooldownState;", text, strategyName + " should own a cooldown state field.");
            StringAssert.Contains("this.cooldownState = new AttackCooldownState(cooldownDuration);", text, strategyName + " should create cooldown state from constructor data.");
            StringAssert.Contains("return cooldownState.GetCooltime();", text, strategyName + " should delegate cooldown value.");
            StringAssert.Contains("cooldownState.GetCurrentCooldown()", text, strategyName + " should delegate current cooldown.");
            StringAssert.Contains("cooldownState.ApplyCooldown()", text, strategyName + " should delegate cooldown apply.");
            StringAssert.Contains("cooldownState.ReduceCooldown()", text, strategyName + " should delegate cooldown reduction.");
            Assert.IsFalse(text.Contains("private int currentCooldown;"), strategyName + " should not store current cooldown directly.");
        }
    }
}
