using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Reflection;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float walkSpeed = 1;
    [SerializeField] private float jumpForce = 45f;
    private float jumpBufferCounter = 0;
    [SerializeField] private float jumpBufferFrames;
    private float coyoteTimeCounter = 0;
    [SerializeField] private float coyoteTime;
    [SerializeField] private Transform groundCheckpoint;
    [SerializeField] private float groundCheckY = 0.2f; 
    [SerializeField] private float groundCheckX = 0.5f; 
    [SerializeField] private LayerMask whatIsGround;

    [SerializeField] int recoilXSteps = 5;
    [SerializeField] int recoilYSteps = 5;
    [SerializeField] float recoilXSpeed = 100;
    [SerializeField] float recoilYSpeed = 100;
    int stepsXRecoil, stepsYRecoil;

    public int health;
    public int maxHealth;
    [HideInInspector] public PlayerStateList pState;
    private Rigidbody2D rb;
    private float xAxis, yAxis;
    Animator anim; 
    bool attack = false;
    [SerializeField] private float timeBetweenAttack; 
    private float timeSinceAttack;
    private float gravity;
    private int airJumpCounter;

    [SerializeField] Transform sideAttackTransform, upAttackTransform, downAttackTransform;
    [SerializeField] Vector2 sideAttackArea, upAttackArea, downAttackArea;
    [SerializeField] LayerMask attackableLayer;
    [SerializeField] float damage;
    [SerializeField] GameObject slashEffect; 
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
        health = maxHealth;
    }

    void Start()
    {
        pState = GetComponent<PlayerStateList>();
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent <Animator>();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(sideAttackTransform.position, sideAttackArea);
        Gizmos.DrawWireCube(upAttackTransform.position, upAttackArea);
        Gizmos.DrawWireCube(downAttackTransform.position, downAttackArea);

    }

    // Update is called once per frame
    void Update()
    {
        GetInputs();
        UpdateJumpVariables();
        Flip();
        Move();
        Jump();
        Attack();
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

    private void Hit(Transform _attackTransform, Vector2 _attackArea, ref bool recoilDir, float recoilsStrength)
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
                e.EnemyHit(damage, (transform.position - objectsToHit[i].transform.position).normalized, recoilsStrength);
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
            airJumpCounter = 0;
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
        health -= Mathf.RoundToInt(damage);
        StartCoroutine(StopDamage());
    }

    IEnumerator StopDamage()
    {
        pState.invincible = true;
        anim.SetTrigger("takeDamage");
        ClampHealth();
        yield return new WaitForSeconds(1f);
        pState.invincible = false;
    }

    void ClampHealth()
    {
        health = Mathf.Clamp(health, 0, maxHealth);
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
        }
        anim.SetBool("jumping", !Grounded());
    }

    void UpdateJumpVariables()
    {
        if (Grounded())
        {
            pState.jumping = false;
            coyoteTimeCounter = coyoteTime;
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
}
