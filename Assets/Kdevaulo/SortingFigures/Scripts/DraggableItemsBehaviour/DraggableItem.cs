using System;
using System.Threading;

using Cysharp.Threading.Tasks;

using DG.Tweening;

using UnityEngine;
using UnityEngine.EventSystems;

namespace Kdevaulo.SortingFigures.DraggableItemsBehaviour
{
    [AddComponentMenu(nameof(DraggableItem) + " in " + nameof(DraggableItemsBehaviour))]
    public class DraggableItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerUpHandler,
        IPointerDownHandler
    {
        public event Action DragStarted = delegate { };
        public event Action<Vector2> Dropped = delegate { };
        public event Action<Vector2> Dragging = delegate { };
        public event Action PointerDown = delegate { };

        [SerializeField] private Transform _movingContainer;
        [SerializeField] private Transform _shadowContainer;

        [SerializeField] private SpriteRenderer _spriteRenderer;
        [SerializeField] private SpriteRenderer _shadowRenderer;

        [SerializeField] private Collider2D _collider;

        [SerializeField] private int _movementSortingOrder;
        [SerializeField] private int _installedSortingOrder;

        private bool _isMoving;
        private bool _canInteract;

        private int _sortingOrder;

        private float _startShadowIntensity;

        private Tween _movingTween;
        private Tween _scalingTween;

        private Vector3 _startScale;
        private PointerEventData _eventData;

        private void Awake()
        {
            _sortingOrder = _spriteRenderer.sortingOrder;
            _startScale = _movingContainer.localScale;
            _startShadowIntensity = _shadowRenderer.color.a;
        }

        private void OnApplicationPause(bool pauseStatus)
        {
            if (!pauseStatus) return;

            if (!_isMoving) return;

            _isMoving = false;

            Dropped.Invoke(_eventData?.position ?? Vector2.one * 500);

            _eventData = null;
        }

        void IBeginDragHandler.OnBeginDrag(PointerEventData eventData)
        {
            if (!_canInteract || _eventData != eventData)
            {
                return;
            }

            _isMoving = true;
            DragStarted.Invoke();
        }

        void IDragHandler.OnDrag(PointerEventData eventData)
        {
            if (!_canInteract || _eventData != eventData)
            {
                return;
            }

            Dragging.Invoke(eventData.position);
        }

        void IEndDragHandler.OnEndDrag(PointerEventData eventData)
        {
            if (_eventData != eventData)
            {
                return;
            }

            Dropped.Invoke(_eventData.position);

            _isMoving = false;
            _eventData = null;
        }

        void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
        {
            if (_canInteract && _eventData == null)
            {
                _eventData = eventData;
            }
            else
            {
                return;
            }

            PointerDown.Invoke();
        }

        void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
        {
            if (_isMoving || _eventData != eventData)
            {
                return;
            }

            _eventData = null;
        }

        public void SetSprite(Sprite sprite)
        {
            _spriteRenderer.sprite = sprite;
        }

        public void SetShadowSprite(Sprite sprite)
        {
            _shadowRenderer.sprite = sprite;
        }

        public void SetPosition(Vector3 position)
        {
            _movingContainer.position = position;
        }

        public void SetShadowOffset(Vector2 offset)
        {
            _shadowContainer.localPosition = offset;
        }

        public async UniTask MoveToPositionWithFadeAsync(float delay, float duration, float fadeDuration,
            Vector3 position, CancellationToken token)
        {
            if (Vector3.Distance(position, _movingContainer.position) < 0.1f)
            {
                await UniTask.CompletedTask;
            }
            else
            {
                DisableInteractions();
                SetMovingOrder();

                FadeOutShadowAsync(fadeDuration, token).Forget();

                await UniTask.WaitForSeconds(delay, cancellationToken: token);

                await _movingContainer.DOMove(position, duration)
                    .SetEase(Ease.InOutSine)
                    .AwaitForComplete(cancellationToken: token);

                await FadeInShadowAsync(fadeDuration, token);

                SetNormalOrder();
                EnableInteractions();
            }
        }

        public async UniTask SmoothMoveToPositionAsync(Vector2 position, float duration, CancellationToken token)
        {
            // note: It would be better to implement it by following the touch position in a loop.
            // Pass the current position to follow with each call to Dragging.
            // But for now the current implementation is enough.
            _movingTween?.Kill();
            _movingTween = _movingContainer.DOMove(position, duration);
            await _movingTween.AwaitForComplete(cancellationToken: token);
        }

        public async UniTask HandleWrongAnimationAsync(float duration, Vector3[] rotations, CancellationToken token)
        {
            float thirdDuration = duration / 3;

            var sequence = DOTween.Sequence();

            foreach (var vector in rotations)
            {
                sequence.Append(_movingContainer.DORotate(vector, thirdDuration));
            }

            await sequence.AwaitForComplete(cancellationToken: token);
        }

        public async UniTask ScaleAsync(float scalePercentage, float duration, CancellationToken token)
        {
            _scalingTween?.Kill();

            _movingContainer.localScale = _startScale;

            _scalingTween = _movingContainer.DOScale(_startScale + _startScale * scalePercentage, duration);
            await _scalingTween.AwaitForComplete(cancellationToken: token);

            _scalingTween = _movingContainer.DOScale(_startScale, duration);
            await _scalingTween.AwaitForComplete(cancellationToken: token);
        }

        public async UniTask FadeInShadowAsync(float duration, CancellationToken token)
        {
            await _shadowRenderer.DOFade(_startShadowIntensity, duration).AwaitForComplete(cancellationToken: token);
        }

        public async UniTask FadeOutShadowAsync(float duration, CancellationToken token)
        {
            await _shadowRenderer.DOFade(0, duration).AwaitForComplete(cancellationToken: token);
        }

        public void SetMovingOrder()
        {
            _spriteRenderer.sortingOrder = _movementSortingOrder;
        }

        public void SetNormalOrder()
        {
            _spriteRenderer.sortingOrder = _sortingOrder;
        }

        public void SetInstalledSortingOrder()
        {
            _spriteRenderer.sortingOrder = _installedSortingOrder;
        }

        public void ClearSubscribes()
        {
            DragStarted = null;
            Dropped = null;
            Dragging = null;
            PointerDown = null;

            _collider.enabled = false;
        }

        public void DisableInteractions()
        {
            _canInteract = false;
        }

        public void EnableInteractions()
        {
            _canInteract = true;
        }
    }
}