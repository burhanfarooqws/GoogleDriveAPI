using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace GoogleDrivePollyWeb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
	    private GoogleDriveApi _googleDriveApi = null;

		public ValuesController(GoogleDriveApi gdAPI)
		{
			_googleDriveApi = gdAPI;

		}
        // GET api/values
        [HttpGet]
        public async Task<ActionResult<IEnumerable<string>>> GetAsync()
        {
	        // Retrieve access token and refresh token from database
	        var accessToken = "ya29.Il-zB3qKzDZu0CVkolS3Es3vIcIZbguaOgXRbRc4FPYhe3bz18aqzATKSvh-KUrOmzo9P__OcSHPexmsa8PoOEwCx0TD2C-xi73LpHtWOXKldj2kerr0jGVY_CwfsvY7Gg";
	        var refreshToken = "1//03NEoyufZufFQCgYIARAAGAMSNwF-L9IrwiKSzbdFnIWWA5kfHYinn1yqJS_-zOu1fhOMAWg8mVE8Twum78pdATc8nHtNnHkV49g";

	        // Get the list of files from Google Drive
			var filesResponse = await _googleDriveApi.ListFiles(accessToken, refreshToken, async token =>
	        {
		        // Update access token in the database
		        // ...
	        });

			return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public ActionResult<string> Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
