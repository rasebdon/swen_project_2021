﻿using System;

namespace MTCG
{
    public abstract class Singleton<T>
    {
        static T _instance;
        public static T Instance
        {
            get
            {
                if (_instance == null)
                    _instance = (T)Activator.CreateInstance(typeof(T));
                return _instance;
            }
            set
            {
                if (value != null)
                    _instance = value;
            }
        }
    }
}
