using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

namespace Drone
{
    public class DroneChase : MonoBehaviour
    {
        public DroneBase droneBase = null;
        public GameObject player = null;
        public AIPath _aipath = null;

        private void Start() => StartFunc();

        private void StartFunc()
        {
            droneBase = GetComponent<DroneBase>();

            player = GameObject.Find("Player");
            _aipath = droneBase._aipath;
        }

        private void Update() => UpdateFunc();

        private void UpdateFunc()
        {

        }

        public void EnemySetDestination()
        {
            _aipath.destination = player.transform.position;
        }
    }
}
