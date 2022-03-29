using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActorController : MonoBehaviour
{
    public GameObject model;
    public float walkspeed = 1.4f;
    public float runMultiplier = 2.5f;

    private PlayerInput pi;

    private Animator anim;
    private Rigidbody rigid;
    private Vector3 planarVec;

    private bool lockPlaner = false;

    void Awake()
    {
        if(model == null)
            model = this.transform.GetChild(0).gameObject;
        
        pi = this.GetComponent<PlayerInput>();
        anim = model.GetComponent<Animator>();
        rigid = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        float targetRunMulti = ((pi.run) ? 2.0f : 1.0f);
        anim.SetFloat("forward", pi.Dmag * Mathf.Lerp(anim.GetFloat("forward"),targetRunMulti,0.5f));
        if (pi.jump)
        {
            anim.SetTrigger("jump");
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
        rigid.velocity = new Vector3(planarVec.x, rigid.velocity.y, planarVec.z);
    }

    public void OnJumpEnter()
    {
        pi.inputEnable = false;
        lockPlaner = true;
    }

    public void OnJumpExit()
    {
        pi.inputEnable = true;
        lockPlaner = false;
    }
}
