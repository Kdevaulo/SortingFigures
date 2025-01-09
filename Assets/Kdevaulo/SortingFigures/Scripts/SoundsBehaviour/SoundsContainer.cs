using UnityEngine;

namespace Kdevaulo.SortingFigures.SoundsBehaviour
{
    [CreateAssetMenu(fileName = nameof(SoundsContainer),
        menuName = nameof(SoundsBehaviour) + "/" + nameof(SoundsContainer))]
    public class SoundsContainer : ScriptableObject
    {
        [field: SerializeField] public ClipByType[] Clips { get; private set; }
    }
}