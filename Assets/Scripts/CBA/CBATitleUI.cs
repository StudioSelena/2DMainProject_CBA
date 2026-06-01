// TitleUI의 버튼 바인딩을 담당하는 UI 컴포넌트
using System;
using UnityEngine;
using UnityEngine.UI;

public class CBATitleUI : DaniTechUIBase
{
    [SerializeField] private DaniTechUIButton Btn_StartAdventure;
    [SerializeField] private DaniTechUIButton Btn_Shop;
    [SerializeField] private DaniTechUIButton Btn_Inventory;
    [SerializeField] private DaniTechUIButton Btn_EndingCollection;

    private const string TUTORIAL_SEEN_KEY = "CBA_TutorialSeen";

    private void OnEnable()
    {
        Btn_StartAdventure.SetInteractable(true);
        Btn_StartAdventure.BindOnClickButtonEvent(OnClickStartAdventureButton);
        Btn_Shop.BindOnClickButtonEvent(OnClickShopButton);
        Btn_Inventory.BindOnClickButtonEvent(OnClickInventoryButton);
        Btn_EndingCollection.BindOnClickButtonEvent(OnClickEndingCollectionButton);
    }

    private void OnClickStartAdventureButton()
    {
        if (PlayerPrefs.GetInt(TUTORIAL_SEEN_KEY, 0) == 1)
        {
            CBAGameManager.Instance.StartAdventure();
            return;
        }

        Btn_StartAdventure.SetInteractable(false);
        DaniTechUIManager.Instance.OpenCBATutorialPopup();
    }
    private void OnClickShopButton()
    {
        // Milestone 3
    }

    private void OnClickInventoryButton()
    {
        // Milestone 3
    }

    private void OnClickEndingCollectionButton()
    {
        // Milestone 3
    }
}