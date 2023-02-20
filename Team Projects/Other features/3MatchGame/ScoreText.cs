using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreText : MemoryPoolObject
{
    public Text scoreText;

    public override void InitObject()
    {
        if (scoreText == null)
            scoreText = GetComponentInChildren<Text>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public IEnumerator SetScore(int Score, Vector3 Pos)
    {
        scoreText.text = Score.ToString();
        this.transform.position = Pos;

        GameManager.instance.AddScore(Score);

        yield return new WaitForSeconds(0.2f);

        ObjectReturn();
        yield break;
    }
}
