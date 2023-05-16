using System.Net.Security;
using System.Security.Claims;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [Authorize]
    public class UsersController : BaseApiController
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public UsersController(IUserRepository userRepository, IMapper mapper)
        {
            _mapper = mapper;
            _userRepository = userRepository;
        }

        // método para devolver TODOS los usuarios
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MemberDto>>> GetUsers()
        {
            var users = await _userRepository.GetMembersAsync();
            return Ok(users);
        }

        // método para devolver solo 1 usuario indicado por parámetro "id"
        [HttpGet("{username}")]
        public async Task<ActionResult<MemberDto>> GetUser(string username)
        {
            return await _userRepository.GetMemberAsync(username);
        }

        [HttpPut]
        public async Task<ActionResult> UpdateUser(MemberUpdateDto memberUpdateDto)
        {
            var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var user = await _userRepository.GetUserByUsernameAsync(username);

            if(user == null) return NotFound();
            
            // vamos a mapear los campos pero todavía no guarda en bbdd
            _mapper.Map(memberUpdateDto, user);
            
            // ahora guardamos en bbd. 
            // NOTA: return NoContent(); -> es la forma de indicar que ha sido correcto y devoverá código 204
            if(await _userRepository.SaveAllAsync()) return NoContent();

            // si ha habido algún problema devolvemos BadRequest
            return BadRequest("Failed to update user");
        }
    }
}