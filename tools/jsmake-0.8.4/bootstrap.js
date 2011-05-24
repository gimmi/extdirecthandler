(function (global, args) {
	load(args[0]);
	var main = new Make.Main();
	main.initGlobalScope(global);
	load('build.js');
	main.run(args.slice(1));
}(this, arguments));
