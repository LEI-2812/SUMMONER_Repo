using UnityEngine;

// 게임 진행 저장의 진입점 역할을 한다.
// 스토리, 전투, 스테이지 코드는 PlayerPrefs 대신 이 컨트롤러를 통해 저장한다.
public class GameSaveController : MonoBehaviour
{
    public static GameSaveController instance;

    private readonly PlayerPrefsSaveStore saveStore = new PlayerPrefsSaveStore();
    private GameSaveData currentSaveData;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);

            // 시작 시 한 번 불러온 뒤 같은 메모리 상태를 기준으로 진행 값을 갱신한다.
            currentSaveData = saveStore.LoadGameSave();
            return;
        }

        // 씬 이동 후 중복 생성된 저장 컨트롤러는 제거한다.
        Destroy(gameObject);
    }

    // 저장된 진행 데이터가 있는지 반환한다.
    public bool HasGameSave()
    {
        return saveStore.HasGameSave();
    }

    // 현재 저장 데이터를 복사본으로 반환한다.
    public GameSaveData GetGameSave()
    {
        if (currentSaveData == null)
        {
            currentSaveData = saveStore.LoadGameSave();
        }

        return currentSaveData.Clone();
    }

    // 새 게임 시작 상태로 진행 데이터를 초기화하고 저장한다.
    public void StartNewGame()
    {
        currentSaveData = new GameSaveData
        {
            savedStage = 1,
            playingStage = 1
        };

        saveStore.SaveGameSave(currentSaveData);
    }

    // 플레이어가 선택한 현재 플레이 스테이지를 저장한다.
    public void SavePlayingStage(int stage)
    {
        EnsureGameSaveLoaded();
        currentSaveData.playingStage = stage;
        saveStore.SaveGameSave(currentSaveData);
    }

    // 전투 클리어 후 열릴 다음 진행 스테이지를 저장한다.
    // 이전 스테이지를 다시 클리어해도 기존 진행도가 낮아지면 안 된다.
    public void SaveClearedStage(int nextStage)
    {
        EnsureGameSaveLoaded();
        currentSaveData.savedStage = Mathf.Max(currentSaveData.savedStage, nextStage);
        currentSaveData.playingStage = nextStage;
        saveStore.SaveGameSave(currentSaveData);
    }

    // 진행 데이터만 초기화한다. 옵션 설정값은 유지한다.
    public void ResetGameProgress()
    {
        saveStore.ResetGameProgress();
        currentSaveData = new GameSaveData();
    }

    // 다른 스크립트가 이른 시점에 호출해도 저장 데이터가 준비되도록 보장한다.
    private void EnsureGameSaveLoaded()
    {
        if (currentSaveData == null)
        {
            currentSaveData = saveStore.LoadGameSave();
        }
    }
}
