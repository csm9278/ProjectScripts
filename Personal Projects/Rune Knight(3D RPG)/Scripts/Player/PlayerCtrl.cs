using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCtrl : MonoBehaviour
{
    enum AttackCombo
    {
        None,
        Attack1,
        Attack2,
        Attack3
    }

    //이동관련
    float h, v;
    public float moveSpeed = 0;
    public Vector3 moveVec;

    Rigidbody _rigidbody;
    Animator _animator;
    int isMoveHash = Animator.StringToHash("isMove");
    int dotgeHash = Animator.StringToHash("Dotge");

    //콤보공격
    int attack1Hash = Animator.StringToHash("Attack1");
    int attack2Hash = Animator.StringToHash("Attack2");
    int attack3Hash = Animator.StringToHash("Attack3");
    float comboTimer = 0.5f;
    AttackCombo attackCombo = AttackCombo.None;

    //방어관련
    int guardHash = Animator.StringToHash("Guard");
    float parringTime = -0.1f;
    float guardDelay = -0.1f;

    // Start is called before the first frame update
    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        //이동부분 업데이트
        if(attackCombo == AttackCombo.None && _animator.GetBool(guardHash) == false)
        {
            MoveUpdate();
        }

        //공격부분 업데이트
        AttackUpdate();

        //방어부분 업데이트
        GuardUpdate();
        if(parringTime >= 0.0f)
            parringTime -= Time.deltaTime;
        if (guardDelay >= 0.0f)
            guardDelay -= Time.deltaTime;
    }

    // 이동부분 업데이트
    void MoveUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            _animator.SetTrigger(dotgeHash);

        h = Input.GetAxis("Horizontal");
        v = Input.GetAxis("Vertical");

        if (h != 0 || v != 0)
            _animator.SetBool(isMoveHash, true);
        else
            _animator.SetBool(isMoveHash, false);


        moveVec = new Vector3(h, 0, v);

        moveVec.Normalize();

        if(moveVec.magnitude > 0.01f)
        {
            Quaternion a_TargetRot = Quaternion.LookRotation(moveVec);
            transform.rotation = Quaternion.Slerp(transform.rotation,
                        a_TargetRot, Time.deltaTime * 150.0f);
        }



        _rigidbody.transform.position += moveVec * moveSpeed * Time.deltaTime;
    }

    //공격부분 업데이트
    void AttackUpdate()
    {
        //Debug.Log(_animator.GetCurrentAnimatorStateInfo(0).length);

        if(Input.GetMouseButtonDown(0))
        {
            switch(attackCombo)
            {
                case AttackCombo.None:
                    _animator.SetBool(attack1Hash, true);
                    attackCombo = AttackCombo.Attack1;
                    comboTimer = 1.0f;
                    break;

                case AttackCombo.Attack1:
                    _animator.SetBool(attack2Hash, true);
                    attackCombo = AttackCombo.Attack2;
                    comboTimer = 1.0f;
                    break;


                case AttackCombo.Attack2:
                    _animator.SetBool(attack3Hash, true);
                    attackCombo = AttackCombo.Attack3;
                    comboTimer = 1.0f;
                    break;

            }

        }


        if(comboTimer >= 0.0f)
        {
            comboTimer -= Time.deltaTime;

            if(comboTimer <= 0.0f)
            {
                attackCombo = AttackCombo.None;
                _animator.SetBool(attack1Hash, false);
                _animator.SetBool(attack2Hash, false);
                _animator.SetBool(attack3Hash, false);

            }
        }
    }

    void GuardUpdate()
    {
        if (Input.GetMouseButtonDown(1) && guardDelay <= 0.0f)
        {
            _animator.SetBool(guardHash, true);
            parringTime = 0.1f;
        }
        
        if(Input.GetMouseButtonUp(1))
        {
            _animator.SetBool(guardHash, false);
            guardDelay = 0.2f;
        }
    }

}
