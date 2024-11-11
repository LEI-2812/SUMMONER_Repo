using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BattleAlert : MonoBehaviour
{
    [Header("���� ��� �˸�â")]
    public GameObject alertClear; // ���� �¸� �˸�â
    public Alert ClearResult; // ���� �¸� �˸� ���
    public GameObject alertFail; // ���� �й� �˸�â
    public Alert FailResult; // ���� �й� �˸� ���

    [Header("ȿ����")]
    [SerializeField] private AudioSource clearSound;
    [SerializeField] private AudioSource failSound;

    private StageController stageController;

    private void Start()
    {
        // StageController �ν��Ͻ��� ã�� ���� ����
        stageController = FindObjectOfType<StageController>();
        if (stageController == null)
        {
            Debug.LogError("StageController�� ���� �����ϴ�. StageController�� �߰��� �ּ���.");
        }
    }

    public void clearAlert(int stageNum)
    {
        alertClear.SetActive(true);
        clearSound.Play();
        ClearResult.ResetAlert();
        StartCoroutine(WaitForAlertResult(alertClear, ClearResult, (result) => {
            if (result) // ó������ �ٽ��ϱ�
            {
                Debug.Log("������ �ٽ� �����մϴ�.");
                stageController.SendFightStage(stageController.getStageNum()); //�������� ��Ʈ�ѷ��� ������ ����
            }
            else // ���� ����������
            {
                if (stageNum == 7)
                {
                    Debug.Log("���ʷα׷� �̵��մϴ�.");
                    stageController.SendEpilogue();
                }
                else
                {
                    Debug.Log("���� �������� " + (stageNum + 1) + " �� �̵��մϴ�.");
                    PlayerPrefs.SetInt("playingStage", stageNum + 1);
                    PlayerPrefs.SetInt("savedStage", stageNum + 1); //������������ ��ȣ ����
                    PlayerPrefs.Save();
                    stageController.SendStage(stageNum + 1);
                }
            }
        }));
    }

    // �й� �� ȣ���� �Լ�
    public void failAlert(int stageNum)
    {
        alertFail.SetActive(true);
        failSound.Play();
        FailResult.ResetAlert();
        StartCoroutine(WaitForAlertResult(alertFail, FailResult, (result) => {
            if (result) // ó������ �ٽ��ϱ�
            {
                Debug.Log("������ �ٽ� �����մϴ�.");
                stageController.SendFightStage(stageNum);
            }
            else // �������� ����ȭ������
            {
                Debug.Log("�������� ���� ȭ������ �̵��մϴ�.");
                SceneManager.LoadScene("Stage Select Screen");
            }
        }));
    }
  
    private IEnumerator WaitForAlertResult(GameObject alertObject, Alert alertScript, System.Action<bool> callback)
    {
        // �˸�â�� Ȱ��ȭ
        alertObject.SetActive(true);

        // ����ڰ� ��ư�� Ŭ���� ������ ���
        while (!alertScript.getIsClicked())
        {
            yield return null;  // �� ������ ���
        }

        // �˸�â ��Ȱ��ȭ
        alertObject.SetActive(false);

        // ��ư Ŭ�� �� ��� �ݹ� ȣ�� (true: Yes, false: No)
        callback(alertScript.getResult());
    }
}
