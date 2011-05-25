load('jsmake.dotnet.DotNetUtils.js');

project('ExtDirectHandler', 'build', function () {
	var fs = jsmake.Fs;
	var dotnet = new jsmake.dotnet.DotNetUtils();
	
	var version;

	task('version', [], function () {
		version = JSON.parse(fs.readFile('version.json'));
	});
	
	task('dependencies', [], function () {
		dotnet.downloadNuGetPackages('src', 'lib');
	});
	
	task('assemblyinfo', [ 'version' ], function () {
		dotnet.writeAssemblyInfo('src/SharedAssemblyInfo.cs', {
			AssemblyTitle: 'ExtDirectHandler',
			AssemblyProduct: 'ExtDirectHandler',
			AssemblyDescription: '',
			AssemblyCopyright: 'Copyright © Gian Marco Gherardi ' + new Date().getFullYear(),
			AssemblyTrademark: '',
			AssemblyCompany: '',
			AssemblyConfiguration: '', // Probably a good place to put Git SHA1 and build date
			AssemblyVersion: [ version.major, version.minor, version.build, 0 ].join('.'),
			AssemblyFileVersion: [ version.major, version.minor, version.build, version.revision ].join('.'),
			AssemblyInformationalVersion: [ version.major, version.minor, version.build, version.revision ].join('.')
		});
	});
	
	task('build', [ 'dependencies', 'assemblyinfo' ], function () {
		dotnet.runMSBuild('src/ExtDirectHandler.sln', [ 'Clean', 'Rebuild' ]);
	});

	task('test', [ 'build' ], function () {
	});

	task('release', [ 'version', 'dependencies', 'assemblyinfo' ], function () {
		fs.deletePath('build');
		dotnet.runMSBuild('src/ExtDirectHandler.sln', [ 'Clean', 'ExtDirectHandler:Rebuild' ]);
		// 'build/bin' contains deploy
		version.revision += 1;
		fs.writeFile('version.json', JSON.stringify(version));
	});
});
