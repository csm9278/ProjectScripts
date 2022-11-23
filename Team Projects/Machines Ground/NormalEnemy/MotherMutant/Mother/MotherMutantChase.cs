using UnityEngine;

namespace Mutant
{
    public class MotherMutantChase : MonoBehaviour
    {
        public MotherMutantBase motherBase = null;
        public GameObject player = null;

        private void Start() => StartFunc();

        private void StartFunc()
        {
            motherBase = GetComponent<MotherMutantBase>();

            player = GameObject.Find("Player");
        }

        //private void Update() => UpdateFunc();

        //private void UpdateFunc()
        //{

        //}

        
        /// <summary>
        /// ���� ��ü Ÿ�� ���� �� ���� ����Ʈ�� ����
        /// </summary>
        public void MotherSetDestination()
        {
            //motherBase._aipath.destination = player.transform.position;

            if (motherBase.mutantList.Length > 0)
            {
                for (int i = 0; i < motherBase.mutantList.Length; i++)
                {
                    motherBase.mutantList[i].GetComponent<EnemyBase_2>()._aipath.destination = player.transform.position;
                }
            }
        }
    }
}