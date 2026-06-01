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
