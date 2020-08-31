using System;

namespace PeakMVP.Helpers.JWTUtils {
    public class SignatureVerificationException : Exception {

        public SignatureVerificationException(string message)
            : base(message) { }
    }
}
