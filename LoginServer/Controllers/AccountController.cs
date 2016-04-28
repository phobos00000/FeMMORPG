using System;
using System.Security.Cryptography;
using System.Text;
using System.Web.Http;
using System.Web.Http.Description;
using FeMMORPG.Common;
using FeMMORPG.Synchronization;
using FeMMORPG.LoginServer.Models;

namespace FeMMORPG.LoginServer.Controllers
{
    /// <summary>
    /// Account-related actions; login and register
    /// </summary>
    [RoutePrefix("")]
    public class AccountController : ApiController
    {
        private IPersistenceService persistenceService;
        private AutoMapper.MapperConfiguration mapConfig;

        /// <summary>
        /// Controller
        /// </summary>
        /// <param name="persistenceService"></param>
        public AccountController(
            IPersistenceService persistenceService
            )
        {
            if (persistenceService == null)
                throw new ArgumentNullException(nameof(persistenceService));

            this.persistenceService = persistenceService;

            mapConfig = new AutoMapper.MapperConfiguration(cfg =>
            {
                //cfg.CreateMap<User, UserModel>().ReverseMap();
            });
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

            byte[] passwordBytes = Encoding.UTF8.GetBytes(model.Password);
            var bytes = new SHA256Managed().ComputeHash(passwordBytes);

            var user = persistenceService.GetUser(model.Username);
            if (user == null || user.Password != Convert.ToBase64String(bytes))
                return BadRequest(ErrorCodes.InvalidLogin.ToString());

            // TODO: get validated login to game server... somehow

            return Ok(new LoginResultModel
            {
                Success = true,
                ServerUrl = "blah",
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
            if (model.Id == null || model.Password == null)
                return BadRequest(ErrorCodes.InvalidRegistration.ToString());

            // TODO: validation

            if (persistenceService.GetUser(model.Id) != null)
                return BadRequest(ErrorCodes.UserAlreadyExists.ToString());

            byte[] passwordBytes = Encoding.UTF8.GetBytes(model.Password);
            var bytes = new SHA256Managed().ComputeHash(passwordBytes);

            persistenceService.AddUser(new User
            {
                Id = model.Id,
                Password = Convert.ToBase64String(bytes),
            });

            return Ok(new RegisterResultModel
            {
                Success = true,
            });
        }
    }
}
