using System;
using System.Collections.Generic;
using System.Web.Http;
using System.Web.Http.Description;
using FeMMORPG.Synchronization;
using FeMMORPG.SyncServer.Models;

namespace FeMMORPG.SyncServer.Controllers
{
    /// <summary>
    /// User-related persistence
    /// </summary>
    [RoutePrefix("users")]
    public class UserController : ApiController
    {
        private IPersistenceService persistenceService;
        private AutoMapper.MapperConfiguration mapConfig;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="persistenceService"></param>
        public UserController(
            IPersistenceService persistenceService
            )
        {
            if (persistenceService == null)
                throw new ArgumentNullException(nameof(persistenceService));

            this.persistenceService = persistenceService;

            mapConfig = new AutoMapper.MapperConfiguration(cfg =>
            {
                cfg.CreateMap<User, UserModel>().ReverseMap();
            });
        }

        /// <summary>
        /// Retrieve user data for all users matching query
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [Route("")]
        [HttpGet]
        [ResponseType(typeof(List<UserModel>))]
        public IHttpActionResult Get([FromUri] UserRequest query)
        {
            if (query == null)
                query = new UserRequest();
            var users = persistenceService.GetUsers(query);
            return Ok(users);
        }

        /// <summary>
        /// Retrieves a single user by ID (username)
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("{id}")]
        [HttpGet]
        [ResponseType(typeof(UserModel))]
        public IHttpActionResult GetOne(string id)
        {
            var user = persistenceService.GetUser(id);
            return Ok(user);
        }

        /// <summary>
        /// Persists user data
        /// </summary>
        /// <param name="model"></param>
        [Route("")]
        [HttpPost]
        [ResponseType(null)]
        public void Post([FromBody] UserModel model)
        {
            var user = mapConfig.CreateMapper().Map<User>(model);
            this.persistenceService.AddUser(user);
        }

        /// <summary>
        /// Updates persisted user data
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        [Route("{id}")]
        [HttpPut]
        [ResponseType(null)]
        public void Put(string id, [FromBody] UserModel model)
        {
            var user = mapConfig.CreateMapper().Map<User>(model);
            user.Id = id;
            this.persistenceService.UpdateUser(user);
        }

        /// <summary>
        /// Deletes persisted user data
        /// </summary>
        /// <param name="id"></param>
        [Route("{id}")]
        [HttpDelete]
        [ResponseType(null)]
        public void Delete(string id)
        {
            this.persistenceService.DeleteUser(id);
        }
    }
}
