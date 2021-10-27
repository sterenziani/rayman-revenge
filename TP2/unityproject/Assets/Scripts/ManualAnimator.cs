using UnityEngine;

[RequireComponent(typeof(Animator))]
public class ManualAnimator : MonoBehaviour
{
    private Animator animator;
    private string currentAnimationName;
    private string abruptAnimationName;
    private string continuousAnimationName;

    // Start is called before the first frame update
    void Start()
    {
        animator = gameObject.GetComponent<Animator>();
    }

    public void PlayContinuous(string animationName)
    {
        continuousAnimationName = animationName;

        if(abruptAnimationName == null)
        {
            currentAnimationName = continuousAnimationName;
            animator.Play(currentAnimationName);
        }
    }

    public void PlayForceContinuous(string animationName)
    {
        CancelInvoke(nameof(RestoreContinuous));
        abruptAnimationName = null;

        PlayContinuous(animationName);
    }

    public void PlayAbrupt(string animationName)
    {
        CancelInvoke(nameof(RestoreContinuous));
        abruptAnimationName = currentAnimationName = animationName;
        animator.Play(currentAnimationName);

        if(continuousAnimationName != null)
        {
            float duration = animator.GetCurrentAnimatorStateInfo(0).length;
            Invoke(nameof(RestoreContinuous), duration);
        }
    }

    public float GetCurrentAnimationTotalDuration()
    {
        if (currentAnimationName == null)
            return 0;

        return animator.GetCurrentAnimatorStateInfo(0).length;
    }

    public float GetCurrentAnimationRemainingDuration()
    {
        if (currentAnimationName == null)
            return float.PositiveInfinity;

        return animator.GetCurrentAnimatorStateInfo(0).length - animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
    }

    private void RestoreContinuous()
    {
        abruptAnimationName = null;
        currentAnimationName = continuousAnimationName;
        if (continuousAnimationName != null)
            animator.Play(currentAnimationName);
    }
}
