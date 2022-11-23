using UnityEngine;

namespace Mutant
{
    public class CheckMother : MonoBehaviour
    {
        public MotherMutantBase motherBase;
        public int spawnIdx = 0;

        //private void Start() => StartFunc();

        //private void StartFunc()
        //{
         
        //}

        //private void Update() => UpdateFunc();

        //private void UpdateFunc()
        //{
        
        //}

        public void SignalToMother()
        {
            if(motherBase != null)
            {
                motherBase.DieSignal(spawnIdx);
            }
        }

        public void FindSignalToMother()
        {
            if ((motherBase.transform.position - this.transform.position).magnitude >= 10.0f)
            {
                motherBase.AiDestSetter(true);
                return;
            }

            if(motherBase != null)
            {
                motherBase.FindSignal();
            }
        }
    }
}