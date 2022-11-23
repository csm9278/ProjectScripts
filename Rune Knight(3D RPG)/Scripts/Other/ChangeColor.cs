using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeColor : MonoBehaviour
{
    SkinnedMeshRenderer[] _skinMr;
    MeshRenderer[] _meshR;
    Material[] _skinMat;
    Material[] _meshMat;

    public float ChangeDelay = -0.1f;
    bool isSKillIn = false;
    public bool isDie = false;
    float disappearTime = 2.0f;
    float alpha;
    Color changeColor;

    // Start is called before the first frame update
    void Start()
    {
        _skinMr = GetComponentsInChildren<SkinnedMeshRenderer>();
        _meshR = GetComponentsInChildren<MeshRenderer>();

        if(_skinMr.Length > 0)
        {
            _skinMat = new Material[_skinMr.Length];

            for(int i = 0; i < _skinMr.Length; i++)
            {
                _skinMat[i] = _skinMr[i].material;
            }
        }

        if (_meshR.Length > 0)
        {
            _meshMat = new Material[_meshR.Length];

            for (int i = 0; i < _meshR.Length; i++)
            {
                _meshMat[i] = _meshR[i].material;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(isDie)
        {
            if (_skinMr.Length <= 0)
                return;

            alpha = _skinMat[0].color.a;
            alpha -= Time.deltaTime / disappearTime;

            changeColor = new Color(1, 1, 1, alpha);

            for (int i = 0; i < _skinMr.Length; i++)
            {
                _skinMat[i].color = changeColor;
            }

            for(int i = 0; i < _meshR.Length; i++)
            {
                _meshMat[i].color = changeColor;
            }

            if (alpha <= 0)
                this.gameObject.SetActive(false);
        }

        if(ChangeDelay >= 0.0f)
        {
            ChangeDelay -= Time.deltaTime;
            if(ChangeDelay <= 0.0f)
            {
                isSKillIn = false;
                SetColor(Color.white);
            }
        }

    }

    public void SetColor(Color32 Scolor)
    {
        for(int i = 0; i < _skinMr.Length; i++)
        {
            _skinMat[i].color = Scolor;
        }
    }

    public void SetDie()
    {
        isDie = true;
    }
}
