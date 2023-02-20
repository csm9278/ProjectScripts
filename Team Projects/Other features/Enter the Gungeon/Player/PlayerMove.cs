using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    Rigidbody2D rigid2D;
    public int moveSpeed = 1;
    Animator _animator;

    [Header("--- MoveLock ---")]
    public float minX;
    public float maxX;
    public float minY;
    public float maxY;

    SpriteRenderer _spRenderer;
    [HideInInspector] public float mouseX;

    private void Start() => StartFunc();

    private void StartFunc()
    {
        rigid2D = GetComponent<Rigidbody2D>();
        _animator = GetComponentInChildren<Animator>();
        _spRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    private void Update() => UpdateFunc();

    private void UpdateFunc()
    {
        if (GlobalData.playerMoveLock)
        {
            rigid2D.velocity = Vector2.zero;
            return;
        }

        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        Vector3 moveVec = new Vector3(h, v, 0);

        ImageRotate();

        if (moveVec.magnitude > 1.0f)
            moveVec.Normalize();

        if(Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D))
            _animator.SetBool("Move", true);
        else
            _animator.SetBool("Move", false);

        //MoveLock
        if (this.transform.position.x >= maxX && moveVec.x > 0)
            moveVec.x = 0;
        else if (this.transform.position.x <= minX && moveVec.x < 0)
            moveVec.x = 0;
        if (this.transform.position.y >= maxY && moveVec.y > 0)
            moveVec.y = 0;
        else if (this.transform.position.y <= minY && moveVec.y < 0)
            moveVec.y = 0;

        rigid2D.velocity = moveVec * moveSpeed;
    }


    public void ImageRotate()
    {
        if(this.transform.position.x > mouseX)
            _spRenderer.gameObject.transform.rotation = Quaternion.Euler(0, -180, 0);
        else
            _spRenderer.gameObject.transform.rotation = Quaternion.Euler(0, 0, 0);
    }
}