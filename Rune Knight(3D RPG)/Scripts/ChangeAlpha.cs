using UnityEngine;
using UnityEngine.UI;

public class ChangeAlpha : MonoBehaviour
{
    Image[] Images;
    Text[] Texts;

    float Speed = 0.4f;

    float[] imagesAlpha;
    float[] textsAlpha;

    private void Start() => StartFunc();

    private void StartFunc()
    {
        Images = GetComponentsInChildren<Image>();
        Texts = GetComponentsInChildren<Text>();

        imagesAlpha = new float[Images.Length];
        textsAlpha = new float[Texts.Length];

        for(int i = 0; i < Images.Length; i++)
        {
            imagesAlpha[i] = Images[i].color.a;
        }

        for(int i = 0; i < textsAlpha.Length; i++)
        {
            textsAlpha[i] = Texts[i].color.a;
        }
    }

    private void Update() => UpdateFunc();

    private void UpdateFunc()
    {

        if(Images.Length > 0)
        {
            if (Images[0].color.a <= 0)
            {
                GlobalData.gameStop = false;
                this.gameObject.SetActive(false);
            }

            for (int i = 0; i < Images.Length;i++)
            {
                Color changeColor = Images[i].color;
                changeColor.a -= Time.deltaTime * imagesAlpha[i] * Speed;

                Images[i].color = changeColor;
            }
        }

        if(Texts.Length > 0)
        {
            for(int i = 0; i < Texts.Length; i++)
            {
                Color changeColor = Texts[i].color;
                changeColor.a -= Time.deltaTime * textsAlpha[i] * Speed;

                Texts[i].color = changeColor;
            }
        }
    }
}