using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Cysharp.Threading.Tasks;

using Kdevaulo.SortingFigures.PositionProviderBehaviour;

using UnityEngine;
using UnityEngine.Assertions;

using VContainer;
using VContainer.Unity;

using Random = UnityEngine.Random;

namespace Kdevaulo.SortingFigures.DraggableItemsBehaviour
{
    public class ItemsBehaviourHandler : IStartable
    {
        [Inject] private ItemsFactory _factory;
        [Inject] private ItemsSettings _itemsSettings;
        [Inject] private GameplaySettings _gameplaySettings;
        [Inject] private UserActionsHandler _actionsHandler;
        [Inject] private PositionsProvider[] _positionsProviders;

        [Inject] private CancellationTokenSource _cts;

        private List<Cell> _cells;

        private Dictionary<DraggableItem, ItemData> _cellsWithItems;

        void IStartable.Start()
        {
            HandleStartAsync(_cts.Token).Forget();
        }

        private async UniTask HandleStartAsync(CancellationToken token)
        {
            var itemDataCollection = _itemsSettings.ItemsDataCollection;

            Assert.IsTrue(_positionsProviders.Length == itemDataCollection.Length);

            _cells = new List<Cell>();
            _cellsWithItems = new Dictionary<DraggableItem, ItemData>();

            SetupItems(itemDataCollection);

            await UniTask.WaitForSeconds(_gameplaySettings.ShuffleGlobalDelay, cancellationToken: token);

            await HandleAppearanceAsync(token);

            _actionsHandler.SetItems(_cellsWithItems);
        }

        private async UniTask HandleAppearanceAsync(CancellationToken token)
        {
            await HandleMovementAsync(token);

            UpdateCells();
            BlockCorrectItems();
        }

        private void UpdateCells()
        {
            int index = 0;

            foreach (var pair in _cellsWithItems)
            {
                pair.Value.CurrentCell = _cells[index++];
            }
        }

        private void BlockCorrectItems()
        {
            var correctItems = new List<DraggableItem>();

            foreach (var pair in _cellsWithItems)
            {
                if (pair.Value.FigureType == pair.Value.CurrentCell.FigureType)
                {
                    correctItems.Add(pair.Key);
                }
            }

            foreach (var item in correctItems)
            {
                item.ClearSubscribes();
                item.SetInstalledSortingOrder();
                _cellsWithItems.Remove(item);
            }
        }

        private async UniTask HandleMovementAsync(CancellationToken token)
        {
            var delayRange = _gameplaySettings.ShuffleDelayRange;
            float fadeDuration = _gameplaySettings.ShadowFadeDuration;
            float movementDuration = _gameplaySettings.ShuffleMovementDuration;

            var operations = new UniTask[_cells.Count];

            int index = 0;

            foreach (var pair in _cellsWithItems)
            {
                float delay = Random.Range(delayRange.x, delayRange.y);

                var cell = _cells[index];

                var operation =
                    pair.Key.MoveToPositionWithFadeAsync(delay, movementDuration, fadeDuration, cell.Position,
                        token);

                operations[index] = operation;

                ++index;
            }

            await UniTask.WhenAll(operations);
        }

        private void PlaceItems()
        {
            foreach (var pair in _cellsWithItems)
            {
                pair.Key.SetPosition(pair.Value.CurrentCell.Position);
            }
        }

        private void SetupItems(ItemSettingsStructure[] itemDataCollection)
        {
            foreach (var currentProvider in _positionsProviders)
            {
                var positionsByType = currentProvider.GetPositions();
                var targetType = positionsByType.FigureType;

                var targetData = itemDataCollection.First(x => x.FigureType == targetType);
                var items = _factory.Create(targetData);

                var positions = positionsByType.Positions;

                Assert.IsTrue(items.Length == positions.Count);

                FillStructures(items, targetType, positions);
            }

            PlaceItems();

            _cells.Shuffle();
        }

        private void FillStructures(DraggableItem[] items, Figure targetType, List<Vector3> positions)
        {
            for (int i = 0; i < items.Length; i++)
            {
                var cell = new Cell()
                {
                    FigureType = targetType,
                    Position = positions[i]
                };

                var data = new ItemData()
                {
                    FigureType = targetType,
                    CurrentCell = cell
                };

                _cellsWithItems.Add(items[i], data);

                _cells.Add(data.CurrentCell);
            }
        }
    }
}