namespace HokmGame.Core.Hokm
{
    public class ValidationResult
    {
        public bool IsValid { get; set; }
        public string ErrorMessage { get; set; }

        public static readonly ValidationResult Valid = new ValidationResult { IsValid = true };
        public static ValidationResult ErrorResult(string errorMessage)
        {
            return new ValidationResult { IsValid = false, ErrorMessage = errorMessage };
        }
    }
}