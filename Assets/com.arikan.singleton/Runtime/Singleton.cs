//***********************************************************************//
// Copyright (C) 2017 Bilal Arikan. All Rights Reserved.
// Author: Bilal Arikan
// Time  : 04.11.2017   
//***********************************************************************//
using System;
using System.Collections.Generic;

namespace Arikan
{
    /// <summary>
    /// Singleton pattern.
    /// </summary>
    public class Singleton<T> where T : new()
    {
        protected static T instance;

        /// <summary>
        /// Singleton design pattern
        /// </summary>
        /// <value>The instance.</value>
        public static T Instance
        {
            get
            {
                if (instance == null)
                    instance = new T();
                return instance;
            }
        }
    }
}