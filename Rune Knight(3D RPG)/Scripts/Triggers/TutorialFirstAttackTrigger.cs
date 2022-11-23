using UnityEngine;

public class TutorialFirstAttackTrigger : MonoBehaviour
{
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
            tutorialMgr.tutorialState = TutorialManager.TutorialState.FirstAttack;
            this.gameObject.SetActive(false);
        }
    }
}