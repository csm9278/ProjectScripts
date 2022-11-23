using UnityEngine;
using Pathfinding;

public class PlayerMove : MonoBehaviour
{

    RaycastHit groundHit;
    public LayerMask targetMask;
    public LayerMask obstacleMask;
    public Camera heroCamera;
    public AIPath _aipath;
    public GameObject pickingEff;
    Skill skill;

    public bool canMove = true;

    //남은 거리 체크
    Player player;
    int distanceFromWayPointHash = Animator.StringToHash("DistanceFromWayPoint");

    Vector3 targetPoint = Vector3.zero;
    Vector3 targetDir = Vector3.zero;
    float targetDist = 0.0f;

    //피킹 공격
    //GameObject target;
    Vector3 targetVec = Vector3.zero;
    bool isAttack = false;

    private void Start() => StartFunc();

    private void StartFunc()
    {
        _aipath = GetComponent<AIPath>();
        targetMask = 1 << 7 | 1 << 8;

        player = GetComponent<Player>();
        targetPoint = this.transform.position;

        heroCamera = Camera.main;
        skill = GetComponent<Skill>();
    }

    private void Update() => UpdateFunc();

    private void UpdateFunc()
    {
        if (GlobalData.gameStop)
            return;

        if (!canMove)
            return;

        PickMoveUpdate();
        CheckDistanceFromWayPoint();
        AttackUpdate();

    }

    void PickMoveUpdate()
    {
        if (Input.GetMouseButtonDown(0) && !skill.casting)
        {
            Vector3 mousePos = Input.mousePosition;
            mousePos.z = heroCamera.farClipPlane;

            Vector3 dir = heroCamera.ScreenToWorldPoint(mousePos);

            Debug.DrawRay(heroCamera.transform.position, dir, Color.red, 5.0f);
            Ray ray = new Ray(heroCamera.transform.position, dir);
            if (Physics.Raycast(ray, out groundHit, mousePos.z, targetMask))
            {
                //지형 클릭
                if (groundHit.collider.gameObject.layer == 7)
                {

                    //지형과 너무 가까운곳을 클릭 시 리턴
                    Collider[] coll = Physics.OverlapSphere(groundHit.point, 0.75f, obstacleMask);

                    if (coll.Length > 0)
                    {
                        return;
                    }

                    player.target = null;   //현재 공격중인 타겟 null 처리
                    pickingEff.SetActive(true); //이동 마크 활성화

                    player.ChangeAnim(Player.AnimType.Move);    // 애니메이션 변경

                    isAttack = false;
                    pickingEff.transform.position = groundHit.point;
                    _aipath.destination = groundHit.point;
                    targetPoint = groundHit.point;
                }
                else if(groundHit.collider.gameObject.layer == 8)   //적 클릭
                {

                    //현재 공격중인 타겟이 아니라면
                    if (player.target != groundHit.collider.gameObject)
                    {
                        isAttack = false;
                    }
                    else //현재 공격중인 타겟이라면 리턴
                        return;

                    player.ChangeAnim(Player.AnimType.Move);
                    player.target = groundHit.collider.gameObject;  //타겟 설정
                    Player.SetBackUpTarget(player.target);  //적 체력 표시 UI용 게임오브젝트 백업

                    pickingEff.SetActive(true);
                    pickingEff.transform.position = player.target.transform.position;
                    targetPoint = player.target.transform.position;
                    _aipath.destination = player.target.transform.position;

                }

            }
        }
    }

    void AttackUpdate()
    {
        if (player.target == null)
            return;

        targetVec = player.target.transform.position - this.transform.position;
        if(targetVec.magnitude <= 1.5f)
        {
            if (!isAttack)
            {
                _aipath.destination = this.transform.position;
                pickingEff.SetActive(false);
                player.ChangeAnim(Player.AnimType.Attack);

                isAttack = true;
            }
        }
        else
        {

        }

    }

    void CheckDistanceFromWayPoint()
    {
        targetDir = targetPoint - this.transform.position;
        targetDist = targetDir.magnitude;

        if (targetDist <= 0.2f && pickingEff.activeSelf)
        {
            pickingEff.SetActive(false);
            player.ChangeAnim(Player.AnimType.Idle);

        }
        player._animator.SetFloat(distanceFromWayPointHash, targetDist);
    }
}