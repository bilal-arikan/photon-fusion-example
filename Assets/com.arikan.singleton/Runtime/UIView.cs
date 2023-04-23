using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
#if COM_CYSHARP_UNITASK
using Cysharp.Threading.Tasks;
#endif

namespace Arikan
{
    public interface IUIView
    {
        string name { get; }
        bool IsOpen { get; }
        string Group { get; }
#if COM_CYSHARP_UNITASK
        UniTask OpenAsPopup();
        UniTask OpenAsPopup(bool withAnim);
#endif
        void Open();
        void Open(bool withAnim);

        void Close();
        void Close(bool withAnim);
    }

    public static class UIViewStorage
    {
        public static List<IUIView> OpenedViews = new List<IUIView>();
    }
    public abstract class UIView<T> : SingletonBehaviour<T>, IUIView where T : SerializedMonoBehaviour
    {
        public IUIView ParentView => !string.IsNullOrEmpty(Group) ? this.GetComponentInParent<IUIView>() : null;
        public bool IsOpen => gameObject.activeInHierarchy;
        public abstract string Group { get; }
        [PropertySpace(0, 12)]
        public CanvasGroup CanvasGroup;
        public RectTransform rectTransform => transform as RectTransform;
        [ShowInInspector]
        List<IUIView> Openeds => UIViewStorage.OpenedViews;

        public event Action onEnabled;
        public event Action onDisabled;

#if COM_CYSHARP_UNITASK
        protected UniTaskCompletionSource promiseOpen { get; private set; }
#endif

#if COM_CYSHARP_UNITASK
        [Button]
        public UniTask OpenAsPopup() => OpenAsPopup(true);
        public virtual UniTask OpenAsPopup(bool withAnim)
        {
            Open(true);
            return promiseOpen.Task;
        }
#endif
        [Button]
        public void Open() => Open(true);
        public virtual void Open(bool withAnim) => gameObject.SetActive(true);
        [Button]
        public void Close() => Close(true);
        public virtual void Close(bool withAnim) => gameObject.SetActive(false);


        public virtual void UpdateUI() { }

        protected virtual void OnEnable()
        {
            //Bir gruba dahilse
            if (!string.IsNullOrEmpty(Group))
                for (int i = 0; i < UIViewStorage.OpenedViews.ToList().Count; i++)
                    if (UIViewStorage.OpenedViews[i].Group == Group)
                        UIViewStorage.OpenedViews[i].Close();
            UIViewStorage.OpenedViews.Add(this);
#if COM_CYSHARP_UNITASK
            promiseOpen = new UniTaskCompletionSource();
#endif
            onEnabled?.Invoke();
            //Debug.Log(name+ " Opened");
        }
        protected virtual void OnDisable()
        {
            //if (ParentView != null)
            //    ParentView.Open();
#if COM_CYSHARP_UNITASK
            promiseOpen?.TrySetResult();
            promiseOpen = null;
#endif
            UIViewStorage.OpenedViews.Remove(this);
            onDisabled?.Invoke();
            //Debug.Log(name+ " Closed");
        }
    }
}