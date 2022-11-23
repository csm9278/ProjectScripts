using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Mutant;
using RootMain;

public class EnemySight : MonoBehaviour
{
    //https://uemonwe.tistory.com/23
    [SerializeField] private bool m_bDebugMode = false;

    //Test
    Animator _animator;

    private int findPlayerHash = Animator.StringToHash("FindPlayer");
    private int HittedHash = Animator.StringToHash("Hitted");

    public GameObject player;
    public GameObject attackTarget; //공격할 타겟?
    Vector3 Cacvec;

    [Header("View Config")]
    [Range(0f, 360f)]
    [SerializeField] private float horizontalViewAngle = 0f;
    public float viewRadius = 1f;
    [SerializeField] private float scanRadius = 1f;
    [Range(-180f, 180f)]
    [SerializeField] private float viewRotateZ = 0f;

    [SerializeField] private LayerMask viewTargetMask;
    [SerializeField] private LayerMask viewObstacleMask;

    private float horizontalViewHalfAngle = 0f;

    //마더 뮤턴트 하위 객체용 변수
    CheckMother checkMother = null;

    public bool ignoreSight = false;
    [HideInInspector] public bool inSight = false;

    private void Awake()
    {
        horizontalViewHalfAngle = horizontalViewAngle * 0.5f;
    }

    // 입력한 -180~180의 값을 Up Vector 기준 Local Direction으로 변환시켜줌.
    private Vector3 AngleToDirZ(float angleInDegree)
    {
        float radian = (angleInDegree - transform.eulerAngles.z) * Mathf.Deg2Rad;
        return new Vector3(Mathf.Sin(radian), Mathf.Cos(radian), 0f);
    }

    private void OnDrawGizmos()
    {
        if (m_bDebugMode)
        {
            horizontalViewHalfAngle = horizontalViewAngle * 0.5f;

            Vector3 originPos = transform.position;

            Gizmos.DrawWireSphere(originPos, viewRadius);
            Gizmos.DrawWireSphere(originPos, scanRadius);

            Vector3 horizontalRightDir = AngleToDirZ(-horizontalViewHalfAngle + viewRotateZ);
            Vector3 horizontalLeftDir = AngleToDirZ(horizontalViewHalfAngle + viewRotateZ);
            Vector3 lookDir = AngleToDirZ(viewRotateZ);

            ADebug.DrawRay(originPos, horizontalLeftDir * viewRadius, Color.cyan);
            ADebug.DrawRay(originPos, lookDir * viewRadius, Color.green);
            ADebug.DrawRay(originPos, horizontalRightDir * viewRadius, Color.cyan);

        }
    }

    private void Start() => StartFunc();

    private void StartFunc()
    {
        _animator = GetComponent<Animator>();
        player = GameObject.Find("Player");
        if (checkMother == null)
            checkMother = GetComponent<CheckMother>();

    }

    private void Update() => UpdateFunc();

    private void UpdateFunc()
    {
        viewRotateZ = this.transform.rotation.z * -1;
        FindTarget();
        CheckAttackTarget();
        
    }

    void CheckAttackTarget()
    {
        if(GameManager.instance.turretList.Count > 0)
        {
            Cacvec = player.transform.position - this.transform.position;

            Cacvec.z = 0;

            for (int i = 0; i < GameManager.instance.turretList.Count; i++)
            {
                Vector3 vec = GameManager.instance.turretList[i].transform.position - this.transform.position;
                vec.z = 0;

                if (Cacvec.magnitude > vec.magnitude)
                {
                    attackTarget = GameManager.instance.turretList[i];
                    Cacvec = vec;
                }
                else
                    attackTarget = player;
            }
        }
        else
        {
            attackTarget = player;
        }
    }

    public void FindTarget()
    {
        if (attackTarget == null)
            return;

        if (ignoreSight)
            return;

        Vector2 originPos = transform.position;

        Cacvec = attackTarget.transform.position - this.transform.position;
        Cacvec.z = 0.0f;
        float Dist = Cacvec.magnitude;



        if (Dist < viewRadius)
        {
            if (Dist < scanRadius)
            {
                ADebug.DrawLine(originPos, attackTarget.transform.position, Color.red);

                if (_animator.GetBool(findPlayerHash) == false)
                {
                    _animator.SetBool(findPlayerHash, true);
                    _animator.SetTrigger(HittedHash);
                    inSight = true;
                }
            }

            Vector2 dir = Cacvec.normalized;
            Vector2 lookDir = AngleToDirZ(viewRotateZ);

            float dot = Vector2.Dot(lookDir, dir);
            float angle = Mathf.Acos(dot) * Mathf.Rad2Deg;

            if (angle < horizontalViewHalfAngle)
            {
                RaycastHit2D rayHitedTarget = Physics2D.Raycast(originPos, dir, Dist, viewObstacleMask);
                if (rayHitedTarget)
                {
                    if (m_bDebugMode)
                        ADebug.DrawLine(originPos, rayHitedTarget.point, Color.yellow);

                    if (_animator.GetBool(findPlayerHash) == true)
                        _animator.SetBool(findPlayerHash, false);
                    inSight = false;
                }
                else
                {
                    if (m_bDebugMode)
                        ADebug.DrawLine(originPos, attackTarget.transform.position, Color.red);
                    if (_animator.GetBool(findPlayerHash) == false)
                        _animator.SetBool(findPlayerHash, true);
                    inSight = true;

                    if (checkMother != null)
                        checkMother.FindSignalToMother();
                }
            }
        }
        else
        {
            if (_animator.GetBool(findPlayerHash) == true)
                _animator.SetBool(findPlayerHash, false);
        }
    }


}
