using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class MasterContorller : MonoBehaviour
{
    [SerializeField] private GameObject winPlace;
    [Header("骑士")]
    //骑士的生命值
    [SerializeField] float maxHealth = 10f;
    [SerializeField] float currentHealth = 10f;
    //骑士的移动速度
    [SerializeField] private float masterSpeed = 0.5f;
    //骑士的攻击范围
    [SerializeField] private float attackRange = 1.0f;
    //骑士的攻击力
    [SerializeField] private float attackDamage = 0.2f;//百分之20

    //检测的半径范围
    private float knightRadius = 1f;
    //哪个层是防御塔层
    [SerializeField] private LayerMask whatIsTower;

    private List<Tower> towersInRange = new List<Tower>(); // 存储防御塔的列表

    private float timeInterval = 1.0f;
    private float attackCooldown = 0f;

    void Start()
    {
        
      //获取宝藏的物体
      winPlace = GameObject.Find("WinPlace").gameObject;
    }

    
   private void FixedUpdate()
    {
        TrackTarget();

    }

    //掉血脚本
    public void takeDamage(float _damage)
    {

        currentHealth -= (maxHealth*_damage);

        gameObject.GetComponent<Animator>().SetBool("IsHurt", true);
       
        if (currentHealth <=0)
        {


            //播放死亡动画
            Destroy(this.gameObject);
        }
    }

    //动画事件调用的函数
    public void ResetHurtTrigger()
    {
        this.gameObject.GetComponent<Animator>().SetBool("IsHurt", false);
        
    }

    
    public void TrackTarget()
    {
        towersInRange.Clear();
        //骑士的范围内有几个塔
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, knightRadius, whatIsTower);
        //第一次foreach找出所有骑士范围内的塔
        foreach(var collider in colliders)
        {
            Tower tower = collider.GetComponent<Tower>();
            if (tower != null)
            {
                towersInRange.Add(tower);
            }
            
        }
        if(towersInRange.Count > 0)
        {
            Tower nearestTower = FindNearestTower();
            if (nearestTower != null)
            {
                

                if (Vector2.Distance(transform.position, nearestTower.transform.position) <= attackRange)
                {
                    Vector2 targetPositionInterval = new Vector2(nearestTower.transform.position.x, nearestTower.transform.position.y);
                    //不愿意写了到达攻击范围的时候可以进行移动的停止
                    //transform.position = Vector2.MoveTowards(transform.position, targetPositionInterval, masterSpeed * Time.deltaTime);
                    AttackTower(nearestTower); // 执行攻击
                }
                else 
                {
                    transform.position = Vector2.MoveTowards(transform.position, nearestTower.transform.position, masterSpeed * Time.deltaTime);
                }
            }
        }
        else
        {
            //朝宝藏方向移动
            transform.position = Vector2.MoveTowards(transform.position, winPlace.transform.position, masterSpeed * Time.deltaTime);
            //触碰到宝藏后触发特效，显示UI游戏胜利
        }



    }

    private Tower FindNearestTower()
    {
        Tower nearestTower = null;
        float nearestDistance = float.MaxValue;
        //第二次foreach在处于骑士范围内的塔中找出距离最近的哪个塔,
        foreach(var tower in towersInRange)
        {
            if(tower.gameObject!=null)
            {
                float distance = Vector2.Distance(transform.position, tower.transform.position);
                if(distance<nearestDistance)
                {
                    
                    nearestDistance = distance;
                    nearestTower = tower;
                }
            }
        }
        return nearestTower;
    }

    public void AttackTower(Tower _tower)
    {

        if (attackCooldown >0)
        {
            attackCooldown-=Time.deltaTime;
        }
        else
        {
            _tower.takeDamage(attackDamage);
            attackCooldown = timeInterval;
        }
        
        
    }

 
}
