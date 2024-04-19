using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class MasterContorller : MonoBehaviour
{
    [SerializeField] private GameObject winPlace;

    //骑士的生命值
    public float maxHealth = 10;
    private float currentHealth = 10;
    //获取炮台

    //骑士的移动速度
    private float masterSpeed = 0.5f;
    

    
    void Start()
    {
        winPlace = GameObject.Find("WinPlace").gameObject;
    }

    
   private void FixedUpdate()
    {
       //transform.position= Vector2.Lerp(transform.position, winPlace.transform.position, masterSpeed * Time.deltaTime);
      transform.position= Vector2.MoveTowards(transform.position,winPlace.transform.position,masterSpeed*Time.deltaTime);
    }
    
    public void takeDamage(float _damage)
    {
        currentHealth -= maxHealth*_damage;
        
        if(currentHealth <=0)
        {
            //播放死亡动画
            Destroy(this.gameObject);
        }
    }
}
