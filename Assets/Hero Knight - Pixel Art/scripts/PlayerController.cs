using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float walkSpeed = 1;
    [SerializeField] private float jumpForce = 45f;
    private float jumpBufferCounter = 0;
    [SerializeField] private float jumpBufferFrames;
    private float coyoteTimeCounter = 0;
    [SerializeField] private float coyoteTime;
    private int airJumpCounter = 0;
    [SerializeField] private int maxAirJump;
    [SerializeField] private float dashSpeed;
    [SerializeField] private float dashTime;
    [SerializeField] private float dashCooldown;

    [SerializeField] private Transform groundCheckpoint;
    [SerializeField] private float groundCheckY = 0.2f; 
    [SerializeField] private float groundCheckX = 0.5f; 
    [SerializeField] private LayerMask whatIsGround;

    private bool canDash = true;
    private bool dashed;

    [SerializeField] int recoilXSteps = 5;
    [SerializeField] int recoilYSteps = 5;
    [SerializeField] float recoilXSpeed = 100;
    [SerializeField] float recoilYSpeed = 100;
    int stepsXRecoil, stepsYRecoil;

    public int health;
    public int maxHealth;
    public delegate void OnHealthChangedDelegate();
    [HideInInspector] public OnHealthChangedDelegate onHealthChangedCallback;

    [HideInInspector] public PlayerStateList pState;
    private Rigidbody2D rb;
    private float xAxis, yAxis;
    Animator anim; 
    bool attack = false;
    [SerializeField] private float timeBetweenAttack; 
    private float timeSinceAttack;
    private float gravity;

    [SerializeField] Transform sideAttackTransform, upAttackTransform, downAttackTransform;
    [SerializeField] Vector2 sideAttackArea, upAttackArea, downAttackArea;
    [SerializeField] LayerMask attackableLayer;
    [SerializeField] float damage;
    [SerializeField] GameObject slashEffect; 
    [SerializeField] private float wallSlideSpeed = 2f;
    [SerializeField] private Transform wallCheck;
    [SerializeField] private LayerMask wallLayer;
    [SerializeField] private float wallJumpDuration;
    [SerializeField] private Vector2 wallJumpPower; 
    [SerializeField] private GameObject myCanvas;
    float wallJumpDirection;
    bool isWallSliding;
    bool isWallJumping;
    public bool unlockedDoubleJump;
    public bool unlockedDash;
    public bool unlockedWallJump;
    public static PlayerController Instance;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else 
        {
            Instance = this;
        }
        Health = maxHealth;
    }

    void Start()
    {
        pState = GetComponent<PlayerStateList>();
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent <Animator>();
        gravity = rb.gravityScale;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(sideAttackTransform.position, sideAttackArea);
        Gizmos.DrawWireCube(upAttackTransform.position, upAttackArea);
        Gizmos.DrawWireCube(downAttackTransform.position, downAttackArea);

    }

    void Update()
    {   
        if(pState.alive)
        {
            GetInputs();
        }
        UpdateJumpVariables();
        if(pState.dashing) return;
        if(pState.alive)
        {
            if(!isWallJumping)
            {
                Flip();
                Move();
                Jump();
            }
            WallSlide();
            WallJump();
            StartDash();
            Attack();
            Recoil();
        }
    }

    void GetInputs()
    {
        xAxis = Input.GetAxisRaw("Horizontal");
        yAxis = Input.GetAxisRaw("Vertical");
        attack = Input.GetMouseButtonDown(0);
    }

    void Flip()
    {
        if(xAxis < 0)
        {
            transform.localScale = new Vector2(-1, transform.localScale.y);
            pState.lookingRight = false;

        }
        else if (xAxis > 0)
        {
            transform.localScale = new Vector2(1, transform.localScale.y);
            pState.lookingRight = true;
        }
    }

    private void Move() 
    {
        rb.linearVelocity = new Vector2(walkSpeed * xAxis, rb.linearVelocity.y);
        anim.SetBool("running", rb.linearVelocity.x != 0 && Grounded());
    }

    void StartDash()
    {
        if(Input.GetButtonDown("Dash") && canDash && !dashed && unlockedDash)
        {
            StartCoroutine(Dash());
            dashed = true;
        }

        if(Grounded())
        {
            dashed = false;
        }
    }

    IEnumerator Dash()
    {
        canDash = false;
        pState.dashing = true;
        anim.SetTrigger("dashing");
        rb.gravityScale = 0;
        rb.linearVelocity = new Vector2(transform.localScale.x * dashSpeed, 0);
        yield return new WaitForSeconds(dashTime);
        rb.gravityScale = gravity;
        pState.dashing = false;
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }

    void Attack()
    {
        timeSinceAttack += Time.deltaTime;
        if(attack && timeSinceAttack >= timeBetweenAttack)
        {
            timeSinceAttack = 0;
            anim.SetTrigger("attacking");
            Debug.Log("Attack!");

            if(yAxis == 0 || yAxis < 0 && Grounded())
            {
                Hit(sideAttackTransform, sideAttackArea, ref pState.recoilX, recoilXSpeed);
                Instantiate(slashEffect, sideAttackTransform);
            }
            else if(yAxis > 0)
            {
                Hit(upAttackTransform, upAttackArea, ref pState.recoilY, recoilYSpeed);
                SlashEffectAngle(slashEffect, 90, upAttackTransform);
            }
            else if(yAxis < 0 && !Grounded())
            {
                Hit(downAttackTransform, downAttackArea, ref pState.recoilY, recoilYSpeed);
                SlashEffectAngle(slashEffect, -90, downAttackTransform);
            }
        }
    }

    private void Hit(Transform _attackTransform, Vector2 _attackArea, ref bool recoilDir, float recoilStrength)
    {
        Collider2D[] objectsToHit = Physics2D.OverlapBoxAll(_attackTransform.position, _attackArea, 0, attackableLayer);
        List<Enemy>hitEnemies = new List<Enemy>();

        if(objectsToHit.Length > 0)
        {
            recoilDir = true;
        }
        for(int i = 0; i < objectsToHit.Length; i++)
        {
            Enemy e = objectsToHit[i].GetComponent<Enemy>();
            if(e && !hitEnemies.Contains(e))
            {
                e.EnemyHit(damage, (transform.position - objectsToHit[i].transform.position).normalized, recoilStrength);
                hitEnemies.Add(e);
            }
        }
    }

    void SlashEffectAngle(GameObject slashEffect, int effectAngle, Transform attackTransform)
    {
        slashEffect = Instantiate(slashEffect, attackTransform);
        slashEffect.transform.eulerAngles = new Vector3(0,0, effectAngle);
        slashEffect.transform.localScale = new Vector2(transform.localScale.x, transform.localScale.y);
    }

    void Recoil()
    {
        if(pState.recoilX)
        {
            if(pState.lookingRight)
            {
                rb.linearVelocity = new Vector2(-recoilXSpeed, 0);
            }
            else
            {
                rb.linearVelocity = new Vector2(recoilXSpeed, 0);
            }
        }
        
        if (pState.recoilY)
        {                
            rb.gravityScale = 0;
            if(yAxis < 0)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocityX, recoilYSpeed);
            }
            else
            {
                rb.linearVelocity = new Vector2(rb.linearVelocityX, -recoilYSpeed);
            }
        }
        else
        {
            rb.gravityScale = gravity;
        }

        if(pState.recoilX && stepsXRecoil < recoilXSteps)
        {
            stepsXRecoil++;
        }
        else
        {
            StopRecoilX();
        }

        if(pState.recoilY && stepsYRecoil < recoilYSteps)
        {
            stepsYRecoil++;
        }
        else
        {
            StopRecoilY();
        }
        
        if(Grounded())
        {
            StopRecoilY();
        }
    }

    void StopRecoilX()
    {
        stepsXRecoil = 0;
        pState.recoilX = false;
    }
    void StopRecoilY()
    {
        stepsYRecoil = 0;
        pState.recoilY = false;
    }
    public void TakeDamage(float damage)
    {
        if(pState.alive)
        {
            Health -= Mathf.RoundToInt(damage);
            if(Health <= 0)
            {
                Health = 0;
                StartCoroutine(Death());
            }
            else
            {
                StartCoroutine(StopDamage());
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Hazard"))
        {
            Health = 0;
            StartCoroutine(Death());
        }
    }

    IEnumerator StopDamage()
    {
        pState.invincible = true;
        anim.SetTrigger("takeDamage");
        yield return new WaitForSeconds(1f);
        pState.invincible = false;
    }

    public int Health
    {
        get{return health;}
        set
        {
            if(health != value)
            {
                health = Mathf.Clamp(value, 0, maxHealth);

                if(onHealthChangedCallback != null)
                {
                    onHealthChangedCallback.Invoke();
                }
            }
        }
    }

    IEnumerator Death()
    {
        myCanvas.SetActive(false);
        pState.alive = false;
        Time.timeScale = 1f;
        anim.SetTrigger("Death");

        yield return new WaitForSeconds(0.9f);
        StartCoroutine(UIManager.Instance.ActiveDeathScreen());
    }

    public void Respawned()
    {
        if(!pState.alive)
        {
            myCanvas.SetActive(true);
            pState.alive = true;
            Health = maxHealth;
            anim.Play("HeroKnight_Idle");
        }
    }

    public bool Grounded()
    {
        if(Physics2D.Raycast(groundCheckpoint.position, Vector2.down, groundCheckY, whatIsGround)
            || Physics2D.Raycast(groundCheckpoint.position + new Vector3(groundCheckX, 0, 0), Vector2.down, groundCheckY, whatIsGround)
            || Physics2D.Raycast(groundCheckpoint.position + new Vector3(-groundCheckX, 0, 0), Vector2.down, groundCheckY, whatIsGround))
        {
            return true;
        }
        else 
        {
            return false;
        }
    }
    void Jump()
    {
        if (Input.GetButtonUp("Jump") && rb.linearVelocity.y > 0)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0);
            pState.jumping = false;
        }

        if(!pState.jumping)
        {
            if(jumpBufferCounter > 0 && coyoteTimeCounter > 0)
            {
                rb.linearVelocity = new Vector3(rb.linearVelocity.x, jumpForce);
                pState.jumping = true;
            }
            else if(!Grounded() && airJumpCounter < maxAirJump && Input.GetButtonDown("Jump") && unlockedDoubleJump)
            {
                pState.jumping = true;
                airJumpCounter++;
                rb.linearVelocity = new Vector3(rb.linearVelocity.x, jumpForce);
            }
        }
        anim.SetBool("jumping", !Grounded());
    }

    void UpdateJumpVariables()
    {
        if (Grounded())
        {
            pState.jumping = false;
            coyoteTimeCounter = coyoteTime;
            airJumpCounter = 0;
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        }

        if (Input.GetButtonDown("Jump"))
        {
            jumpBufferCounter = jumpBufferFrames;
        }
        else 
        {
            jumpBufferCounter = jumpBufferCounter - Time.deltaTime * 10;
        }
    }

    private bool Walled()
    {
        return Physics2D.OverlapCircle(wallCheck.position, 0.2f, wallLayer);
    }
    void WallSlide()
    {
        if(Walled() && !Grounded() && xAxis != 0 && unlockedWallJump)
        {
            isWallSliding = true;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, Mathf.Clamp(rb.linearVelocity.y, -wallSlideSpeed, float.MaxValue));
        }
        else
        {
            isWallSliding = false;
        }
    }

    void WallJump()
    {
        if(isWallSliding)
        {
            isWallJumping = false;
            wallJumpDirection = !pState.lookingRight ? 1 : -1;

            CancelInvoke(nameof(StopWallJump));
        }
        
        if(Input.GetButtonDown("Jump") && isWallSliding)
        {
            isWallJumping = true;
            rb.linearVelocity = new Vector2(wallJumpDirection * wallJumpPower.x, wallJumpPower.y);

            dashed = false;
            airJumpCounter = 0;

            pState.lookingRight = !pState.lookingRight;
            transform.eulerAngles = new Vector2(transform.eulerAngles.x, 180);

            Invoke(nameof(StopWallJump), wallJumpDuration);
        }
    }

    void StopWallJump()
    {
        isWallJumping = false;
        transform.eulerAngles = new Vector2(transform.eulerAngles.x, 0);
    }

    public void GainHealth(int val)
    {
       
            Health += val;
    }

    public void GainStrength(int val)
    {
        damage += val;
    }
}


