using System;
using System.Collections.Generic;
#if COM_CYSHARP_UNITASK
using Cysharp.Threading.Tasks;
#endif
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Arikan
{
    public class DialogBox : MonoBehaviour
    {
        public enum Callback
        {
            Yes,
            No,
            Cancel
        }
        public Text MessageText;
        public Image MessageImage;
        public DialogBoxCallback DefaultCallback;
        [Space]
        public Button Yes;
        public Button No;
        public Button Cancel;

#if COM_CYSHARP_UNITASK
        public UniTask<Callback> Show(string msg, Sprite msgS = null)
        {
            if (MessageText)
                MessageText.text = msg;
            if (MessageImage)
                MessageImage.sprite = msgS;
            return Show();
        }
#else
        public void Show(string msg, Sprite msgS = null)
        {
            if (MessageText)
                MessageText.text = msg;
            if (MessageImage)
                MessageImage.sprite = msgS;
            Show();
        }
#endif

        public void Show(DialogBoxCallback cb)
        {
            DefaultCallback = cb;
            Show();
        }
        public void Show(UnityAction<Callback> cb)
        {
            if (Yes != null)
            {
                if (cb != null)
                    Yes.onClick.AddListener(() => cb(Callback.Yes));
                if (DefaultCallback)
                    Yes.onClick.AddListener(DefaultCallback.Yes.Invoke);
                // Yes.onClick.AddListener(() => Debug.Log("DialogBox Yes"));
                Yes.onClick.AddListener(Hide);
                Yes.onClick.AddListener(RemoveAllListeners);
            }
            if (No != null)
            {
                if (cb != null)
                    No.onClick.AddListener(() => cb(Callback.No));
                if (DefaultCallback)
                    No.onClick.AddListener(DefaultCallback.No.Invoke);
                // No.onClick.AddListener(() => Debug.Log("DialogBox No"));
                No.onClick.AddListener(Hide);
                No.onClick.AddListener(RemoveAllListeners);
            }
            if (Cancel != null && Cancel != No)
            {
                if (cb != null)
                    Cancel.onClick.AddListener(() => cb(Callback.Cancel));
                if (DefaultCallback)
                    Cancel.onClick.AddListener(DefaultCallback.Cancel.Invoke);
                // Cancel.onClick.AddListener(() => Debug.Log("DialogBox Cancel"));
                Cancel.onClick.AddListener(Hide);
                Cancel.onClick.AddListener(RemoveAllListeners);
            }
            gameObject.SetActive(true);
        }

#if COM_CYSHARP_UNITASK
        public UniTask<Callback> Show()
        {
            var promiseOpen = new UniTaskCompletionSource<Callback>();
            if (Yes != null)
            {
                Yes.onClick.AddListener(() => promiseOpen.TrySetResult(Callback.Yes));
                if (DefaultCallback)
                    Yes.onClick.AddListener(DefaultCallback.Yes.Invoke);
                // Yes.onClick.AddListener(() => Debug.Log("DialogBox Yes"));
                Yes.onClick.AddListener(Hide);
                Yes.onClick.AddListener(RemoveAllListeners);
            }
            if (No != null)
            {
                No.onClick.AddListener(() => promiseOpen.TrySetResult(Callback.No));
                if (DefaultCallback)
                    No.onClick.AddListener(DefaultCallback.No.Invoke);
                // No.onClick.AddListener(() => Debug.Log("DialogBox No"));
                No.onClick.AddListener(Hide);
                No.onClick.AddListener(RemoveAllListeners);
            }
            if (Cancel != null && Cancel != No)
            {
                Cancel.onClick.AddListener(() => promiseOpen.TrySetResult(Callback.Cancel));
                if (DefaultCallback)
                    Cancel.onClick.AddListener(DefaultCallback.Cancel.Invoke);
                // Cancel.onClick.AddListener(() => Debug.Log("DialogBox Cancel"));
                Cancel.onClick.AddListener(Hide);
                Cancel.onClick.AddListener(RemoveAllListeners);
            }
            gameObject.SetActive(true);
            return promiseOpen.Task;
        }
#else
        public void Show()
        {
            Show(null as UnityAction<Callback>);
        }
#endif

        void Hide()
        {
            gameObject.SetActive(false);
        }
        void RemoveAllListeners()
        {
            Yes.onClick.RemoveAllListeners();
            No.onClick.RemoveAllListeners();
            Cancel.onClick.RemoveAllListeners();
        }
    }
}
