namespace PeakMVP.Models.Rests.EndPoints {
    public sealed class AuthenticationEndpoints {

        internal const string LOGIN_API_KEY = "api/account/login";
        internal const string ACCOUNT_API_KEY = "api/account";
        internal const string CHECK_EXISTING_API_KEY = "api/account/check-existing?Key={0}&Category={1}";
        private static readonly string _IMPERSONATE_LOG_IN = "api/account/login-as-child";
        private static readonly string _RESET_PASSWORD_END_POINT = "api/account/forgot-password";

        /// <summary>
        /// Creation of user.
        /// </summary>
        public string RegistrationEndPoints { get; private set; }

        /// <summary>
        /// Login user.
        /// </summary>
        public string LoginEndPoints { get; private set; }

        /// <summary>
        /// Cheking of existed user by specific parameters.
        /// </summary>
        public string CheckExistingEndPoints { get; private set; }

        /// <summary>
        /// Impersonate log in
        /// </summary>
        public string ImpersonateLogIn { get; private set; }

        public string ResetPassword { get; private set; }

        /// <summary>
        ///     ctor().
        /// </summary>
        /// <param name="host"></param>
        public AuthenticationEndpoints(string host) {
            UpdateEndpoint(host);
        }

        private void UpdateEndpoint(string host) {
            LoginEndPoints = $"{host}{LOGIN_API_KEY}";
            RegistrationEndPoints = $"{host}{ACCOUNT_API_KEY}";
            CheckExistingEndPoints = $"{host}{CHECK_EXISTING_API_KEY}";
            ImpersonateLogIn = string.Format("{0}{1}", host, _IMPERSONATE_LOG_IN);
            ResetPassword = string.Format("{0}{1}", host, _RESET_PASSWORD_END_POINT);
        }
    }
}
