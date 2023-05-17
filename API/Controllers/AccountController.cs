using System.Security.Cryptography;
using System.Text;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using API.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace API.Controllers
{
    public class AccountController : BaseApiController
    {
        private readonly DataContext _context;
        private readonly ITokenService _tokenService;
        public AccountController(DataContext context, ITokenService tokenService)
        {
            _tokenService = tokenService;
            _context = context;
        }
        // EndPoint para registrar un usuario
        [HttpPost("register")] // POST: api/account/register?username=dave&password=pwd
        public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
        {
            if (await UserExists(registerDto.UserName)) return BadRequest("Username is taken");

            using var hmac = new HMACSHA512();

            var user = new AppUser
            {
                UserName = registerDto.UserName.ToLower(),
                PassWordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password)),
                PasswordSalt = hmac.Key
            };
            // añadimos el usuario al context
            _context.Users.Add(user);
            // GUARDAMOS el usuario en BBDD
            await _context.SaveChangesAsync();
            // devolvemos el nuevo usuario añadido en BBDD
            return new UserDto
            {
                Username = user.UserName,
                Token = _tokenService.CreateToken(user)
            };
        }

        [HttpPost("login")] // POST api/account/login
        public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
        {
            var user = await _context.Users
                .Include(p => p.Photos)
                .SingleOrDefaultAsync(x => x.UserName == loginDto.UserName);

            if(user == null) return Unauthorized("User name not valid.");

            using var hmac = new HMACSHA512(user.PasswordSalt);
            // convertimos el HASH con el SALT del usuario para poder ser tratado y verificar bit a bit si son iguales
            var computeHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));

            // ahora comprobamos si el array de bits de la contraseña recibida es igual que la calculada de bbdd
            for(int i = 0; i < computeHash.Length; i++)
            {
                if (computeHash[i] != user.PassWordHash[i]) return Unauthorized("Password not valid");
            }
            // si todo es correcto, devolvemos el user de BBDD
            return new UserDto
            {
                Username = user.UserName,
                Token = _tokenService.CreateToken(user),
                PhotoUrl = user.Photos.FirstOrDefault(x => x.IsMain).Url
            };
        }

        // esta función mira si existe el usuario en bbdd con el mismo username indicado
        private async Task<bool> UserExists(string username)
        {
            // recordar que await es para indicar algo a la BBDD
            return await _context.Users.AnyAsync(x => x.UserName == username.ToLower());
        }
    }
}