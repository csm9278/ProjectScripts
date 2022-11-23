using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

namespace Drone
{
    public class DroneFindTarget : MonoBehaviour, ICheckPlace
    {


        // EnemyBase
        public DroneBase droneBase = null;
        public AIPath _aipath;
        public Animator _animator;
        public DroneMoveToTarget droneMove = null;
        public int posIdx;
        public bool debugMode = true;

        //������ ������ ���� ����Ʈ, ���� ���� ����
        public List<Vector2> randPoses;
        public float maxRange;

        //���� ����
        [HideInInspector] public Vector2 firstPos;
        [HideInInspector] public Vector2 goPos;

        //�ִϸ��̼� �ؽ�
        [HideInInspector] int soundedHash = Animator.StringToHash("Sounded");
        [HideInInspector] int findPlayerHash = Animator.StringToHash("FindPlayer");

        //������ �ޱ����� ����
        float randX, randY;
        Vector2 cacVec = Vector2.zero;

        //�Ҹ��������
        bool isSounded = false;

        LayerMask obMask; //Obstacle ���̾� ����ũ

        private void Start() => StartFunc();

        private void StartFunc()
        {
            droneBase = GetComponent<DroneBase>();
            this.TryGetComponent(out droneMove);
            if (droneBase != null)
            {
                _aipath = droneBase._aipath;
                _animator = droneBase._animator;
            }

            obMask = 1 << LayerMask.NameToLayer("Obstacle") | 1 << LayerMask.NameToLayer("MaintanaceArea");
            firstPos = this.gameObject.transform.position;
            debugMode = false;
        }

        private void OnDrawGizmos()
        {
            if(debugMode)
                Gizmos.DrawWireSphere(this.transform.position, maxRange);
            else
                Gizmos.DrawWireSphere(firstPos, maxRange);

            Gizmos.DrawSphere(goPos, 1.2f);
        }

        private void Update() => UpdateFunc();

        private void UpdateFunc()
        {

        }

        /// <summary>
        /// ��� ���� ������ǥ ���� �Լ�
        /// </summary>
        public void DroneMake()
        {
            if(isSounded == true)
            {
                isSounded = false;
                return;
            }    

            if(droneBase.droneType == DroneBase.DroneType.Minimum)
            {
                int findRand = Random.Range(0, 100);
                if(findRand < GlobalData.minimumDroneChasePercent)   // �⺻ 25%Ȯ���� �÷��̾� ���� �̵�
                {
                    droneBase.HittedEnemy();
                    return;
                }
            }


            for(int i = 0; i < 100; i++)
            {
                randX = Random.Range(firstPos.x - (maxRange * .5f), firstPos.x + (maxRange * .5f));
                randY = Random.Range(firstPos.y - (maxRange * .5f), firstPos.y + (maxRange * .5f));

                cacVec = new Vector2(randX, randY);

                Collider2D CheckColl = Physics2D.OverlapCircle(cacVec, 1.2f, obMask);

                if(CheckColl != null)
                {
                    continue;
                }

                goPos = cacVec;
                _aipath.destination = goPos;

                return;
            }
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            RootMain.Breakable_Wall_Ctrl breakwall;
            if(collision.gameObject.TryGetComponent(out breakwall))
            {
                _aipath.destination = firstPos;
                droneMove.reTargetTimer = 20.0f;
                goPos = firstPos;
            }
        }

        public void SoundCheck(Vector3 soundFrom)
        {
            // ���� �߻��� 20 �Ÿ����� �ָ��� ������ ����
            if ((soundFrom - this.transform.position).magnitude >= 20.0f)
                return;
            if (_animator.GetBool(findPlayerHash) == true)
                return;
            if (isSounded)
                return;

            isSounded = true;   //�Ҹ������

            float maxpos = 5.0f;

            for (int i = 0; i < 100; i++)
            {
                randX = Random.Range(soundFrom.x - maxpos, soundFrom.x + maxpos);
                randY = Random.Range(soundFrom.y - maxpos, soundFrom.y + maxpos);

                cacVec = new Vector2(randX, randY);
                Collider2D check = Physics2D.OverlapCircle(cacVec, 1.2f, obMask);

                if (check != null)
                    continue;

                goPos = cacVec;
                _aipath.destination = goPos;
                _animator.SetTrigger(soundedHash);
                return;
            }
        }
    }
}
