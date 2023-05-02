using System.Collections.Generic;
using System.Linq;

using UnityEngine;

namespace Arikan
{
    public abstract class MultipleScriptableObject<T> : ScriptableObject where T : ScriptableObject //SharedInstance<T> where T : SharedInstance<T>
    {
        protected static List<T> _instances = null;
        public static List<T> Instances
        {
            get
            {
                if (_instances == null || _instances.Count == 0)
                {
                    var atr = ((ResourcePathAttribute[])typeof(T).GetCustomAttributes(typeof(ResourcePathAttribute), true)).FirstOrDefault();
                    if (atr == null)
                        throw new UnityException("MultipleScriptableObject'in Mutlaka ResorucePath() Attribute i olmalı " +
                            "ve Path sadece o Nesnelerden oluşan dosyaları içermeli" +
                            "aksi halde o klasördeki bütün objeleri yüklüyor");
                    else
                    {
                        if (!string.IsNullOrWhiteSpace(atr.Path))
                            Resources.LoadAll<T>(atr.Path);
                        else
                            Debug.LogError("MultipleScriptableObject ResourcePath value null or empty!");
                    }
                    _instances = Resources.FindObjectsOfTypeAll<T>().ToList();
                    //Debug.Log( typeof(T).Name +": SingletonScriptableObject Reference Initialized:" + _instances.Count, _instances.FirstOrDefault());
                }
                return _instances;
            }
        }
    }
}
