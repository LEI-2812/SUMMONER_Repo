using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnermySummonController : MonoBehaviour
{
    private List<Summon> enermySummonList;

    public Summon slime;
    public Summon kingSlime;
    public List<Summon> EnermySummonSetting(int stage)
    {
        switch(stage){
            case 1:
                Debug.Log("������, ŷ�������� ��ȯ�� ");
                enermySummonList.Add(slime);
                enermySummonList.Add(kingSlime);
                break;
            case 2:

                break;
            case 3:

                break;
            case 4:

                break;
            case 5:

                break;
            case 6:

                break;
            case 7:

                break;
            //�������� �߰��� ���⿡ �߰�
            default:
                Debug.Log("�߸��� �������� �Է��� �޾ҽ��ϴ�.");
                break;
        }
        return enermySummonList;
    }
}
