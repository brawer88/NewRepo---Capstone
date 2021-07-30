using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.SessionState;

namespace Reciplease.Handlers {
	public class KeepAlive : IHttpHandler, IRequiresSessionState {
		public void ProcessRequest( HttpContext context ) {
			context.Response.AddHeader( "Cache-Control", "no-cache" );
			context.Response.AddHeader( "Pragma", "no-cache" );
			context.Response.ContentType = "text/plain";
			context.Response.Write( "OK" );
		}
		public bool IsReusable { get { return false; } }
	}
}