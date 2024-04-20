using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class MasterContorller : MonoBehaviour
{
    [SerializeField] private GameObject winPlace;
    [Header("骑士")]
    //骑士的生命值
    public float maxHealth = 10f;
    private float currentHealth = 10f;
    //骑士的移动速度
    [SerializeField] private float masterSpeed = 0.5f;

    //颜色变化特效和变形同步持续时间
    private float blendAndScaleTime = 0.5f;
    void Start()
    {
        
      //获取宝藏的物体
      winPlace = GameObject.Find("WinPlace").gameObject;
    }

    
   private void FixedUpdate()
    {
       //transform.position= Vector2.Lerp(transform.position, winPlace.transform.position, masterSpeed * Time.deltaTime);
      transform.position= Vector2.MoveTowards(transform.position,winPlace.transform.position,masterSpeed*Time.deltaTime);
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

}
