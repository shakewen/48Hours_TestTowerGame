using UnityEngine;

namespace SupanthaPaul
{
    public class PlayerController : MonoBehaviour
    {
        // 序列化字段，用于设置角色的移动速度
        [SerializeField] private float speed;

        // 跳跃相关配置
        [Header("Jumping")] // Unity 编辑器中显示的标题
        [SerializeField] private float jumpForce; // 跳跃力量
        [SerializeField] private float fallMultiplier; // 掉落时的重力乘数
        [SerializeField] private Transform groundCheck; // 检测角色是否在地面上的 Transform
        [SerializeField] private float groundCheckRadius; // 地面检测的半径
        [SerializeField] private LayerMask whatIsGround; // 哪些层被认为是地面
        [SerializeField] private int extraJumpCount = 1; // 额外的跳跃次数
        [SerializeField] private GameObject jumpEffect; // 跳跃时的特效

        // 冲刺相关配置
        [Header("Dashing")]
        [SerializeField] private float dashSpeed = 30f; // 冲刺速度
        [SerializeField] private float startDashTime = 0.1f; // 冲刺开始的时间
        [SerializeField] private float dashCooldown = 0.2f; // 冲刺冷却时间
        [SerializeField] private GameObject dashEffect; // 冲刺时的特效

        // 用于动画处理和其他用途的公共变量
        [HideInInspector] public bool isGrounded; // 是否在地面上
        [HideInInspector] public float moveInput; // 角色的水平输入
        [HideInInspector] public bool canMove = true; // 是否可以移动
        [HideInInspector] public bool isDashing = false; // 是否正在冲刺
        [HideInInspector] public bool actuallyWallGrabbing = false; // 是否实际抓墙
        [HideInInspector] public bool isCurrentlyPlayable = false; // 是否当前可玩

        // 抓墙和跳跃相关的配置
        public Vector2 grabRightOffset; // 抓墙检测右边界的偏移量
        public Vector2 grabLeftOffset; // 抓墙检测左边界的偏移量
        public float grabCheckRadius = 0.24f; // 抓墙检测的半径
        public float slideSpeed = 2.5f; // 滑行速度
        public Vector2 wallJumpForce = new Vector2(10.5f, 18f); // 墙壁跳跃的力量
        public Vector2 wallClimbForce = new Vector2(4f, 14f); // 墙壁攀爬的力量

        // 私有字段
        private Rigidbody2D m_rb; // 角色的刚体组件
        private ParticleSystem m_dustParticle; // 角色的尘埃粒子系统
        private bool m_facingRight = true; // 角色面向右边
        private float m_groundedRememberTime = 0.25f; // 记住角色是否在地面上的时间
        private float m_groundedRemember = 0f; // 记住角色在地面上的时间
        private int m_extraJumps; // 额外的跳跃次数
        private float m_extraJumpForce; // 额外跳跃的力量
        private float m_dashTime; // 冲刺时间
        private bool m_hasDashedInAir = false; // 是否在空中冲刺过
        private bool m_onWall = false; // 是否在墙上
        private bool m_onRightWall = false; // 是否在右墙上
        private bool m_onLeftWall = false; // 是否在左墙上
        private bool m_wallGrabbing = false; // 是否正在抓墙
        private readonly float m_wallStickTime = 0.25f; // 抓墙的粘滞时间
        private float m_wallStick = 0f; // 抓墙的粘滞时间计数
        private bool m_wallJumping = false; // 是否正在墙壁跳跃
        private float m_dashCooldown; // 冲刺冷却时间

        // 角色在墙壁的哪一侧
        private int m_onWallSide = 0;
        private int m_playerSide = 1;

        // 输入管理器
        [SerializeField] private InputManager inputManager;

        void Start()
        {
            //// 创建粒子池，用于重复使用特效
            //PoolManager.instance.CreatePool(dashEffect, 2);
            //PoolManager.instance.CreatePool(jumpEffect, 2);

            // 如果是玩家角色，则设置为当前可玩
            if (transform.CompareTag("Player"))
                isCurrentlyPlayable = true;

            SetupAndGetComponent();



        }

        private void FixedUpdate()
        {
            // 检测角色是否在地面上
            //检测半径内是否重叠，如果充电则返回True
            isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, whatIsGround);
            var position = transform.position;

            // 检测角色是否在墙上
            m_onWall = Physics2D.OverlapCircle((Vector2)position + grabRightOffset, grabCheckRadius, whatIsGround)
                        || Physics2D.OverlapCircle((Vector2)position + grabLeftOffset, grabCheckRadius, whatIsGround);
            m_onRightWall = Physics2D.OverlapCircle((Vector2)position + grabRightOffset, grabCheckRadius, whatIsGround);
            m_onLeftWall = Physics2D.OverlapCircle((Vector2)position + grabLeftOffset, grabCheckRadius, whatIsGround);

            // 计算角色和墙壁的侧面
            CalculateSides();

            // 如果在抓墙并且墙壁跳跃，则重置墙壁跳跃标志
            if ((m_wallGrabbing || isGrounded) && m_wallJumping)
            {
                m_wallJumping = false;
            }

            // 如果当前可玩
            if (isCurrentlyPlayable)
            {
                // 当在挂壁时跳跃
                if (m_wallJumping)
                {
                    // 插值移动速度
                    m_rb.velocity = Vector2.Lerp(m_rb.velocity, (new Vector2(moveInput * speed, m_rb.velocity.y)), 1.5f * Time.fixedDeltaTime);
                }
                else
                {
                    // 如果能够正常移动且没挂在墙壁上
                    if (canMove && !m_wallGrabbing)
                        m_rb.velocity = new Vector2(moveInput * speed, m_rb.velocity.y);
                    //如果不能正常移动，设置跳跃参数
                    else if (!canMove)
                        m_rb.velocity = new Vector2(0f, m_rb.velocity.y);
                }

                // 当跳跃结束下降时
                if (m_rb.velocity.y < 0f)
                {

                    // 根据速度和重力调整垂直速度
                    m_rb.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.fixedDeltaTime;
                }

                // 翻转角色
                if (!m_facingRight && moveInput > 0f)
                    Flip();
                else if (m_facingRight && moveInput < 0f)
                    Flip();

                // 处理冲刺逻辑
                if (isDashing)
                {
                    // 如果冲刺时间小于等于0，则结束冲刺
                    if (m_dashTime <= 0f)
                    {
                        isDashing = false;
                        m_dashCooldown = dashCooldown;
                        m_dashTime = startDashTime;
                        m_rb.velocity = Vector2.zero;
                    }
                    else
                    {
                        // 减少冲刺时间，并根据角色面向方向设置速度
                        m_dashTime -= Time.deltaTime;
                        if (m_facingRight)
                            m_rb.velocity = Vector2.right * dashSpeed;
                        else
                            m_rb.velocity = Vector2.left * dashSpeed;
                    }
                }
            }

            // 处理墙壁抓取
            if (m_onWall && !isGrounded && m_rb.velocity.y <= 0f && m_playerSide == m_onWallSide)
            {
                // 开始抓墙
                actuallyWallGrabbing = true;
                m_wallGrabbing = true;
                m_rb.velocity = new Vector2(moveInput * speed, -slideSpeed);
                m_wallStick = m_wallStickTime;
            }
            else
            {
                // 结束抓墙
                m_wallStick -= Time.deltaTime;
                actuallyWallGrabbing = false;
                if (m_wallStick <= 0f)
                    m_wallGrabbing = false;
            }
            if (m_wallGrabbing && isGrounded)
                m_wallGrabbing = false;

            // 启用/禁用尘埃粒子
            float playerVelocityMag = m_rb.velocity.sqrMagnitude;
            if (m_dustParticle.isPlaying && playerVelocityMag == 0f)
            {
                // 如果粒子系统正在播放且角色速度为0，则停止粒子系统
                m_dustParticle.Stop();
            }
            else if (!m_dustParticle.isPlaying && playerVelocityMag > 0f)
            {
                //如果粒子系统未播放且角色速度不为0，则播放粒子系统
                m_dustParticle.Play();
            }
        }

        private void Update()
        {
            // 获取水平输入
            moveInput = inputManager.HorizontalRaw();

            // 如果在地面上，则重置额外跳跃次数
            if (isGrounded)
            {
                m_extraJumps = extraJumpCount;
            }

            // 1.更新记住角色上次在地面上的时间
            // 2.防止物理碰撞迅速回弹时为false
            m_groundedRemember -= Time.deltaTime;
            if (isGrounded)
                m_groundedRemember = m_groundedRememberTime;

            // 该方法（UPdate）会立刻停止，不是返回上一个代码语句，而是跳出方法，重新执行该方法，直到为True为止
            if (!isCurrentlyPlayable) return;

            // 处理冲刺输入
            if (!isDashing && !m_hasDashedInAir && m_dashCooldown <= 0f)
            {
                // 检测冲刺输入
                if (inputManager.Dash())
                {
                    isDashing = true;
                    // 播放冲刺特效
                    PoolManager.instance.ReuseObject(dashEffect, transform.position, Quaternion.identity);
                    // 如果角色在空中，则标记为在空中冲刺
                    if (!isGrounded)
                    {
                        m_hasDashedInAir = true;
                    }
                    // 冲刺逻辑在 FixedUpdate 中处理
                }
            }
            m_dashCooldown -= Time.deltaTime;

            // 如果角色在空中冲刺过一次但现在在地面上
            if (m_hasDashedInAir && isGrounded)
            {
                m_hasDashedInAir = false;
            }
            
            // 处理跳跃输入
            if (inputManager.Jump() && m_extraJumps > 0 && !isGrounded && !m_wallGrabbing) // 额外跳跃
            {
                // 设置垂直速度为额外跳跃力量
                m_rb.velocity = new Vector2(m_rb.velocity.x, m_extraJumpForce);
                m_extraJumps--;
                // 播放跳跃特效
                PoolManager.instance.ReuseObject(jumpEffect, groundCheck.position, Quaternion.identity);
            }
            else if (inputManager.Jump() && (isGrounded || m_groundedRemember > 0f)) // 正常单次跳跃
            {
                // 设置垂直速度为跳跃力量
                m_rb.velocity = new Vector2(m_rb.velocity.x, jumpForce);
                // 播放跳跃特效
                PoolManager.instance.ReuseObject(jumpEffect, groundCheck.position, Quaternion.identity);
            }
            else if (inputManager.Jump() && m_wallGrabbing && moveInput != m_onWallSide) // 墙壁跳跃离开墙壁
            {
                // 结束抓墙
                m_wallGrabbing = false;
                m_wallJumping = true;
                Debug.Log("Wall jumped");
                // 根据角色面向方向进行翻转
                if (m_playerSide == m_onWallSide)
                    Flip();
                // 添加墙壁跳跃的力量
                m_rb.AddForce(new Vector2(-m_onWallSide * wallJumpForce.x, wallJumpForce.y), ForceMode2D.Impulse);
            }
            else if (inputManager.Jump() && m_wallGrabbing && moveInput != 0 && (moveInput == m_onWallSide)) // 墙壁攀爬跳跃
            {
                // 结束抓墙
                m_wallGrabbing = false;
                m_wallJumping = true;
                Debug.Log("Wall climbed");
                // 根据角色面向方向进行翻转
                if (m_playerSide == m_onWallSide)
                    Flip();
                // 添加墙壁攀爬的力量
                m_rb.AddForce(new Vector2(-m_onWallSide * wallClimbForce.x, wallClimbForce.y), ForceMode2D.Impulse);
            }
        }

        // 翻转角色面向方向的函数
        private void Flip()
        {
            m_facingRight = !m_facingRight;
            Vector3 scale = transform.localScale;
            scale.x *= -1;
            transform.localScale = scale;
        }

        // 计算角色和墙壁的侧面的函数
        private void CalculateSides()
        {
            if (m_onRightWall)
                m_onWallSide = 1;
            else if (m_onLeftWall)
                m_onWallSide = -1;
            else
                m_onWallSide = 0;

            if (m_facingRight)
                m_playerSide = 1;
            else
                m_playerSide = -1;
        }

        //初始化设置参数和获取组件
        private void SetupAndGetComponent()
        {
            // 初始化额外跳跃次数和冲刺时间
            m_extraJumps = extraJumpCount;
            m_dashTime = startDashTime;
            m_dashCooldown = dashCooldown;
            m_extraJumpForce = jumpForce * 0.7f;

            // 获取角色的 Rigidbody2D 组件
            m_rb = GetComponent<Rigidbody2D>();
            // 获取角色的 ParticleSystem 组件
            m_dustParticle = GetComponentInChildren<ParticleSystem>();
            // 获取输入管理器
            inputManager = GameObject.Find("_GameManager").GetComponent<InputManager>();
        }

        // 当对象被选中时绘制辅助线
        //private void OnDrawGizmosSelected()
        //{
        //    Gizmos.color = Color.red;
        //    Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        //    Gizmos.DrawWireSphere((Vector2)transform.position + grabRightOffset, grabCheckRadius);
        //    Gizmos.DrawWireSphere((Vector2)transform.position + grabLeftOffset, grabCheckRadius);
        //}
    }
}
