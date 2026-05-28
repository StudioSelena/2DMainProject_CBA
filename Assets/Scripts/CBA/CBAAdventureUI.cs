// AdventureUI의 버튼 바인딩과 이벤트 텍스트 세팅을 담당하는 UI 컴포넌트
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CBAAdventureUI : DaniTechUIBase
{
    [Header("말풍선 다이얼로그")]
    [SerializeField] private GameObject Panel_BearDialogue;
    [SerializeField] private TextMeshProUGUI Text_BearDialogue;
    [SerializeField] private GameObject Panel_NPCDialogue;
    [SerializeField] private TextMeshProUGUI Text_NPCDialogue;

    [Header("이벤트 텍스트")]
    [SerializeField] private TextMeshProUGUI Text_EventTitle;
    [SerializeField] private TextMeshProUGUI Text_EventDescription;

    [Header("선택지 버튼")]
    [SerializeField] private TextMeshProUGUI Text_Choice1;
    [SerializeField] private TextMeshProUGUI Text_Choice2;
    [SerializeField] private DaniTechUIButton Btn_Choice1;
    [SerializeField] private DaniTechUIButton Btn_Choice2;
    [SerializeField] private DaniTechUIButton Btn_Next;

    [Header("상태 UI 하트")]
    [SerializeField] private GameObject Heart1;
    [SerializeField] private GameObject Heart2;
    [SerializeField] private GameObject Heart3;

    private void OnEnable()
    {
        Btn_Choice1.BindOnClickButtonEvent(OnClickChoice1Button);
        Btn_Choice2.BindOnClickButtonEvent(OnClickChoice2Button);
        Btn_Next.gameObject.SetActive(false);
    }

    // 일반 이벤트 UI 세팅
    public void SetEventUI(string title, string description, string choice1, string choice2)
    {
        Text_EventTitle.text = title;
        Text_EventDescription.text = description;
        Text_Choice1.text = choice1;
        Text_Choice2.text = choice2;
        SetChoiceButtonsInteractable(true);
        Btn_Next.gameObject.SetActive(false);
        SetDialoguePanelsActive(false, string.Empty, false, string.Empty);
    }

    // 특별 이벤트 UI 세팅 — 말풍선 패널 표시, 일반 텍스트 비움
    public void SetSpecialEventUI(string bearDialogue, string npcDialogue, string choice1, string choice2)
    {
        Text_EventTitle.text = string.Empty;
        Text_EventDescription.text = string.Empty;
        Text_Choice1.text = choice1;
        Text_Choice2.text = choice2;
        SetChoiceButtonsInteractable(true);
        Btn_Next.gameObject.SetActive(false);

        bool showBear = string.IsNullOrEmpty(bearDialogue) == false;
        bool showNPC = string.IsNullOrEmpty(npcDialogue) == false;
        SetDialoguePanelsActive(showBear, bearDialogue, showNPC, npcDialogue);
    }

    // 결과 텍스트 표시 — 말풍선 숨기고 일반 텍스트 영역에 결과 표시
    public void ShowResultText(string resultText)
    {
        SetDialoguePanelsActive(false, string.Empty, false, string.Empty);
        Text_EventTitle.text = "결과";
        Text_EventDescription.text = resultText;
        SetChoiceButtonsInteractable(false);
        Btn_Next.BindOnClickButtonEvent(OnClickNextButton);
        Btn_Next.gameObject.SetActive(true);
    }

    private void SetDialoguePanelsActive(bool showBear, string bearText, bool showNPC, string npcText)
    {
        Panel_BearDialogue.SetActive(showBear);
        Panel_NPCDialogue.SetActive(showNPC);

        if (showBear)
        {
            Text_BearDialogue.text = bearText;
        }
        if (showNPC)
        {
            Text_NPCDialogue.text = npcText;
        }
    }

    public void SetChoiceButtonsInteractable(bool isInteractable)
    {
        Btn_Choice1.GetComponentInChildren<Button>().interactable = isInteractable;
        Btn_Choice2.GetComponentInChildren<Button>().interactable = isInteractable;
    }

    private void OnClickChoice1Button()
    {
        if (CBAGameManager.Instance.IsInSpecialEvent)
        {
            CBAGameManager.Instance.SelectChoiceInSpecialEvent(0);
            return;
        }
        CBAGameManager.Instance.SelectChoice(0);
    }

    private void OnClickChoice2Button()
    {
        if (CBAGameManager.Instance.IsInSpecialEvent)
        {
            CBAGameManager.Instance.SelectChoiceInSpecialEvent(1);
            return;
        }
        CBAGameManager.Instance.SelectChoice(1);
    }

    private void OnClickNextButton()
    {
        CBAGameManager.Instance.OnClickNext();
    }

    public void UpdateHeartUI(int currentHearts)
    {
        Heart1.SetActive(currentHearts >= 1);
        Heart2.SetActive(currentHearts >= 2);
        Heart3.SetActive(currentHearts >= 3);
    }
}