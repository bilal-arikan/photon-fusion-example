// using System;
// using System.Collections;
// using System.Linq;
// using System.Threading.Tasks;
// using UnityEngine;

// namespace Arikan.Util
// {
//     public static class MicrophoneInput
//     {
//         public static string Device { get; private set; }

// #if ENABLE_MICROPHONE
//         public static async Task InitDevice()
//         {
//             // Debug.Log("MicDevices:" + string.Join(", ", Microphone.devices));
//             Device = Microphone.devices.FirstOrDefault();
//         }

//         public static bool IsRecording()
//         {
//             return Microphone.IsRecording(Device);
//         }

//         public static IEnumerator StartRecordingRoutine(Action<AudioClip> source, int lengthSec = 15)
//         {
//             Microphone.GetDeviceCaps(Device, out int minFrequency, out int maxFrequency);
//             int frequency = Mathf.Min(44100, maxFrequency);
//             if (frequency == 0)
//             {
//                 frequency = 16000;
//             }
//             var clip = Microphone.Start(Device, true, lengthSec, frequency);

//             float startWaiting = Time.unscaledTime;
//             float waitDuration = 4f;
//             while (startWaiting + waitDuration < Time.unscaledTime)
//             {
//                 if (Microphone.GetPosition(Device) <= 0)
//                     goto waitEnd;
//                 yield return null;
//             }
//             Debug.LogError($"Microphone.GetPosition() waits more than {waitDuration} seconds");
//         waitEnd:
//             while (Microphone.GetPosition(Device) <= 0) ;
//             yield return null;
//             source.Invoke(clip);
//         }

//         public static async Task<AudioClip> StartRecordingWait(int lengthSec = 15)
//         {
//             Microphone.GetDeviceCaps(Device, out int minFrequency, out int maxFrequency);
//             int frequency = Mathf.Min(44100, maxFrequency);
//             if (frequency == 0)
//             {
//                 frequency = 16000;
//             }
//             var clip = Microphone.Start(Device, true, lengthSec, frequency);

//             while (Microphone.GetPosition(Device) <= 0) ;
//             return clip;
//         }

//         public static void StopRecording()
//         {
//             Microphone.End(Device);
//         }
// #endif
//     }
// }
