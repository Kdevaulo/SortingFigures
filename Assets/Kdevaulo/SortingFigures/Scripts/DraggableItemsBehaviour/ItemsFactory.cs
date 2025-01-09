using UnityEngine;

using VContainer;

namespace Kdevaulo.SortingFigures.DraggableItemsBehaviour
{
    public class ItemsFactory
    {
        [Inject] private DraggableItem _prefab;
        [Inject] private Transform _itemsContainer;

        public DraggableItem[] Create(ItemSettingsStructure settingsStructure)
        {
            var items = new DraggableItem[settingsStructure.Count];

            for (int i = 0; i < settingsStructure.Count; i++)
            {
                var item = Object.Instantiate(_prefab, _itemsContainer);

                item.SetSprite(settingsStructure.Sprite);
                item.SetShadowSprite(settingsStructure.ShadowSprite);
                item.SetShadowOffset(settingsStructure.ShadowOffset);

                items[i] = item;
            }

            return items;
        }
    }
}