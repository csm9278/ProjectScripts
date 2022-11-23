using System.Collections;
using UnityEngine;
using Pathfinding;

public class EnemyPatrol : MonoBehaviour
{

    int idleTimeHash = Animator.StringToHash("IdleTime");
    int distanceFromWayPointHash = Animator.StringToHash("DistanceFromWayPoint");

    public float idleTime;

    LayerMask groundLayer;
    public LayerMask obstacleMask;
    RaycastHit groundHit;

    Vector3 nextPos;
    Vector3 calcNextPos;

    Enemy _enemy;

    private void Start() => StartFunc();

    private void StartFunc()
    {
        _enemy = GetComponent<Enemy>();
        idleTime = Random.Range(2.0f, 3.0f);

        groundLayer = 1 << 7;

    }

    private void Update() => UpdateFunc();

    private void UpdateFunc()
    {
        if (_enemy.findPlayer)
            return;

        IdleTimeFunc();
        CheckDistance();
    }

    void CheckDistance()
    {
        if (idleTime >= 0.0f)
            return;

        calcNextPos = nextPos - this.transform.position;

        float dist = calcNextPos.magnitude;
        _enemy._animator.SetFloat(distanceFromWayPointHash, dist);

        if(dist <= 0.2f)
        {
            idleTime = Random.Range(4.0f, 5.0f);
        }
    }

    void IdleTimeFunc()
    {
        if(idleTime >= 0.0f)
        {
            idleTime -= Time.deltaTime;
            _enemy._animator.SetFloat(idleTimeHash, idleTime);
            if(idleTime <= 0.0f)
            {
                SetPos();
            }
        }
    }

    void SetPos()
    {
        Vector3 SetPos = this.transform.position;
        SetPos.y += 5.0f;

        for(int i = 0; i < 100; i++)
        {
            Vector3 randPos = new Vector3(Random.Range(this.transform.position.x - 20.0f, this.transform.position.x + 20.0f),
                                          -20.0f,
                                          Random.Range(this.transform.position.z - 20.0f, this.transform.position.z + 20.0f));
            Vector3 dir = randPos - SetPos;

            Debug.DrawRay(SetPos, dir, Color.yellow, 5.0f);
            Ray ray = new Ray(SetPos, dir);
            if (Physics.Raycast(ray, out groundHit, 50.0f, groundLayer))
            {
                if (groundHit.collider.gameObject.layer == 7)
                {
                    Collider[] coll = Physics.OverlapSphere(groundHit.point, 0.75f, obstacleMask);

                    if (coll.Length > 0)
                    {
                        continue;
                    }


                    _enemy._aipath.destination = groundHit.point;
                    nextPos = groundHit.point;
                    break;
                }
            }
        }

    }
}