using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class EnemyController : KiwiController
{
    private bool waitStep;

    public Direction OnLeftPress = Direction.RIGHT, 
                     OnRightPress = Direction.LEFT, 
                     OnUpPress = Direction.DOWN, 
                     OnDownPress = Direction.UP;

    public Dictionary<Direction, Vector3> directionMap = new Dictionary<Direction, Vector3>();

    public void Start()
    {
        directionMap.Add(Direction.RIGHT, Vector3.right);
        directionMap.Add(Direction.LEFT, Vector3.left);
        directionMap.Add(Direction.UP, Vector3.up);
        directionMap.Add(Direction.DOWN, Vector3.down);

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
                if (horizontalMovement > 0)
                {
                    // player is moving Right
                    direction = directionMap[OnRightPress];
                }
                else
                {
                    direction = directionMap[OnLeftPress];
                }

                receivedValidInput = true;
            }

            // Moving Vertically, not both
            if (Mathf.Abs(verticalMovement) > .1f && !(Mathf.Abs(horizontalMovement) > .1f))
            {
                if (verticalMovement > 0)
                {
                    // player moving Up
                    direction = directionMap[OnUpPress];
                }
                else
                {
                    // player moving Down
                    direction = directionMap[OnDownPress];
                }
                receivedValidInput = true;
            }

            if (receivedValidInput && Mathf.Abs(direction.x) > .1)
            {
                GetComponent<SpriteRenderer>().flipX = direction.x > 0;
            }

            if (Input.GetKeyDown(KeyCode.Return))
            {
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
                    if (!waitStep)
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

    public override void CollideAction()
    {
        StartCoroutine(WaitForInteract());
    }

    public override void DeathEffect()
    {
        StartCoroutine(LevelMenus.Singleton.ShowVictoryMenu());
    }

    public enum Direction
    {
        RIGHT = 1,
        LEFT = 2,
        UP = 3,
        DOWN = 4
    }
}
