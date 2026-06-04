using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CBAGameManager : MonoBehaviour
{
    public static CBAGameManager Instance { get; private set; }

    private DaniTechPlayerModel _playerModel;
    private List<CBAEventData> _eventPool = new List<CBAEventData>();
    private bool _hatEventSuccess = false;
    private bool _isGomsuniCompanion = false;
    private CBASpecialEventStepData _currentSpecialEventStep;
    private CBAEventData _currentEvent;
    private bool _isBeeEventSuccess;
    private string _lastFailResultText = string.Empty;
    private bool _isSuccessEnding;

    private bool _hasBeeEventOccurred = false;
    private bool _hasGomsuniEventOccurred = false;

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
        DaniTechUIManager.Instance.CloseCBAEndingCollectionPopupUI();
        _playerModel = new DaniTechPlayerModel();
        _playerModel.CurrentHearts = 3;
        _playerModel.CurrentTurn = 0;
        _currentSpecialEventStep = null;
        _hatEventSuccess = false;
        _isGomsuniCompanion = false;
        _isBeeEventSuccess = false;
        _lastFailResultText = string.Empty;

        _hasBeeEventOccurred = false;
        _hasGomsuniEventOccurred = false;

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

        if (PlayerPrefs.GetInt("CBA_DanceUnlocked", 0) == 1)
        {
            DaniTechUIManager.Instance.PlayCBABearAnimation(BearAnimState.DanceBack);
        }
        else
        {
            DaniTechUIManager.Instance.PlayCBABearAnimation(BearAnimState.Walk);
        }

        DaniTechUIManager.Instance.UpdateCBAHeartUI(_playerModel.CurrentHearts);
        DaniTechUIManager.Instance.UpdateCBATurnUI(_playerModel.CurrentTurn);

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
        if (_playerModel.CurrentTurn == 17)
        {
            LoadTrueEnding();
            return;
        }

        Debug.Log($"[CBA] LoadRandomEvent 호출 / 턴: {_playerModel.CurrentTurn} / 풀 남은 수: {_eventPool.Count}");

        int randomIndex = Random.Range(0, _eventPool.Count);
        _currentEvent = _eventPool[randomIndex];
        _eventPool.RemoveAt(randomIndex);

        DaniTechUIManager.Instance.OpenCBAAdventureUI(_currentEvent.EventTitle, _currentEvent.EventDescription, _currentEvent.Choice1Text, _currentEvent.Choice2Text);
        DaniTechUIManager.Instance.UpdateCBABackground(_currentEvent.BackgroundImageKey);
        DaniTechUIManager.Instance.UpdateCBANPC(_currentEvent.NPCPrefabPath);

        DaniTechUIManager.Instance.PlayCBABearAnimation(GetDefaultBearAnimState());

        DaniTechUIManager.Instance.UpdateCBATurnUI(_playerModel.CurrentTurn);
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
            _lastFailResultText = _currentEvent.DeathResult;
            ReduceHeart();
            DaniTechUIManager.Instance.PlayCBABearAnimation(BearAnimState.Dead);
        }
        else
        {
            int heartsChange = choiceIndex == 0 ? _currentEvent.Choice1HeartsChange : _currentEvent.Choice2HeartsChange;
            if (heartsChange > 0)
            {
                RecoverHeart(heartsChange);
            }
            
            DaniTechUIManager.Instance.PlayCBABearAnimation(BearAnimState.Jump);
        }

        DaniTechSoundManager.Inst.PlaySFX("Sounds/SFX_Select_2", 0.1f);
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
        DaniTechUIManager.Instance.UpdateCBATurnUI(_playerModel.CurrentTurn);
    }

    private void RecoverHeart(int amount)
    {
        _playerModel.CurrentHearts += amount;

        if(_playerModel.CurrentHearts > 3)
        {
            _playerModel.CurrentHearts = 3;
        }

        DaniTechUIManager.Instance.UpdateCBAHeartUI(_playerModel.CurrentHearts);
        DaniTechUIManager.Instance.UpdateCBATurnUI(_playerModel.CurrentTurn);
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
            
            string beeResult = _hasBeeEventOccurred
                ? (_isBeeEventSuccess ? failEnding.BeeResultSuccess : failEnding.BeeResultFail)
                : string.Empty;
            string gomsuniResult = _hasGomsuniEventOccurred
                ? (_isGomsuniCompanion ? failEnding.GomsuniResultSuccess : failEnding.GomsuniResultFail)
                : string.Empty;

            string causeText = beeResult;
            if (string.IsNullOrEmpty(gomsuniResult) == false)
            {
                causeText += (string.IsNullOrEmpty(causeText) ? "" : " ") + gomsuniResult;
            }
            if (string.IsNullOrEmpty(_lastFailResultText) == false)
            {
                causeText += (string.IsNullOrEmpty(causeText) ? "" : " ") + _lastFailResultText;
            }
            string failDisplayText = failEnding.EndingDescription + "\n" + causeText;
            SaveEndingLog(failDisplayText, _playerModel.CurrentTurn, false);

            DaniTechUIManager.Instance.OpenCBAEndingUI(
                failEnding.EndingTitle,
                failEnding.EndingDescription,
                _playerModel.CurrentTurn,
                beeResult,
                gomsuniResult,
                _lastFailResultText,
                _isSuccessEnding
            );

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

            string beeResultSuccess = _hasBeeEventOccurred
                ? (_isBeeEventSuccess ? successEnding.BeeResultSuccess : successEnding.BeeResultFail)
                : string.Empty;
            string gomsuniResultSuccess = _hasGomsuniEventOccurred
                ? (_isGomsuniCompanion ? successEnding.GomsuniResultSuccess : successEnding.GomsuniResultFail)
                : string.Empty;
            DaniTechUIManager.Instance.OpenCBAEndingUI(
                successEnding.EndingTitle,
                successEnding.EndingDescription,
                _playerModel.CurrentTurn,
                beeResultSuccess,
                gomsuniResultSuccess,
                _lastFailResultText,
                _isSuccessEnding
            );

            DaniTechSoundManager.Inst.PlayBGM("Sounds/BGM_Win_LevelClearJingle", 0.1f);
            return;
        }

        LoadRandomEvent();
    }

    private void SaveEndingLog(string displayText, int turnCount, bool isSuccess)
    {
        CBAEndingLogList LogList = LoadEndingLogList();
        CBAEndingLogEntry entry = new CBAEndingLogEntry();
        entry.ResultText = displayText;
        entry.TurnCount = turnCount;
        entry.IsSuccess = isSuccess;
        LogList.Entries.Add(entry);
        string json = JsonUtility.ToJson(LogList);
        PlayerPrefs.SetString("CBA_EndingLog", json);
        PlayerPrefs.Save();
    }

    private CBAEndingLogList LoadEndingLogList()
    {
        string json = PlayerPrefs.GetString("CBA_EndingLog", "");
        if (string.IsNullOrEmpty(json))
        {
            return new CBAEndingLogList();
        }
        return JsonUtility.FromJson<CBAEndingLogList>(json);
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

        DaniTechUIManager.Instance.PlayCBABearAnimation(GetDefaultBearAnimState());

        DaniTechUIManager.Instance.UpdateCBATurnUI(_playerModel.CurrentTurn);
    }

    private void LoadTrueEnding()
    {
        PlayerPrefs.SetInt("CBA_DanceUnlocked", 1);
        PlayerPrefs.Save();
        _isSuccessEnding = true;
        CBAEndingData trueEnding = null;
        foreach (var ending in DaniTechGameDataManager.Instance.CBAEndingDataList.Values)
        {
            if (ending.IsSuccessEnding == true)
            {
                trueEnding = ending;
                break;
            }
        }

        if (trueEnding == null)
        {
            Debug.LogError("[CBAGameManager] 진엔딩 데이터를 찾을 수 없습니다.");
            return;
        }

        DaniTechUIManager.Instance.CloseCBAAdventureUI();
        DaniTechUIManager.Instance.PlayCBABearAnimation(BearAnimState.Walk);

        string beeResultSuccess = _hasBeeEventOccurred
                ? (_isBeeEventSuccess ? trueEnding.BeeResultSuccess : trueEnding.BeeResultFail)
                : string.Empty;
        string gomsuniResultSuccess = _hasGomsuniEventOccurred
            ? (_isGomsuniCompanion ? trueEnding.GomsuniResultSuccess : trueEnding.GomsuniResultFail)
            : string.Empty;

        string trueDisplayText = trueEnding.EndingDescription;
        SaveEndingLog(trueDisplayText, _playerModel.CurrentTurn, true);

        DaniTechUIManager.Instance.OpenCBAEndingUI(
            trueEnding.EndingTitle,
            trueEnding.EndingDescription,
            _playerModel.CurrentTurn,
            beeResultSuccess,
            gomsuniResultSuccess,
            _lastFailResultText,
            _isSuccessEnding
        );

        DaniTechSoundManager.Inst.PlayBGM("Sounds/BGM_Win_LevelClearJingle", 0.1f);
    }

    private void ProcessSpecialEventFinalStep()
    {
        SpecialEventResultType resultType = _currentSpecialEventStep.GetResultType();

        if (resultType == SpecialEventResultType.Fail)
        {
            if (_currentSpecialEventStep.GetSpecialEventType() == SpecialEventType.Gomsuni)
            {
                DaniTechUIManager.Instance.PlayCBANPCAnimation(NPCAnimState.Nope);
            }
            else
            {
                _isBeeEventSuccess = false;
                _lastFailResultText = _currentSpecialEventStep.DeathResult;
                ReduceHeart();
                DaniTechUIManager.Instance.PlayCBABearAnimation(BearAnimState.Dead);
                DaniTechUIManager.Instance.PlayCBANPCAnimation(NPCAnimState.Win);
            }
        }
        else if (resultType == SpecialEventResultType.Success)
        {
            DaniTechUIManager.Instance.PlayCBABearAnimation(BearAnimState.Jump);
            if (_currentSpecialEventStep.GetSpecialEventType() == SpecialEventType.Bee)
            {
                _isBeeEventSuccess = true;
            }
            else if (_currentSpecialEventStep.GetSpecialEventType() == SpecialEventType.Gomsuni)
            {
                _isGomsuniCompanion = true;
                DaniTechUIManager.Instance.PlayCBANPCAnimation(NPCAnimState.Smile);
            }
        }

        DaniTechSoundManager.Inst.PlaySFX("Sounds/SFX_Select_2", 0.1f);

        string bearDialogue = _currentSpecialEventStep.BearDialogue;
        string npcDialogue = _currentSpecialEventStep.NPCDialogue;
        string resultText = resultType == SpecialEventResultType.Success ? "성공!" : "실패!";

        _currentSpecialEventStep = null;

        if (_isGomsuniCompanion)
        {
            DaniTechSoundManager.Inst.PlayBGM("Sounds/BGM_Gomsuni_Adv_SailorBearParade_1", 0.1f);
        }
        else
        {
            DaniTechSoundManager.Inst.PlayBGM("Sounds/BGM_Adv_BearSwanWaltz_5", 0.1f);
        }

        DaniTechUIManager.Instance.ShowCBAAdventureResult(resultText, bearDialogue, npcDialogue);
    }

    private void LoadSpecialEvent(string stepId)
    {
        _currentSpecialEventStep = DaniTechGameDataManager.Instance.GetCBASpecialEventStepData(stepId);

        if (_currentSpecialEventStep == null)
        {
            Debug.LogError($"[CBAGameManager] 특별 이벤트 스텝을 찾을 수 없습니다: {stepId}");
            return;
        }

        if (_currentSpecialEventStep.GetSpecialEventType() == SpecialEventType.Bee)
        {
            _hasBeeEventOccurred = true;
        }
        else if (_currentSpecialEventStep.GetSpecialEventType() == SpecialEventType.Gomsuni)
        {
            _hasGomsuniEventOccurred = true;
        }

        // 추가: 결과 스텝이면 선택지 없이 바로 결과 처리
        SpecialEventResultType resultType = _currentSpecialEventStep.GetResultType();
        if (resultType == SpecialEventResultType.Success || resultType == SpecialEventResultType.Fail)
        {
            DaniTechSoundManager.Inst.PlayBGM(GetSpecialEventBGM(_currentSpecialEventStep.GetSpecialEventType()), 0.1f);
            DaniTechUIManager.Instance.UpdateCBABackground(_currentSpecialEventStep.BackgroundImageKey);
            DaniTechUIManager.Instance.UpdateCBANPC(GetSpecialEventNPCPath(_currentSpecialEventStep.GetSpecialEventType()));
            ProcessSpecialEventFinalStep();
            return;
        }

        DaniTechSoundManager.Inst.PlayBGM(GetSpecialEventBGM(_currentSpecialEventStep.GetSpecialEventType()), 0.1f);
        DaniTechUIManager.Instance.OpenCBAAdventureUIForSpecialEvent(_currentSpecialEventStep.BearDialogue, _currentSpecialEventStep.NPCDialogue, _currentSpecialEventStep.Choice1Text, _currentSpecialEventStep.Choice2Text);
        DaniTechUIManager.Instance.UpdateCBABackground(_currentSpecialEventStep.BackgroundImageKey);
        DaniTechUIManager.Instance.UpdateCBANPC(GetSpecialEventNPCPath(_currentSpecialEventStep.GetSpecialEventType()));

        DaniTechUIManager.Instance.PlayCBABearAnimation(GetDefaultBearAnimState());

        DaniTechUIManager.Instance.UpdateCBATurnUI(_playerModel.CurrentTurn);
    }

    private string GetSpecialEventBGM(SpecialEventType eventType)
    {
        switch (eventType)
        {
            case SpecialEventType.Bee:
                return "Sounds/BGM_Bee_PixelSyrup_Battle_1";
            case SpecialEventType.Gomsuni:
                return "Sounds/BGM_Gomsuni_LanternVillage_Romantic_1";
            default:
                return "Sounds/BGM_Adv_BearOnTheTrain";
        }
    }

    private BearAnimState GetDefaultBearAnimState()
    {
        if (PlayerPrefs.GetInt("CBA_DanceUnlocked", 0) == 1)
        {
            return BearAnimState.DanceBack;
        }
        return BearAnimState.Walk;
    }

    private string GetSpecialEventNPCPath(SpecialEventType eventType)
    {
        switch (eventType)
        {
            case SpecialEventType.Bee:
                return "Prefabs/2D/NPC_CBA/NPC_Bee";
            case SpecialEventType.Gomsuni:
                return "Prefabs/2D/NPC_CBA/NPC_Gomsuni";
            default:
                return string.Empty;
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

        if (resultType == SpecialEventResultType.None)
        {
            string bearDialogue = _currentSpecialEventStep.BearDialogue;
            string npcDialogue = _currentSpecialEventStep.NPCDialogue;
            string noneResultText = _currentSpecialEventStep.ResultText;
            _currentSpecialEventStep = null;
            DaniTechUIManager.Instance.ShowCBAAdventureResult(noneResultText, bearDialogue, npcDialogue);
            return;
        }

        if (resultType == SpecialEventResultType.Fail)
        {
            if (_currentSpecialEventStep.GetSpecialEventType() == SpecialEventType.Gomsuni)
            {
                DaniTechUIManager.Instance.PlayCBANPCAnimation(NPCAnimState.Nope);
            }
            else
            {
                _isBeeEventSuccess = false;
                _lastFailResultText = _currentSpecialEventStep.DeathResult;
                ReduceHeart();
                DaniTechUIManager.Instance.PlayCBABearAnimation(BearAnimState.Dead);
                DaniTechUIManager.Instance.PlayCBANPCAnimation(NPCAnimState.Win);

            }
        }
        else if (resultType == SpecialEventResultType.Success)
        {
            DaniTechUIManager.Instance.PlayCBABearAnimation(BearAnimState.Jump);

            if (_currentSpecialEventStep.GetSpecialEventType() == SpecialEventType.Bee)
            {
                _isBeeEventSuccess = true;
            }
            
            else if (_currentSpecialEventStep.GetSpecialEventType() == SpecialEventType.Gomsuni)
            {
                _isGomsuniCompanion = true;
                DaniTechUIManager.Instance.PlayCBANPCAnimation(NPCAnimState.Smile);
            }
        }

        DaniTechSoundManager.Inst.PlaySFX("Sounds/SFX_Select_2", 0.1f);

        string bear = _currentSpecialEventStep.BearDialogue;
        string npc = _currentSpecialEventStep.NPCDialogue;
        string resultText;
        if (resultType == SpecialEventResultType.Success)
        {
            resultText = "성공!";
        }
        else if (resultType == SpecialEventResultType.Fail)
        {
            resultText = "실패!";
        }
        else
        {
            resultText = _currentSpecialEventStep.ResultText;
        }
        _currentSpecialEventStep = null;

        if (_isGomsuniCompanion)
        {
            DaniTechSoundManager.Inst.PlayBGM("Sounds/BGM_Gomsuni_Adv_SailorBearParade_1", 0.1f);
        }
        else
        {
            DaniTechSoundManager.Inst.PlayBGM("Sounds/BGM_Adv_BearSwanWaltz_5", 0.1f);
        }

        DaniTechUIManager.Instance.ShowCBAAdventureResult(resultText, bear, npc);
    }
}
