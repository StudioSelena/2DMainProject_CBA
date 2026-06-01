// EndingUI의 버튼 바인딩과 엔딩 텍스트 세팅을 담당하는 UI 컴포넌트
using TMPro;
using UnityEngine;

public class CBAEndingUI : DaniTechUIBase
{
    [SerializeField] private TextMeshProUGUI Text_EndingTitle;
    [SerializeField] private TextMeshProUGUI Text_EndingDescription;
    [SerializeField] private DaniTechUIButton Btn_ToTitle;
    [SerializeField] private DaniTechUIButton Btn_Restart;
    [SerializeField] private TextMeshProUGUI Text_TurnCount;
    [SerializeField] private GameObject EndingAnimArea;

    private const string DANCE_UNLOCKED_KEY = "CBA_DanceUnlocked";

    private void OnEnable()
    {
        Btn_ToTitle.BindOnClickButtonEvent(OnClickToTitleButton);
        Btn_Restart.BindOnClickButtonEvent(OnClickRestartButton);
    }

    public void SetEndingUI(string title, string description, int turnCount, string beeResult, string gomsuniResult, string lastFailResult, bool isSuccessEnding)
    {
        Text_EndingTitle.text = title;
        Text_TurnCount.text = "총 " + (turnCount -1) + "턴 생존";

        if (isSuccessEnding)
        {
            Text_EndingDescription.text = description;
        }
        else
        {
            string causeText = beeResult + " " + gomsuniResult + " " + lastFailResult;
            Text_EndingDescription.text = description + "\n" + causeText;
        }

        PlayEndingAnimation(isSuccessEnding);
    }

    private void PlayEndingAnimation(bool isSuccessEnding)
    {
        if (EndingAnimArea == null)
        {
            Debug.LogError("[CBAEndingUI] EndingAnimArea가 null입니다.");
            return;
        }

        CBABearAnimatorController bearAnim = EndingAnimArea.GetComponentInChildren<CBABearAnimatorController>(true);
        if (bearAnim == null)
        {
            Debug.LogError("[CBAEndingUI] CBABearAnimatorController를 찾지 못했습니다.");
            return;
        }

        Debug.Log($"[CBAEndingUI] PlayEndingAnimation 호출 / isSuccessEnding: {isSuccessEnding}");

        if (isSuccessEnding)
        {
            bearAnim.SetState(BearAnimState.DanceBack);
        }
        else
        {
            bearAnim.SetState(BearAnimState.Dead);
        }
    }

    private void OnClickToTitleButton()
    {
        CBAGameManager.Instance.GoToTitle();
    }

    private void OnClickRestartButton()
    {
        CBAGameManager.Instance.RestartAdventure();
    }
}
