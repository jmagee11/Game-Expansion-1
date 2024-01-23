using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : KiwiController
{
    [SerializeField] bool safeMode;
    [SerializeField] bool waitStep;

    public void Start()
    {
        safeMode = false;
        waitStep = true;
    }


    private void Update()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        horizontalMovement = Input.acceleration.x / 2;
        verticalMovement = Input.acceleration.y / 2;
#else
        horizontalMovement = Input.GetAxisRaw("Horizontal");

        verticalMovement = Input.GetAxisRaw("Vertical");
#endif
        if (!isPerformingAction && !IsDefeated)
        {
            // Moving Horizontally, not both
            var direction = Vector3.zero;
            var receivedValidInput = false;
            if (Mathf.Abs(horizontalMovement) > .1f && !(Mathf.Abs(verticalMovement) > .1f))
            {
                direction = horizontalMovement * Vector3.right;
                receivedValidInput = true;
            }

            // Moving Vertically, not both
            if (Mathf.Abs(verticalMovement) > .1f && !(Mathf.Abs(horizontalMovement) > .1f))
            {
                direction = verticalMovement * Vector3.up;
                receivedValidInput = true;
            }
                
            if (receivedValidInput && Mathf.Abs(direction.x) > .1)
            {
                GetComponent<SpriteRenderer>().flipX = direction.x > 0;
            }

            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                Debug.Log("AAA");
                waitStep = false;
            }

            var animator = GetComponent<Animator>();
            animator.SetFloat("HorizontalMovement", direction.x);
            animator.SetFloat("VerticalMovement", direction.y);
            animator.SetBool("IsInteracting", false); // Set this in TryMoveOrInteract()
            if (receivedValidInput)
            {
                if (safeMode)
                {
                    if (waitStep)
                    {
                        if (Input.GetKeyDown(KeyCode.Mouse0))
                        {
                            Debug.Log("AAA");
                            waitStep = false;
                        }
                    }
                    else
                    {
                        TryMoveOrInteract(direction);
                        waitStep = true;
                    }
                }
                else
                {
                    TryMoveOrInteract(direction);
                }
            }
                
        }
    }

    public override void DeathEffect()
    {
        
        GetComponent<AudioSource>().Play();
        StartCoroutine(LevelMenus.Singleton.ShowDeathMenu());
    }

    public override void CollideAction()
    {
        DefeatCharacter();
    }
}
