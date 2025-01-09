using UnityEngine;

namespace Kdevaulo.SortingFigures
{
    [CreateAssetMenu(fileName = nameof(ItemsSettings), menuName = nameof(SortingFigures) + "/" + nameof(ItemsSettings))]
    public class ItemsSettings : ScriptableObject
    {
        [field: SerializeField] public ItemSettingsStructure[] ItemsDataCollection { get; private set; }
    }
}