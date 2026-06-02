// 게임 시작 전 튜토리얼 안내를 표시하는 팝업 UI
using UnityEngine;
using TMPro;

public class CBATutorialPopup : DaniTechUIBase
{
    [SerializeField] private TextMeshProUGUI Text_Msg;
    [SerializeField] private DaniTechUIButton Btn_Confirm;

    private const string TUTORIAL_SEEN_KEY = "CBA_TutorialSeen";

    private void OnEnable()
    {
        Text_Msg.text = "엉춤을 찾아 떠나자\r\n7턴 벌끔이 등장\r\n12턴 곰순이 등장\r\n17턴 엔딩\r\n!성공 시 엉춤 해금!";
        Btn_Confirm.BindOnClickButtonEvent(OnClickConfirmButton);
    }

    private void OnClickConfirmButton()
    {
        PlayerPrefs.SetInt(TUTORIAL_SEEN_KEY, 1);
        PlayerPrefs.Save();
        DaniTechUIManager.Instance.ClosePopupUI(DaniTechUIType.CBATutorialPopup);
        CBAGameManager.Instance.StartAdventure();
    }
}