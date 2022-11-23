using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneChangeManager : MonoBehaviour
{
    public enum changeType
    {
        FadeIn,
        FadeOut
    }

    public changeType type = changeType.FadeIn;

    Image fadeImg;
    Color fadeColor;
    float fadeSpeed = 2;
    float fadeAlpha;

    string changeSceneName;
    bool changeStart = false;


    // Start is called before the first frame update
    void Start()
    {
        fadeImg = GetComponent<Image>();

        fadeAlpha = fadeImg.color.a;
        if (type == changeType.FadeOut)
            changeStart = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (!changeStart)
            return;

        switch (type)
        {
            case changeType.FadeIn:
                FadeInFunc();
                break;

            case changeType.FadeOut:
                FadeOutFunc();
                break;
        }
    }

    void FadeInFunc()
    {
        fadeAlpha += Time.deltaTime;
        fadeColor = new Color(255, 255, 255, fadeAlpha);

        fadeImg.color = fadeColor;

        if (fadeAlpha >= 1)
        {
            SceneManager.LoadScene(changeSceneName);
        }
    }

    void FadeOutFunc()
    {
        fadeAlpha -= Time.deltaTime;
        fadeColor = new Color(255, 255, 255, fadeAlpha);

        fadeImg.color = fadeColor;

        if (fadeAlpha <= 0)
        {
            this.gameObject.SetActive(false);
        }
    }

    public void ChangeScene(string sceneName)
    {
        changeSceneName = sceneName;
        changeStart = true;
    }
}
