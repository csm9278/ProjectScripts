using UnityEngine;

public class GhoulAttack : MonoBehaviour, IAttackPlayer
{
    GameObject playerObj;
    Player _player;

    Vector3 playerVec;
    Ghoul ghoul;


    float playerDist;
    public float CheckDist;

    bool findPlayer = false;
    float damage1Delay = 0.76f;
    float curDamage1Delay;
    float damage2Delay = 1.14f;
    float curDamage2Delay;
    float attackAnimTime = 1.75f;
    float curAttackAnimTime;
    public int AttackDamage;

    bool attacking = false;
    bool firstAttack = false;
    bool secondAttack = false;

    private void Start() => StartFunc();

    private void StartFunc()
    {
        playerObj = GameObject.Find("Player");
        _player = playerObj.GetComponent<Player>();

        ghoul = GetComponent<Ghoul>();
    }

    private void Update() => UpdateFunc();

    private void UpdateFunc()
    {
        if (ghoul.curHp <= 0.0f)
            return;

        CheckDistFromPlayer();
        AttackPlayer();
        LookPlayer();
    }

    void CheckDistFromPlayer()
    {
        if (playerObj != null && ghoul.curHp > 0)
        {
            playerVec = playerObj.transform.position - this.transform.position;
            playerDist = playerVec.magnitude;

            if (playerDist <= CheckDist && !findPlayer) //첫 한번만 발동 발견후는 진행 안함
            {
                findPlayer = true;
                ghoul.ChangeAnim(Ghoul.AnimType.Run);
                ghoul._aipath.destination = playerObj.transform.position;
                ghoul._aipath.endReachedDistance = 1.5f;
            }

            if(findPlayer && playerDist <= ghoul.attackDistance && !attacking)  //플레이어 발견시
            {
                curDamage1Delay = damage1Delay;
                curDamage2Delay = damage2Delay;
                curAttackAnimTime = attackAnimTime;
                ghoul.ChangeAnim(Ghoul.AnimType.Attack1);
                ghoul._aipath.canMove = false;
                attacking = true;
            }

            if (findPlayer && !attacking)
                ghoul._aipath.destination = _player.transform.position;

        }
    }



    public void AttackPlayer()
    {
        if(attacking)
        {
            curDamage1Delay -= Time.deltaTime;
            curDamage2Delay -= Time.deltaTime;
            curAttackAnimTime -= Time.deltaTime;
            
            if(curDamage1Delay <= 0.0f && !firstAttack)
            {
                if(playerDist <= ghoul.attackDistance + 1.5f)
                    playerObj.GetComponent<Player>().TakeDamage(AttackDamage);
                firstAttack = true;
            }

            if (curDamage2Delay <= 0.0f && !secondAttack)
            {
                if (playerDist <= ghoul.attackDistance + 1.5f)
                    playerObj.GetComponent<Player>().TakeDamage(AttackDamage);

                secondAttack = true;
            }


            if (curAttackAnimTime <= 0.0f)
            {
                attacking = false;
                ghoul.ChangeAnim(Ghoul.AnimType.Run);
                ghoul._aipath.canMove = true;
                ghoul._aipath.destination = _player.transform.position;
                firstAttack = false;
                secondAttack = false;
            }
        }

    }

    public void LookPlayer()
    {
        if (!attacking)
            return;

        Vector3 rot = _player.transform.position - this.transform.position;
        rot.y = 0;

        this.transform.rotation = Quaternion.LookRotation(rot);
    }

    public void SetAttack()
    {
        findPlayer = true;
        ghoul.ChangeAnim(Ghoul.AnimType.Run);
        ghoul._aipath.destination = playerObj.transform.position;
        ghoul._aipath.endReachedDistance = 1.5f;
    }
}