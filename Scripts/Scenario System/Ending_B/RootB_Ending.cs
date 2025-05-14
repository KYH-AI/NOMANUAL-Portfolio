using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class RootB_Ending : MonoBehaviour
{
    /*
     * 1. scColl0 ���� ��, ���� ��� (���� �����ϱ� ������)
     *    - ���� ���̸� ������, �����ϴ� ���� ���� ��ȣ�ۿ��� �� ������ ����
     * 
     * 2. scColl1 ���� ��  (���� ���� ����, ���� �� ����)
     *    - Gamigin 1���� Animation ���
     *    - scBGM FadeIn (���� 0.5)
     *    - ����1 ���
     *    - ���� ���̸� ������, �����ϴ� ���� scColl2�� isTrigger�� false
     * 
     *  3. scColl2 ���� ��, Gamigin���� ī�޶� ���� (scColl1�� ������ ����� ��, ������ ���̱����� �ٰ��� ��)
     * -> Update LookAt, ���ʹϾ� Lerp Ȱ��
     *    - scBGM FadeIn (���� 1)
     *    - Gamigin 2��° Animation ���
     *    - scSfx ���
     *    - Camera ȿ�� ������ �� ������, ���û���
     * 
     *  4. Gamigin 2��° Animation ��� ���� ��
     *    - Camera FadeOut
     *    - ����2 ���
     *    "���� ���������� ���Ҵ�."
     *    "��� �ܷο� �����̾���. ���� ���� ����� ����Ǿ���."
     *    "��� �ǹ̸� �Ұ�, �������� �̵��� �ʹ����� ����."
     *    "��¼�� ���� �� ��ü�� �꿡�� ���߱� ���̴�."
     *    "�װ��� ���� ���� ������. �Ƹ���, ���� ���� ���̴�."
     *    "��� ���� ������ ��Ȥ�ߴ�."
     *    "���� �� ������ ����������."
     *    "�׸��� �̰� ���� ���� ���ϰ� ���� �����̾���."
     *    "������, �����ϰ� ���� ������ ���̴�." 
     *    "����, �ʴ� ���� �Բ� �״´�."
     *    "Gamigin."
     *
     *   - ��� ��, Credit Scene �Ѿ
     */

     [SerializeField] private Transform gamiginTarget;
    [SerializeField] private float rotationSpeed = 1.0f;


}
