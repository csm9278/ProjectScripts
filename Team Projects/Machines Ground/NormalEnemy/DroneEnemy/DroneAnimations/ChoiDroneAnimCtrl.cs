using UnityEngine;

namespace Drone
{
    public class ChoiDroneAnimCtrl : MonoBehaviour
    {
        public enum DroneAnimType
        {
            Idle,
            Turn_L,
            Turn_R,
            Walk,
            Death
        }


        public Animator _animator;
        DroneAnimType droneAnimType = DroneAnimType.Idle;

        //ÇØ½¬ Ä³½Ì
        int IdleHash = Animator.StringToHash("Idle");
        int TurnLHash = Animator.StringToHash("TurnL");
        int TurnRHash = Animator.StringToHash("TurnR");
        int WalkHash = Animator.StringToHash("Walk");
        int deathHash = Animator.StringToHash("Death");

        private void Start() => StartFunc();

        private void StartFunc()
        {
            _animator = this.GetComponent<Animator>();
        }

        private void Update() => UpdateFunc();

        private void UpdateFunc()
        {
        
        }

        public void ChangeAnim(DroneAnimType animType)
        {


            switch(animType)
            {
                case DroneAnimType.Idle:
                    _animator.SetTrigger(IdleHash);
                    break;

                case DroneAnimType.Walk:
                    _animator.SetTrigger(WalkHash);
                    break;

                case DroneAnimType.Turn_L:
                    _animator.SetTrigger(TurnLHash);
                    break;

                case DroneAnimType.Turn_R:
                    _animator.SetTrigger(TurnRHash);
                    break;

                case DroneAnimType.Death:
                    _animator.SetTrigger(deathHash);
                    break;
            }
        }
    }
}