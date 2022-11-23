using UnityEngine;
using Pathfinding;

public class Enemy : MonoBehaviour, IDamageable, GetHp, SetStauts
{
    public int maxHp = 100;
    public int attackDamage = 10;
    [HideInInspector] public int curHp;
    public float chaseDistance;
    public float attackDistance;

    CapsuleCollider _collider;
    Player _player;
    public AIPath _aipath;
    public Animator _animator;
    [HideInInspector] public EnemyPatrol _enemyPatrol;
    [HideInInspector] public EnemyAttack _enemyAttack;

    public bool findPlayer = false;

    [HideInInspector] public Rigidbody _rigid;

    public ParticleSystem hitEff;

    //°ñµå ¶³±¸±â
    public GameObject GoldObj;

    private void Start() => StartFunc();

    private void StartFunc()
    {
        _rigid = GetComponent<Rigidbody>();

        curHp = maxHp;
        _collider = GetComponent<CapsuleCollider>();
        _player = FindObjectOfType<Player>();
        _aipath = GetComponent<AIPath>();
        _animator = GetComponent<Animator>();
        _enemyPatrol = GetComponent<EnemyPatrol>();
        _enemyAttack = GetComponent<EnemyAttack>();
    }

    //private void Update() => UpdateFunc();

    //private void UpdateFunc()
    //{
        
    //}

    public void TakeDamage(int val)
    {
        if (curHp <= 0)
            return;

        curHp -= val;
        _enemyAttack.EnemyFind();

        if (curHp <= 0)
        {
            curHp = 0;
            ItemDrop();
            _aipath.enabled = false;
            this.gameObject.layer = 9;
            _animator.SetTrigger("Die");

        }
    }

    void ItemDrop()
    {
        int rand = Random.Range(0, 10);
        if(rand < 11)
        {
            GameObject gold = Instantiate(GoldObj);
            gold.transform.position = this.transform.position;
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