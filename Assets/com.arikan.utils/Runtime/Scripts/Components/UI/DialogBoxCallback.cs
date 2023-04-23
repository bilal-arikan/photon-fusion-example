using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Arikan
{
    public class DialogBoxCallback : MonoBehaviour
    {
        public UnityEvent Yes;
        public UnityEvent No;
        public UnityEvent Cancel;

        public void Show(DialogBox db)
        {
            db.Show(this);
        }
    }
}
