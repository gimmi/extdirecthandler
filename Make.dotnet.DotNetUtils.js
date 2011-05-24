Make.dotnet = {};

Make.dotnet.DotNetUtils = function () {
	this._nugetPath = 'tools/nuget/NuGet.exe';
	this._frameworkVersion = '4.0.30319';
};
Make.dotnet.DotNetUtils.prototype = {
	updateNuGet: function () {
		Make.Sys.createRunner(this._nugetPath).args('update').run();
	},
	downloadNuGetPackages: function (srcPath, libPath) {
		var pkgs = Make.Fs.createScanner(srcPath).include('**/packages.config').scan();
		Make.Utils.each(pkgs, function (pkg) {
			Make.Sys.createRunner(this._nugetPath)
				.args('install', pkg)
				.args('-OutputDirectory', libPath)
				.run();
		}, this);
	},
	runMSBuild: function (projectPath, targets, parameters) {
		var runner = Make.Sys.createRunner(Make.Fs.combinePaths(Make.Sys.getEnvVar('SystemRoot'), 'Microsoft.NET', 'Framework', 'v' + this._frameworkVersion, 'MSBuild.exe'));
		runner.args(projectPath, '/verbosity:minimal', '/nologo');
		if (targets) {
			runner.args('/t:' + Make.Utils.toArray(targets).join(';'));
		}
		if (parameters) {
			parameters = Make.Utils.map(parameters, function (val, key) { return key + '="' + val + '"'; });
			runner.args('/p:' + parameters.join(';'));
		}
		runner.run();
	}
};
