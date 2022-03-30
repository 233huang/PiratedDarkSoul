using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActorController : MonoBehaviour
{
    public GameObject model;
    private PlayerInput pi;
    public float walkspeed = 2.5f;
    public float runMultiplier = 2f;
    public float jumpVelocity = 3.0f;
    public float rollVelocity = 3.0f;
    public float jabMultiplier = 1.0f;

    [Space(10)]
    [Header("==== Friction Settings ====")]
    public PhysicMaterial DefaultFriction;
    public PhysicMaterial ZeroFriction;

    private Animator anim;
    private Rigidbody rigid;
    private Vector3 planarVec;
    private Vector3 thrustVec;
    private bool canAttack;
    private bool lockPlaner = false;
    private CapsuleCollider col;
    private float lerpTarget;

    void Awake()
    {
        if(model == null)
            model = this.transform.GetChild(0).gameObject;
        
        pi = this.GetComponent<PlayerInput>();
        anim = model.GetComponent<Animator>();
        rigid = GetComponent<Rigidbody>();
        col = GetComponent<CapsuleCollider>();
    }

    // Update is called once per frame
    void Update()
    {
        float targetRunMulti = ((pi.run) ? 2.0f : 1.0f);
        anim.SetFloat("forward", pi.Dmag * Mathf.Lerp(anim.GetFloat("forward"),targetRunMulti,0.5f));
        
        if(rigid.velocity.magnitude > 1.0f)
        {
            anim.SetTrigger("roll");
        } 
        if (pi.jump)
        {
            anim.SetTrigger("jump");
            canAttack = false;
        }
        if (pi.attack && CheckState("Ground") && canAttack)
        {
            anim.SetTrigger("attack");
        }

        if (pi.Dmag > 0.1f)
        {
            model.transform.forward = Vector3.Slerp(model.transform.forward, pi.Dvec, 0.5f);
        }
        if(lockPlaner == false)
        {
            planarVec = pi.Dmag * model.transform.forward * walkspeed * ((pi.run) ? runMultiplier : 1.0f);
        }
    }

    private void FixedUpdate()
    {
        //rigid.position += planarVec * Time.fixedDeltaTime * walkspeed;
        rigid.velocity = new Vector3(planarVec.x, rigid.velocity.y, planarVec.z) + thrustVec;
        thrustVec = Vector3.zero;
    }

    private bool CheckState(string stateName,string layerName = "Base Layer")
    {
        return anim.GetCurrentAnimatorStateInfo(anim.GetLayerIndex(layerName)).IsName(stateName);
    }

    #region OnAnimFuntion

    public void OnJumpEnter()
    {
        thrustVec = new Vector3(0, jumpVelocity, 0);
        pi.inputEnable = false;
        lockPlaner = true;
    }

    /*public void OnJumpExit()
    {
        pi.inputEnable = true;
        lockPlaner = false;
    }*/

    public void IsGround()
    {
        anim.SetBool("isground", true);
    }

    public void IsNotGround()
    {
        anim.SetBool("isground", false);
    }

    public void OnGroundEnter()
    {
        pi.inputEnable = true;
        lockPlaner = false;
        canAttack = true;
        col.material = DefaultFriction;
    }

    public void OnGroundExit()
    {
        col.material = ZeroFriction;
    }

    public void OnFallEnter()
    {
        pi.inputEnable = false;
        lockPlaner = true;
    }

    public void OnRollEnter()
    {
        thrustVec = new Vector3(0, rollVelocity, 0);
        pi.inputEnable = false;
        lockPlaner = true;
    }

    public void OnJabEnter()
    {
        pi.inputEnable = false;
        lockPlaner = true;
    }

    public void OnJabUpdate()
    {
        thrustVec = model.transform.forward * anim.GetFloat("jabVelocity") * jabMultiplier;
    }

    public void OnAttack1hAEnter()
    {
        pi.inputEnable = false;
        lerpTarget = 1.0f;
    }
    public void OnAttack1hAUpdate()
    {
        thrustVec = model.transform.forward * anim.GetFloat("attack1hAVelocity");
        anim.SetLayerWeight(1, Mathf.Lerp(anim.GetLayerWeight(1), lerpTarget, 0.1f));
    }

    public void OnAttackIdleEnter()
    {
        pi.inputEnable = true;
        lerpTarget = 0f;
    }

    public void OnAttackIdleUpdate()
    {
        anim.SetLayerWeight(1, Mathf.Lerp(anim.GetLayerWeight(1), lerpTarget, 0.1f));
    }
    #endregion
}
