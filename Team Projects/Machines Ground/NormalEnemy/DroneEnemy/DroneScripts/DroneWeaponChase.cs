using System.Collections;
using UnityEngine;
using static Calculator;

namespace Drone
{
    public class DroneWeaponChase : MonoBehaviour
    {
        DroneAttack droneattack;
        Transform playerTr;
        Transform enemyTargeting;
        WaitForFixedUpdate wff = new WaitForFixedUpdate();

        Vector2 playerVec;

        private void Start() => StartFunc();

        private void StartFunc()
        {
            droneattack = GetComponentInParent<DroneAttack>();
            playerTr = GameObject.Find("Player").GetComponent<Transform>();
            enemyTargeting = GameObject.Find("Player").GetComponentInChildren<RootMain.EnemyTargeting>().gameObject.transform;
            StartCoroutine(FixedUpdateCo());

        }

        private void Update()
        {
        }

        public void LookPlayerFunc()
        {
            playerVec = new Vector2(this.transform.position.x - playerTr.transform.position.x,
                                        this.transform.position.y - playerTr.transform.position.y);
            //playerVec = new Vector2(playerTr.transform.position.x - this.transform.position.x,
            //                playerTr.transform.position.y - this.transform.position.y);

            float angle = Mathf.Atan2(playerVec.y, playerVec.x) * Mathf.Rad2Deg;
            Quaternion angleAxis = Quaternion.AngleAxis(angle + 90.0f, Vector3.forward);
            Quaternion rotation = Quaternion.Slerp(transform.rotation, angleAxis, 5 * Time.deltaTime);
            transform.rotation = rotation;
        }

        private IEnumerator FixedUpdateCo()
        {
            while (true)
            {
                if(droneattack._animator != null)
                if (droneattack._animator.GetBool("FindPlayer"))
                    LookPlayerFunc();
                else
                    LookAtTarget();
                yield return wff;
            }
        }

        private void LootAtFront()
        {
            Vector3 dir = (playerTr.position + new Vector3(0, 10, 0)) - this.transform.position;
            dir.z = 0;
            Vector3 lookPos = new Vector3(0, dir.y, 0);

            this.transform.LookAt(lookPos);

            //Vector2 dir = GetVector(-playerTr.eulerAngles.z);
            //float angle = GetAngle(dir);
            //Quaternion angleAxis = Quaternion.AngleAxis(angle, Vector3.up);
            //transform.rotation = angleAxis;
        }

        private void LookAtTarget()
        {
            //Vector2 dir = controller.frontTarget.position - transform.position;
            //float angle = GetAngle(dir);
            //Quaternion angleAxis = Quaternion.AngleAxis(angle - 90f, Vector3.forward);
            //transform.rotation = angleAxis;
            transform.rotation = droneattack.gameObject.transform.rotation;
        }
    }
}