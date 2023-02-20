using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileGenerator : MonoBehaviour
{
    public GameObject tileObj;

    public int tileNum = 7;
    public float firstX;
    public float firstY;
    public float between = 1;

    public static Tile[,] tileList;                //������ �ִ� Ÿ�� ����
    public static Vector2[,] tileListPosition;     //Ÿ�� ����Ʈ�� ��ġ
    public static ListInfo[,] listInfos;           //���� ����Ʈ�� ����

    Queue<Point> delQueue;        //������ Ÿ�� ����Ʈ
    Queue<Point> curDelQueue;     //������ Ÿ�� �ӽ� �����
    Queue<Point> emptyMoveQueue;    //����ִ°����� �̵��� ����Ʈ �����

    public Tile nowClickTile;   //���� ������ Ŭ���� Ÿ��

    
    bool[,] makeMunchKinPos;    //��ġŲ ���� �� ���ó�� üũ�� ����
    bool[,] useTiled;           //��� ���Ž� ���� ����ó�� üũ�� ����
    bool[,] useMunchKinTile;    //��ġŲ ������ ���ó�� üũ�� ����

    bool moveingTile = false;

    bool autoDeleting = false;  //�ڵ� ��� ���� ������ Ȯ�� ����
    bool tileFalling = false;   //��尡 �������� �ִ��� Ȯ�� ����

    WaitForSeconds emptyCheckSeconds = new WaitForSeconds(.2f);

    //��ġŲ üũ�� ����(�ð���� ����)
    int[] manCheckX = { 1, 1, -1, -1 };
    int[] manCheckY = { -1, 1, 1, -1 };

    public bool isClick = false;

    //�������� �ʿ��� ������
    public int comboCount = 1;

    private void Awake()
    {
        tileList = new Tile[tileNum, tileNum];
        makeMunchKinPos = new bool[tileNum, tileNum];
        useTiled = new bool[tileNum, tileNum];
        useMunchKinTile = new bool[tileNum, tileNum];

        tileListPosition = new Vector2[tileNum, tileNum];
        listInfos = new ListInfo[tileNum, tileNum];
        emptyMoveQueue = new Queue<Point>();
        delQueue = new Queue<Point>();
        curDelQueue = new Queue<Point>();
    }

    private void Start() => StartFunc();

    private void StartFunc()
    {
        firstX = -((int)(7 * 0.5f) * between);
        firstY = ((int)(7 * 0.5f) * between);

        Time.timeScale = 1.0f;
        //Ÿ�� ���� ����
        TileType initType = (TileType)UnityEngine.Random.Range(0, (int)TileType.Special_Munchkin);
        int initNum = UnityEngine.Random.Range(1, 3);
        for (int i = 0; i < tileNum; i++)
        {
            for(int j = 0; j < tileNum; j++)
            {
                //GameObject t = Instantiate(tileObj);
                GameObject t = MemoryPoolManager.instance.GetObject("Tile");
                Vector3 pos = new Vector2(firstX + (between * i), firstY - (between * j));
                pos.y += this.transform.position.y;
                t.transform.position = pos;

                tileListPosition[i, j] = pos;
                t.gameObject.name = "Tile " + i + " : " + j;
                if (t.TryGetComponent(out Tile tile))
                {
                    tile.InitTile(initType);
                    tile.generator = this;
                    tileList[i, j] = tile;
                    tile.listPos = new Point(i, j);
                }
                listInfos[i, j] = ListInfo.Tile;

                initNum--;
                if(initNum <= 0)
                {
                    initNum = UnityEngine.Random.Range(0, 3);
                    initType++;
                    if (initType >= TileType.Special_Munchkin)  //����� ������ �Ѿ�� �ʱ�ȭ
                        initType = TileType.BonBon;
                }
            }
        }

        ResetCheck();
        //�ʱ� �����ų �ڷ�ƾ
        StartCoroutine(TestComplete());
    }

    IEnumerator NewALLEmptyCheckCo()    //��ĭ���� �ƴ� ��ġ ������ ������ �ڷ�ƾ
    {
        tileFalling = true;
        bool noneMoveTile = false;
        emptyMoveQueue.Clear();

        if (nowClickTile)   //���� Ŭ���ϰ� �ִ� Ÿ���� ������ �� Null;
        {
            nowClickTile.isPick(false);
            nowClickTile = null;
        }

        //�� ���� üũ
        for (int i = 0; i < tileNum; i++)
        {
            for (int j = tileNum - 1; j >= 0; j--)
            {
                if (listInfos[i, j] == ListInfo.Empty)      // üũ�ϴ� ����Ʈ�� ���������..
                {
                    emptyMoveQueue.Enqueue(new Point(i, j));
                }
                else    //üũ�ϴ� ����Ʈ�� Ÿ���� ������ ��
                {
                    if (emptyMoveQueue.Count > 0)    //����� �� ������ ������ �̵���Ŵ
                    {
                        //���� ��带 �̵���Ű�⿡ ����� ó�� �� queue�� �־���
                        listInfos[i, j] = ListInfo.Empty;
                        emptyMoveQueue.Enqueue(new Point(i, j));

                        //���� ��� ���� �̵� ó��
                        Point p = emptyMoveQueue.Dequeue();
                        tileList[i, j].SetmoveTile(tileListPosition[p.x, p.y], p);
                        tileList[p.x, p.y] = tileList[i, j];
                        listInfos[p.x, p.y] = ListInfo.Tile;
                    }
                }
            }


            int emptyCount = emptyMoveQueue.Count;
            //���ο� ��� ����
            for (int j = 0; j < emptyCount; j++)
            {
                //GameObject t = Instantiate(tileObj);
                GameObject t = MemoryPoolManager.instance.GetObject("Tile");

                Vector3 pos = new Vector2(firstX + (between * i), firstY + (between * (j + 1)));
                t.transform.position = pos;

                Point p = emptyMoveQueue.Dequeue();

                if(t.TryGetComponent(out Tile tile))
                {
                    tile.generator = this;
                    tileList[p.x, p.y] = tile;
                    tile.InitTile();
                    tile.listPos = new Point(p.x, p.y);
                    tile.SetmoveTile(tileListPosition[p.x, p.y], tile.listPos);
                }
                listInfos[p.x, p.y] = ListInfo.Tile;
            }
        }

        while (!noneMoveTile)
        {
            //���⼭�� ���� �̵����� ��� �˻��� ����
            for(int i = 0; i < tileNum; i++)
            {
                for(int j = 0; j < tileNum; j++)
                {
                    if (tileList[i, j].moveType == moveType.Move)
                    {
                        noneMoveTile = true;
                        break;
                    }
                }
            }

            if (noneMoveTile)
            {
                noneMoveTile = false;
                //yield return emptyCheckSeconds;
                yield return new WaitForEndOfFrame();
                continue;
            }
            else
            {
                tileFalling = false;
                //yield return emptyCheckSeconds;
                yield return new WaitForEndOfFrame();
                break;
            }
        }

        StartCoroutine(TestComplete());
    }

    IEnumerator TestComplete()
    {
        for (int i = 0; i < tileNum; i++)
        {
            for (int j = 0; j < tileNum; j++)
            {
                if (moveingTile)
                    yield break;

                if (listInfos[i, j] == ListInfo.Empty)
                    continue;

                yield return StartCoroutine(CheckCompleteTileCo(tileList[i, j].listPos, tileList[i, j].tileType));
            }
        }

        if (!moveingTile && delQueue.Count >= 3)
        {
            DeleteTile();
        }
        else
        {
            GameManager.instance.WinLoseCheck();
            comboCount = 1;
        }

    }


    public void ClickTile(Tile tile)
    {
        if (GameManager.instance.moveNumber <= 0)   // ���갡 �������� ������..
            return;

        if (moveingTile) //���� ����� �����̰� ������ return;
            return;

        if (autoDeleting || tileFalling)   //��� �ڵ� �������̸� return;
            return;

        if(nowClickTile == null)    //���� ���õǾ��ִ� Ÿ���� �������� ���� �� Ŭ��Ÿ�� ��ġ
        {
            nowClickTile = tile;
            tile.isPick(true);
            return;
        }


        //if(nowClickTile == tile)    //���� Ŭ���� Ÿ���� �Ǵٽ� Ŭ���� ����
        //{
        //    nowClickTile = null;
        //    tile.isPick(false);
        //    return;
        //}

        if(nowClickTile.listPos.y == tile.listPos.y &&
           (nowClickTile.listPos.x - 1 == tile.listPos.x || nowClickTile.listPos.x + 1 == tile.listPos.x))  //���δ� �����ϸ� �� �� ����� ���
        {
            //SwapTile(nowClickTile, tile);
            StartCoroutine(SwapCo(nowClickTile, tile));

            nowClickTile.isPick(false);
            nowClickTile = null;
            tile.isPick(false);
        }
        else if(nowClickTile.listPos.x == tile.listPos.x &&
           (nowClickTile.listPos.y - 1 == tile.listPos.y || nowClickTile.listPos.y + 1 == tile.listPos.y))  //���δ� �����ϸ� �� �Ʒ� ����� ���
        {
            StartCoroutine(SwapCo(nowClickTile, tile));

            nowClickTile.isPick(false);
            nowClickTile = null;
            tile.isPick(false);
        }
        else    //�� ��带 Ŭ����
        {
            nowClickTile.isPick(false);
            nowClickTile = tile;
            tile.isPick(true);
        }
    }

    IEnumerator SwapCo(Tile firstTile, Tile secondTile)
    {
        delQueue.Clear();
        moveingTile = true;

        if(firstTile.tileType == TileType.Special_Munchkin)   //��ġŲ ����
        {
            firstTile.MunchkinEffect(true);
            Point goPoint = secondTile.listPos - firstTile.listPos;
            GameManager.instance.UseMove();         //���� ���
            yield return StartCoroutine(UseMunchkinCo(firstTile.listPos, goPoint));
            moveingTile = false;
            GameManager.instance.MissionCount();
            yield break;
        }

        firstTile.SetmoveTile(tileListPosition[secondTile.listPos.x, secondTile.listPos.y], secondTile.listPos, .25f);
        secondTile.SetmoveTile(tileListPosition[firstTile.listPos.x, firstTile.listPos.y], firstTile.listPos, .25f);

        int res = 0;
        int res2 = 0;
        while(true)
        {
            if (firstTile.moveType == moveType.None)
            {
                res = NewCheckTile(firstTile.listPos, firstTile.tileType);  //ù���� ���� ��ġ���� �ϼ��� ����� �ִ��� Ž��
                if (res < 3)    // �ϼ��� ����� ������ 
                {
                    delQueue.Clear();
                }
                res2 = NewCheckTile(secondTile.listPos, secondTile.tileType);
                break;
            }
            else
                yield return new WaitForSeconds(0.1f);
        }

        if (res >= 3 || res2 >= 3)  //���� ��尡 ���� ��
        {
            GameManager.instance.UseMove();         //���� ���

            if (delQueue.Count > 0)
                DeleteTile();
        }
        else    //���� ��
        {
            firstTile.SetmoveTile(tileListPosition[secondTile.listPos.x, secondTile.listPos.y], secondTile.listPos, .25f);
            secondTile.SetmoveTile(tileListPosition[firstTile.listPos.x, firstTile.listPos.y], firstTile.listPos, .25f);
            delQueue.Clear();
        }

        moveingTile = false;
        nowClickTile = null;
        yield break;
    }

    int result = 0;
    public int CheckTile(Point checkPoint, TileType type)   //����Ǿ��ִ� ������ Ÿ�� ������ ����� üũ �ϴ� �Լ�
    {
        if (checkPoint.x < 0 || checkPoint.x >= tileNum ||
            checkPoint.y < 0 || checkPoint.y >= tileNum)
            return 0;

        if (makeMunchKinPos[checkPoint.x, checkPoint.y])
            return 0;
        else
            makeMunchKinPos[checkPoint.x, checkPoint.y] = true;

        if (tileList[checkPoint.x, checkPoint.y].tileType == type)
        {
            result++;
            delQueue.Enqueue(tileList[checkPoint.x, checkPoint.y].listPos);
        }
        else
            return 0;

        return result;
    }

    IEnumerator CheckCompleteTileCo(Point CheckPoint, TileType type)
    {
        if (type >= TileType.Special_Munchkin)  //����� ���� ���žȵ�
            yield break;

        curDelQueue.Clear();
        int widthNum = 1;
        int checkX = CheckPoint.x;
        int checkY = CheckPoint.y;

        bool checkEnd = false;
        bool checkEnd2 = false;
        bool makePair = false;

        #region ����üũ ����
        for (int i = 1; i < tileNum; i++)
        {
            if (checkEnd && checkEnd2)
                break;
            //������ ���� ����
            if (checkX + i < tileNum && !checkEnd)
            {
                if (tileList[checkX + i, checkY].tileType == type && listInfos[checkX + i, checkY] == ListInfo.Tile && !useTiled[checkX, checkY])    // Ÿ��Ÿ���� �����ϸ� ����� Ÿ���� �ƴ� ��
                {
                    widthNum++;
                    curDelQueue.Enqueue(tileList[checkX + i, checkY].listPos);
                }
                else
                    checkEnd = true;
            }

            //���� ���� ����
            if (checkX - i >= 0 && !checkEnd2)
            {
                if (tileList[checkX - i, checkY].tileType == type && listInfos[checkX - i, checkY] == ListInfo.Tile && !useTiled[checkX, checkY])
                {
                    widthNum++;
                    curDelQueue.Enqueue(tileList[checkX - i, checkY].listPos);
                }
                else
                    checkEnd2 = true;
            }
        }
        #endregion

        if (curDelQueue.Count < 2)
        {
            curDelQueue.Clear();
        }
        else
        {
            makePair = true;
            //�޺� �� ����
            comboCount++;
            Debug.Log(comboCount);

            foreach (var que in curDelQueue)
            {
                delQueue.Enqueue(que);
                useTiled[que.x, que.y] = true;
            }
            curDelQueue.Clear();
        }

        checkEnd = false;
        checkEnd2 = false;

        int heightNum = 1;
        #region ����üũ ����
        for (int i = 1; i < tileNum; i++)
        {
            if (checkEnd && checkEnd2)
                break;

            //�Ʒ� ���� ����
            if (checkY + i < tileNum && !checkEnd)
            {
                if (tileList[checkX, checkY + i].tileType == type && listInfos[checkX, checkY + i] == ListInfo.Tile && !useTiled[checkX, checkY])
                {
                    heightNum++;
                    curDelQueue.Enqueue(tileList[checkX, checkY + i].listPos);
                }
                else
                    checkEnd = true;
            }

            //�� ���� ����
            if (checkY - i >= 0 && !checkEnd2)
            {
                if (tileList[checkX, checkY - i].tileType == type && listInfos[checkX, checkY - i] == ListInfo.Tile && !useTiled[checkX, checkY])
                {
                    heightNum++;
                    curDelQueue.Enqueue(tileList[checkX, checkY - i].listPos);
                }
                else
                    checkEnd2 = true;
            }
        }
        #endregion

        if (curDelQueue.Count < 2)
        {
            curDelQueue.Clear();
        }
        else
        {
            makePair = true;

            //�޺� �� ����
            comboCount++;
            Debug.Log(comboCount);

            foreach (var que in curDelQueue)
            {
                delQueue.Enqueue(que);
                useTiled[que.x, que.y] = true;
            }
            curDelQueue.Clear();
        }

        #region �׸�üũ ����
        bool makeMunchkin = false;
        for (int i = 0; i < 4; i++)
        {
            if ((checkX + manCheckX[i] >= 0 && tileNum > checkX + manCheckX[i]) &&
               (checkY + manCheckY[i] >= 0 && tileNum > checkY + manCheckY[i]))  //���� üũ
            {
                if (useMunchKinTile[checkX + manCheckX[i], checkY] ||
                    useMunchKinTile[checkX, checkY + manCheckY[i]] ||
                    useMunchKinTile[checkX + manCheckX[i], checkY + manCheckY[i]])   // ��ġŲ ������ ���� ����̸� �������� �ʴ´�.
                    continue;

                if (tileList[checkX + manCheckX[i], checkY].tileType == type &&
                   tileList[checkX, checkY + manCheckY[i]].tileType == type &&
                   tileList[checkX + manCheckX[i], checkY + manCheckY[i]].tileType == type) //3���밡 ��� ������ ��ġŲ����
                {
                    //delqueue�� �־���
                        delQueue.Enqueue(tileList[checkX + manCheckX[i], checkY].listPos);
                        delQueue.Enqueue(tileList[checkX, checkY + manCheckY[i]].listPos);
                        delQueue.Enqueue(tileList[checkX + manCheckX[i], checkY + manCheckY[i]].listPos);

                    //���� ��� ó��
                    useMunchKinTile[checkX + manCheckX[i], checkY] = true;
                    useMunchKinTile[checkX, checkY + manCheckY[i]] = true;
                    useMunchKinTile[checkX + manCheckX[i], checkY + manCheckY[i]] = true;
                    makeMunchkin = true;
                }
            }
        }

        if (!makeMunchkin)   //��ġŲ�� ������ �ʾ�����
        {
            if (makePair)    //���� �� ���ο��� 3���̻� ������ ��� �ڽŵ� ������
                delQueue.Enqueue(tileList[checkX, checkY].listPos);
        }
        else    //��ġŲ�� ����� �ڱ� �ڽ��� �������� �ʰ� ��ġŲ ���� ����
        {
            //�޺� �� ����
            comboCount++;
            Debug.Log(comboCount);

            makeMunchKinPos[checkX, checkY] = true;
            if(!useTiled[checkX, checkY])
                delQueue.Enqueue(tileList[checkX, checkY].listPos);
        }
        #endregion

        yield break;
    }

    public int NewCheckTile(Point CheckPoint, TileType type)
    {
        if (type >= TileType.Special_Munchkin)  //����� ���� ���žȵ�
            return 0;

        curDelQueue.Clear();
        int widthNum = 1;
        int checkX = CheckPoint.x;
        int checkY = CheckPoint.y;

        bool checkEnd = false;
        bool checkEnd2 = false;
        bool makePair = false;

        #region ����üũ ����
        for (int i = 1; i < tileNum; i++)
        {
            if (checkEnd && checkEnd2)
                break;
            //������ ���� ����
            if (checkX + i < tileNum && !checkEnd)
            {
                if (tileList[checkX + i, checkY].tileType == type && listInfos[checkX + i, checkY] == ListInfo.Tile)    // Ÿ��Ÿ���� �����ϸ� ����� Ÿ���� �ƴ� ��
                {
                    widthNum++;
                    curDelQueue.Enqueue(tileList[checkX + i, checkY].listPos);
                }
                else
                    checkEnd = true;
            }

            //���� ���� ����
            if (checkX - i >= 0 && !checkEnd2)
            {
                if (tileList[checkX - i, checkY].tileType == type && listInfos[checkX - i, checkY] == ListInfo.Tile)
                {
                    widthNum++;
                    curDelQueue.Enqueue(tileList[checkX - i, checkY].listPos);
                }
                else
                    checkEnd2 = true;
            }
        }
        #endregion

        if (curDelQueue.Count < 2)
        {
            curDelQueue.Clear();
        }
        else
        {
            makePair = true;
            foreach(var que in curDelQueue)
            {
                delQueue.Enqueue(que);
            }
            curDelQueue.Clear();
        }

        checkEnd = false;
        checkEnd2 = false;

        int heightNum = 1;
        #region ����üũ ����
        for (int i = 1; i < tileNum; i++)
        {
            if (checkEnd && checkEnd2)
                break;

            //�Ʒ� ���� ����
            if (checkY + i < tileNum && !checkEnd)
            {
                if (tileList[checkX, checkY + i].tileType == type && listInfos[checkX, checkY + i] == ListInfo.Tile)
                {
                    heightNum++;
                    curDelQueue.Enqueue(tileList[checkX, checkY + i].listPos);
                }
                else
                    checkEnd = true;
            }

            //�� ���� ����
            if (checkY - i >= 0 && !checkEnd2)
            {
                if (tileList[checkX, checkY - i].tileType == type && listInfos[checkX, checkY - i] == ListInfo.Tile)
                {
                    heightNum++;
                    curDelQueue.Enqueue(tileList[checkX, checkY - i].listPos);
                }
                else
                    checkEnd2 = true;
            }
        }
        #endregion

        if (curDelQueue.Count < 2)
        {
            curDelQueue.Clear();
        }
        else
        {
            makePair = true;

            foreach (var que in curDelQueue)
            {
                delQueue.Enqueue(que);
            }
            curDelQueue.Clear();
        }

        #region �׸�üũ ����
        bool makeMunchkin = false;
        for (int i = 0; i < 4; i++)
        {
            if ((checkX + manCheckX[i] >= 0 && tileNum > checkX + manCheckX[i]) &&
               (checkY + manCheckY[i] >= 0 && tileNum > checkY + manCheckY[i]))  //���� üũ
            {
                if (useMunchKinTile[checkX + manCheckX[i], checkY] ||
                    useMunchKinTile[checkX, checkY + manCheckY[i]] ||
                    useMunchKinTile[checkX + manCheckX[i], checkY + manCheckY[i]])   // ��ġŲ ������ ���� ����̸� �������� �ʴ´�.
                    continue;

                if (tileList[checkX + manCheckX[i], checkY].tileType == type &&
                   tileList[checkX, checkY + manCheckY[i]].tileType == type &&
                   tileList[checkX + manCheckX[i], checkY + manCheckY[i]].tileType == type) //3���밡 ��� ������ ��ġŲ����
                {
                    //delqueue�� �־���
                    delQueue.Enqueue(tileList[checkX + manCheckX[i], checkY].listPos);
                    delQueue.Enqueue(tileList[checkX, checkY + manCheckY[i]].listPos);
                    delQueue.Enqueue(tileList[checkX + manCheckX[i], checkY + manCheckY[i]].listPos);

                    //���� ��� ó��
                    useMunchKinTile[checkX + manCheckX[i], checkY] = true;
                    useMunchKinTile[checkX, checkY + manCheckY[i]] = true;
                    useMunchKinTile[checkX + manCheckX[i], checkY + manCheckY[i]] = true;
                    makeMunchkin = true;
                }
            }
        }

        if (!makeMunchkin)   //��ġŲ�� ������ �ʾ�����
        {
            if (makePair)    //���� �� ���ο��� 3���̻� ������ ��� �ڽŵ� ������
                delQueue.Enqueue(tileList[checkX, checkY].listPos);
        }
        else    //��ġŲ�� ����� �ڱ� �ڽ��� �������� �ʰ� ��ġŲ ���� ����
        {
            makeMunchKinPos[checkX, checkY] = true;
            if (!useTiled[checkX, checkY])
                delQueue.Enqueue(tileList[checkX, checkY].listPos);
        }
        #endregion


        return delQueue.Count;
    }   //����Ʈ�������� ����� ����,����,�簢 üũ ó�� �Լ�

    public void DeleteTile()
    {
        while (delQueue.Count > 0)
        {
            Point p = delQueue.Dequeue();
            if (makeMunchKinPos[p.x, p.y])  //����Ų�� ������ ��ǥ�� �� �������� ����
            {
                tileList[p.x, p.y].MakeSpacialTile(TileType.Special_Munchkin);
                continue;
            }

            listInfos[p.x, p.y] = ListInfo.Empty;
            if (tileList[p.x, p.y].TryGetComponent(out MemoryPoolObject obj))
                obj.ObjectReturn();

            GameObject particle = MemoryPoolManager.instance.GetObject("TileBrokeBubble");
            if(particle != null)
            if(particle.TryGetComponent(out ParticleManager pMgr))
                {
                    StartCoroutine(pMgr.PlayParticleCo(tileListPosition[p.x, p.y]));
                }

            GameObject scoreText = MemoryPoolManager.instance.GetObject("ScoreText");
            if(scoreText != null)
                if(scoreText.TryGetComponent(out ScoreText text))
                {
                    StartCoroutine(text.SetScore(10 * comboCount, tileListPosition[p.x, p.y]));
                }
        }



        ResetCheck();   //��ġŲ�� ��ǥ ����
        if(!tileFalling)
            StartCoroutine(NewALLEmptyCheckCo());
    }

    IEnumerator UseMunchkinCo(Point startPoint, Point goPoint)
    {
        //���� �ڱ� �ڽ��� �־���
        Point origin = startPoint;
        delQueue.Enqueue(tileList[startPoint.x, startPoint.y].listPos);
        listInfos[startPoint.x, startPoint.y] = ListInfo.Empty;

        while (true)
        {
            GameObject particle = MemoryPoolManager.instance.GetObject("TileBrokePoof");
            if (particle.TryGetComponent(out ParticleManager pMgr))
                StartCoroutine(pMgr.PlayParticleCo(tileListPosition[startPoint.x, startPoint.y]));

            startPoint += goPoint;
            if (0 <= startPoint.x && startPoint.x < tileNum &&
               0 <= startPoint.y && startPoint.y < tileNum)     //������ �� �ִ� ���� ��
            {
                delQueue.Enqueue(tileList[startPoint.x, startPoint.y].listPos);
                listInfos[startPoint.x, startPoint.y] = ListInfo.Empty;
                tileList[startPoint.x, startPoint.y].gameObject.SetActive(false);
                tileList[origin.x, origin.y].SetmoveTile(tileListPosition[startPoint.x, startPoint.y], startPoint, .18f);

                yield return emptyCheckSeconds;
            }
            else
                break;

        }
        yield return emptyCheckSeconds;

        DeleteTile();

        yield break;
    }

    public void ResetCheck()
    {
        for(int i = 0; i < tileNum; i++)
        {
            for (int j = 0; j < tileNum; j++)
            {
                makeMunchKinPos[i, j] = false;
                useMunchKinTile[i, j] = false;
                useTiled[i, j] = false;
            }
        }
    }   //����Ǿ� �ִ� ��� Ž�� �� ���� �ʱ�ȭ �Լ�

}