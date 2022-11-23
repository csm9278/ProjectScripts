using UnityEngine;

public class TutorialFireBallTrigger : MonoBehaviour
{
    public enum LearnSKill
    {
        Rotate,
        fireBall,
        Water,
        Gain
    }

    public LearnSKill learnSkill = LearnSKill.fireBall;
    public TutorialManager tutorialMgr;

    private void Start()
    {
        if (tutorialMgr == null)
            tutorialMgr = FindObjectOfType<TutorialManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            GlobalData.gameStop = true;
            if(learnSkill == LearnSKill.fireBall)
            {
                tutorialMgr.tutorialState = TutorialManager.TutorialState.LearnFireBall;
                tutorialMgr.playerSkill[1].SetActive(true);
                tutorialMgr.infoText.text = "W키를 눌러 파이어볼을 사용해보세요";
                tutorialMgr.Arrow.transform.localPosition = new Vector3(334, -172, 0);
                Time.timeScale = 0;
            }
            else if(learnSkill == LearnSKill.Water)
            {
                tutorialMgr.tutorialState = TutorialManager.TutorialState.LearnWaterSkill;
                tutorialMgr.playerSkill[2].SetActive(true);
                tutorialMgr.infoText.text = "E키를 눌러 폭포스킬을 사용해보세요";
                tutorialMgr.Arrow.transform.position = new Vector3(443, -172, 0);

                Time.timeScale = 0;
            }
            else if(learnSkill == LearnSKill.Gain)
            {
                tutorialMgr.tutorialState = TutorialManager.TutorialState.LearnGainSkill;
                tutorialMgr.playerSkill[3].SetActive(true);
                tutorialMgr.infoText.text = "R키를 눌러 폭발스킬을 사용해보세요";
                tutorialMgr.Arrow.transform.position = new Vector3(551, -172, 0);
                Time.timeScale = 0;
            }
            else if(learnSkill == LearnSKill.Rotate)
            {
                tutorialMgr.infoText.text = "마우스 오른쪽 쿨릭을 누른 상태로 카메라 회전이 가능합니다.";
                GlobalData.gameStop = false;
            }

            this.gameObject.SetActive(false);
        }
    }
}