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
        _playerModel.CurrentTurn = 1;

        _eventPool = new List<CBAEventData>(DaniTechGameDataManager.Instance.CBAEventDataList.Values);

        DaniTechUIManager.Instance.CloseCBATitleUI();
        LoadRandomEvent();
    }

    public void LoadRandomEvent()
    {
        //널체크
        if (_eventPool == null || _eventPool.Count == 0)
        {
            Debug.LogError("[CBAGameManager] 이벤트 풀이 비어 있습니다.");
            return;
        }

        // 풀에서 랜덤 인덱스로 이벤트 하나 뽑기
        int randomIndex = Random.Range(0, _eventPool.Count);
        _currentEvent = _eventPool[randomIndex];

        DaniTechUIManager.Instance.OpenCBAAdventureUI(_currentEvent.EventTitle, _currentEvent.EventDescription, _currentEvent.Choice1Text, _currentEvent.Choice2Text);
    }

    public void SelectChoice(int choiceIndex)
    {
        if (_currentEvent == null) return;

        int probability = choiceIndex == 0 ? _currentEvent.Choice1SuccessProbability : _currentEvent.Choice2SuccessProbability;
        bool isSuccess = JudgeSuccessorFail(probability);

        if (isSuccess == false)
        {
            ReduceHeart();
        }

        CheckEnding();
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
    }

    private void CheckEnding()
    {
        if (_playerModel.CurrentHearts <=0)
        {
            Debug.Log("[CBAGameManager] 게임오버 - 실패 엔딩");
            return;
        }

        _playerModel.CurrentTurn += 1;
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
        StartAdventure();
        Debug.Log("[CBAGameManager] 어드벤처 재시작");
    }
}
