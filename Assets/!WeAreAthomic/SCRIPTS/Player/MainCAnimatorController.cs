using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCAnimatorController : MonoBehaviour
{
    private Animator _anim;

    [SerializeField] private string attackCount = "attackCount";
    [SerializeField] private string moveSpeed = "moveSpeed";
    [SerializeField] private string isCrouch = "isCrouch";
    [SerializeField] private string isCrouchWalking = "isCrouchWalking";
    [SerializeField] private string isFalling = "isFalling";
    [SerializeField] private string isJumping = "isJumping";
    [SerializeField] private string isSliding = "isSliding";
    [SerializeField] private string pistolMoveX = "pistolMoveX";
    [SerializeField] private string pistolMoveY = "pistolMoveY";
    [SerializeField] private string hack = "hack";

    protected void Awake()
    {
        _anim = GetComponent<Animator>();
    }

    public void SetCrouch(bool condition)
    {
        _anim.SetBool(isCrouch, condition);
    }

    public void SetCrouchWalking(bool condition)
    {
        _anim.SetBool(isCrouchWalking, condition);
    }

    public void SetFalling(bool condition)
    {
        _anim.SetBool(isCrouchWalking, condition);
    }

    public void SetJumping(bool condition)
    {
        _anim.SetBool(isJumping, condition);
    }

    public void SetSliding(bool condition)
    {
        _anim.SetBool(isSliding, condition);
    }

    public void SetPistolMoveX(float value)
    {
        _anim.SetFloat(pistolMoveX, value);
    }

    public void SetPistolMoveY(float value)
    {
        _anim.SetFloat(pistolMoveY, value);
    }

}
