using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using MinimalApiJWT.Utils;

namespace MinimalApiJWT.Handlers
{
    public static class ApiEndpointsExtentions
    {
        public static void RegisterEndPointsAPIs(this WebApplication app)
        {
            app.MapGet("api/person",
                [Authorize]
            (ApplicationDbContext _dbContext) =>
                {
                    return Results.Ok(_dbContext.persons);
                });

            app.MapGet("api/person/{id}",
                (ApplicationDbContext _dbContext, int id) =>
                {
                    return Results.Ok(_dbContext.persons.Find(id));
                }).AllowAnonymous();

            app.MapPost("api/person",
                (ApplicationDbContext _dbContext, Person person) =>
                {
                    _dbContext.persons.Add(person);
                    _dbContext.SaveChanges();
                    return Results.Created($"api/person/{person.PersonId}", person);
                });

            app.MapPut("api/person/{id}",
                (ApplicationDbContext _dbContext, int id, Person person) =>
                {
                    _dbContext.persons.Update(person);
                    _dbContext.SaveChanges();
                    return Results.NoContent();
                }).RequireAuthorization();

            app.MapDelete("api/person/{id}",
                (ApplicationDbContext _dbContext, int id) =>
                {
                    var employee = _dbContext.persons.FirstOrDefault(p => p.PersonId == id);
                    _dbContext.persons.Remove(employee);
                    _dbContext.SaveChanges();
                    return Results.NoContent();
                });
        }

        public static void RegisterAuthenticationAPIs(this WebApplication app)
        {
            app.MapPost("api/login",
                [AllowAnonymous]
            async (UserManager<IdentityUser> userManager, User user) =>
                {
                    var identityUser = await userManager.FindByNameAsync(user.UserName);

                    if (await userManager.CheckPasswordAsync(identityUser, user.Password))
                    {
                        string stringToken = AuthenticationUtils.GenerateToken(app.Configuration);

                        return Results.Ok(stringToken);
                    }
                    else
                    {
                        return Results.Unauthorized();
                    }
                });

            app.MapPost("api/createUser",
                [AllowAnonymous]
            async (UserManager<IdentityUser> userManager, User user) =>
                {
                    var identityUser = new IdentityUser()
                    {
                        UserName = user.UserName,
                        Email = user.UserName + "@example.com"
                    };

                    var result = await userManager.CreateAsync(identityUser, user.Password);

                    return result.Succeeded ? Results.Ok() : Results.BadRequest();
                });
        }
    }
}
