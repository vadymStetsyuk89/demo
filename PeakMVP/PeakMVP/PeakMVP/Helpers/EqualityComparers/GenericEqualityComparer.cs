using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace PeakMVP.Helpers.EqualityComparers {
    public class GenericEqualityComparer<TComparable> : IEqualityComparer<TComparable> where TComparable : class {

        private Func<TComparable, object> _comparingFunc;

        public GenericEqualityComparer(Func<TComparable, object> comparingFunc) {
            _comparingFunc = comparingFunc;
        }

        public bool Equals(TComparable x, TComparable y) {
            object xPropertyValue = _comparingFunc.Invoke(x);
            object yPropertyValue = _comparingFunc.Invoke(y);

            if (xPropertyValue == null && yPropertyValue == null) {
                return true;
            }
            else if (xPropertyValue != null && xPropertyValue.Equals(yPropertyValue)) {
                return true;
            }
            else {
                return false;
            }
        }

        public int GetHashCode(TComparable obj) =>
            _comparingFunc.Invoke(obj).GetHashCode();
    }
}
