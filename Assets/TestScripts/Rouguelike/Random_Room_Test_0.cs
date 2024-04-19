using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
/// <summary>
/// 生成初始三个绿色房间然后再根据最后随机生成的房间生成绿色房间
/// 当生成一定数量时，随机生成boss房间并所有房间必须相连
/// 初始绿色房间难点：生成的随机房间位置不能重复，如果重复则销毁并重新生成，并保证一定会有三个房间
/// </summary>
public class Random_Room_Test_0 : MonoBehaviour
{
    public GameObject while_Room0;
    public GameObject green_Room1;
    public GameObject red_Room2;

    //储存生成的绿色房间(记得在外面设置，不然会栈溢出）
    public GameObject[] GameObjectgreen_Room3;

    private bool isGreenRoomGenerate = true;

    private Vector3 targetRoom;//生成的房间位置必须相邻

    private Vector3 greenRoomPosition;//绿色房间位置

    private Vector3 redRoomPosition;//boos红色房间位置
    //上，下，右，左
    public Vector3[] randomRoomPosition = new Vector3[4];
    
    void Start()
    {
        Instantiate(while_Room0, Vector2.zero, Quaternion.identity);
        
    }

    
    void Update()
    {
        GenerateStartRoom();
    }

    private void GenerateStartRoom()
    {
        
        if (isGreenRoomGenerate)
        {
            GenerateThreeGreenRoom();

            GenerateContinueGreenRoom();

            isGreenRoomGenerate= false;
            
        }
    
    }

    //初始生成的三个绿色方块
    private void GenerateThreeGreenRoom()
    {
        for (int i = 0; i < 3; i++)
        {
            targetRoom = randomRoomPosition[Random.Range(0, 4)];
            greenRoomPosition = while_Room0.transform.position + targetRoom;


            GameObjectgreen_Room3[i] = Instantiate(green_Room1, greenRoomPosition, Quaternion.identity);

            if (GameObjectgreen_Room3[1] != null)
            {
                if (GameObjectgreen_Room3[1].transform.position == GameObjectgreen_Room3[0].transform.position)
                {
                    Destroy(GameObjectgreen_Room3[1]);
                    i -= 1;
                }

            }

            if (GameObjectgreen_Room3[2] != null)
            {

                if (GameObjectgreen_Room3[2].transform.position == GameObjectgreen_Room3[1].transform.position || GameObjectgreen_Room3[2].transform.position == GameObjectgreen_Room3[0].transform.position)
                {

                    Destroy(GameObjectgreen_Room3[2]);
                    i -= 1;
                }
                else
                {
                    for (int j = 3; j < GameObjectgreen_Room3.Length; j++)
                    {
                        targetRoom = randomRoomPosition[Random.Range(0, 4)];
                        greenRoomPosition = GameObjectgreen_Room3[j - 1].transform.position + targetRoom;

                        GameObjectgreen_Room3[j] = Instantiate(green_Room1, greenRoomPosition, Quaternion.identity);

                        if (GameObjectgreen_Room3[3] != null)
                        {
                            if (GameObjectgreen_Room3[3].transform.position == Vector3.zero)
                            {
                                Destroy(GameObjectgreen_Room3[3]);
                                j -= 1;
                            }
                        }
                        for (int k = 0; k < 4; k++)
                        {

                            if (GameObjectgreen_Room3[j].transform.position == GameObjectgreen_Room3[j].transform.position + randomRoomPosition[k]
                                || GameObjectgreen_Room3[j].transform.position==Vector3.zero)
                            {
                                Destroy(GameObjectgreen_Room3[j]);
                                j-= 1;
                                
                            }
                        }
                        
                    }
                }
            }
        }
    }

    private void GenerateContinueGreenRoom()
    {

    }
}
