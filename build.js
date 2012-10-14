load('jsmake.dotnet.DotNetUtils.js');
load('tools/JSLint-2011.06.08/jslint.js');
load('jsmake.javascript.JavascriptUtils.js');

var fs = jsmake.Fs;
var utils = jsmake.Utils;
var sys = jsmake.Sys;
var dotnet = new jsmake.dotnet.DotNetUtils();
var javascript = new jsmake.javascript.JavascriptUtils();

var version, assemblyVersion;

task('default', 'test');

task('version', function () {
	version = JSON.parse(fs.readFile('version.json'));
	assemblyVersion = [ version.major, version.minor, version.build, 0 ].join('.');
});

task('dependencies', function () {
	var pkgs = fs.createScanner('src').include('**/packages.config').scan();
	dotnet.downloadNuGetPackages(pkgs, 'lib');
});

task('assemblyinfo', 'version', function () {
	dotnet.writeAssemblyInfo('src/SharedAssemblyInfo.cs', {
		AssemblyTitle: 'ExtDirectHandler',
		AssemblyProduct: 'ExtDirectHandler',
		AssemblyDescription: 'Ext Direct router implementation for ASP.NET',
		AssemblyCopyright: 'Copyright © Gian Marco Gherardi ' + new Date().getFullYear(),
		AssemblyTrademark: '',
		AssemblyCompany: 'Gian Marco Gherardi',
		AssemblyConfiguration: '', // Probably a good place to put Git SHA1 and build date
		AssemblyVersion: assemblyVersion,
		AssemblyFileVersion: assemblyVersion,
		AssemblyInformationalVersion: assemblyVersion
	});
});

task('jslint', function () {
	// Visual Studio set Javascript files encoding as "UTF-8 with signature". This cause problem with JSLint.
	// As a workarount, when creating a new js file, select File => Save as... => Save with encoding... => "UTF-8 without signature"
	// See http://forums.silverlight.net/forums/t/144306.aspx
	var files = fs.createScanner('src/SampleWebApplication')
		.include('**/*.js')
		.exclude('jasmine-*')
		.exclude('extjs')
		.scan();
	var options = { white: true, onevar: true, undef: true, regexp: true, plusplus: true, bitwise: true, newcap: true, sloppy: true };
	var globals = { 'Ext': false, 'Sample': false };
	javascript.jslint(files, options, globals);
});

task('build', [ 'dependencies', 'assemblyinfo' ], function () {
	dotnet.runMSBuild('src/ExtDirectHandler.sln', [ 'Clean', 'Rebuild' ]);
});

task('test', 'build', function () {
	var testDlls = fs.createScanner('build/bin').include('**/*Tests.dll').scan();
	dotnet.runNUnit(testDlls);
});

task('release', 'test', function () {
	fs.deletePath('build');
	fs.createDirectory('build');

	sys.run('tools/nuget/nuget.exe', 'pack', 'src\\ExtDirectHandler\\ExtDirectHandler.csproj', '-Build', '-OutputDirectory', 'build', '-Symbols');
	sys.run('tools/nuget/nuget.exe', 'push', 'build\\ExtDirectHandler.' + assemblyVersion + '.nupkg');

	version.build += 1;
	fs.writeFile('version.json', JSON.stringify(version));
});

