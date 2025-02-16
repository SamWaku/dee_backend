using api.Data;
using api.Dtos.User;
using api.Models;
using api.Utils;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace api.Controllers
{
    public class AuthController: ControllerBase
    {
        private readonly ApplicationDBContext _context;

        public AuthController(ApplicationDBContext context) => _context = context;

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginDto userdto)
        {
            Dictionary<string, object> response = [];
            var userByEmail = await _context.Users.FirstOrDefaultAsync(x => x.Email == userdto.Email);

            if (userByEmail == null)
            {
                response["message"] = "fail";
                response["hint"] = "credentials mismatch";
                return BadRequest(response);
            }

            PasswordHasher hasher = new();
            string DecryptPassword = hasher.DecryptPassword($"{userByEmail.Password}", "hobbiton@2025!");
            if (DecryptPassword != userdto.Password)
            {
                response["message"] = "fail";
                response["hint"] = "credentials mismatch";
                return BadRequest(response);
            }


            return Ok(response);

        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(UserRegisterDto userregisterdto)
        {
            Dictionary<string, object> response = [];
            
            var UserByEmail = await _context.Users.FirstOrDefaultAsync(x => x.Email == userregisterdto.Email);
            if (UserByEmail != null)
            {
                response["message"] = "fail";
                response["hint"] = "Account with email or phone number entered already exists!";
                return BadRequest(response);
            }

            // Encrypt the password
            PasswordHasher hasher = new();
            string EncryptedPassword = hasher.EncryptPassword($"{userregisterdto.Password}", "hobbiton@2025!");

            // Create the user model
            User userModel = new()
            {
                Email = userregisterdto.Email,
                Username = userregisterdto.Username,
                Password = EncryptedPassword,
            };

            // Start a database transaction
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // Add user to the database
                await _context.Users.AddAsync(userModel);
                await _context.SaveChangesAsync();

                // Create a wallet for the user with an initial amount of 0.0
                Wallet wallet = new()
                {
                    UserId = userModel.Id,  // Use the newly created user's ID
                    Amount = 0.0m,
                    CreatedOn = DateTime.Now
                };

                await _context.Wallets.AddAsync(wallet);
                await _context.SaveChangesAsync();

                // Commit transaction
                await transaction.CommitAsync();

                response["message"] = "success";
                response["body"] = userregisterdto;
                return Ok(response);
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();

                response["message"] = "fail";
                response["hint"] = "An error occurred on our side. Try again later";
                return BadRequest(response);
            }
        }


        // [HttpPost("register")]
        // public async Task<IActionResult> Register(UserRegisterDto userregisterdto)
        // {
        //     Dictionary<string, object> response = [];
        //     var UserByEmail = await _context.Users.FirstOrDefaultAsync(x => x.Email == userregisterdto.Email);
        //     if (UserByEmail != null)
        //     {
        //         response["message"] = "fail";
        //         response["hint"] = "Account with email or phone number entered already exists!";
        //         return BadRequest(response);
        //     }
        //     // 3. Encrypt the password
        //     PasswordHasher hasher = new();
        //     string EncryptedPassword = hasher.EncryptPassword($"{userregisterdto.Password}", "hobbiton@2025!");
        //     User userModel = new()
        //     {
        //         Email = userregisterdto.Email,
        //         Username = userregisterdto.Username,
        //         Password = EncryptedPassword,
        //     };
        //     // 4. Add user to the DB
        //     try
        //     {
        //         await _context.Users.AddAsync(userModel);
        //         await _context.SaveChangesAsync();
        //         response["message"] = "success";
        //         response["body"] = userregisterdto;
        //         return Ok(response);
        //     }
        //     catch (Exception)
        //     {
        //         response["message"] = "fail";
        //         response["hint"] = "An error occurred on our side. Try again later";
        //         return BadRequest(response);
        //     };
        // }


        // public async Task<IActionResult> Register(UserDto user)
        // {
        //     // Create a dictionary with the message and any additional information
        //     Dictionary<string, object> response = [];
        //     // 1. Validate fields
        //     UserValidators validators = new();
        //     var IsUserValid = validators.IsUserDtoValid(user);
        //     if (!IsUserValid)
        //     {

        //         response["message"] = "fail";
        //         if (!validators.IsNumberValid($"{user.Phone}"))
        //         {
        //             response["hint"] = "Phone number should be 10 digits and valid.";
        //         }
        //         else if (!validators.IsValidEmail($"{user.Email}"))
        //         {
        //             response["hint"] = "Email should be valid.";
        //         }
        //         // else if (!validators.IsProvinceValid($"{user.Province}"))
        //         // {
        //         //     response["hint"] = "Province should be a Zambian province e.g Lusaka, Southern etc";
        //         // }
        //         return BadRequest(response);
        //     }
        //     else if (user.Password != user.ConfirmPassword)
        //     {
        //         response["message"] = "fail";
        //         response["hint"] = "Password and Confirm password values should match";
        //         return BadRequest(response);
        //     }
        //     // 2. Check if key values are already in use
        //     var UserByPhone = await _db.Tb_Users.FirstOrDefaultAsync(x => x.Phone == user.Phone);
        //     var UserByEmail = await _db.Tb_Users.FirstOrDefaultAsync(x => x.Email == user.Email);
        //     if (UserByEmail != null || UserByPhone != null)
        //     {
        //         response["message"] = "fail";
        //         response["hint"] = "Account with email or phone number entered already exists!";
        //         return BadRequest(response);
        //     }
        //     // 3. Encrypt the password
        //     PasswordHasher hasher = new();
        //     string EncryptedPassword = hasher.EncryptPassword($"{user.Password}", "ecocube@2024!");
        //     TokenManager tokenManager = new();
        //     UserModel userModel = new()
        //     {
        //         Email = user.Email,
        //         Phone = user.Phone,
        //         FirstName = user.FirstName,
        //         OtherName = user.OtherName,
        //         LastName = user.LastName,
        //         Password = EncryptedPassword,
        //         Street = user.Street,
        //         Area = user.Area,
        //         Province = user.Province,
        //         PhoneNumberVerified = false,
        //         Role = "citizen",
        //         Token = tokenManager.GenerateToken(user.Phone!)
        //     };
        //     // 4. Add user to the DB
        //     try
        //     {
        //         await _db.Tb_Users.AddAsync(userModel);
        //         await _db.SaveChangesAsync();
        //         AuthResponseDto authResponse = new(userModel: userModel);
        //         response["message"] = "success";
        //         response["body"] = authResponse;
        //         return Ok(response);
        //     }
        //     catch (Exception)
        //     {
        //         response["message"] = "fail";
        //         response["hint"] = "An error occurred on our side. Try again later";
        //         return BadRequest(response);
        //     };
        // }

        // [HttpPost("login")]
        // public async Task<IActionResult> Login([FromBody ]UserLoginDto loginDto)
        // {
        //     Dictionary<string, object> response = [];
        //     // check if phone number is in DB
        //     var UserByPhone = await _db.Tb_Users.FirstOrDefaultAsync(x => x.Phone == loginDto.PhoneNumber);
        //     if (UserByPhone == null)
        //     {
        //         response["message"] = "fail";
        //         response["hint"] = "user is not registered";
        //         return BadRequest(response);
        //     }
        //     // if available, encrypt the password and check if it matches the one stored in the DB
        //     PasswordHasher hasher = new();
        //     // string EncryptedPassword = hasher.EncryptPassword($"{loginDto.Password}", "ecocube@2024!");
        //     string DecryptPassword = hasher.DecryptPassword($"{UserByPhone.Password}", "ecocube@2024!");

        //     if (DecryptPassword != loginDto.Password)
        //     {
        //         response["message"] = "fail";
        //         response["hint"] = "credentials mismatch";
        //         return BadRequest(response);
        //     }
        //     // EAAAAHc/wUlBr6rQ15Gn2BtovRsudVAcv9AyL/8KmkvxeMhs
        //     // EAAAAJVu1iJNtdH61GA5D0RGoyAiqMP0obV2Ghy3vkLM8kug

        //     // replace old token
        //     TokenManager tokenManager = new();
        //     UserByPhone.Token = tokenManager.GenerateToken(UserByPhone.Phone!);
        //     await _db.SaveChangesAsync();
        //     AuthResponseDto authResponse = new(UserByPhone);
        //     response["message"] = "success";
        //     response["body"] = authResponse;
        //     return Ok(response);
        // }
    }
}