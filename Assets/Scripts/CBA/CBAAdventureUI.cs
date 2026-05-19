// AdventureUI의 버튼 바인딩과 이벤트 텍스트 세팅을 담당하는 UI 컴포넌트
using TMPro;
using UnityEngine;

public class CBAAdventureUI : DaniTechUIBase
{
    [SerializeField] private TextMeshProUGUI Text_EventTitle;
    [SerializeField] private TextMeshProUGUI Text_EventDescription;
    [SerializeField] private DaniTechUIButton Btn_Choice1;
    [SerializeField] private DaniTechUIButton Btn_Choice2;
    [SerializeField] private TextMeshProUGUI Text_Choice1;
    [SerializeField] private TextMeshProUGUI Text_Choice2;

    private void OnEnable()
    {
        Btn_Choice1.BindOnClickButtonEvent(OnClickChoice1Button);
        Btn_Choice1.BindOnClickButtonEvent(OnClickChoice2Button);
    }

    public void SetEventUI(string title, string description, string choice1, string choice2)
    {
        Text_EventTitle.text = title;
        Text_EventDescription.text = description;
        Text_Choice1.text = choice1;
        Text_Choice2.text = choice2;
    }

    private void OnClickChoice1Button()
    {
        CBAGameManager.Instance.SelectChoice(0);
    }

    private void OnClickChoice2Button()
    {
        CBAGameManager.Instance.SelectChoice(1);
    }
}
