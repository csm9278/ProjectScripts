using UnityEngine;

public class WarningIcon : MemoryPoolObject
{
    Color _color;
    SpriteRenderer _spRenderer;

    float changeAlpha = 0.0f;

    public float maxAlpha = 0.0f;
    public int changeSpeed;
    bool isDecrease = false;

    public float spawnTimer = 3.0f;

    private void Start() => StartFunc();

    private void StartFunc()
    {
        if(_spRenderer == null)
            _spRenderer = GetComponent<SpriteRenderer>();

        if(_spRenderer != null)
        {
            _color = _spRenderer.color;
            changeAlpha = _color.a;
        }
    }

    private void Update() => UpdateFunc();

    private void UpdateFunc()
    {
        if (EnemyGenerator.roundEnd)
            ObjectReturn();

        changeAlphaFunc();

        if(spawnTimer >= 0.0f)
        {
            spawnTimer -= Time.deltaTime;
            if(spawnTimer <= 0.0f)
            {
                spawnTimer = 3.0f;
                GameObject enemy = MemoryPoolManager.instance.GetObject("NormalEnemy");
                enemy.transform.position = this.transform.position;

                ObjectReturn();
            }    
        }
    }

    void changeAlphaFunc()
    {
        if(isDecrease)
        {
            changeAlpha -= Time.deltaTime * changeSpeed;
            _color.a = changeAlpha;
            _spRenderer.color = _color;
            if(changeAlpha <= 0.0f)
            {
                isDecrease = false;
            }
        }
        else
        {
            changeAlpha += Time.deltaTime * changeSpeed;
            _color.a = changeAlpha;
            _spRenderer.color = _color;
            if (changeAlpha >= maxAlpha)
            {
                isDecrease = true;
            }
        }
    }
}