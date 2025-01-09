using System.Collections.Generic;

using UnityEngine;

using Random = System.Random;

namespace Kdevaulo.SortingFigures
{
    public static class ArrayExtensions
    {
        private static readonly Random Random = new Random();

        public static void Shuffle<T>(this List<T> items)
        {
            int n = items.Count;

            while (n > 1)
            {
                --n;
                int i = Random.Next(n + 1);
                (items[i], items[n]) = (items[n], items[i]);
            }
        }

        public static T TryFindClosePosition<T>(this Dictionary<T, ItemData> dictionary,
            Vector3 position, float sqrMaxDistance) where T : MonoBehaviour
        {
            foreach (var pair in dictionary)
            {
                float sqrDistance = (position - pair.Value.CurrentCell.Position).sqrMagnitude;

                if (sqrDistance < sqrMaxDistance)
                {
                    return pair.Key;
                }
            }

            return null;
        }
    }
}