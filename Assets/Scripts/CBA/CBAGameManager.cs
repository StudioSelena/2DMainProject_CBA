using System.Collections.Generic;
using UnityEngine;

public class CBAGameManager : MonoBehaviour
{
    public static CBAGameManager Instance { get; private set; }

    private DaniTechPlayerModel _playerModel;
    private List<CBAEventData> _eventPool = new List<CBAEventData>();
    private CBAEventData _currentEvent;

    public DaniTechPlayerModel PlayerModel { get { return _playerModel; } }
    public CBAEventData CurrentEvent { get { return _currentEvent; } }

    private void Awake()
    {
        Instance = this;
    }

    public void StartAdventure()
    {
        _playerModel = new DaniTechPlayerModel();
        _playerModel.CurrentHearts = 3;
        _playerModel.CurrentTurn = 0;

        _eventPool = new List<CBAEventData>(DaniTechGameDataManager.Instance.CBAEventDataList.Values);

        DaniTechUIManager.Instance.CloseCBATitleUI();
        LoadRandomEvent();
        DaniTechUIManager.Instance.PlayCBABearAnimation(BearAnimState.Walk);

        DaniTechUIManager.Instance.UpdateCBAHeartUI(_playerModel.CurrentHearts);
    }

    public void LoadRandomEvent()
    {
        if (_eventPool == null || _eventPool.Count == 0)
        {
            return;
        }

        _playerModel.CurrentTurn += 1;
        Debug.Log($"[CBA] LoadRandomEvent 호출 / 턴: {_playerModel.CurrentTurn} / 풀 남은 수: {_eventPool.Count}");

        int randomIndex = Random.Range(0, _eventPool.Count);
        _currentEvent = _eventPool[randomIndex];
        _eventPool.RemoveAt(randomIndex);

        DaniTechUIManager.Instance.OpenCBAAdventureUI(_currentEvent.EventTitle, _currentEvent.EventDescription, _currentEvent.Choice1Text, _currentEvent.Choice2Text);
        DaniTechUIManager.Instance.PlayCBABearAnimation(BearAnimState.Walk);
    }

    public void SelectChoice(int choiceIndex)
    {
        if (_currentEvent == null) return;

        int probability = choiceIndex == 0 ? _currentEvent.Choice1SuccessProbability : _currentEvent.Choice2SuccessProbability;
        bool isSuccess = JudgeSuccessorFail(probability);

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
            return;
        }

        LoadRandomEvent();
    }

    public void GoToTitle()
    {
        DaniTechUIManager.Instance.CloseCBAAdventureUI();
        DaniTechUIManager.Instance.CloseCBAEndingUI();
        DaniTechUIManager.Instance.OpenCBATitleUI();
    }

    public void RestartAdventure()
    {
        DaniTechUIManager.Instance.CloseCBAEndingUI();
        StartAdventure();
    }

    public void OnClickNext()
    {
        CheckEnding();
    }
}
