using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


namespace RootMain
{
    public class FadeIn : MonoBehaviour
    {
        public string sceneName;
        Image _image;

        Color changeColor;

        private void Start() => StartFunc();

        private void StartFunc()
        {
            _image = this.GetComponent<Image>();
            changeColor = _image.color;
        }

        private void Update() => UpdateFunc();

        private void UpdateFunc()
        {
            if(_image.color.a <= 1.0f)
            {
                changeColor.a += Time.deltaTime;

                _image.color = changeColor;
            }
            else
            {
                if (sceneName != null)
                    SceneManager.LoadScene(sceneName);
            }
        }
    }
}