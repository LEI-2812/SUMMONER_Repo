// 역할: IPlateState의 책임을 정의한다.
public interface IPlateState
{
    Summon GetCurrentSummon();

    int GetPlateIndex();
}
