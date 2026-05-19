// AdventureUI의 버튼 바인딩과 이벤트 텍스트 세팅을 담당하는 UI 컴포넌트
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CBAAdventureUI : DaniTechUIBase
{
    [SerializeField] private TextMeshProUGUI Text_EventTitle;
    [SerializeField] private TextMeshProUGUI Text_EventDescription;
    [SerializeField] private TextMeshProUGUI Text_Choice1;
    [SerializeField] private TextMeshProUGUI Text_Choice2;
    [SerializeField] private DaniTechUIButton Btn_Choice1;
    [SerializeField] private DaniTechUIButton Btn_Choice2;
    [SerializeField] private DaniTechUIButton Btn_Next;

    private void OnEnable()
    {
        Btn_Choice1.BindOnClickButtonEvent(OnClickChoice1Button);
        Btn_Choice2.BindOnClickButtonEvent(OnClickChoice2Button);
        Btn_Next.BindOnClickButtonEvent(OnClickNextButton);
        Btn_Next.gameObject.SetActive(false);
    }

    public void SetEventUI(string title, string description, string choice1, string choice2)
    {
        Text_EventTitle.text = title;
        Text_EventDescription.text = description;
        Text_Choice1.text = choice1;
        Text_Choice2.text = choice2;
        SetChoiceButtonsInteractable(true);
        Btn_Next.gameObject.SetActive(false);
    }

    public void ShowResultText(string resultText)
    {
        Text_EventTitle.text = "결과";
        Text_EventDescription.text = resultText;
        SetChoiceButtonsInteractable(false);
        Btn_Next.gameObject.SetActive(true);
    }

    public void SetChoiceButtonsInteractable(bool isInteractable)
    {
        Btn_Choice1.GetComponentInChildren<Button>().interactable = isInteractable;
        Btn_Choice2.GetComponentInChildren<Button>().interactable = isInteractable;
    }

    private void OnClickChoice1Button()
    {
        CBAGameManager.Instance.SelectChoice(0);
    }

    private void OnClickChoice2Button()
    {
        CBAGameManager.Instance.SelectChoice(1);
    }

    private void OnClickNextButton()
    {
        CBAGameManager.Instance.OnClickNext();
    }
}