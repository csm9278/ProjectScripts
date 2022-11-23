using UnityEngine;
using Pathfinding;

public class EnemyAttack : MonoBehaviour, IAttackPlayer
{
    GameObject playerObj;
    Player _player;

    Vector3 playerVec;
    Enemy enemy;

    float playerDist;
    public float CheckDist;

    private void Start() => StartFunc();

    private void StartFunc()
    {
        InitMonster();
    }

    private void Update() => UpdateFunc();

    private void UpdateFunc()
    {
        CheckDistFromPlayer();
    }

    void CheckDistFromPlayer()
    {
        //플레이어가 존재하고 자신의 체력이 0 이상일 때
        if(playerObj != null && enemy.curHp > 0)
        {
            //방향 벡터 및 거리 구하기
            playerVec = playerObj.transform.position - this.transform.position;
            playerDist = playerVec.magnitude;
            enemy._animator.SetFloat("DistanceFromPlayer", playerDist);

            //플레이어 발견 시 플레이어 추적
            if (enemy.findPlayer)
                enemy._aipath.destination = playerObj.transform.position;

            if (playerDist <= CheckDist)
            {
                if (Vector3.Dot(transform.forward, playerVec.normalized) < 0.6f && !enemy.findPlayer)
                {
                    return;
                }

                //공격 시 움직이지 않도록
                if (playerDist <= 1.5f)
                {
                    enemy._aipath.destination = this.transform.position;
                    return;
                }

                EnemyFind();
            }
        }
    }

    public void EnemyFind()
    {
        if (enemy.findPlayer)
            return;

        enemy.findPlayer = true;
        enemy._animator.SetBool("FindPlayer", enemy.findPlayer);
        enemy._animator.SetTrigger("Hitted");
    }

    public void InitMonster()
    {
        playerObj = GameObject.Find("Player");
        _player = playerObj.GetComponent<Player>();

        enemy = GetComponent<Enemy>();
    }

    public void AttackPlayer()
    {
        if (GlobalData.gameStop)
            return;

        if (Vector3.Dot(transform.forward, playerVec.normalized) >= 0.5f && playerDist <= 2.5f)
        {
            playerObj.GetComponent<Player>().TakeDamage(enemy.attackDamage);
        }
    }

    public void LookPlayer()
    {
        Vector3 rot = _player.transform.position - this.transform.position;
        rot.y = 0;

        this.transform.rotation = Quaternion.LookRotation(rot);
    }

    public void SetAttack()
    {
        if (enemy.findPlayer)
            return;

        enemy.findPlayer = true;
        enemy._animator.SetBool("FindPlayer", enemy.findPlayer);
        enemy._animator.SetTrigger("Hitted");
    }
}