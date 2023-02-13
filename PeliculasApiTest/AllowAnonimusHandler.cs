using Microsoft.AspNetCore.Authorization;

namespace PeliculasApiTest
{
    public class AllowAnonimusHandler : IAuthorizationHandler
    {
        public Task HandleAsync(AuthorizationHandlerContext context)
        {
           foreach(var requirement in context.PendingRequirements.ToList())
           {
                context.Succeed(requirement);
           } 
            
           return Task.CompletedTask;
        }
    }
}
