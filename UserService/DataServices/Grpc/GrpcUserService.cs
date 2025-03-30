using AutoMapper;
using Grpc.Core;
using UserService.Services;

namespace UserService.DataServices.Grpc;

public class GrpcUserService(
    IUserService userService,
    IMapper mapper) : GrpcUser.GrpcUserBase
{
    public override async Task<GrpcGetUserProductsResponse> GetUserProducts(GrpcGetUserProductRequest request, ServerCallContext context)
    {
        var products = await userService.GetUserProducts(Guid.Parse(request.UserId));
        var response = new GrpcGetUserProductsResponse();

        response.Products.AddRange(mapper.Map<IEnumerable<UserService.GrpcProductModel>>(products));
        return response;
    }
}
