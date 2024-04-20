using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerBullet : MonoBehaviour
{   //子弹速度
    [SerializeField] private float BulletSpeed = 0.8f;
    //百分之20伤害
    [SerializeField] private float bulletDamage = 0.2f;

    private GameObject target;

    void Start()
    {
        
    }

    
    void Update()
    {
        
         if(target!=null)
        {
            //子弹朝着骑士移动
            transform.position = Vector2.MoveTowards(transform.position, target.transform.position, BulletSpeed * Time.deltaTime);
            
        }
        
    }

    //设置目标
    public void SetTarget(GameObject _Knight)
    {
        target= _Knight;
        
    }
   
   //private void OnCollisionEnter2D(Collision2D _collision2D)
   // {
        
   //     if (_collision2D.gameObject.CompareTag("Knight"))
   //     {
            
   //         target.GetComponent<MasterContorller>().takeDamage(bulletDamage);

   //         //播放子弹打中骑士后的特效
   //         Destroy(this.gameObject);
   //     } 
   // }

    //用碰撞器会产生自碰撞，所以我们这里使用触发器
    private void OnTriggerEnter2D(Collider2D _collider2D)
    {
        //如果触发器碰到碰撞体为骑士
        if (_collider2D.gameObject.CompareTag("Knight"))
        {
            //骑士掉血
            target.GetComponent<MasterContorller>().takeDamage(bulletDamage);

           
            //销毁子弹
            Destroy(this.gameObject);
        }
    }
    
}
