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

    public static Tile[,] tileList;                //가지고 있는 타일 정보
    public static Vector2[,] tileListPosition;     //타일 리스트의 위치
    public static ListInfo[,] listInfos;           //현재 리스트의 상태

    Queue<Point> delQueue;        //제거할 타일 리스트
    Queue<Point> curDelQueue;     //제거할 타일 임시 저장소
    Queue<Point> emptyMoveQueue;    //비어있는곳으로 이동할 포인트 저장소

    public Tile nowClickTile;   //현재 유저가 클릭한 타일

    
    bool[,] makeMunchKinPos;    //먼치킨 생성 시 사용처리 체크용 변수
    bool[,] useTiled;           //블록 제거시 삭제 예외처리 체크용 변수
    bool[,] useMunchKinTile;    //먼치킨 생성시 사용처리 체크용 변수

    bool moveingTile = false;

    bool autoDeleting = false;  //자동 노드 제거 진행중 확인 변수
    bool tileFalling = false;   //노드가 떨어지고 있는지 확인 변수

    WaitForSeconds emptyCheckSeconds = new WaitForSeconds(.2f);

    //먼치킨 체크용 변수(시계방향 변수)
    int[] manCheckX = { 1, 1, -1, -1 };
    int[] manCheckY = { -1, 1, 1, -1 };

    public bool isClick = false;

    //점수계산시 필요한 변수들
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
        //타일 생성 로직
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
                    if (initType >= TileType.Special_Munchkin)  //스페셜 노드까지 넘어가면 초기화
                        initType = TileType.BonBon;
                }
            }
        }

        ResetCheck();
        //초기 실행시킬 코루틴
        StartCoroutine(TestComplete());
    }

    IEnumerator NewALLEmptyCheckCo()    //한칸씩이 아닌 위치 저장후 내리는 코루틴
    {
        tileFalling = true;
        bool noneMoveTile = false;
        emptyMoveQueue.Clear();

        if (nowClickTile)   //현재 클릭하고 있는 타일이 존재할 시 Null;
        {
            nowClickTile.isPick(false);
            nowClickTile = null;
        }

        //빈 공간 체크
        for (int i = 0; i < tileNum; i++)
        {
            for (int j = tileNum - 1; j >= 0; j--)
            {
                if (listInfos[i, j] == ListInfo.Empty)      // 체크하는 포인트가 비어있으면..
                {
                    emptyMoveQueue.Enqueue(new Point(i, j));
                }
                else    //체크하는 포인트가 타일이 존재할 시
                {
                    if (emptyMoveQueue.Count > 0)    //저장된 빈 공간이 있으면 이동시킴
                    {
                        //현재 노드를 이동시키기에 빈상태 처리 후 queue에 넣어줌
                        listInfos[i, j] = ListInfo.Empty;
                        emptyMoveQueue.Enqueue(new Point(i, j));

                        //현재 노드 실제 이동 처리
                        Point p = emptyMoveQueue.Dequeue();
                        tileList[i, j].SetmoveTile(tileListPosition[p.x, p.y], p);
                        tileList[p.x, p.y] = tileList[i, j];
                        listInfos[p.x, p.y] = ListInfo.Tile;
                    }
                }
            }


            int emptyCount = emptyMoveQueue.Count;
            //새로운 노드 생성
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
            //여기서는 현재 이동중인 노드 검색만 해줌
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
        if (GameManager.instance.moveNumber <= 0)   // 무브가 남아있지 않으면..
            return;

        if (moveingTile) //현재 블록이 움직이고 있으면 return;
            return;

        if (autoDeleting || tileFalling)   //노드 자동 제거중이면 return;
            return;

        if(nowClickTile == null)    //현재 선택되어있는 타일이 존재하지 않을 때 클릭타일 배치
        {
            nowClickTile = tile;
            tile.isPick(true);
            return;
        }


        //if(nowClickTile == tile)    //현재 클릭한 타일을 또다시 클릭시 해제
        //{
        //    nowClickTile = null;
        //    tile.isPick(false);
        //    return;
        //}

        if(nowClickTile.listPos.y == tile.listPos.y &&
           (nowClickTile.listPos.x - 1 == tile.listPos.x || nowClickTile.listPos.x + 1 == tile.listPos.x))  //세로는 동일하며 양 옆 노드일 경우
        {
            //SwapTile(nowClickTile, tile);
            StartCoroutine(SwapCo(nowClickTile, tile));

            nowClickTile.isPick(false);
            nowClickTile = null;
            tile.isPick(false);
        }
        else if(nowClickTile.listPos.x == tile.listPos.x &&
           (nowClickTile.listPos.y - 1 == tile.listPos.y || nowClickTile.listPos.y + 1 == tile.listPos.y))  //가로는 동일하며 위 아래 노드일 경우
        {
            StartCoroutine(SwapCo(nowClickTile, tile));

            nowClickTile.isPick(false);
            nowClickTile = null;
            tile.isPick(false);
        }
        else    //먼 노드를 클릭시
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

        if(firstTile.tileType == TileType.Special_Munchkin)   //먼치킨 사용시
        {
            firstTile.MunchkinEffect(true);
            Point goPoint = secondTile.listPos - firstTile.listPos;
            GameManager.instance.UseMove();         //무브 사용
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
                res = NewCheckTile(firstTile.listPos, firstTile.tileType);  //첫번재 누른 위치부터 완성된 블록이 있는지 탐색
                if (res < 3)    // 완성된 블록이 없으면 
                {
                    delQueue.Clear();
                }
                res2 = NewCheckTile(secondTile.listPos, secondTile.tileType);
                break;
            }
            else
                yield return new WaitForSeconds(0.1f);
        }

        if (res >= 3 || res2 >= 3)  //지울 노드가 있을 시
        {
            GameManager.instance.UseMove();         //무브 사용

            if (delQueue.Count > 0)
                DeleteTile();
        }
        else    //없을 시
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
    public int CheckTile(Point checkPoint, TileType type)   //연결되어있는 동일한 타일 개수가 몇개인지 체크 하는 함수
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
        if (type >= TileType.Special_Munchkin)  //스페셜 노드면 제거안됨
            yield break;

        curDelQueue.Clear();
        int widthNum = 1;
        int checkX = CheckPoint.x;
        int checkY = CheckPoint.y;

        bool checkEnd = false;
        bool checkEnd2 = false;
        bool makePair = false;

        #region 가로체크 로직
        for (int i = 1; i < tileNum; i++)
        {
            if (checkEnd && checkEnd2)
                break;
            //오른쪽 방향 증가
            if (checkX + i < tileNum && !checkEnd)
            {
                if (tileList[checkX + i, checkY].tileType == type && listInfos[checkX + i, checkY] == ListInfo.Tile && !useTiled[checkX, checkY])    // 타일타입이 동일하며 비워진 타일이 아닐 시
                {
                    widthNum++;
                    curDelQueue.Enqueue(tileList[checkX + i, checkY].listPos);
                }
                else
                    checkEnd = true;
            }

            //왼쪽 방향 증가
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
            //콤보 수 증가
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
        #region 세로체크 로직
        for (int i = 1; i < tileNum; i++)
        {
            if (checkEnd && checkEnd2)
                break;

            //아래 방향 증가
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

            //위 방향 증가
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

            //콤보 수 증가
            comboCount++;
            Debug.Log(comboCount);

            foreach (var que in curDelQueue)
            {
                delQueue.Enqueue(que);
                useTiled[que.x, que.y] = true;
            }
            curDelQueue.Clear();
        }

        #region 네모체크 로직
        bool makeMunchkin = false;
        for (int i = 0; i < 4; i++)
        {
            if ((checkX + manCheckX[i] >= 0 && tileNum > checkX + manCheckX[i]) &&
               (checkY + manCheckY[i] >= 0 && tileNum > checkY + manCheckY[i]))  //범위 체크
            {
                if (useMunchKinTile[checkX + manCheckX[i], checkY] ||
                    useMunchKinTile[checkX, checkY + manCheckY[i]] ||
                    useMunchKinTile[checkX + manCheckX[i], checkY + manCheckY[i]])   // 먼치킨 생성시 사용된 노드이면 생성하지 않는다.
                    continue;

                if (tileList[checkX + manCheckX[i], checkY].tileType == type &&
                   tileList[checkX, checkY + manCheckY[i]].tileType == type &&
                   tileList[checkX + manCheckX[i], checkY + manCheckY[i]].tileType == type) //3군대가 모두 같으면 먼치킨생성
                {
                    //delqueue에 넣어줌
                        delQueue.Enqueue(tileList[checkX + manCheckX[i], checkY].listPos);
                        delQueue.Enqueue(tileList[checkX, checkY + manCheckY[i]].listPos);
                        delQueue.Enqueue(tileList[checkX + manCheckX[i], checkY + manCheckY[i]].listPos);

                    //사용된 노드 처리
                    useMunchKinTile[checkX + manCheckX[i], checkY] = true;
                    useMunchKinTile[checkX, checkY + manCheckY[i]] = true;
                    useMunchKinTile[checkX + manCheckX[i], checkY + manCheckY[i]] = true;
                    makeMunchkin = true;
                }
            }
        }

        if (!makeMunchkin)   //먼치킨을 만들지 않았으면
        {
            if (makePair)    //가로 및 세로에서 3줄이상 존재할 경우 자신도 지워줌
                delQueue.Enqueue(tileList[checkX, checkY].listPos);
        }
        else    //먼치킨을 만들면 자기 자신을 제거하지 않고 먼치킨 노드로 변경
        {
            //콤보 수 증가
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
        if (type >= TileType.Special_Munchkin)  //스페셜 노드면 제거안됨
            return 0;

        curDelQueue.Clear();
        int widthNum = 1;
        int checkX = CheckPoint.x;
        int checkY = CheckPoint.y;

        bool checkEnd = false;
        bool checkEnd2 = false;
        bool makePair = false;

        #region 가로체크 로직
        for (int i = 1; i < tileNum; i++)
        {
            if (checkEnd && checkEnd2)
                break;
            //오른쪽 방향 증가
            if (checkX + i < tileNum && !checkEnd)
            {
                if (tileList[checkX + i, checkY].tileType == type && listInfos[checkX + i, checkY] == ListInfo.Tile)    // 타일타입이 동일하며 비워진 타일이 아닐 시
                {
                    widthNum++;
                    curDelQueue.Enqueue(tileList[checkX + i, checkY].listPos);
                }
                else
                    checkEnd = true;
            }

            //왼쪽 방향 증가
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
        #region 세로체크 로직
        for (int i = 1; i < tileNum; i++)
        {
            if (checkEnd && checkEnd2)
                break;

            //아래 방향 증가
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

            //위 방향 증가
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

        #region 네모체크 로직
        bool makeMunchkin = false;
        for (int i = 0; i < 4; i++)
        {
            if ((checkX + manCheckX[i] >= 0 && tileNum > checkX + manCheckX[i]) &&
               (checkY + manCheckY[i] >= 0 && tileNum > checkY + manCheckY[i]))  //범위 체크
            {
                if (useMunchKinTile[checkX + manCheckX[i], checkY] ||
                    useMunchKinTile[checkX, checkY + manCheckY[i]] ||
                    useMunchKinTile[checkX + manCheckX[i], checkY + manCheckY[i]])   // 먼치킨 생성시 사용된 노드이면 생성하지 않는다.
                    continue;

                if (tileList[checkX + manCheckX[i], checkY].tileType == type &&
                   tileList[checkX, checkY + manCheckY[i]].tileType == type &&
                   tileList[checkX + manCheckX[i], checkY + manCheckY[i]].tileType == type) //3군대가 모두 같으면 먼치킨생성
                {
                    //delqueue에 넣어줌
                    delQueue.Enqueue(tileList[checkX + manCheckX[i], checkY].listPos);
                    delQueue.Enqueue(tileList[checkX, checkY + manCheckY[i]].listPos);
                    delQueue.Enqueue(tileList[checkX + manCheckX[i], checkY + manCheckY[i]].listPos);

                    //사용된 노드 처리
                    useMunchKinTile[checkX + manCheckX[i], checkY] = true;
                    useMunchKinTile[checkX, checkY + manCheckY[i]] = true;
                    useMunchKinTile[checkX + manCheckX[i], checkY + manCheckY[i]] = true;
                    makeMunchkin = true;
                }
            }
        }

        if (!makeMunchkin)   //먼치킨을 만들지 않았으면
        {
            if (makePair)    //가로 및 세로에서 3줄이상 존재할 경우 자신도 지워줌
                delQueue.Enqueue(tileList[checkX, checkY].listPos);
        }
        else    //먼치킨을 만들면 자기 자신을 제거하지 않고 먼치킨 노드로 변경
        {
            makeMunchKinPos[checkX, checkY] = true;
            if (!useTiled[checkX, checkY])
                delQueue.Enqueue(tileList[checkX, checkY].listPos);
        }
        #endregion


        return delQueue.Count;
    }   //포인트지점에서 연결된 가로,세로,사각 체크 처리 함수

    public void DeleteTile()
    {
        while (delQueue.Count > 0)
        {
            Point p = delQueue.Dequeue();
            if (makeMunchKinPos[p.x, p.y])  //먼지킨이 생성된 좌표일 시 제거하지 않음
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



        ResetCheck();   //먼치킨용 좌표 리셋
        if(!tileFalling)
            StartCoroutine(NewALLEmptyCheckCo());
    }

    IEnumerator UseMunchkinCo(Point startPoint, Point goPoint)
    {
        //먼저 자기 자신을 넣어줌
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
               0 <= startPoint.y && startPoint.y < tileNum)     //움직일 수 있는 범위 내
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
    }   //연결되어 있는 노드 탐색 전 변수 초기화 함수

}