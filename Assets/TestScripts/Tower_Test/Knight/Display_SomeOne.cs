using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using UnityEngine;
using UnityEngine.UIElements;

public class Display_SomeOne : MonoBehaviour
{





    [Header("骑士")]
    //获取骑士（原先设计的是怪物）的Prefbs
    [SerializeField] private GameObject Master;
    //设置当前临时储存的骑士
    [SerializeField] private GameObject currentMaster;
    //骑士层
    [SerializeField] private LayerMask wahtIs_knightRange;
    // 存储生成的 Master 对象及其生成炮台的状态,键值对应，设置每个物体的判定状态，防止重复判定
    private Dictionary<GameObject, bool> generationMastersWithTowerGenerated = new Dictionary<GameObject, bool>();
    //骑士的半径
    [SerializeField] private float masterRadius = 0.01f;
    //是否骑士出现在敌方领土内
    [HideInInspector] public bool isMaster_InBatteryRange;

    



    [Header("防御塔")]
    //获取防御塔的Prefabs（初始名称为炮台）
    [SerializeField] private GameObject Battery;
    //设置当前的防御塔
    [SerializeField] private GameObject currentBattery;
    //领地层
    [SerializeField] private LayerMask whatIs_BatteryGenetation_Range;
    //初始设置为第二次防御塔生成为5秒钟，后续每次生成时间为3秒
    [SerializeField] private float timeInterval = 5f;
    //防御塔生成数量
    [SerializeField] private int batteryCount = 2;
    //防御塔是否进入冷却状态
    private bool tower_CoolDown = false;
    //防御塔生成的位置
    private Vector2 battery_Genetate_Position;


    [Header("UI界面")]
    public GameObject knight_Cooldown;

    private bool isKnight_Cooldown;


    //屏幕获取的点位
    private Vector2 point;





    void Start()
    {
        
    }


    void Update()
    {
        
        MouseClick_InRange();


        whatIsHitKnight();
        

        //技能是否冷却，后续每次生成时间
        CoolDown(tower_CoolDown,3f);
        


    }

    //鼠标点击生成小怪
    public bool IsClickOn2DCollider()
    {
        RaycastHit2D hitPlace = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

        if (hitPlace.collider != null)
        {
            point = hitPlace.point;

            if (hitPlace.collider.name == "DisplayMaster")
            {
                return true;
            }

        }
        // 如果射线没有检测到碰撞体
        else
        {
            // 提示用户请在有效区域内点击

            // 可以设置一个默认的生成位置，或者返回 false 以阻止生成
            point = new Vector2(0, 0); // 这里设置了一个默认位置，例如屏幕中心或其他预定位置
            return false;
        }

        return false; // 如果没有满足上述条件，返回 false
    }
    //判断是否有能打到骑士的防御塔
    public void whatIsHitKnight()
    {
        //如果防御塔在营地中已经有了
        if (currentBattery != null)
        {
            //检测当前的所有防御塔内是否有骑士，并用数组保存
            Collider2D[] KnightInTowerRange = Physics2D.OverlapCircleAll(currentBattery.transform.position, 1f, wahtIs_knightRange);
            //如果场上所有的防御塔都没能够达到骑士的射击范围
            if (KnightInTowerRange.Length == 0)
            {
                //判断是否生成防御塔
                JudgmentTowerGeneration();
            }
            else
            {
                return;
            }
        }
        else
        {
            //当前生成初始防御塔
            JudgmentTowerGeneration();
        }
    }
    //判断是否能够生成
    public void JudgmentTowerGeneration()
    {


        //当怪兽不为空且没进入冷却时间
        if (currentMaster != null && !tower_CoolDown&&batteryCount>0)
        {
           
                isMaster_InBatteryRange = Physics2D.OverlapCircle(currentMaster.transform.position, masterRadius, whatIs_BatteryGenetation_Range);
                //如果当前骑士在敌方领地内且还未生成与之对应的防御塔
                if (isMaster_InBatteryRange && !generationMastersWithTowerGenerated[currentMaster])
                {
                    //生成防御塔
                    TowerGenerate();

                }
            
        }
        else
        {
            return;
        }
    }
    //防御塔随机生成
    public void TowerGenerate()
    {
        //根据怪物的距离在随机在范围内生成炮台；
        float createBattery = Random.Range(-0.99f, 0.99f);
        //当临近边界时，进行二次随机
        float twiceRandom = Random.Range(0.05f, 0.2f);

        battery_Genetate_Position = new Vector2(currentMaster.transform.position.x + createBattery, currentMaster.transform.position.y + createBattery);
        //初始精确生成数值x坐标为：-3,3
        //y坐标为：-2.5,2.5
        battery_Genetate_Position.x = Mathf.Clamp(battery_Genetate_Position.x, -3f, 3f);
        battery_Genetate_Position.y = Mathf.Clamp(battery_Genetate_Position.y, -2.5f, 2.5f);
        //二次随机确保塔不会生成在边界
        if (battery_Genetate_Position.x == -3f)
        {

            battery_Genetate_Position.x = battery_Genetate_Position.x + twiceRandom;
        }
        else if (battery_Genetate_Position.y == -3f)
        {
            battery_Genetate_Position.y = battery_Genetate_Position.x + twiceRandom;
        }
        else if (battery_Genetate_Position.x == 3f)
        {
            battery_Genetate_Position.x = battery_Genetate_Position.x - twiceRandom;
        }
        else if (battery_Genetate_Position.y == -3f)
        {
            battery_Genetate_Position.y = battery_Genetate_Position.x - twiceRandom;
        }

        currentBattery = Instantiate(Battery, battery_Genetate_Position, Quaternion.identity);
        //目前可生成的防御塔数量-1
        batteryCount--;
        //炮台标记为已生成
        generationMastersWithTowerGenerated[currentMaster] = true;
        //进入冷却
        tower_CoolDown = true;
    }
    //CoolDown（是否开启技能冷却，循环冷却时间
    public void CoolDown(bool _isCoolDown,float _repeatFloatTime)
    {
        
        //进入冷却时间，五秒钟
        if (_isCoolDown)
        {
            
            timeInterval -= Time.deltaTime;
            if (timeInterval < 0)
            {
                timeInterval = _repeatFloatTime;
                _isCoolDown = false;
                
                tower_CoolDown = _isCoolDown;
                
            }
        }
    }
    //鼠标是否点击在设定范围内，且该键值对应的防御塔未生成
    public void MouseClick_InRange()
    {
        //判断是否冷却是否完毕
        isKnight_Cooldown = knight_Cooldown.GetComponent<Knight_Cooldown>().skillButton.interactable;
        //如果点击鼠标左键且在规定范围内且冷却完毕
        if (Input.GetMouseButtonDown(0) && IsClickOn2DCollider()&&isKnight_Cooldown)
        {
            knight_Cooldown.GetComponent<Knight_Cooldown>().ActivateSkill();


            //临时储存的骑士 
            currentMaster = Instantiate(Master, point, Quaternion.identity);

            generationMastersWithTowerGenerated[currentMaster] = false;

        }
    }

}