using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace AspNetCoreIdentityApp.Web.Requirements
{
    public class ViolanceRequirement:IAuthorizationRequirement
    {
        public int ThresholdAge { get; set; }
    }

    public class ViolanceRequirementHandler : AuthorizationHandler<ViolanceRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ViolanceRequirement requirement)
        {

            if (!context.User.HasClaim(x => x.Type == "birthday"))
            {
                context.Fail();
                return Task.CompletedTask;
            }

            Claim birthdayClaim = context.User.FindFirst("birthday")!;

            var today = DateTime.Now;
            var birthDate = Convert.ToDateTime(birthdayClaim.Value);

            var age = today.Year - birthDate.Year;


            if (birthDate > today.AddYears(-age)) age--;


            if (requirement.ThresholdAge>age)
            {
                context.Fail();
                return Task.CompletedTask;
            }


            context.Succeed(requirement);
            return Task.CompletedTask;



        }
    }
}
