using System;
using System.Collections.Generic;

[System.Serializable]
public class GameDataBase
{
    public string Id;
}

// C# 때와 약간 달라진 점
    // Syste.Text.Json대신 유니티 내장 JsonUtility를 사용
    // 따라서 프로퍼티말고 그냥 일반 public 멤버변수로 변경함
    // [System.Serializable]가 없다면 JsonUtility는 데이터를 무시

[System.Serializable]
public class DNCharacterData : GameDataBase
{
    public string Name;
    public string SkillList;
    public string UseWeaponId;
    public string BasicCostumeId;
}

[System.Serializable]
public class DNSkillData : GameDataBase
{
    public string Name;
    public string Description;
}

[System.Serializable]
public class DNWeaponData : GameDataBase
{
    public string Name;
    public string Description;
}

[System.Serializable] 
public class DNCostumeData : GameDataBase
{
    public string Name;
    public string Description;
}

[System.Serializable]
public class DNItemData : GameDataBase
{
    public string Name;
    public string Description;
    public string ItemType;
    public string Grade;
    public string MaxStackCount;
    public string SellingPrice;
    public string IconPath;
}

[System.Serializable]
public class DNDialogueGroupData : GameDataBase
{
    public List<string> DialogueIdList;
}

[System.Serializable]
public class DNDialogueData : GameDataBase
{
    public string CharacterDataId;
    public string Description;
    public string NextDialogueId;
    public List<string> SelectionNameList;
    public List<string> SelectionDialogueIdList;
    public string TexturePath;
    public string VoicePath;
}

[System.Serializable]
public class DNFieldObjectData : GameDataBase
{
    public string Name;
    public string Description;
    public string FieldObjectType;
    public List<int> DropCountRange;
    public string DropItemDataId;
    public string IconPath;
    public string PrefabPath;
}

[System.Serializable]
public class DNMonsterData : GameDataBase
{
    public string Name;
    public string Description;
    public string IconPath;
    public string PrefabPath;
}

[System.Serializable]
public class CBAChoiceData
{
    public string ChoiceText;
    public string SuccessResultText;
    public string FailResultText;
    public int SuccessProbability;
}

[System.Serializable]
public class CBAEventData : GameDataBase
{
    public string EventTitle;
    public string EventDescription;
    public string BackgroundImageKey;
    public string NPCPrefabPath;
    public string Choice1Text;
    public string Choice1SuccessResult;
    public string Choice1FailResult;
    public int Choice1SuccessProbability;
    public string Choice2Text;
    public string Choice2SuccessResult;
    public string Choice2FailResult;
    public int Choice2SuccessProbability;
    public int Choice1HeartsChange;
    public int Choice2HeartsChange;
}

[System.Serializable]
public class CBAEventDataList
{
    public List<CBAEventData> Events;
}

[System.Serializable]
public class CBAEndingData : GameDataBase

{
    public string EndingTitle;
    public string EndingDescription;
    public bool IsSuccessEnding;
}

[System.Serializable]
public class CBAEndingDataList
{
    public List<CBAEndingData> Endings;
}