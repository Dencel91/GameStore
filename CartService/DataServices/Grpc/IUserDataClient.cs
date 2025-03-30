using CartService.DTOs;

namespace CartService.DataServices.Grpc;

public interface IUserDataClient
{
    IEnumerable<ProductDto> GetUserProducts(Guid userId);
}
