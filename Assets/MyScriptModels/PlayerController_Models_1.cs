using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController_Models_1 : MonoBehaviour
{
    [SerializeField] private float speed;

    [Header("Jumping")]
    [SerializeField] private float jumpFoce;
    [SerializeField] private LayerMask whatIsGround;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius;

    //用于与动画联动和处理移动的变量
    [HideInInspector] public float moveInput;//获取输入值
    [HideInInspector] public bool canMove = true;//是否正常移动
    [HideInInspector] public bool isCurrentlyPlayable = false;//是否当前可玩
    public bool isGrounded;//是否在地面
    private float m_groundedRememberTime = 0.25f; // 记住角色是否在地面上的时间
    private float m_groundedRemember = 0f; // 记住角色在地面上的时间

    //组件获取
    private InputManager inputManager;//引入输入脚本
    private Rigidbody2D m_rb;
    private ParticleSystem m_dustParticle;

    //私有成员变量
    private bool m_facingRight = true;//角色面向右边


    void Start()
    {
        if(transform.CompareTag("Player"))
            isCurrentlyPlayable=true;

        SetupAndGetComponent();
    }

    private void FixedUpdate()
    {
        Move();
        
    }
    
    void Update()
    {
        moveInput = inputManager.HorizontalRaw();
        Jump();
    }

    private void SetupAndGetComponent()
    {
        // 获取角色的 Rigidbody2D 组件
        m_rb = GetComponent<Rigidbody2D>();
        // 获取角色的 ParticleSystem 组件
        m_dustParticle = GetComponentInChildren<ParticleSystem>();
        // 获取输入管理器
        inputManager = GameObject.Find("_GameManager").GetComponent<InputManager>();

        
    }
    //角色翻转
    private void Flip()
    {
        m_facingRight= !m_facingRight;
        Vector2 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    private void Move()
    {
        if (isCurrentlyPlayable)
        {
            if (canMove)
            {
                m_rb.velocity = new Vector2(moveInput * speed, m_rb.velocity.y);

            }
            else if (!canMove)
            {
                m_rb.velocity = new Vector2(0, m_rb.velocity.y);
            }
        }

        if (!m_facingRight && moveInput > 0)
            Flip();
        else if (m_facingRight && moveInput < 0)
            Flip();
    }

    private void Jump()
    {
        //判断半径与地面是否重叠
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, whatIsGround);
        // 1.更新记住角色上次在地面上的时间
        // 2.防止物理碰撞迅速回弹时为false
        m_groundedRemember -= Time.deltaTime;
        if (isGrounded)
            m_groundedRemember = m_groundedRememberTime;

        //单次跳跃
        if (inputManager.Jump()&&isGrounded)
        {
            m_rb.velocity = new Vector2(m_rb.velocity.x, jumpFoce);
            
        }
    }
}
