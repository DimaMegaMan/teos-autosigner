namespace Teos.Autosigner.Services
{
	public class ValidationResult
	{
		public bool Success { get; private set; }
		public string ErrorMessage { get; private set; }

		public static ValidationResult SuccessResult => new() { Success = true };
		public static ValidationResult Failure(string message) => new() { ErrorMessage = message };
	}
}
