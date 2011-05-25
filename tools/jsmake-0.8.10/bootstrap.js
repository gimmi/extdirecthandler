(function (global, args) {
	load(args.shift());
	var main = new jsmake.Main();
	main.initGlobalScope(global);
	load('build.js');
	main.getProject().runBody(global);
	main.getProject().runTask(args.shift(), args);
}(this, arguments));
