using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web.Http;
using System.Web.Http.Description;
using AutoMapper;
using FeMMORPG.Common;
using FeMMORPG.Data;
using FeMMORPG.LoginServer.Models;

namespace FeMMORPG.LoginServer.Controllers
{
    /// <summary>
    /// Account-related actions; login and register
    /// </summary>
    [RoutePrefix("")]
    public class AccountController : ApiController
    {
        private IUserUnitOfWork unitOfWork;
        private IMapper mapper;

        /// <summary>
        /// Controller
        /// </summary>
        /// <param name="unitOfWork"></param>
        /// <param name="mapper"></param>
        public AccountController(
            IUserUnitOfWork unitOfWork,
            IMapper mapper
            )
        {
            if (unitOfWork == null)
                throw new ArgumentNullException(nameof(unitOfWork));
            if (mapper == null)
                throw new ArgumentNullException(nameof(mapper));

            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        /// <summary>
        /// Process a login request. Returns game server connection info if successful.
        /// </summary>
        /// <param name="model">Login credentials</param>
        [Route("login")]
        [HttpPost]
        [ResponseType(typeof(LoginResultModel))]
        public IHttpActionResult Login(LoginModel model)
        {
            model = model ?? new LoginModel();
            if (model.Username == null || model.Password == null)
                return BadRequest(ErrorCodes.InvalidLogin.ToString());

            // Authenticate user
            byte[] passwordBytes = Encoding.UTF8.GetBytes(model.Password);
            var password = Convert.ToBase64String(new SHA256Managed().ComputeHash(passwordBytes));

            var user = unitOfWork.Users.Query()
                .Where(u => u.Username == model.Username)
                .Where(u => u.Password == password)
                .SingleOrDefault();
            if (user == null)
                return BadRequest(ErrorCodes.InvalidLogin.ToString());

            if (user.LoginToken != null)
            {
                user.LoginToken.Server.CurrentUsers -= 1;
                unitOfWork.Servers.Update(user.LoginToken.Server);

                unitOfWork.LoginTokens.Remove(user.LoginToken);
            }

            // Determine optimal world server
            var server = unitOfWork.Servers.Query()
                .Where(s => s.Enabled)
                .Where(s => s.CurrentUsers < s.MaxUsers)
                .OrderBy(s => s.CurrentUsers)
                .FirstOrDefault();

            if (server == null)
                return Content(System.Net.HttpStatusCode.ServiceUnavailable, ErrorCodes.NoGameServersAvailable.ToString());

            // Generate login token
            var token = new LoginToken
            {
                Id = Guid.NewGuid(),
                User = user,
                LoginTime = DateTime.UtcNow,
                Server = server,
            };
            unitOfWork.LoginTokens.Add(token);

            server.CurrentUsers += 1;
            unitOfWork.Servers.Update(server);

            user.LastLogin = DateTime.UtcNow;
            unitOfWork.Users.Update(user);

            unitOfWork.SaveChanges();

            // Send success message with token info to user
            return Ok(new LoginResultModel
            {
                Success = true,
                Token = token.Id,
                ServerIP = server.IP,
            });
        }

        /// <summary>
        /// Processes a registration request.
        /// </summary>
        /// <param name="model">Registration data</param>
        /// <returns></returns>
        [Route("register")]
        [HttpPost]
        [ResponseType(typeof(RegisterResultModel))]
        public IHttpActionResult Register(RegisterModel model)
        {
            model = model ?? new RegisterModel();
            if (model.Username == null || model.Password == null)
                return BadRequest(ErrorCodes.InvalidRegistration.ToString());

            // TODO: validation

            var user = this.unitOfWork.Users.Query()
                .SingleOrDefault(u => u.Username == model.Username);
            if (user != null)
                return BadRequest(ErrorCodes.UserAlreadyExists.ToString());

            byte[] passwordBytes = Encoding.UTF8.GetBytes(model.Password);
            var password = Convert.ToBase64String(new SHA256Managed().ComputeHash(passwordBytes));

            this.unitOfWork.Users.Add(new User
            {
                Username = model.Username,
                Password = password,
                Enabled = true,
            });
            this.unitOfWork.SaveChanges();

            return Ok(new RegisterResultModel
            {
                Success = true,
            });
        }
    }
}
