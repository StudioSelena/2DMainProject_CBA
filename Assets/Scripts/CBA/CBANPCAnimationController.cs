// NPC 캐릭터(벌끔이, 곰순이 등)의 Animator 파라미터를 제어하는 공통 컴포넌트
using UnityEngine;

public enum NPCAnimState
{
    None = 0,
    Idle,
    Shy,
    WalkCompanion,
    Win
}

public class CBANPCAnimationController : MonoBehaviour
{
    [SerializeField] private Animator Animator_NPC;

    private NPCAnimState _currentAnimState;

    public void SetState(NPCAnimState newState)
    {
        if (newState == _currentAnimState)
        {
            return;
        }

        _currentAnimState = newState;

        ResetAllAnimParameters();

        switch (_currentAnimState)
        {
            case NPCAnimState.Idle:
                Animator_NPC.SetBool("IsIdle", true);
                break;
            case NPCAnimState.Shy:
                Animator_NPC.SetBool("IsShy", true);
                break;
            case NPCAnimState.WalkCompanion:
                Animator_NPC.SetBool("IsWalkCompanion", true);
                break;
            case NPCAnimState.Win:
                Animator_NPC.SetBool("IsWin", true);
                break;
        }
    }

    private void ResetAllAnimParameters()
    {
        Animator_NPC.SetBool("IsIdle", false);
        Animator_NPC.SetBool("IsShy", false);
        Animator_NPC.SetBool("IsWalkCompanion", false);
        Animator_NPC.SetBool("IsWin", false);
    }
}
