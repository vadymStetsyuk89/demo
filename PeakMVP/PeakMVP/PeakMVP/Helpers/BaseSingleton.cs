﻿using System;
using System.Collections.Generic;
using System.Text;

namespace PeakMVP.Helpers {
    public abstract class BaseSingleton<T> where T : class, new() {

        /// <summary>
        /// Lock from multithreading access.
        /// </summary>
        private static object _locker = new object();

        /// <summary>
        /// Current T instance.
        /// </summary>
        private static T _instance;

        /// <summary>
        /// Protected ctor.
        /// </summary>
        protected BaseSingleton() { }

        /// <summary>
        /// Get instance of T.
        /// </summary>
        public static T Instance => InstanceCreation();

        /// <summary>
        /// Build new instance or retrieve already created.
        /// </summary>
        /// <returns></returns>
        private static T InstanceCreation() {
            if (_instance == null) {
                lock (_locker) {
                    if (_instance == null) {
                        _instance = new T();
                    }
                }
            }

            return _instance;
        }
    }
}
