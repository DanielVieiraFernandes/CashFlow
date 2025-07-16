namespace CashFlow.Communication.Responses;
public class ResponseErrorJson
{
    public required string ErrorMessage { get; set; } = string.Empty;

    public ResponseErrorJson(string ErrorMessage) => this.ErrorMessage = ErrorMessage;
}
