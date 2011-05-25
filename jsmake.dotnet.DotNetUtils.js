jsmake.dotnet = {};

jsmake.dotnet.DotNetUtils = function () {
	this._nugetPath = 'tools/nuget/NuGet.exe';
	this._frameworkVersion = '4.0.30319';
};
jsmake.dotnet.DotNetUtils.prototype = {
	updateNuGet: function () {
		jsmake.Sys.createRunner(this._nugetPath).args('update').run();
	},
	downloadNuGetPackages: function (srcPath, libPath) {
		var pkgs = jsmake.Fs.createScanner(srcPath).include('**/packages.config').scan();
		jsmake.Utils.each(pkgs, function (pkg) {
			jsmake.Sys.createRunner(this._nugetPath)
				.args('install', pkg)
				.args('-OutputDirectory', libPath)
				.run();
		}, this);
	},
	runMSBuild: function (projectPath, targets, parameters) {
		var runner = jsmake.Sys.createRunner(jsmake.Fs.combinePaths(jsmake.Sys.getEnvVar('SystemRoot'), 'Microsoft.NET', 'Framework', 'v' + this._frameworkVersion, 'MSBuild.exe'));
		runner.args(projectPath, '/verbosity:minimal', '/nologo');
		if (targets) {
			runner.args('/t:' + jsmake.Utils.toArray(targets).join(';'));
		}
		if (parameters) {
			parameters = jsmake.Utils.map(parameters, function (val, key) { return key + '="' + val + '"'; });
			runner.args('/p:' + parameters.join(';'));
		}
		runner.run();
	},
	writeAssemblyInfo: function (path, attributes) {
		/*
		Mapping between Assembly attributes and properties visible in windows explorer:
		AssemblyTitle => File description
		AssemblyProduct => Product name
		AssemblyCopyright => Copyright
		AssemblyTrademark => Legal trademark
		AssemblyFileVersion => File version
		AssemblyInformationalVersion => Product version
		*/
		var rows = jsmake.Utils.map(attributes, function (value, name) {
			return '[assembly: System.Reflection.' + name + '("' + value + '")]';
		}, this);
		jsmake.Fs.writeFile(path, rows.join('\n'));
	}
};
