using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CalculateManager : MonoBehaviour
{
    [Header("���� �� UI")]
    [SerializeField] int basicPay = 100;
    [SerializeField] GameObject basicPayElement1;
    [SerializeField] GameObject basicPayElement2;

    [SerializeField] int extraPay = 10;
    [SerializeField] GameObject extraPayElement1;
    [SerializeField] GameObject extraPayElement2;

    [SerializeField] int etcPay = 0;
    [SerializeField] GameObject etcPayElement1;
    [SerializeField] GameObject etcPayElement2;

    [Header("���� �� UI")]
    [SerializeField] int hospitalEx = 35;
    [SerializeField] GameObject hospitalExElement1;
    [SerializeField] GameObject hospitalExElement2;

    [SerializeField] int foodEx = 15;
    [SerializeField] GameObject foodExElement1;
    [SerializeField] GameObject foodExElement2;

    [SerializeField] int etcEx = 0;
    [SerializeField] GameObject etcExElement1;
    [SerializeField] GameObject etcExElement2;

    [Header("����")]
    [SerializeField] int totalPay;
    [SerializeField] GameObject totalPayElement1;
    [SerializeField] GameObject totalPayElement2;

    [SerializeField] int totalEx;
    [SerializeField] GameObject totalExElement1;
    [SerializeField] GameObject totalExElement2;

    [SerializeField] int totalIncome; // totalIncome = totalPay - totalEx

    private void Awake()
    {
        FloatingPay(basicPay, basicPayElement1, basicPayElement2, "�⺻ �޿�");
        FloatingPay(extraPay, extraPayElement1, extraPayElement2, "�߰� �޿�");
        FloatingPay(etcPay, etcPayElement1, etcPayElement2, "�׿� �޿�");
        FloatingEx(hospitalEx, hospitalExElement1, hospitalExElement2, "������");
        FloatingEx(foodEx, foodExElement1, foodExElement2, "�ĺ�");
        FloatingEx(etcEx, etcExElement1, etcExElement2, "�׿� ����");
    }

    private void FloatingPay(int pay, GameObject element1, GameObject element2, string label)
    {
        element1.GetComponent<TextMeshProUGUI>().text = label + ": ";
        element2.GetComponent<TextMeshProUGUI>().text = " $ " + pay.ToString();
        element1.SetActive(true);
        element2.SetActive(true);
    }

    private void FloatingEx(int ex, GameObject element1, GameObject element2, string label)
    {
        element1.GetComponent<TextMeshProUGUI>().text = label + ": ";
        element2.GetComponent<TextMeshProUGUI>().text = " $ " + ex.ToString();
        element1.SetActive(true);
        element2.SetActive(true);
    }
}