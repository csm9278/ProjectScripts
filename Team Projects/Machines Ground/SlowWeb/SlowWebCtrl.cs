using UnityEngine;
using Altair_Memory_Pool_Pro;

namespace RootMain
{
    public class SlowWebCtrl : MemoryPoolingFlag
    {
        

        SpriteRenderer slowWebSR = null;
        public GameObject slowWebObj = null;
        public GameObject player;

        public bool isSlowObj = true;
        public Sprite[] slowWebImgs;

        public float webMaxSize;
        public float changespeed;

        public float maxStayTime = -0.1f;
        float curStayTime = -0.1f;
        float curWebScale;
        Vector3 curWebScaleVec;

        //백업용 변수 (오브젝트 리턴시 초기화용)
        bool isFirst = false;
        Vector2 originScale;

        //슬로우 효과
        Vector3 playerVec;

        private void OnEnable()
        {
            if (!isFirst)
                return;

            int rand = Random.Range(0, slowWebImgs.Length);
            slowWebSR.sprite = slowWebImgs[rand];

            rand = Random.Range(0, 360);

            slowWebObj.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, rand));

            slowWebObj.transform.localScale = originScale;
            curWebScale = slowWebObj.transform.localScale.x;

        }

        private void Start() => StartFunc();

        private void StartFunc()
        {
            player = GameObject.Find("Player");

            if (slowWebSR == null)
                slowWebSR = GetComponentInChildren<SpriteRenderer>();

            int rand = Random.Range(0, slowWebImgs.Length);
            slowWebSR.sprite = slowWebImgs[rand];

            rand = Random.Range(0, 360);

            slowWebObj.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, rand));
            
            curWebScale = slowWebObj.transform.localScale.x;

            originScale = new Vector2(0.1f, 0.1f);

            isFirst = true;

        }

        private void Update() => UpdateFunc();

        private void UpdateFunc()
        {
            slowWebSizeCtrl();
            
            if(isSlowObj)
            SlowEff();
        }

        //슬로우 효과주는 함수
        void SlowEff()
        {
            playerVec = player.transform.position - this.transform.position;
            playerVec.z = 0;

            if(playerVec.magnitude <= curWebScale * 6)
            {
                if(player.TryGetComponent(out Player.PlayerMovement move))
                {
                    move.SetSlow(0.2f);
                }
            }
        }

        //사이즈 및 제거 컨트롤 
        void slowWebSizeCtrl()
        {
            if(curWebScale <= webMaxSize)
            {
                curWebScale += Time.deltaTime * changespeed;

                curWebScaleVec = new Vector3(curWebScale, curWebScale, 1);
                slowWebObj.transform.localScale = curWebScaleVec;
                if (curWebScale >= webMaxSize)
                {
                    curStayTime = maxStayTime;
                }
            }
            else
            {
                if(curStayTime >= 0.0f)
                {
                    curStayTime -= Time.deltaTime;

                    if (curStayTime <= 0.0f)
                    {
                        ObjectReturn();
                    }
                }
            }
        }
    }
}