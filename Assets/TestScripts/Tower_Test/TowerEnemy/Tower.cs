using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class Tower : MonoBehaviour
{
    
    //攻击范围
    private float attackRange = 0.7f;
    //
    [SerializeField] private LayerMask whatIsEnemy;

    [SerializeField] private GameObject towerBulletPrefab;
    //子弹的速度
    [SerializeField] private float towerbullet_Speed = 2f;

    //攻击间隔时间（秒）
    [SerializeField] private float attackInterval = 2f;
    //攻击冷却计时器
    private float attackCooldown = 0.0f;
    //防御塔的生命值
    public int health = 50;
    //攻击对骑士造成的伤害比
    //private float damagePercentage = 0.2f;//百分之20伤害
    


    void Start()
    {
        
    }

    
   private void Update()
    {
        if(attackCooldown> 0.0f)
        {
            attackCooldown-=Time.deltaTime;
        }
        else
        {
            Attack();
            attackCooldown = attackInterval;
        }
    }

    //找到目标后就先攻击一次
   private void Attack()
    {
        GameObject nearestEnemy = FindNearesEnemy();
        if(nearestEnemy!= null )
        {
            GameObject bullet = Instantiate(towerBulletPrefab, transform.position, Quaternion.identity);
            ////复习知识点，velocity是一个二维向量，用来设置x和y上的速度，但Kinematic不适用
            bullet.GetComponent<Rigidbody2D>().velocity = (nearestEnemy.transform.position - transform.position).normalized * towerbullet_Speed;
            
            bullet.GetComponent<TowerBullet>().SetTarget(nearestEnemy);
        }
    }

    //1.当第一个骑士先进入后一个骑士再进入
    //2.当二个骑士先后进入后第二个骑士超过第一个骑士
    //3.当三个骑士先后进入后，第二个骑士超过第一个骑士，第三个骑士超过第二个骑士；
   private GameObject FindNearesEnemy()
    {
        GameObject nearestKnight = null;
        float nearestDistance =float.MaxValue;  

        //检测圆圈半径覆盖的所有敌人
        Collider2D[] KnightsInRange = Physics2D.OverlapCircleAll(transform.position, attackRange,whatIsEnemy);
        
        //遍历所有在范围内的敌人
        //foreach会对范围内的检测到的所有骑士进行if验证判断，当收集到有二个或以上的骑士出现在范围之内，
        //会依次执行if语句内的代码而不是从头开始执行
        foreach (Collider2D knightCollider in KnightsInRange)
        {
            if (knightCollider.CompareTag("Knight"))
            {
                GameObject knight = knightCollider.gameObject;

                //当前防御塔和骑士的距离
                float distance =Vector2.Distance(transform.position,knightCollider.transform.position);
                //如果防御和骑士的距离小于全局最大距离
                if(distance < nearestDistance)
                {
                    //将当前骑士设置为最近的骑士
                    nearestKnight = knight;
                    //将上一个骑士的距离设置为最近距离
                    nearestDistance= distance;
                }
               
            }
        }
        return nearestKnight;
    }
}
