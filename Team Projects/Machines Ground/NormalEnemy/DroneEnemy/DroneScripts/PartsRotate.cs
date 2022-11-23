using UnityEngine;

namespace RootMain
{
    public class PartsRotate : MonoBehaviour
    {
        float firstSpeed = 100.0f;
        Vector3 rotVec;

        Rigidbody rigid;

        private void Start() => StartFunc();

        private void StartFunc()
        {
            rotVec = Random.insideUnitSphere;

            rigid = GetComponent<Rigidbody>();
            rigid.angularVelocity = Random.insideUnitSphere * firstSpeed;
        }

        private void Update() => UpdateFunc();

        private void UpdateFunc()
        {
            this.GetComponentInParent<Transform>().eulerAngles = this.transform.eulerAngles;
        }
    }
}