using System.Threading;

using Cysharp.Threading.Tasks;

using UnityEngine;

using VContainer;

namespace Kdevaulo.SortingFigures.DraggableItemsBehaviour
{
    public class Animator
    {
        private readonly float ScaleDuration;
        private readonly float ScalePercentage;
        private readonly float ShadowFadeDuration;
        private readonly float FollowMovingDuration;
        private readonly float WrongActionMoveDuration;
        private readonly float CorrectActionMoveDuration;
        private readonly float WrongActionRotateDuration;

        private readonly Vector3[] WrongActionRotations;

        [Inject] private CancellationTokenSource _cts;

        public Animator(GameplaySettings settings)
        {
            ScaleDuration = settings.ScaleDuration;
            ScalePercentage = settings.ScalePercentage;
            ShadowFadeDuration = settings.ShadowFadeDuration;
            WrongActionRotations = settings.WrongActionRotations;
            FollowMovingDuration = settings.FollowMovementDuration;
            WrongActionMoveDuration = settings.WrongActionMoveDuration;
            WrongActionRotateDuration = settings.WrongActionRotateDuration;
            CorrectActionMoveDuration = settings.CorrectActionMoveDuration;
        }

        public void AnimateDragStart(DraggableItem item)
        {
            item.SetMovingOrder();
            item.FadeOutShadowAsync(ShadowFadeDuration, _cts.Token).Forget();
        }

        public void AnimateDragging(DraggableItem item, Vector2 targetPosition)
        {
            item.SmoothMoveToPositionAsync(targetPosition, FollowMovingDuration, _cts.Token).Forget();
        }

        public void AnimatePointerDown(DraggableItem item)
        {
            item.ScaleAsync(ScalePercentage, ScaleDuration, _cts.Token).Forget();
        }

        public void AnimateWrongAction(DraggableItem item, Vector3 position)
        {
            AnimateWrongActionAsync(item, position, _cts.Token).Forget();
        }

        private async UniTask AnimateWrongActionAsync(DraggableItem item, Vector3 position, CancellationToken token)
        {
            await item.HandleWrongAnimationAsync(WrongActionRotateDuration, WrongActionRotations, token);

            await item.SmoothMoveToPositionAsync(position, WrongActionMoveDuration,
                token);

            item.SetNormalOrder();

            await item.FadeInShadowAsync(ShadowFadeDuration, token);

            item.EnableInteractions();
        }

        public void AnimateCorrectAction(DraggableItem droppedItem, DraggableItem closestItem, ItemData droppedItemData,
            ItemData closestItemData)
        {
            var closestItemPosition = droppedItemData.CurrentCell.Position;
            var droppedItemPosition = closestItemData.CurrentCell.Position;
            AnimateCorrectActionAsync(droppedItem, closestItem, closestItemPosition, droppedItemPosition, _cts.Token)
                .Forget();
        }

        private async UniTask AnimateCorrectActionAsync(DraggableItem droppedItem, DraggableItem closestItem,
            Vector3 closestItemPosition, Vector3 droppedItemPosition, CancellationToken token)
        {
            var closestItemOperation =
                closestItem.MoveToPositionWithFadeAsync(0, CorrectActionMoveDuration, ShadowFadeDuration,
                    closestItemPosition, token);

            var droppedItemOperation = MoveToPositionAsync(droppedItem, droppedItemPosition, token);

            await droppedItemOperation;
            await closestItemOperation;

            closestItem.SetInstalledSortingOrder();
        }

        private async UniTask MoveToPositionAsync(DraggableItem item, Vector3 position, CancellationToken token)
        {
            await item.SmoothMoveToPositionAsync(position, CorrectActionMoveDuration, token);

            item.SetInstalledSortingOrder();

            await item.FadeInShadowAsync(ShadowFadeDuration, token);
        }
    }
}