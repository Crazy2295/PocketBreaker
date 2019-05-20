using System.Linq;
using UnityEngine;

public static class AnimationUtils
{
    public static float GetDurationOfClip(this Animator parent, string name)
    {
        return parent.runtimeAnimatorController.animationClips.First(a => a.name == name).length;
    }
}