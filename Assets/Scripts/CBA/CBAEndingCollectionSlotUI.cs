// 엔딩 도감 슬롯 UI를 담당하는 컴포넌트
using TMPro;
using UnityEngine;

public class CBAEndingCollectionSlotUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI Text_ResultText;
    [SerializeField] private TextMeshProUGUI Text_TurnCount;
    [SerializeField] private TextMeshProUGUI Text_IsSuccess;

    public void SetSlotData(string resultText, int turnCount, bool isSuccess)
    {
        Text_ResultText.text = resultText.TrimEnd();
        Text_TurnCount.text = "생존 턴 수 " + (turnCount - 1) + " / 16";
        Text_IsSuccess.text = isSuccess ? "<모험 성공>" : "<모험 실패>";
    }
}