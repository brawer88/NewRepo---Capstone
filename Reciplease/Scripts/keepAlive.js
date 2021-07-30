//calls the keep alive handler every 600,000 miliseconds, which is 10 minutes
var keepAlive = {
	refresh: function () {
		$.get('/keep-alive.ashx');
		setTimeout(keepAlive.refresh, 6000000);
	}
}; $(document).ready(keepAlive.refresh());