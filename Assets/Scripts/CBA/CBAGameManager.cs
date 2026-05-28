using System.Collections.Generic;
using UnityEngine;

public class CBAGameManager : MonoBehaviour
{
    public static CBAGameManager Instance { get; private set; }

    private DaniTechPlayerModel _playerModel;
    private List<CBAEventData> _eventPool = new List<CBAEventData>();
    private bool _hatEventSuccess = false;
    private CBASpecialEventStepData _currentSpecialEventStep;
    private CBAEventData _currentEvent;

    public DaniTechPlayerModel PlayerModel { get { return _playerModel; } }
    public CBAEventData CurrentEvent { get { return _currentEvent; } }

    public bool IsInSpecialEvent { get { return _currentSpecialEventStep != null; } }

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        DaniTechSoundManager.Inst.PlayBGM("Sounds/BGM_Title_PixelOverworldRun", 0.1f);
    }


    public void StartAdventure()
    {
        _playerModel = new DaniTechPlayerModel();
        _playerModel.CurrentHearts = 3;
        _playerModel.CurrentTurn = 0;
        _currentSpecialEventStep = null;
        _hatEventSuccess = false;

        //모자이벤트는 일반이벤트랜덤풀에서 제외
        _eventPool = new List<CBAEventData>();
        foreach (CBAEventData eventData in DaniTechGameDataManager.Instance.CBAEventDataList.Values)
        {
            if (eventData.Id == "event_hat")
            {
                continue;
            }
            _eventPool.Add(eventData);
        }

        DaniTechUIManager.Instance.CloseCBATitleUI();
        DaniTechGameObjectManager.Inst.SpawnCBAWorldObjects();
        LoadRandomEvent();
        DaniTechUIManager.Instance.PlayCBABearAnimation(BearAnimState.Walk);

        DaniTechUIManager.Instance.UpdateCBAHeartUI(_playerModel.CurrentHearts);

        DaniTechSoundManager.Inst.PlayBGM("Sounds/BGM_Adv_BearOnTheTrain", 0.1f);
    }

    public void LoadRandomEvent()
    {
        if (_eventPool == null || _eventPool.Count == 0)
        {
            return;
        }

        _playerModel.CurrentTurn += 1;

        if (_playerModel.CurrentTurn == 7)
        {
            LoadSpecialEvent("bee_step1");
            return;
        }
        if (_playerModel.CurrentTurn == 11)
        {
            LoadHatEvent();
            return;
        }
        if (_playerModel.CurrentTurn == 12)
        {
            string startStepId = _hatEventSuccess ? "gomsuni_step1_hat_success" : "gomsuni_step1_hat_fail";
            LoadSpecialEvent(startStepId);
            return;
        }

        Debug.Log($"[CBA] LoadRandomEvent 호출 / 턴: {_playerModel.CurrentTurn} / 풀 남은 수: {_eventPool.Count}");

        int randomIndex = Random.Range(0, _eventPool.Count);
        _currentEvent = _eventPool[randomIndex];
        _eventPool.RemoveAt(randomIndex);

        DaniTechUIManager.Instance.OpenCBAAdventureUI(_currentEvent.EventTitle, _currentEvent.EventDescription, _currentEvent.Choice1Text, _currentEvent.Choice2Text);
        DaniTechUIManager.Instance.UpdateCBABackground(_currentEvent.BackgroundImageKey);
        DaniTechUIManager.Instance.UpdateCBANPC(_currentEvent.NPCPrefabPath);
        DaniTechUIManager.Instance.PlayCBABearAnimation(BearAnimState.Walk);
    }

    public void SelectChoice(int choiceIndex)
    {
        if (_currentEvent == null) return;

        int probability = choiceIndex == 0 ? _currentEvent.Choice1SuccessProbability : _currentEvent.Choice2SuccessProbability;


        bool isSuccess = JudgeSuccessorFail(probability);

        if (_currentEvent.Id == "event_hat" && isSuccess)
        {
            _hatEventSuccess = true;
        }

        string resultText;
        if (choiceIndex == 0)
        {
            resultText = isSuccess ? _currentEvent.Choice1SuccessResult : _currentEvent.Choice1FailResult;
        }
        else
        {
            resultText = isSuccess ? _currentEvent.Choice2SuccessResult : _currentEvent.Choice2FailResult;
        }

        if (isSuccess == false)
        {
            ReduceHeart();
            DaniTechUIManager.Instance.PlayCBABearAnimation(BearAnimState.Dead);
        }
        else
        {
            DaniTechUIManager.Instance.PlayCBABearAnimation(BearAnimState.Jump);
        }

        DaniTechSoundManager.Inst.PlaySFX("Sounds/SFX_Select_2", 0.5f);
        DaniTechUIManager.Instance.ShowCBAAdventureResult(resultText);
    }

    private bool JudgeSuccessorFail(int probability)
    {
        int roll = Random.Range(0, 100);
        return roll < probability;
    }

    private void ReduceHeart()
    {
        _playerModel.CurrentHearts -= 1;

        if (_playerModel.CurrentHearts < 0)
        {
            _playerModel.CurrentHearts = 0;
        }

        DaniTechUIManager.Instance.UpdateCBAHeartUI(_playerModel.CurrentHearts);
    }

    private void CheckEnding()
    {
        if (_playerModel.CurrentHearts <= 0)
        {
            CBAEndingData failEnding = null;
            foreach (var ending in DaniTechGameDataManager.Instance.CBAEndingDataList.Values)
            {
                if (ending.IsSuccessEnding == false)
                {
                    failEnding = ending;
                    break;
                }
            }

            if (failEnding == null)
            {
                return;
            }

            DaniTechUIManager.Instance.CloseCBAAdventureUI();
            DaniTechUIManager.Instance.PlayCBABearAnimation(BearAnimState.Walk);
            Debug.Log($"[CBA] 엔딩 표시 / CurrentTurn: {_playerModel.CurrentTurn}");
            DaniTechUIManager.Instance.OpenCBAEndingUI(failEnding.EndingTitle, failEnding.EndingDescription, _playerModel.CurrentTurn);
            DaniTechSoundManager.Inst.PlayBGM("Sounds/BGM_Lose_GameOverDrift", 0.1f);
            return;
        }

        if (_eventPool == null || _eventPool.Count == 0)
        {
            CBAEndingData successEnding = null;
            foreach (var ending in DaniTechGameDataManager.Instance.CBAEndingDataList.Values)
            {
                if (ending.IsSuccessEnding == true)
                {
                    successEnding = ending;
                    break;
                }
            }

            if (successEnding == null)
            {
                Debug.LogError("[CBAGameManager] 성공 엔딩 데이터가 없습니다.");
                return;
            }

            DaniTechUIManager.Instance.CloseCBAAdventureUI();
            DaniTechUIManager.Instance.PlayCBABearAnimation(BearAnimState.Walk);
            DaniTechUIManager.Instance.OpenCBAEndingUI(successEnding.EndingTitle, successEnding.EndingDescription, _playerModel.CurrentTurn);
            DaniTechSoundManager.Inst.PlayBGM("Sounds/BGM_Win_LevelClearJingle", 0.1f);
            return;
        }

        LoadRandomEvent();
    }

    public void GoToTitle()
    {
        DaniTechSoundManager.Inst.StopBGM();
        DaniTechGameObjectManager.Inst.DestroyCBAWorldObjects();
        DaniTechUIManager.Instance.CloseCBAAdventureUI();
        DaniTechUIManager.Instance.CloseCBAEndingUI();
        DaniTechUIManager.Instance.OpenCBATitleUI();
        DaniTechSoundManager.Inst.PlayBGM("Sounds/BGM_Title_PixelOverworldRun", 0.1f);
    }

    public void RestartAdventure()
    {
        DaniTechGameObjectManager.Inst.DestroyCBAWorldObjects();
        DaniTechUIManager.Instance.CloseCBAEndingUI();
        StartAdventure();
    }

    public void OnClickNext()
    {
        CheckEnding();
    }

    private void LoadHatEvent()
    {
        _currentEvent = DaniTechGameDataManager.Instance.GetCBAEventData("event_hat");

        if (_currentEvent == null)
        {
            Debug.LogError("[CBAGameManager] event_hat 데이터를 찾을 수 없습니다.");
            return;
        }

        DaniTechUIManager.Instance.OpenCBAAdventureUI(_currentEvent.EventTitle, _currentEvent.EventDescription, _currentEvent.Choice1Text, _currentEvent.Choice2Text);
        DaniTechUIManager.Instance.UpdateCBABackground(_currentEvent.BackgroundImageKey);
        DaniTechUIManager.Instance.UpdateCBANPC(_currentEvent.NPCPrefabPath);
        DaniTechUIManager.Instance.PlayCBABearAnimation(BearAnimState.Walk);
    }

    private void LoadSpecialEvent(string stepId)
    {
        _currentSpecialEventStep = DaniTechGameDataManager.Instance.GetCBASpecialEventStepData(stepId);

        if (_currentSpecialEventStep == null)
        {
            Debug.LogError($"[CBAGameManager] 특별 이벤트 스텝을 찾을 수 없습니다: {stepId}");
            return;
        }

        DaniTechSoundManager.Inst.PlayBGM(GetSpecialEventBGM(_currentSpecialEventStep.GetSpecialEventType()), 0.1f);
        DaniTechUIManager.Instance.OpenCBAAdventureUIForSpecialEvent(_currentSpecialEventStep.BearDialogue, _currentSpecialEventStep.NPCDialogue, _currentSpecialEventStep.Choice1Text, _currentSpecialEventStep.Choice2Text);
        DaniTechUIManager.Instance.UpdateCBABackground(_currentSpecialEventStep.BackgroundImageKey);
        DaniTechUIManager.Instance.PlayCBABearAnimation(BearAnimState.Walk);
    }

    private string GetSpecialEventBGM(SpecialEventType eventType)
    {
        switch (eventType)
        {
            case SpecialEventType.Bee:
                return "Sounds/BGM_Bee_PixelSyrup_Battle_1";
            default:
                return "Sounds/BGM_Adv_BearOnTheTrain";
        }
    }

    public void SelectChoiceInSpecialEvent(int choiceIndex)
    {
        
        if (_currentSpecialEventStep == null) return;

        int probability = choiceIndex == 0 ? _currentSpecialEventStep.Choice1SuccessProbability : _currentSpecialEventStep.Choice2SuccessProbability;
        bool isSuccess = JudgeSuccessorFail(probability);

        string nextStepId;
        if (choiceIndex == 0)
        {
            nextStepId = isSuccess ? _currentSpecialEventStep.Choice1SuccessNextStepId : _currentSpecialEventStep.Choice1FailNextStepId;
        }
        else
        {
            nextStepId = isSuccess ? _currentSpecialEventStep.Choice2SuccessNextStepId : _currentSpecialEventStep.Choice2FailNextStepId;
        }

        SpecialEventResultType resultType = _currentSpecialEventStep.GetResultType();

        if (resultType == SpecialEventResultType.None && string.IsNullOrEmpty(nextStepId) == false)
        {
            LoadSpecialEvent(nextStepId);
            return;
        }

        if (resultType == SpecialEventResultType.Fail)
        {
            ReduceHeart();
            DaniTechUIManager.Instance.PlayCBABearAnimation(BearAnimState.Dead);
        }
        else if (resultType == SpecialEventResultType.Success)
        {
            DaniTechUIManager.Instance.PlayCBABearAnimation(BearAnimState.Jump);
        }
        else if (resultType == SpecialEventResultType.Companion)
        {
            DaniTechUIManager.Instance.PlayCBABearAnimation(BearAnimState.Jump);
            // 곰순이 동행 처리 — 추후 구현
        }

        DaniTechSoundManager.Inst.PlaySFX("Sounds/SFX_Select_2", 0.1f);
        _currentSpecialEventStep = null;
        DaniTechSoundManager.Inst.PlayBGM("Sounds/BGM_Adv_BearSwanWaltz_5", 0.1f);
        DaniTechUIManager.Instance.ShowCBAAdventureResult(string.Empty);
    }
}
