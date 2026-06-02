// 엔딩 도감 팝업 UI를 담당하는 컴포넌트
using UnityEngine;
using UnityEngine.UI;

public class CBAEndingCollectionPopupUI : DaniTechUIBase
{
    [SerializeField] private DaniTechUIButton Btn_Close;
    [SerializeField] private Transform Content_EndingLog;

    private void  OnEnable()
    {
        Btn_Close.BindOnClickButtonEvent(OnClickCloseButton);
        PopulateEndingLog();
    }

    private void PopulateEndingLog()
    {
        string debugJson = PlayerPrefs.GetString("CBA_EndingLog", "");
        Debug.Log($"[CBA] EndingLog JSON: {debugJson}");

        foreach (Transform child in Content_EndingLog)
        {
            Destroy(child.gameObject);
        }

        string json = PlayerPrefs.GetString("CBA_EndingLog", "");
        if (string.IsNullOrEmpty(json))
        {
            return;
        }

        CBAEndingLogList logList = JsonUtility.FromJson<CBAEndingLogList>(json);
        if (logList == null || logList.Entries == null)
        {
            return;
        }

        foreach (CBAEndingLogEntry entry in logList.Entries)
        {
            Debug.Log($"[CBA] 슬롯 생성 시도: {entry.ResultText}");
            GameObject slotObj = DaniTechGameObjectManager.Inst.SpawnCBAEndingSlot(Content_EndingLog);
            Debug.Log($"[CBA] 슬롯 생성 결과: {slotObj}");
            CBAEndingCollectionSlotUI slot = slotObj.GetComponent<CBAEndingCollectionSlotUI>();
            slot.SetSlotData(entry.ResultText, entry.TurnCount, entry.IsSuccess);
        }
    }

    private void OnClickCloseButton()
    {
        DaniTechUIManager.Instance.CloseCBAEndingCollectionPopupUI();
    }
}