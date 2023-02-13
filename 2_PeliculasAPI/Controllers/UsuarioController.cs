using _2_PeliculasAPI.Dto;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace _2_PeliculasAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuarioController : CustomBaseController
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IConfiguration _configuration;

        #region Constructor
        public UsuarioController(
            ApplicationDbContext context,
            IMapper mapper,
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            IConfiguration configuration
            ) 
        : base(context, mapper)
        {
            _context = context;
            _mapper = mapper;
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
        }

        #endregion

        #region GET

        [Authorize(Roles = "Admin")]
        [HttpGet("Usuarios")]
        public async Task<ActionResult<List<UsuarioGetDto>>> Get([FromQuery] PaginacionDto paginationDTO)
        {
            var queryable = _context.Users.AsQueryable();
            queryable = queryable.OrderBy(x => x.Email);
            return await Get<IdentityUser, UsuarioGetDto>(paginationDTO);
        }

        [HttpGet("Roles")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<List<string>>> GetRoles()
        {
            return await _context.Roles.Select(x => x.Name).ToListAsync();
        }

        #endregion

        #region POST

        [HttpPost("Crear")]
        public async Task<ActionResult<UserToken>> CreateUser([FromBody] UserInfo model)
        {
            var user = new IdentityUser { UserName = model.Email, Email = model.Email };
            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                return await ConstruirToken(model);
            }
            else
            {
                return BadRequest(result.Errors);
            }
        }

        [HttpPost("Login")]
        public async Task<ActionResult<UserToken>> Login([FromBody] UserInfo model)
        {
            IdentityUser user = await _userManager.FindByEmailAsync(model.Email);
            if(user is null) return BadRequest("Invalid login attempt");

            var resultado = await _signInManager.CheckPasswordSignInAsync(user,model.Password, lockoutOnFailure: false);

            if (!resultado.Succeeded) return BadRequest("Invalid login attempt"); 
            
            return await ConstruirToken(model);

        }

        [HttpPost("RenovarToken")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<UserToken>> Renovar()
        {
            var userInfo = new UserInfo
            {
                Email = HttpContext.User.Identity!.Name
            };

            return await ConstruirToken(userInfo);
        }

        [HttpPost("AsignarRol")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> AsignarRol(EditarRolDto editarRolDTO)
        {
            var user = await _userManager.FindByIdAsync(editarRolDTO.UsuarioId);
            if (user == null)
            {
                return NotFound();
            }

            await _userManager.AddClaimAsync(user, new Claim(ClaimTypes.Role, editarRolDTO.NombreRol!));
            return NoContent();
        }

        [HttpPost("RemoveRol")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> RemoverRol(EditarRolDto editarRolDTO)
        {
            var user = await _userManager.FindByIdAsync(editarRolDTO.UsuarioId);
            if (user == null)
            {
                return NotFound();
            }

            await _userManager.RemoveClaimAsync(user, new Claim(ClaimTypes.Role, editarRolDTO.NombreRol!));
            return NoContent();
        }

        #endregion

        private async Task<UserToken> ConstruirToken(UserInfo userInfo)
        {
            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, userInfo.Email!),
                new Claim(ClaimTypes.Email, userInfo.Email!),
            };

            var identityUser = await _userManager.FindByEmailAsync(userInfo.Email);

            claims.Add(new Claim(ClaimTypes.NameIdentifier, identityUser.Id));

            var claimsDB = await _userManager.GetClaimsAsync(identityUser);

            claims.AddRange(claimsDB);

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var expiracion = DateTime.UtcNow.AddYears(1);

            JwtSecurityToken token = new(
                issuer: null,
                audience: null,
                claims: claims,
                expires: expiracion,
                signingCredentials: creds);

            return new UserToken()
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                Expiracion = expiracion
            };

        }
    }
}
