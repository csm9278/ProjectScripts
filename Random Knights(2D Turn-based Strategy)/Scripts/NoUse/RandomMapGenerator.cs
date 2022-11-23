using System.Collections.Generic;
using UnityEngine;

public class RandomMapGenerator : MonoBehaviour
{
    

    public class MapType
    {
        public enum kind
        {
            normalEnemy,
            elitEnemy,
            Shop,
            BaseCamp,
            RandomEvent
        }

        public kind mapkind = kind.normalEnemy;
        public int floor = 0;
        public Vector3 location = Vector3.zero;
        public GameObject circle;

        public MapType(int a_floor, Vector3 loc, GameObject cir)
        {
            floor = a_floor;
            location = loc;
            circle = cir;
        }
    }

    public GameObject circle;
    public int increaseY = 5;
    public int increaseX = 2;
    public int maxX = 30;
    int floor = 0;
    [SerializeField] List<MapType> mapList;
    [SerializeField] GameObject canvas;

    private void Start() => StartFunc();

    private void StartFunc()
    {
        mapList = new List<MapType>();

        for(int y = 0 ; y < maxX; y += increaseY)
        {
            floor++;

            for(int i = -4; i < 5; i +=4)
            {
                int plusx = Random.Range(-1, 2);
                int x = plusx * increaseX;

                x += i;
                Vector2 circleVec = new Vector2(x, y);

                GameObject obj = Instantiate(circle);
                obj.transform.position = circleVec;

                MapType maptype = new MapType(floor, circleVec, obj);
                mapList.Add(maptype);
            }
        }

        //¶óÀÎ ·»´õ·¯
        for (int i = 0; i < mapList.Count; i++)
        {
            for (int j = 0; j < mapList.Count; j++)
            {
                if (mapList[i].floor + 1 == mapList[j].floor)
                {
                    Debug.Log(mapList[i].location.x - mapList[j].location.x);

                    if (Mathf.Abs(mapList[i].location.x - mapList[j].location.x) <= 4)
                    {
                        Debug.Log("enter " + (mapList[i].location.x - mapList[j].location.x).ToString());
                        mapList[i].circle.GetComponent<LineRenderer>().SetPosition(0, mapList[i].circle.transform.position);
                        mapList[i].circle.GetComponent<LineRenderer>().SetPosition(1, mapList[j].circle.transform.position);
                    }
                }
            }

        }
    }

    private void Update() => UpdateFunc();

    private void UpdateFunc()
    {
        
    }
}