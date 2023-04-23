// namespace Arikan
// {
//     public class UnityRNG : IRandom
//     {

//         public float Next()
//         {
//             //return Random.value;
//             //because unity's Random returns in range 0->1, which is dumb
//             //why you might say? Well it means that the 1 is the least likely value to generate, so for generating indices you get uneven results
//             return UnityEngine.Random.value * 0.9999f;
//         }

//         public double NextDouble()
//         {
//             //return (double)Random.value;
//             //because unity's Random returns in range 0->1, which is dumb
//             //why you might say? Well it means that the 1 is the least likely value to generate, so for generating indices you get uneven results
//             return (double)UnityEngine.Random.value * 0.99999999d;
//         }

//         public int Next(int size)
//         {
//             return (int)((double)size * NextDouble());
//         }


//         public int Next(int low, int high)
//         {
//             return (int)(NextDouble() * (high - low)) + low;
//         }
//     }
// }
