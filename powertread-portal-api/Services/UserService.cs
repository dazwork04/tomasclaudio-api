using B1SLayer;
using SAPB1SLayerWebAPI.Context;
using SAPB1SLayerWebAPI.Models;
using SAPB1SLayerWebAPI.Models.SLayer;
using SLayerConnectionLib;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace SAPB1SLayerWebAPI.Services
{
    public class UserService
    {
        private readonly AuthDbContext authDbContext;

        public UserService(AuthDbContext authDbContext)
        {
            this.authDbContext = authDbContext;
        }

        // GET USERS
        public async Task<Response> GetUsersAsync() => await Task.Run(() =>
        {
            try
            {
                List<User> users = authDbContext.OUSR.Select(o => new User
                {
                    Id = o.Id,
                    UserId = o.UserId,
                    EmpId = o.EmpId,
                    Name = o.Name,
                    Modules = o.Modules,
                    UserCode = o.UserCode,
                    UserPass = o.UserPass,
                    SapUser = o.SapUser,
                    SapPass = o.SapPass,
                    EmpCode =o.EmpCode,
                    Status = o.Status,
                    WhseCode = o.WhseCode,
                    Branch = o.Branch
                }).ToList();
                return new Response
                {
                    Status = "success",
                    Payload = users
                };
            }
            catch (Exception ex)
            {

                return new Response
                {
                    Status = "failed",
                    Message = ex.Message,
                };
            }
        });


        // GET EMPLOYEES
        public async Task<Response> GetEmployeesAsync(int userId, string companyDB) => await Task.Run(async () =>
        {
            try
            {
                var connection = Main.GetConnection(userId, companyDB);

                string request = $"$crossjoin({EntitiesKeys.EmployeesInfo}, {EntitiesKeys.Users})"; // InventoryGenEntries, InventoryGenExits
                string expand = $"{EntitiesKeys.EmployeesInfo}($select=EmployeeID,EmployeeCode,LastName,FirstName,ApplicationUserID),{EntitiesKeys.Users}($select=InternalKey,UserCode,UserName)";
                string filter = $"{EntitiesKeys.EmployeesInfo}/ApplicationUserID eq {EntitiesKeys.Users}/InternalKey and {EntitiesKeys.EmployeesInfo}/Active eq 'Y'";

                var result = await connection.Request(request).Expand(expand).Filter(filter).OrderBy("EmployeeCode asc").GetAllAsync<dynamic>();

                return new Response
                {
                    Status = "success",
                    Payload = result
                };
            }
            catch (Exception ex)
            {
                return new Response
                {
                    Status = "failed",
                    Message = ex.Message
                };
            }
        });

        // CREATE USER
        public async Task<Response> CreateUserAsync(User user) => await Task.Run(() =>
        {
            try
            {
                //var connection = Main.GetConnection(userId, companyDB);

                //var results = await connection.Request(EntitiesKeys.EmployeesInfo)
                //    .Filter($"EmployeeCode eq '{user.EmpCode}' and Active eq 'Y'").Top(1)
                //    .GetAllAsync<EmployeeInfo>();

                //if (results.Count > 0)
                //{

                //}
                var newUser = new Entities.Auth.OUSR
                {
                    EmpCode = user.EmpCode,
                    Name = user.Name,
                    UserCode = user.UserCode,
                    UserPass = user.UserPass,
                    SapUser = user.SapUser,
                    SapPass = user.SapPass,
                    Status = user.Status,
                    Modules = user.Modules,
                    UserId = user.UserId,
                    EmpId = user.EmpId,
                    WhseCode = user.WhseCode,
                    Branch = user.Branch,
                };

                authDbContext.OUSR.Add(newUser);
                authDbContext.SaveChanges();

                return new Response
                {
                    Status = "success",
                    Message = "User created successfully.",
                    Payload = newUser
                };

                //return new Response
                //{
                //    Status = "failed",
                //    Message = "Invalid Employee Code."
                //};
                
            }
            catch (Exception ex)
            {

                return new Response
                {
                    Status = "failed",
                    Message = ex.Message,
                };
            }
        });

        // UPDATE USER
        public async Task<Response> UpdateUserAsync(User user) => await Task.Run(() =>
        {
            try
            {
                //var connection = Main.GetConnection(userId, companyDB);

                //var results = await connection.Request(EntitiesKeys.EmployeesInfo)
                //    .Filter($"EmployeeCode eq '{user.EmpCode}' and Active eq 'Y'").Top(1)
                //    .GetAllAsync<EmployeeInfo>();

                //if (results != null)
                //{

                authDbContext.OUSR.Update(new Entities.Auth.OUSR
                {
                    Id = user!.Id,
                    EmpCode = user.EmpCode,
                    Name = user.Name,
                    UserCode = user.UserCode,
                    UserPass = user.UserPass,
                    SapUser = user.SapUser,
                    SapPass = user.SapPass,
                    Status = user.Status,
                    Modules = user.Modules,
                    UserId = user.UserId,
                    EmpId = user.EmpId,
                    WhseCode = user.WhseCode,
                    Branch = user.Branch,
                });
                authDbContext.SaveChanges();

                return new Response
                {
                    Status = "success",
                    Message = "User updated successfully.",
                    Payload = user
                };

                //}
                //return new Response
                //{
                //    Status = "failed",
                //    Message = "Invalid Employee Code."
                //};

            }
            catch (Exception ex)
            {
                return new Response
                {
                    Status = "failed",
                    Message = ex.Message,
                };
            }
        });
    }
}
