#region using

using System.Collections.Generic;

#endregion

namespace RaBe.ResponseModel
{
	public class AllErrorsResponse
	{
		public Dictionary<long, ErrorsResponse> rooms = new Dictionary<long, ErrorsResponse>();
	}
}