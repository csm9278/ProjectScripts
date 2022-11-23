using UnityEngine;
using Altair_Memory_Pool_Pro;

namespace Mutant
{
    public class SlowWebBullet : MemoryPoolingFlag
    {

        Vector3 targetPoint;
        Vector3 goVec;
        GameObject targetObj;

        public GameObject slowObj;

        private void Start() => StartFunc();

        private void StartFunc()
        {
         
        }

        private void Update() => UpdateFunc();

        private void UpdateFunc()
        {
            if (targetPoint == null)
                return;

            goVec = targetPoint - this.transform.position;
            goVec.z = 0;

            if (goVec.magnitude <= 1.0f)
            {
                GameObject obj = MemoryPoolManager.instance.GetObject("SlowWeb", this.transform.position);
                ObjectReturn();
            }

            Vector3 nextVec = Vector3.Lerp(this.transform.position, targetPoint, 0.05f);
            this.transform.position = nextVec;
        }

        public void SetTarget(Vector3 targetvec, GameObject target)
        {
            targetvec.z = 0;
            targetPoint = targetvec;
            targetObj = target;
        }
    }
}