using UnityEngine;
using Pathfinding;

namespace Mutant
{
    public class MotherMutantAttack : MonoBehaviour
    {
        MotherMutantBase motherBase;
        Vector2 m_PlayerVec = Vector2.zero;

        private void Start() => StartFunc();

        private void StartFunc()
        {
            if (motherBase == null)
                TryGetComponent(out motherBase);
        }

        private void Update() => UpdateFunc();

        private void UpdateFunc()
        {

        }

        public void LookPlayerFunc()
        {
            m_PlayerVec = new Vector2(this.transform.position.x - motherBase.player.transform.position.x,
                                      this.transform.position.y - motherBase.player.transform.position.y);

            float angle = Mathf.Atan2(m_PlayerVec.y, m_PlayerVec.x) * Mathf.Rad2Deg;
            Quaternion angleAxis = Quaternion.AngleAxis(angle + 90.0f, Vector3.forward);
            Quaternion rotation = Quaternion.Slerp(transform.rotation, angleAxis, 5 * Time.deltaTime);
            transform.rotation = rotation;
        }

        public void AttackOrder()
        {
            if (motherBase == null)
                return;

            if(!motherBase.m_AttackedFromPlayer)
            {
                motherBase.m_AttackedFromPlayer = true;
                RootMain.GameManager.instance.AddBattleNum();
                SwitchManager.SetSwitch("isFighting", motherBase.m_AttackedFromPlayer);
            }

            if (motherBase.mutantList.Length <= 0)
            {
                //motherBase._animator.SetBool("FindPlayer", false);
                return;
            }

            for (int i = 0; i < motherBase.mutantList.Length; i++)
            {
                motherBase.mutantList[i].GetComponent<BabyMutantBase>()._aipath.destination = motherBase.player.transform.position;

            }
        }

    }
}