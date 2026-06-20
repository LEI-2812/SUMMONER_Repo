// 역할: 이미 선택된 소환수를 플레이어 플레이트에 배치한다.
public class SummonPlaceExecutor
{
    public void PlaceSummon(
        PlateController plateController,
        PlayerController player,
        int plateIndex,
        Summon summon)
    {
        PlacePlayerSummon(plateController, player, plateIndex, summon, false);
    }

    public void PlaceRedrawSummon(
        PlateController plateController,
        PlayerController player,
        int plateIndex,
        Summon summon)
    {
        PlacePlayerSummon(plateController, player, plateIndex, summon, true);
    }

    private void PlacePlayerSummon(
        PlateController plateController,
        PlayerController player,
        int plateIndex,
        Summon summon,
        bool isResummon)
    {
        plateController.GetPlayerPlates()[plateIndex]
            .SummonPlaceOnPlate(summon, isResummon);
        player.SetHasSummonedThisTurn(true);
    }
}
