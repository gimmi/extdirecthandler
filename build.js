project('ExtDirectHandler', 'nuget', function () {
	var sys = Make.Sys, fs = Make.Fs, util = Make.Utils;
	
	task('nuget', [], function () {
		sys.createRunner('tools/nuget/NuGet.exe').args('update').run();
		var pkgs = fs.createScanner('src').include('**/packages.config').scan();
		util.each(pkgs, function (pkg) {
			sys.createRunner('tools/nuget/NuGet.exe')
				.args('install', pkg)
				.args('-OutputDirectory', 'lib')
				.run();
		});
	});
});