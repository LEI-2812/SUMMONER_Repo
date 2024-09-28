using System.Collections;
using System.Collections.Generic;
using UnityEditor.Search;
using UnityEngine;

public class DatabaseManager : MonoBehaviour
{
    public static DatabaseManager instance; //�̱������� ��𼭵� �ϳ��� ������Ʈ�� �����ϱ� ����

    [SerializeField] string csv_FileName; //csv�����̸�

    Dictionary<int, Dialogue> dialogueDic = new Dictionary<int, Dialogue>(); //��ųʸ��� ����

    public static bool isFinish;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DialogueParser theParser = GetComponent<DialogueParser>();
            Dialogue[] dialgues = theParser.Parse(csv_FileName);
            for(int i=0; i<dialgues.Length; i++)
            {
                dialogueDic.Add(i+1, dialgues[i]); //�������� Ű�� 1���� ������ ��ųʸ��� �ִ´�. ù����� Ű���� 1�� �Ǵ°���
            }
            isFinish = true;
        }
    }


    public Dialogue[] getDialogue(int startNum, int endNum) //�̰ɷ� ����������
    {
        List<Dialogue> dialogueList = new List<Dialogue>();

        for(int i=0; i <= endNum - startNum; i++) //�� ĳ������ ��簹����ŭ �ݺ�
        {
            dialogueList.Add(dialogueDic[startNum + i]); //������ dialogueDic.Add(i+1, dialgues[i]);�� i+1�̱⶧�� 
        }

        return dialogueList.ToArray(); //�迭���·� �ٲ㼭 ��ȯ
    }
}
