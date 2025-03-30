using AutoMapper;
using CartService.DTOs;
using UserService;

namespace CartService.DataServices.Grpc;

public class UserDataClient(
	GrpcUser.GrpcUserClient client,
	IMapper mapper,
	ILogger<UserDataClient> logger) : IUserDataClient
{
    public IEnumerable<ProductDto> GetUserProducts(Guid userId)
    {
		try
		{
			var request = new GrpcGetUserProductRequest { UserId = userId.ToString() };
            var products = client.GetUserProducts(request).Products;

			return mapper.Map<IEnumerable<ProductDto>>(products);

        }
		catch (Exception ex)
		{
            logger.LogError("gRPC error in {methodName}: {message}", nameof(GetUserProducts), ex.Message);
            throw;
		}
    }
}
