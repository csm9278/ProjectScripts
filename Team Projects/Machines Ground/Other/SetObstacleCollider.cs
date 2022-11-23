using UnityEngine;

namespace RootMain
{
    public class SetObstacleCollider : MonoBehaviour
    {
        private void Start() => StartFunc();

        private void StartFunc()
        {
            this.gameObject.SetActive(false);    
        }
    }
}