using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;

public class DialogueParser : MonoBehaviour
{
    public Dialogue[] Parse(string CSV_FileName)
    {
        List<Dialogue> dialogueList = new List<Dialogue>(); //dialogue����Ʈ
        TextAsset csvData = Resources.Load<TextAsset>(CSV_FileName); //csv���� ������

        string[] data = csvData.text.Split(new char[] {'\n'}); //Enter �� �ึ�� �б�
        Debug.Log(data.Length);
        //csv������ ��ü ��縦 �б� ����
        for(int i=1; i < data.Length;) //�ι�°�ٺ��� �е��� 1���ͽ���
        {
            string[] row = data[i].Split(new char[]{','}); //��ǥ������ �� ���� ���� �е���

            Dialogue dialogue = new Dialogue(); //��� ����Ʈ ����

            dialogue.name = row[1]; //0�� ID, 1�� �̸�, 2�� ���
            Debug.Log(row[1]);
            List<string> contextList = new List<string>(); //��縦 ���� ����Ʈ

            // �� ĳ������ ��縦 ��� �е��� �ϱ� ���� �ݺ���
            do
            {
                // ��� ������ �ִ��� Ȯ���ϰ� �߰�
                //�̰� ���ϸ� ��簡 ����ִµ� ������ �־�����鼭 �����ɸ�
                if (row.Length > 2)
                {
                    contextList.Add(row[2]); // ������ ����Ʈ�� �߰�
                    Debug.Log(row[2]);
                }

                // ���� ���� �ִ��� Ȯ��
                if (++i < data.Length)
                {
                    row = data[i].Split(new char[] { ',' }); // ���� ���� ��ǥ ������ �о��
                }
                else
                {
                    break; // �� �̻� ���� �����Ͱ� ������ �ݺ� ����
                }

            } while (row[0].ToString() == ""); // ���� �ٿ� ID�� ��ĭ�� �ƴ� ������ �ݺ�

            //Dialogue�� ĳ��1�� ��� �������� �ְ� �Ѵ�.
            dialogue.context = contextList.ToArray(); //Dialogue

            dialogueList.Add(dialogue);

        }


        return dialogueList.ToArray();
    }

}
