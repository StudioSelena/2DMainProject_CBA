// 곰 캐릭터의 Animator 파라미터를 제어하는 컴포넌트
using UnityEngine;

public enum BearAnimState
{
    None = 0,
    Walk,
    Jump,
    Dead,
    Idle,
    DanceBack,
    Atk
}

public class CBABearAnimatorController : MonoBehaviour
{
    [SerializeField] private Animator Animator_Bear;

    private BearAnimState _currentAnimState;

    public void SetState(BearAnimState newState)
    {
        if (newState == BearAnimState.Walk && _currentAnimState == BearAnimState.Walk)
        {
            return;
        }

        _currentAnimState = newState;

        ResetAllAnimParameters();

        switch (_currentAnimState)
        {
            case BearAnimState.Jump:
                Animator_Bear.SetBool("IsJump", true);
                break;
            case BearAnimState.Idle:
                Animator_Bear.SetBool("IsIdle", true);
                break;
            case BearAnimState.Dead:
                Animator_Bear.SetBool("IsDead", true);
                break;
            case BearAnimState.Atk:
                Animator_Bear.SetTrigger("TriggerAtk");
                break;
            case BearAnimState.DanceBack:
                // Milestone 3, 엔딩 해금 시 구현
                break;
            default:
                break;
        }
    }

    private void ResetAllAnimParameters()
    {
        Animator_Bear.SetBool("IsJump", false);
        Animator_Bear.SetBool("IsIdle", false);
        Animator_Bear.SetBool("IsDead", false);
    }
}