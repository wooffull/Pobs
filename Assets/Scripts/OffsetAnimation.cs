using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OffsetAnimation : MonoBehaviour
{
    public float animationOffset;
    public string startingAnimationName = "Default Take";
    public string startingAnimationLayer = "Base Layer";

    private Animator animator;

    // Use this for initialization
    void Awake()
    {
        animator = GetComponent<Animator>();

        int layerIndex = animator.GetLayerIndex(startingAnimationLayer);
        AnimatorClipInfo[] clipInfo = animator.GetCurrentAnimatorClipInfo(layerIndex);
        
        AnimationClip clip = clipInfo[0].clip;
        animator.Play(clip.name, layerIndex, animationOffset);
    }
	
	// Update is called once per frame
	void Update () {
	}
}
