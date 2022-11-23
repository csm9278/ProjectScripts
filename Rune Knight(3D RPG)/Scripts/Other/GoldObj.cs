using UnityEngine;

public class GoldObj : MonoBehaviour
{
    public GameObject coinObj;
    public ParticleSystem getParticle;
    bool isGet = false;

    private void Start() => StartFunc();

    private void StartFunc()
    {
         
    }

    private void Update() => UpdateFunc();

    private void UpdateFunc()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {

        if(other.CompareTag("Player") && !isGet)
        {
            isGet = true;
            coinObj.SetActive(false);
            getParticle.gameObject.SetActive(true);
            if (other.TryGetComponent(out Player player))
                player.GetGold();

            Destroy(this.gameObject, 3.0f);
        }
    }
}