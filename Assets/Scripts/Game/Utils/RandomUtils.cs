using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using UnityEngine;

namespace Game.Utils
{
    [SuppressMessage("ReSharper", "PossibleMultipleEnumeration")]
    public static class RandomUtils
    {
        public static bool Random(float probability)
        {
            probability /= 100;
            float picked = UnityEngine.Random.value;
            return picked <= probability;
        }

        public static int PickRandomByWeights(IEnumerable<int> weights)
        {
            int totalWeight = weights.Sum();
            float probe = UnityEngine.Random.value;
            float s = 0f;

            int i = 0;
            foreach (int weight in weights) {
                s += (float) weight / totalWeight;
                if (s >= probe) {
                    return i;
                }

                i++;
            }

            return -1;
        }

        public static int PickRandomByWeights(IEnumerable<float> weights, float totalWeight = 100f)
        {
            float probe = UnityEngine.Random.value;
            float s = 0f;

            int i = 0;
            foreach (float weight in weights) {
                s += weight / totalWeight;
                if (s >= probe) {
                    return i;
                }

                i++;
            }

            return -1;
        }

        public static int PickRandomByProbes(IEnumerable<float> probes)
        {
            float total = probes.Sum();
            float randomPoint = UnityEngine.Random.value * total;

            int i = 0;
            foreach (float probe in probes) {
                if (randomPoint < probe) {
                    return i;
                }

                randomPoint -= probe;
                i++;
            }

            return -1;
        }

        /// <summary>
        /// https://stackoverflow.com/questions/48087/select-n-random-elements-from-a-listt-in-c-sharp
        /// </summary>
        public static IEnumerable<T> PickRandomCollection<T>(this IEnumerable<T> initialCollection, int pickCount)
        {
            int available = initialCollection.Count();
            int needed = pickCount;

            foreach (T? item in initialCollection) {
                if (UnityEngine.Random.Range(0, available) < needed) // random.Next(available) < needed
                {
                    needed--;
                    yield return item;

                    if (needed == 0) {
                        break;
                    }
                }

                available--;
            }
        }

        public static T PickRandom<T>(this List<T> collection)
        {
            int index = UnityEngine.Random.Range(0, collection.Count);
            return collection[index];
        }

        public static T PickRandom<T>(this IReadOnlyList<T> collection)
        {
            int index = UnityEngine.Random.Range(0, collection.Count);
            return collection[index];
        }

        public static T PickRandom<T>(this IEnumerable<T> collection)
        {
            int index = UnityEngine.Random.Range(0, collection.Count());
            return collection.ElementAt(index);
        }

        public static T PickRandom<T>(this ICollection<T> collection)
        {
            int index = UnityEngine.Random.Range(0, collection.Count);
            return collection.ElementAt(index);
        }

        public static T PickRandom<T>(this T[] collection)
        {
            int index = UnityEngine.Random.Range(0, collection.Length);
            return collection[index];
        }

        public static Vector3 GetRandomPointOnCircle(float minRadius, float maxRadius)
        {
            Vector3 spawnDirection = UnityEngine.Random.onUnitSphere;
            spawnDirection.y = 0;
            spawnDirection.Normalize();
            float radius = UnityEngine.Random.Range(minRadius, maxRadius);
            return spawnDirection * radius;
        }

        public static Vector2 GetRandomPointNotOnScreen(float screenOffset)
        {
            float xPos = UnityEngine.Random.Range(0f, 1f);
            float yPos = UnityEngine.Random.Range(0f, 1f);

            bool xRoll = Random(50f);
            if (xRoll) {
                bool overOneRoll = Random(50f);
                if (overOneRoll) {
                    xPos = 1f + screenOffset;
                } else {
                    xPos = 0f - screenOffset;
                }
            } else {
                bool overOneRoll = Random(50f);
                if (overOneRoll) {
                    yPos = 1f + screenOffset;
                } else {
                    yPos = 0f - screenOffset;
                }
            }
            
            return new(xPos, yPos);
        }
        
        public static int GetRandomFromRange(this List<int> collection)
        {
            return UnityEngine.Random.Range(collection.First(), collection.Last() + 1);
        }
        
        public static void Shuffle<T>(this IList<T> collection)
        {
            for (int i = collection.Count - 1; i > 0; i--) {
                int randomColumn = UnityEngine.Random.Range(0, collection.Count);
                (collection[i], collection[randomColumn]) = (collection[randomColumn], collection[i]);
            }
        }
        
        public static void Shuffle<T>(this T[] collection)
        {
            for (int i = collection.Length - 1; i > 0; i--) {
                int randomColumn = UnityEngine.Random.Range(0, collection.Length);
                (collection[i], collection[randomColumn]) = (collection[randomColumn], collection[i]);
            }
        }
    }
}