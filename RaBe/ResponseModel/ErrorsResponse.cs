#region using

using System.Collections.Generic;
using RaBe.Model;

#endregion

namespace RaBe.ResponseModel
{
	public class ErrorsResponse
	{
		public List<Fehler> errors = new List<Fehler>();
		public long roomId;
		public string roomName;
	}
}