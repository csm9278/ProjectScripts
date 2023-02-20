using System.Collections;
using UnityEngine;

public class Tile : MemoryPoolObject
{
    public TileType tileType = TileType.BonBon;
    public moveType moveType = moveType.None;
    SpriteRenderer _spRenderer;
    public GameObject pickImage;

    public Point listPos; //��ϸ���Ʈ ��ǥ��
    public Point curPoint;  //���� �� ����� ����Ʈ ����
    public TileType curType = TileType.Count;
    public Vector2 target;
    public Vector2 firstMovePos;

    //Ÿ�� �̵�
    float curMoveTime = 10;
    float curTime = 0;

    bool isDown = false;

    Vector3 slideVec;

    [HideInInspector] public TileGenerator generator;

    //��ġŲ ���������
    bool munchkin = false;

    private void Start() => StartFunc();

    private void StartFunc()
    {
    }

    private void Update() => UpdateFunc();

    private void UpdateFunc()
    {
        MovedTile();

        if(munchkin)
        {
            this.gameObject.transform.Rotate(Vector3.forward * 10);
        }
    }

    public void InitTile(TileType initType = TileType.Count)
    {
        if(initType == TileType.Count)  // ������ ���� ������ ������ ����
        {
            TileType randType = (TileType)Random.Range(0, (int)TileType.Special_Munchkin);
            this.tileType = randType;
            if (_spRenderer != null)
                _spRenderer.sprite = GlobalData.blockSprites[(int)randType];
            else
            {
                _spRenderer = GetComponent<SpriteRenderer>();
                _spRenderer.sprite = GlobalData.blockSprites[(int)randType];
            }
        }
        else    //�̸� �������� Ÿ������ ����
        {
            this.tileType = initType;
            if (_spRenderer != null)
                _spRenderer.sprite = GlobalData.blockSprites[(int)initType];
            else
            {
                _spRenderer = GetComponent<SpriteRenderer>();
                _spRenderer.sprite = GlobalData.blockSprites[(int)initType];
            }
        }

        munchkin = false;
        this.transform.eulerAngles = Vector3.zero;
        firstMovePos = this.transform.position;
        curType = TileType.Count;
    }

    private void OnMouseDown()
    {
        if (GameManager.instance.isEnd)
            return;

        generator.isClick = true;
        generator.ClickTile(this);
    }

    private void OnMouseDrag()
    {
        if (GameManager.instance.isEnd)
            return;

        Vector2 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(worldPoint, Vector2.zero);

        if (hit.collider != null && generator.nowClickTile != null)
        {
            if(hit.collider.gameObject.TryGetComponent(out Tile tile))
            {
                generator.ClickTile(tile);
            }
        }
    }

    private void OnMouseUp()
    {
        generator.isClick = false;
    }



    public void ChangeTile(TileType type)
    {
        tileType = type;
    }

    public void MunchkinEffect(bool active)
    {
        munchkin = active;
    }

    public void isPick(bool isOn)
    {
        if(isOn)
        {
            pickImage.gameObject.SetActive(true);
        }
        else
        {
            pickImage.gameObject.SetActive(false);
        }
    }

    public void SetmoveTile(Vector2 targetPos, Point point, float moveSpeed = .5f)
    {
        target = targetPos;
        moveType = moveType.Move;
        curPoint = point;
        TileGenerator.tileList[point.x, point.y] = this;
        curMoveTime = moveSpeed;
        //curType = type;
    }

    public void MakeSpacialTile(TileType type)
    {
        tileType = type;
        if(_spRenderer != null)
            _spRenderer.sprite = GlobalData.blockSprites[(int)type];
        else
        {
            _spRenderer = GetComponent<SpriteRenderer>();
            _spRenderer.sprite = GlobalData.blockSprites[(int)type];
        }
    }

    void MovedTile()
    {
        if (moveType == moveType.None)
            return;

        Vector2 targetVec = target - (Vector2)this.transform.position;
        
        if(targetVec.magnitude > 0.01f)
        {
            curTime += Time.deltaTime / curMoveTime;
            this.transform.position = Vector2.Lerp(firstMovePos, target, curTime);
        }
        else    //�̵� ��� ��������
        {
            moveType = moveType.None;
            target = Vector2.zero;
            //TileGenerator.tileList[curPoint.x, curPoint.y] = this;
            listPos = curPoint;
            if (curType != TileType.Count)
            {
                tileType = curType;
                curType = TileType.Count;
            }

            firstMovePos = this.transform.position;
            curTime = 0;
        }
    }
}