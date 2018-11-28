using UnityEngine;
using UnityEngine.Assertions;

public class PlayerAnimationController : MonoBehaviour{

    #region OnGUI
    public Rect GetRectPos(int raw, int column, float _width = 0, float _height = 0) {
        return new Rect(_width * raw, _height * column, _width, _height);
    }

    void OnGUI() {
        if (GUI.Button(GetRectPos(5, 1, 200, 50), "Run")) {
            Run();
        }

        if (GUI.Button(GetRectPos(5, 2, 200, 50), "Aiming")) {
            Aiming();
        }
    }

    #endregion

    public Animator animator;
    const int countOfDamageAnimations = 3;
    int lastDamageAnimation = -1;
    void Awake() {
        Assert.IsNotNull(this.animator);
    }

    public void Stay() {
        animator.SetBool("Aiming", false);
        animator.SetFloat("Speed", 0f);
    }

    public void Walk() {
        animator.SetBool("Aiming", false);
        animator.SetFloat("Speed", 0.5f);
    }

    public void Run() {
        animator.SetBool("Aiming", false);
        animator.SetFloat("Speed", 1f);
    }

    public void Attack() {
        Aiming();
        animator.SetTrigger("Attack");
    }

    public void Death() {
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Death"))
            animator.Play("Idle", 0);
        else
            animator.SetTrigger("Death");
    }

    public void Damage() {
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Death")) return;
        int id = Random.Range(0, countOfDamageAnimations);
        if (countOfDamageAnimations > 1)
            while (id == lastDamageAnimation)
                id = Random.Range(0, countOfDamageAnimations);
        lastDamageAnimation = id;
        animator.SetInteger("DamageID", id);
        animator.SetTrigger("Damage");
    }

    public void Jump() {
        animator.SetBool("Squat", false);
        animator.SetFloat("Speed", 0f);
        animator.SetBool("Aiming", false);
        animator.SetTrigger("Jump");
    }

    public void Aiming() {
        animator.SetBool("Squat", false);
        //animator.SetFloat("Speed", 0f);
        animator.SetBool("Aiming", true);
    }

    public void Sitting() {
        animator.SetBool("Squat", !animator.GetBool("Squat"));
        animator.SetBool("Aiming", false);
    }
}
