namespace RepositoryPatternWithUOW.Core.DTO;

public class GenralResponse
{
    //public T Data { get; set; } 
    public dynamic? Data { get; set; }
    public int StatusCode { get; set; } = 200;
    public bool IsSuccess { get; set; } = true;
    public string Message { get; set; } = "";
}
