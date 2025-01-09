using System.Collections.Generic;
using System.Threading;

using Kdevaulo.SortingFigures.SoundsBehaviour;

using UnityEngine;

using VContainer;

namespace Kdevaulo.SortingFigures.DraggableItemsBehaviour
{
    public class UserActionsHandler
    {
        private readonly float SqrMaxDistanceToSwap;

        [Inject] private Animator _animator;
        [Inject] private AudioService _audioService;
        [Inject] private CameraUtilities _cameraUtilities;

        [Inject] private CancellationTokenSource _cts;

        private Dictionary<DraggableItem, ItemData> _itemsWithCells;

        public UserActionsHandler(GameplaySettings gameplaySettings)
        {
            SqrMaxDistanceToSwap = gameplaySettings.MaxDistanceToSwap * gameplaySettings.MaxDistanceToSwap;
        }

        public void SetItems(Dictionary<DraggableItem, ItemData> itemsWithPositions)
        {
            _itemsWithCells = itemsWithPositions;

            foreach (var pair in itemsWithPositions)
            {
                var item = pair.Key;

                item.Dropped += position => HandleDropped(item, position);
                item.Dragging += position => HandleDragging(item, position);
                item.DragStarted += () => HandleDragStarted(item);
                item.PointerDown += () => HandlePointerDown(item);

                item.EnableInteractions();
            }
        }

        private void HandleDragging(DraggableItem item, Vector2 position)
        {
            var targetPosition = _cameraUtilities.ScreenToWorldPoint(position);
            _animator.AnimateDragging(item, targetPosition);
        }

        private void HandleDragStarted(DraggableItem item)
        {
            _animator.AnimateDragStart(item);
        }

        private void HandleDropped(DraggableItem droppedItem, Vector2 position)
        {
            droppedItem.DisableInteractions();

            var targetPosition = _cameraUtilities.ScreenToWorldPoint(position);

            var closestItem = _itemsWithCells.TryFindClosePosition(targetPosition, SqrMaxDistanceToSwap);

            var droppedItemData = _itemsWithCells[droppedItem];

            if (closestItem != null)
            {
                var closestItemData = _itemsWithCells[closestItem];

                if (droppedItemData.FigureType == closestItemData.CurrentCell.FigureType)
                {
                    _audioService.PlayOneShot(Sound.Correct);

                    HandleCorrectAction(droppedItem, closestItem, droppedItemData, closestItemData);
                }
                else
                {
                    HandleWrongAction(droppedItem, droppedItemData);
                }
            }
            else
            {
                HandleWrongAction(droppedItem, droppedItemData);
            }
        }

        private void HandlePointerDown(DraggableItem item)
        {
            _audioService.PlayOneShot(Sound.Press);

            _animator.AnimatePointerDown(item);
        }

        private void HandleWrongAction(DraggableItem item, ItemData itemData)
        {
            _audioService.PlayOneShot(Sound.Wrong);

            _animator.AnimateWrongAction(item, itemData.CurrentCell.Position);
        }

        private void HandleCorrectAction(DraggableItem droppedItem, DraggableItem closestItem, ItemData droppedItemData,
            ItemData closestItemData)
        {
            _animator.AnimateCorrectAction(droppedItem, closestItem, droppedItemData, closestItemData);

            TryReleaseCells(droppedItem, closestItem, droppedItemData, closestItemData);

            if (_itemsWithCells.Count == 0)
            {
                _audioService.PlayOneShot(Sound.Final);
            }
        }

        private void TryReleaseCells(DraggableItem droppedItem, DraggableItem closestItem, ItemData droppedItemData,
            ItemData closestItemData)
        {
            (droppedItemData.CurrentCell, closestItemData.CurrentCell) =
                (closestItemData.CurrentCell, droppedItemData.CurrentCell);

            _itemsWithCells.Remove(droppedItem);
            droppedItem.ClearSubscribes();

            if (closestItemData.FigureType == closestItemData.CurrentCell.FigureType)
            {
                _itemsWithCells.Remove(closestItem);
                closestItem.ClearSubscribes();
            }
        }
    }
}