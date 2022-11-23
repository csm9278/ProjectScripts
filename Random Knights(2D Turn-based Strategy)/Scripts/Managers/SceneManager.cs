using UnityEngine;

public class SceneManager : MonoBehaviour
{
    public GameObject SceneChangeImg;

    private void Start() => StartFunc();

    private void StartFunc()
    {
         
    }

    private void Update() => UpdateFunc();

    private void UpdateFunc()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            if(SceneChangeImg != null)
            {
                SceneChanger changer = SceneChangeImg.GetComponent<SceneChanger>();
                if(changer != null)
                {
                    changer.ChangeStart();
                }
            }
        }
    }


}