namespace Testing.Helpers
{
    /// <summary>
    /// Based on DsnSamples from Sentry.Net
    /// </summary>
    public class DsnHelper
    {
        /// <summary>
        /// Sentry has dropped the use of secrets
        /// </summary>
        public const string ValidDsnWithoutSecret = "https://d4d82fc1c2c4032a83f3a29aa3a3aff@fake-sentry.io:65535/2147483647";
    }
}
