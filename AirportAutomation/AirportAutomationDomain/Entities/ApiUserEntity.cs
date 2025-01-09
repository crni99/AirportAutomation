using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AirportAutomation.Core.Entities
{
	[Table("apiuser")]
	public class ApiUserEntity
	{
		[Key]
		[Column("apiuserid")]
		public int ApiUserId { get; set; }

		[Column("username")]
		public string UserName { get; set; }

		[Column("password")]
		public string Password { get; set; }

		[Column("roles")]
		public string Roles { get; set; }
	}
}
