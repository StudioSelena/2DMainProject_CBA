// 특별 이벤트(벌끔이/곰순이) 데이터 구조 및 결과 타입 정의

using System.Collections.Generic;

// 특별 이벤트 결과 타입
public enum SpecialEventResultType
{
    None,       // 중간 단계 — 다음 스텝으로 계속 진행
    Success,    // 성공 — 랜덤 이벤트로 복귀
    Fail,       // 실패 — 체력 감소 후 랜덤 이벤트로 복귀
    Companion   // 곰순이 동행 — 월드 오브젝트 생성 후 랜덤 이벤트로 복귀
}

// 특별 이벤트 타입
public enum SpecialEventType
{
    Bee,        // 벌끔이 이벤트 (7턴 고정 등장)
    Gomsuni     // 곰순이 이벤트 (12턴 고정 등장)
}

// JSON 필드명은 엑셀 3행(영어 필드명)과 반드시 일치해야 함
[System.Serializable]
public class CBASpecialEventStepData : GameDataBase
{
    // 어떤 특별 이벤트인지 (Bee / Gomsuni)
    public string SpecialEventType;

    // 몇 번째 단계인지 (1~5)
    public int StepIndex;

    // 곰돌이 말풍선 대사
    public string BearDialogue;

    // NPC(벌끔이/곰순이) 말풍선 대사
    public string NPCDialogue;

    // 배경 이미지 키
    public string BackgroundImageKey;

    // 선택지 1
    public string Choice1Text;
    public string Choice1SuccessNextStepId;
    public string Choice1FailNextStepId;
    public int Choice1SuccessProbability;

    // 선택지 2
    public string Choice2Text;
    public string Choice2SuccessNextStepId;
    public string Choice2FailNextStepId;
    public int Choice2SuccessProbability;

    // 결과 타입 — 비어있으면 None(중간 단계), 최종 단계에서만 값 존재
    // "Success" / "Fail" / "Companion" 중 하나
    public string ResultType;
    public string ResultText;

    // ResultType 문자열을 enum으로 변환하는 헬퍼
    public SpecialEventResultType GetResultType()
    {
        switch (ResultType)
        {
            case "Success":
                return SpecialEventResultType.Success;
            case "Fail":
                return SpecialEventResultType.Fail;
            case "Companion":
                return SpecialEventResultType.Companion;
            default:
                return SpecialEventResultType.None;
        }
    }

    // SpecialEventType 문자열을 enum으로 변환하는 헬퍼
    public SpecialEventType GetSpecialEventType()
    {
        switch (SpecialEventType)
        {
            case "Gomsuni":
                return global::SpecialEventType.Gomsuni;
            default:
                return global::SpecialEventType.Bee;
        }
    }
}

[System.Serializable]
public class CBASpecialEventDataList
{
    public List<CBASpecialEventStepData> Steps;
}
