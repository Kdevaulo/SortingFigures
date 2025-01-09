using System;

using UnityEngine;

namespace Kdevaulo.SortingFigures
{
    [Serializable]
    public struct ItemSettingsStructure
    {
        public Figure FigureType;

        public Sprite Sprite;
        public Sprite ShadowSprite;

        public int Count;

        public Vector2 ShadowOffset;
    }
}