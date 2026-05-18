using System.Collections.Generic;
using System.ComponentModel;
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

        LoadRandomEvent();
    }

    public void LoadRandomEvent()
    {
        if (_eventPool == null || _eventPool.Count == 0)
        {
            Debug.LogError("[CBAGameManager] 이벤트 풀이 비어 있습니다.");
            return;
        }
        
        int randomIndex = Random.Range(0, _eventPool.Count);
        _currentEvent = _eventPool[randomIndex];
    }

    public void SelectChoice(int choiceIndex)
    {
        if (_currentEvent == null) return;

        if (choiceIndex < 0 || choiceIndex >= _currentEvent.Choices.Count) return;

        CBAChoiceData choice = _currentEvent.Choices[choiceIndex];
        bool isSuccess = JudgeSuccessorFail(choice.SuccessProbability);

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
}
