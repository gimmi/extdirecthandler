load('Make.dotnet.DotNetUtils.js');

project('ExtDirectHandler', 'build', function () {
	var sys = Make.Sys, fs = Make.Fs, util = Make.Utils, dotnet = new Make.dotnet.DotNetUtils();
	
	task('nuget', [], function () {
		dotnet.updateNuGet();
		dotnet.downloadNuGetPackages('src', 'lib');
	});
	
	task('build', [ 'nuget' ], function () {
		dotnet.runMSBuild('src/ExtDirectHandler.sln', [ 'Clean', 'Rebuild' ]);
	});
});
