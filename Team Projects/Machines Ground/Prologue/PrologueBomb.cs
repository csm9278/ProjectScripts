using UnityEngine;

namespace RootMain
{
    public class PrologueBomb : MonoBehaviour
    {
        public GameObject attackTrigger;

        private void OnCollisionEnter2D(Collision2D collision)
        {
            attackTrigger.gameObject.SetActive(true);
        }
    }
}