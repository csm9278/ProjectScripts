using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mutant
{
    public class MotherMutantIdle : MonoBehaviour
    {
        float idleDelyTime = 0.0f;
        [SerializeField] float minIdleTime;
        [SerializeField] float maxIdleTime;
        Animator _animator;

        private void OnEnable()
        {
            idleDelyTime = Random.Range(minIdleTime, maxIdleTime);
        }

        // Start is called before the first frame update
        void Start()
        {
            _animator = GetComponent<Animator>();
        }

        // Update is called once per frame
        void Update()
        {
            if (idleDelyTime >= 0.0f)
            {
                idleDelyTime -= Time.deltaTime;
                _animator.SetFloat("DelayTime", idleDelyTime);
            }
        }


    }
}
