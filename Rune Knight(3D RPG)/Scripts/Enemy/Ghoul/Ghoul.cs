using UnityEngine;
using Pathfinding;

public class Ghoul : MonoBehaviour, IDamageable, GetHp, SetStauts
{
    public enum AnimType
    {
        Idle,
        Walk,
        Run,
        Attack1,
        Attack2,
        Death
    }

    public int maxHp = 100;
    [HideInInspector] public int curHp;
    public int attackDamage = 20;
    public float chaseDistance;
    public float attackDistance;

    AnimType animType = AnimType.Idle;
    Player _player;
    public AIPath _aipath;
    public Animation _animation;
    string[] animationName;
    public ParticleSystem hitEff;


    private void Start() => StartFunc();

    private void StartFunc()
    {
        _player = FindObjectOfType<Player>();
        _aipath = GetComponent<AIPath>();
        _animation = GetComponent<Animation>();

        //애니메이션 클립명 저장
        animationName = new string[_animation.GetClipCount()];

        curHp = maxHp;

        int idx = 0;
        foreach(AnimationState state in _animation)
        {
            animationName[idx] = state.name;
            idx++;
        }

        ChangeAnim(AnimType.Idle);
    }

    private void Update() => UpdateFunc();

    private void UpdateFunc()
    {
        
    }

    public void ChangeAnim(AnimType type)
    {
        if (animType == type)
            return;

        _animation.Play(animationName[(int)type]);
        animType = type;
    }

    public void TakeDamage(int val)
    {
        if (curHp <= 0.0f)
            return;

        curHp -= val;

        if(curHp <= 0.0f)
        {
            curHp = 0;
            _aipath.canMove = false;
            ChangeAnim(AnimType.Death);
            this.gameObject.layer = 9;
            if(TryGetComponent(out ChangeColor color))
            {
                color.isDie = true;
            }
        }
    }

    public void PlayEff()
    {
        hitEff.Play();
    }

    public int GetMaxHp()
    {
        return maxHp;
    }

    public int GetCurHp()
    {
        return curHp;
    }

    public void SetStat(int hp, int attackDmg)
    {
        maxHp += hp;
        curHp = maxHp;
        attackDamage += attackDmg;
    }
}