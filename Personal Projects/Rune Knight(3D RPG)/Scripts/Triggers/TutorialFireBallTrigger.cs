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
                tutorialMgr.infoText.text = "WŰ�� ���� ���̾�� ����غ�����";
                tutorialMgr.Arrow.transform.localPosition = new Vector3(334, -172, 0);
                Time.timeScale = 0;
            }
            else if(learnSkill == LearnSKill.Water)
            {
                tutorialMgr.tutorialState = TutorialManager.TutorialState.LearnWaterSkill;
                tutorialMgr.playerSkill[2].SetActive(true);
                tutorialMgr.infoText.text = "EŰ�� ���� ������ų�� ����غ�����";
                tutorialMgr.Arrow.transform.position = new Vector3(443, -172, 0);

                Time.timeScale = 0;
            }
            else if(learnSkill == LearnSKill.Gain)
            {
                tutorialMgr.tutorialState = TutorialManager.TutorialState.LearnGainSkill;
                tutorialMgr.playerSkill[3].SetActive(true);
                tutorialMgr.infoText.text = "RŰ�� ���� ���߽�ų�� ����غ�����";
                tutorialMgr.Arrow.transform.position = new Vector3(551, -172, 0);
                Time.timeScale = 0;
            }
            else if(learnSkill == LearnSKill.Rotate)
            {
                tutorialMgr.infoText.text = "���콺 ������ ���� ���� ���·� ī�޶� ȸ���� �����մϴ�.";
                GlobalData.gameStop = false;
            }

            this.gameObject.SetActive(false);
        }
    }
}