#region using

using RaBe.Model;

#endregion

namespace RaBe.ResponseModel
{
	public class LoginResponse
	{
		public bool administrator;
		public bool blocked;
		public string email;
		public string name;
		public bool passwordGeaendert;
		public string token;

		public static LoginResponse FromTeacher(Lehrer teacher)
		{
			var response = new LoginResponse
			{
				administrator = teacher.Administrator,
				blocked = teacher.Blocked,
				email = teacher.Email,
				name = teacher.Name,
				passwordGeaendert = teacher.PasswordGeaendert,
				token = teacher.Token
			};


			return response;
		}
	}
}