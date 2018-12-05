using UnityEngine;
using UnityEngine.Assertions;

public class PlayerAnimationController : MonoBehaviour {
    #region OnGUI
    //public Rect getrectpos(int raw, int column, float _width = 0, float _height = 0) {
    //    return new Rect(_width * raw, _height * column, _width, _height);
    //}

    //void ongui() {
    //    if (GUI.Button(getrectpos(5, 1, 200, 50), "run")) {
    //        Run();
    //    }

    //    if (GUI.Button(getrectpos(5, 2, 200, 50), "aiming")) {
    //        Aiming();
    //    }
    //}
    #endregion

    public Animator animator;
    const int countOfDamageAnimations = 3;
    public int lastDamageAnimation = -1;
    private bool isLocalPlayer = false;
    private int playerNum = 0;
    private PlayerActionType currentAction = PlayerActionType.Idle;
    void Awake() {
        Assert.IsNotNull(this.animator);
    }

    public void Init(int playerNum){
        this.playerNum = playerNum;
    }

    public void SetLocalPlayer(bool isLocalPlayer) {
        this.isLocalPlayer = isLocalPlayer;
    }

    public PlayerActionType GetCurrentAction() {
        return this.currentAction;
    }

    public void OnAcion(PlayerActionType actionType) {
        if (this.currentAction != actionType) {
            this.currentAction = actionType;
            if (this.isLocalPlayer) {
                TcpSocket.inst.Request.ActionPlayer(this.playerNum, actionType);
            }            
        }
        switch (actionType) {
            case PlayerActionType.Aiming:
                Aiming();
                break;            
            case PlayerActionType.Attack:
                Attack();
                break;
            case PlayerActionType.Damage:
                Damage();
                break;
            case PlayerActionType.Death:
                Death();
                break;
            case PlayerActionType.Idle:
                Stay();
                break;
            case PlayerActionType.Jump:
                Jump();
                break;
            case PlayerActionType.Run:
                Run();
                break;
            case PlayerActionType.Sitting:
                Sitting();
                break;
            case PlayerActionType.Walk:
                Walk();
                break;
        }
    }

    public bool IsAiming() {
        return animator.GetBool("Aiming");
    }

    private void Stay() {
        animator.SetBool("Aiming", false);
        animator.SetFloat("Speed", 0f);
    }

    private void Walk() {
        animator.SetBool("Aiming", false);
        animator.SetFloat("Speed", 0.5f);
    }

    private void Run() {
        animator.SetBool("Aiming", false);
        animator.SetFloat("Speed", 1f);
    }

    private void Attack() {
        Aiming();
        animator.SetTrigger("Attack");
    }

    private void Death() {
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Death"))
            animator.Play("Idle", 0);
        else
            animator.SetTrigger("Death");
    }

    private void Damage() {
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Death")) {
            return;
        }
        int id = Random.Range(0, countOfDamageAnimations);
        if (countOfDamageAnimations > 1) {
            while (id == this.lastDamageAnimation) {
                id = Random.Range(0, countOfDamageAnimations);
            }              
        }            
        this.lastDamageAnimation = id;
        animator.SetInteger("DamageID", id);
        animator.SetTrigger("Damage");
    }

    private void Jump() {
        animator.SetBool("Squat", false);
        animator.SetFloat("Speed", 0f);
        animator.SetBool("Aiming", false);
        animator.SetTrigger("Jump");
    }

    private void Aiming() {
        animator.SetBool("Squat", false);
        animator.SetFloat("Speed", 0f);
        animator.SetBool("Aiming", true);
    }

    private void Sitting() {
        animator.SetBool("Squat", !animator.GetBool("Squat"));
        animator.SetBool("Aiming", false);
    }
}
