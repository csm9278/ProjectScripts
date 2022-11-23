using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TitleManager : MonoBehaviour
{
    public GameObject player;

    public Button gameStartBtn;
    public Button optionsBtn;
    public Button exitBtn;

    bool isStart = false;

    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 1;

        if (player == null)
            player = GameObject.Find("Player");

        if (gameStartBtn != null)
            gameStartBtn.onClick.AddListener(() =>
            {
                player.GetComponent<Player>()._animator.SetTrigger("Walk");
                isStart = true;

            });

        if (exitBtn != null)
            exitBtn.onClick.AddListener(() =>
            {
                Application.Quit();
            });
    }

    // Update is called once per frame
    void Update()
    {
       if(isStart)
        {
            player.transform.Translate(Vector3.forward * Time.deltaTime * 2);
        }
    }
}
