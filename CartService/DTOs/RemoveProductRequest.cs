namespace CartService.DTOs;

public class RemoveProductRequest
{
    public int CartId { get; set; }

    public int ProductId { get; set; }
}
