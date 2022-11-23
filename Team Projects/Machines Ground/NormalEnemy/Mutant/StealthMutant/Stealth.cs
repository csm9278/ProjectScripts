using UnityEngine;

namespace Mutant
{
    public class Stealth : MonoBehaviour
    {
        public enum StealthState
        {
            Idle,
            Change,
            ChangeAfter,
            FindPlayer
        }

        //SpriteRenderer _spRenderer;
        SkinnedMeshRenderer[] _skRenders;
        Material[] _spMat;

        bool increaseAlpha = true;
        public StealthState eStealthState = StealthState.Idle;

        [SerializeField] private float changeTimer = 0.0f;
        private float curChangeTimer = 0.0f;
        [SerializeField] private float minAlpha;
        [SerializeField] private float maxAlpha;
        Color changeColor;
        float changeAlpha;

        private void Start() => StartFunc();

        private void StartFunc()
        {
            _skRenders = GetComponentsInChildren<SkinnedMeshRenderer>();
            _spMat = new Material[_skRenders.Length];
            for(int i = 0; i < _skRenders.Length; i++)
            {
                _spMat[i] = _skRenders[i].material;
                _spMat[i].color = new Color(_spMat[i].color.r, _spMat[i].color.g, _spMat[i].color.b, minAlpha);
                //ADebug.Log(_spMat.color);
                changeColor = _spMat[i].color;
            }



            changeAlpha = minAlpha;

            curChangeTimer = changeTimer;
        }

        private void Update() => UpdateFunc();

        private void UpdateFunc()
        {
            StealthMode();
        }

        public void CheckAlpha()
        {
            if (_spMat[0].color.a <= minAlpha)
            {
                increaseAlpha = true;
                //Debug.Log("true");
                eStealthState = StealthState.ChangeAfter;
            }
            else if (_spMat[0].color.a >= maxAlpha)
            {
                increaseAlpha = false;
                //Debug.Log("false");
                eStealthState = StealthState.ChangeAfter;
            }
        }

        /// <summary>
        /// 스테이트머신에서 Enum형만 변경하여 조절 예정
        /// Idle상태 : 말그대로 Idle 대기타임 Alpha값도 수정안하고 시간 값 계싼
        /// Change상태 : Alpha값을 변경
        /// ChangeAfter 상태 : Idle상태로 변경하고 대기타임 재지정
        /// FindPlayer 상태 : 플레이어를 발견시 작동할 스테이트
        /// </summary>
        public void StealthMode()
        {
            if(eStealthState == StealthState.Idle)
            {
                if (curChangeTimer >= 0.0f)
                {
                    curChangeTimer -= Time.deltaTime;
                    if (curChangeTimer <= 0.0f)
                    {
                        eStealthState = StealthState.Change;
                    }
                }
                else
                    eStealthState = StealthState.Change;
            }
            else if(eStealthState == StealthState.Change)
            {
                if (increaseAlpha)
                {
                    changeAlpha += Time.deltaTime;
                    for(int i = 0; i < _skRenders.Length; i++)
                    {
                        changeColor = new Color(_spMat[i].color.r, _spMat[i].color.g, _spMat[i].color.b, changeAlpha);
                        _spMat[i].color = changeColor;
                    }


                }
                else
                {
                    changeAlpha -= Time.deltaTime;
                    for (int i = 0; i < _skRenders.Length; i++)
                    {
                        changeColor = new Color(_spMat[i].color.r, _spMat[i].color.g, _spMat[i].color.b, changeAlpha);
                        _spMat[i].color = changeColor;
                    }
                }

                CheckAlpha();
            }
            else if(eStealthState == StealthState.ChangeAfter)
            {
                if(increaseAlpha)
                    curChangeTimer = changeTimer;
                else
                    curChangeTimer = changeTimer * 0.2f;

                eStealthState = StealthState.Idle;
            }
            else if(eStealthState == StealthState.FindPlayer)
            {
                AttackStealthMode();
            }

        }

        public void AttackStealthMode()
        {
            if (_spMat[0].color.a <= minAlpha)
            {
                increaseAlpha = true;
            }
            else if (_spMat[0].color.a >= maxAlpha)
            {
                increaseAlpha = false;
            }
            if (increaseAlpha)
            {
                changeAlpha += Time.deltaTime;
                for (int i = 0; i < _skRenders.Length; i++)
                {
                    changeColor = new Color(_spMat[i].color.r, _spMat[i].color.g, _spMat[i].color.b, changeAlpha);
                    _spMat[i].color = changeColor;
                }
            }
            else
            {
                changeAlpha -= Time.deltaTime;
                for (int i = 0; i < _skRenders.Length; i++)
                {
                    changeColor = new Color(_spMat[i].color.r, _spMat[i].color.g, _spMat[i].color.b, changeAlpha);
                    _spMat[i].color = changeColor;
                }
            }
        }
    }
}