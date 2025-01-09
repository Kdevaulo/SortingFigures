using UnityEngine;

namespace Kdevaulo.SortingFigures
{
    [CreateAssetMenu(fileName = nameof(GameplaySettings),
        menuName = nameof(SortingFigures) + "/" + nameof(GameplaySettings))]
    public class GameplaySettings : ScriptableObject
    {
        [field: Header("Shuffle")]
        [field: Min(0)]
        [field: SerializeField] public float ShuffleGlobalDelay { get; private set; } = 1f;
        [field: Min(0)]
        [field: SerializeField] public float ShuffleMovementDuration { get; private set; } = 2f;

        [field: SerializeField] public Vector2 ShuffleDelayRange { get; private set; }

        [field: Header("Shadow")]
        [field: Min(0)]
        [field: SerializeField] public float ShadowFadeDuration { get; private set; } = 0.5f;

        [field: Header("Drag")]
        [field: Min(0)]
        [field: SerializeField] public float ScaleDuration { get; private set; } = 0.1f;
        [field: Min(0)]
        [field: SerializeField] public float FollowMovementDuration { get; private set; } = 0.3f;
        [field: Min(0)]
        [field: SerializeField] public float ScalePercentage { get; private set; } = 0.01f;
        [field: Min(0)]
        [field: SerializeField] public float MaxDistanceToSwap { get; private set; } = 0.4f;

        [field: Header("CorrectAction")]
        [field: Min(0)]
        [field: SerializeField] public float CorrectActionMoveDuration { get; private set; } = 0.5f;

        [field: Header("WrongAction")]
        [field: Min(0)]
        [field: SerializeField] public float WrongActionMoveDuration { get; private set; } = 0.5f;
        [field: Min(0)]
        [field: SerializeField] public float WrongActionRotateDuration { get; private set; } = 0.5f;
        [field: SerializeField] public Vector3[] WrongActionRotations { get; private set; }
    }
}