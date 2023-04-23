// namespace Arikan
// {
//     public class MicrosoftRNG : System.Random, IRandom
//     {
//         public static MicrosoftRNG Instance = new MicrosoftRNG();

//         public MicrosoftRNG() : base()
//         {

//         }

//         public MicrosoftRNG(int seed) : base(seed)
//         {

//         }


//         float IRandom.Next()
//         {
//             return (float)this.NextDouble();
//         }

//         double IRandom.NextDouble()
//         {
//             return this.NextDouble();
//         }

//         int IRandom.Next(int size)
//         {
//             return this.Next(size);
//         }

//         int IRandom.Next(int low, int high)
//         {
//             return this.Next(low, high);
//         }
//     }
// }
