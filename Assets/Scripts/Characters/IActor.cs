using Coffee.Characters;
using UnityEngine;

namespace Coffee
{
    public interface IActor
    {
        Transform Transform { get; }
        AnimationComponent Animator { get; }
    }
}