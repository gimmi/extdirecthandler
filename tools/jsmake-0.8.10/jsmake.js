/*
JSMake version 0.8.10

Copyright 2011 Gian Marco Gherardi

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/
jsmake = this.jsmake || {};

jsmake.Utils = {
	escapeForRegex: function (str) {
		return str.replace(/[\-\[\]{}()*+?.,\\\^$|#\s]/g, "\\$&");
	},
	isArray: function (v) {
		// Check ignored test 'isArray should show strange behavior on Firefox'
		return Object.prototype.toString.apply(v) === '[object Array]';
	},
	isArguments: function (v) {
		return !!(v && Object.prototype.hasOwnProperty.call(v, 'callee'));
	},
	toArray: function (v) {
		if (this.isEmpty(v)) {
			return [];
		} else if (this.isArray(v)) {
			return v;
		} else if (this.isArguments(v)) {
			return Array.prototype.slice.call(v);
		} else {
			return [ v ];
		}
	},
	isObject : function (v) {
		return !!v && Object.prototype.toString.call(v) === '[object Object]';
	},
	isNumber: function (v) {
		return typeof v === 'number' && isFinite(v);
	},
	isEmpty : function (v) {
		return v === null || v === undefined || ((this.isArray(v) && !v.length));
	},
	trim: function (str) {
		return str.replace(/(?:^\s+)|(?:\s+$)/g, '');
	},
	each: function (items, fn, scope) {
		var key;
		if (this.isObject(items)) {
			for (key in items) {
				if (items.hasOwnProperty(key)) {
					if (fn.call(scope, items[key], key, items)) {
						return;
					}
				}
			}
		} else {
			items = this.toArray(items);
			for (key = 0; key < items.length; key += 1) {
				if (fn.call(scope, items[key], key, items)) {
					return;
				}
			}
		}
	},
	filter: function (items, fn, scope) {
		var ret = [];
		this.each(items, function (item) {
			if (fn.call(scope, item)) {
				ret.push(item);
			}
		}, this);
		return ret;
	},
	map: function (items, fn, scope) {
		var ret = [];
		this.each(items, function (item, key) {
			ret.push(fn.call(scope, item, key, items));
		}, this);
		return ret;
	},
	reduce: function (items, fn, memo, scope) {
		this.each(items, function (item) {
			memo = fn.call(scope, memo, item);
		}, this);
		return memo;
	},
	contains: function (items, item) {
		var ret = false;
		this.each(items, function (it) {
			ret = (it === item);
			return ret;
		}, this);
		return ret;
	},
	distinct: function (items) {
		var ret = [];
		this.each(items, function (item) {
			if (!this.contains(ret, item)) {
				ret.push(item);
			}
		}, this);
		return ret;
	},
	flatten: function (items) {
		return this.reduce(items, function (memo, item) {
			if (this.isArray(item)) {
				memo = memo.concat(this.flatten(item));
			} else {
				memo.push(item);
			}
			return memo;
		}, [], this);
	}
};

jsmake.Project = function (name, defaultTaskName, body, logger) {
	this._name = name;
	this._defaultTaskName = defaultTaskName;
	this._tasks = {};
	this._body = body;
	this._logger = logger;
};
jsmake.Project.prototype = {
	getName: function () {
		return this._name;
	},
	addTask: function (task) {
		this._tasks[task.getName()] = task;
	},
	getTask: function (name) {
		var task = this._tasks[name];
		if (!task) {
			throw "Task '" + name + "' not defined";
		}
		return task;
	},
	getTasks: function (name) {
		var tasks = [];
		this._fillDependencies(this.getTask(name), tasks, new jsmake.RecursionChecker('Task recursion found'));
		return jsmake.Utils.distinct(tasks);
	},
	runBody: function (global) {
		var me = this;
		global.task = function (name, tasks, body) {
			me.addTask(new jsmake.Task(name, tasks, body, me._logger));
		};
		this._body.apply({}, []);
		global.task = undefined;
	},
	runTask: function (name, args) {
		var tasks, taskNames;
		name = name || this._defaultTaskName;
		tasks = this.getTasks(name);
		taskNames = jsmake.Utils.map(tasks, function (task) {
			return task.getName();
		}, this);
		this._logger.log('Task execution order: ' + taskNames.join(', '));
		jsmake.Utils.each(tasks, function (task) {
			task.run(task.getName() === name ? args : []);
		}, this);
	},
	_fillDependencies: function (task, tasks, recursionChecker) {
		recursionChecker.wrap(task.getName(), function () {
			jsmake.Utils.each(task.getTaskNames(), function (taskName) {
				var task = this.getTask(taskName);
				this._fillDependencies(task, tasks, recursionChecker);
			}, this);
			tasks.push(task);
		}, this);
	}
};

jsmake.Task = function (name, taskNames, body, logger) {
	this._name = name;
	this._taskNames = taskNames;
	this._body = body;
	this._logger = logger;
};
jsmake.Task.prototype = {
	getName: function () {
		return this._name;
	},
	getTaskNames: function () {
		return this._taskNames;
	},
	run: function (args) {
		this._logger.log('Executing task ' + this._name);
		this._body.apply({}, args);
	}
};

jsmake.RecursionChecker = function (message) {
	this._message = message;
	this._stack = [];
};
jsmake.RecursionChecker.prototype = {
	enter: function (id) {
		this._check(id);
		this._stack.push(id);
	},
	exit: function () {
		this._stack.pop();
	},
	wrap: function (id, fn, scope) {
		this.enter(id);
		try {
			fn.call(scope);
		} finally {
			this.exit();
		}
	},
	_check: function (id) {
		if (jsmake.Utils.contains(this._stack, id)) {
			this._stack.push(id);
			throw this._message + ': ' + this._stack.join(' => ');
		}
	}
};

jsmake.AntPathMatcher = function (pattern, caseSensitive) {
	this._pattern = pattern;
	this._caseSensitive = caseSensitive;
};
jsmake.AntPathMatcher.prototype = {
	match: function (path) {
		var patternTokens, pathTokens;
		patternTokens = this._tokenize(this._pattern);
		pathTokens = this._tokenize(path);
		return this._matchTokens(patternTokens, pathTokens);
	},
	_matchTokens: function (patternTokens, pathTokens) {
		var patternToken, pathToken;
		while (true) {
			patternToken = patternTokens.shift();
			if (patternToken === '**') {
				pathTokens = pathTokens.slice(-patternTokens.length).reverse();
				patternTokens = patternTokens.reverse();
				return this._matchTokens(patternTokens, pathTokens);
			}
			pathToken = pathTokens.shift();
			if (patternToken && pathToken) {
				if (!this._matchToken(patternToken, pathToken)) {
					return false;
				}
			} else if (patternToken && !pathToken) {
				return false;
			} else if (!patternToken && pathToken) {
				return false;
			} else {
				return true;
			}
		}
	},
	_matchToken: function (patternToken, pathToken) {
		var regex = '', i, ch;
		for (i = 0; i < patternToken.length; i += 1) {
			ch = patternToken.charAt(i);
			if (ch === '*') {
				regex += '.*';
			} else if (ch === '?') {
				regex += '.{1}';
			} else {
				regex += jsmake.Utils.escapeForRegex(ch);
			}
		}
		return new RegExp(regex, (this._caseSensitive ? '' : 'i')).test(pathToken);
	},
	_tokenize: function (pattern) {
		var tokens = pattern.split(/\\+|\/+/);
		tokens = jsmake.Utils.map(tokens, function (token) {
			return jsmake.Utils.trim(token);
		}, this);
		tokens = jsmake.Utils.filter(tokens, function (token) {
			return !/^[\s\.]*$/.test(token);
		}, this);
		if (tokens[tokens.length - 1] === '**') {
			throw 'Invalid ** wildcard at end pattern, use **/* instead'; // TODO maybe useless
		}
		// TODO invalid more then one **
		return tokens;
	}
};

jsmake.Sys = {
	isWindowsOs: function () {
		return jsmake.Fs.getPathSeparator() === '\\';
	},
	runCommand: function (command, opts) {
		return runCommand(command, opts);
	},
	createRunner: function (command) {
		return new jsmake.CommandRunner(command);
	},
	getEnvVar: function (name, def) {
		return java.lang.System.getenv(name) || def;
	},
	log: function (msg) {
		print(msg);
	}
};

jsmake.Fs = {
	createScanner: function (basePath) {
		return new jsmake.FsScanner(basePath, this.isCaseSensitive());
	},
	getCharacterEncoding: function () {
		return java.lang.System.getProperty("file.encoding", "UTF-8"); // Windows default is "Cp1252"
	},
	getPathSeparator: function () {
		return java.io.File.separator;
	},
	isCaseSensitive: function () {
		return !jsmake.Sys.isWindowsOs();
	},
	readFile: function (path, characterEncoding) {
		characterEncoding = characterEncoding || this.getCharacterEncoding();
		if (!this.fileExists(path)) {
			throw "File '" + path + "' not found";
		}
		return readFile(path, characterEncoding);
	},
	writeFile: function (path, data, characterEncoding) {
		characterEncoding = characterEncoding || this.getCharacterEncoding();
		this.createDirectory(this.getParentDirectory(path));
		var out = new java.io.FileOutputStream(new java.io.File(path));
		data = new java.lang.String(data || '');
		try {
			out.write(data.getBytes(characterEncoding));
		} finally {
			out.close();
		}
	},
	getName: function (path) {
		return this._translateJavaString(new java.io.File(path).getName());
	},
	copyPath: function (srcPath, destDirectory) {
		if (this.fileExists(srcPath)) {
			this._copyFile(srcPath, destDirectory);
		} else if (this.directoryExists(srcPath)) {
			this._copyDirectory(srcPath, destDirectory);
		} else {
			throw "Cannot copy source path '" + srcPath + "', it does not exists";
		}
	},
	pathExists: function (path) {
		return new java.io.File(path).exists();
	},
	directoryExists: function (path) {
		var file = new java.io.File(path);
		return file.exists() && file.isDirectory();
	},
	fileExists: function (path) {
		var file = new java.io.File(path);
		return file.exists() && file.isFile();
	},
	createDirectory: function (path) {
		var file = new java.io.File(path);
		if (file.exists() && file.isDirectory()) {
			return;
		}
		if (!file.mkdirs()) {
			throw "Failed to create directories for path '" + path + "'";
		}
	},
	deletePath: function (path) {
		jsmake.Utils.each(this.getFileNames(path), function (fileName) {
			new java.io.File(path, fileName)['delete']();
		}, this);
		jsmake.Utils.each(this.getDirectoryNames(path), function (dirName) {
			this.deletePath(this.combinePaths(path, dirName));
		}, this);
		new java.io.File(path)['delete']();
	},
	getCanonicalPath: function (path) {
		return this._translateJavaString(new java.io.File(path).getCanonicalPath());
	},
	getParentDirectory: function (path) {
		return this._translateJavaString(new java.io.File(path).getCanonicalFile().getParent());
	},
	combinePaths: function () {
		var paths = jsmake.Utils.flatten(arguments);
		return jsmake.Utils.reduce(paths, function (memo, path) {
			return (memo ? this._javaCombine(memo, path) : path);
		}, null, this);
	},
	getFileNames: function (basePath) {
		return this._getPathNames(basePath, function (fileName) {
			return new java.io.File(fileName).isFile();
		});
	},
	getDirectoryNames: function (basePath) {
		return this._getPathNames(basePath, function (fileName) {
			return new java.io.File(fileName).isDirectory();
		});
	},
	_javaCombine: function (path1, path2) {
		return this._translateJavaString(new java.io.File(path1, path2).getPath());
	},
	_copyDirectory: function (srcDirectory, destDirectory) {
		this.deletePath(destDirectory);
		this.createDirectory(destDirectory);
		jsmake.Utils.each(this.getFileNames(srcDirectory), function (path) {
			this.copyPath(this.combinePaths(srcDirectory, path), destDirectory);
		}, this);
		jsmake.Utils.each(this.getDirectoryNames(srcDirectory), function (path) {
			this.copyPath(this.combinePaths(srcDirectory, path), this.combinePaths(destDirectory, path));
		}, this);
	},
	_copyFile: function (srcFile, destDirectory) {
		var destFile = this.combinePaths(destDirectory, this.getName(srcFile));
		this.deletePath(destFile);
		this.createDirectory(destDirectory);
		this._copyFileToFile(srcFile, destFile);
	},
	_copyFileToFile: function (srcPath, destPath) {
		var srcFile, destFile, output, input, buffer, n;
		srcFile = new java.io.File(srcPath);
		destFile = new java.io.File(destPath);
		input = new java.io.FileInputStream(srcFile);
		try {
			output = new java.io.FileOutputStream(destFile);
			try {
				buffer = java.lang.reflect.Array.newInstance(java.lang.Byte.TYPE, 1024 * 4);
				while (-1 !== (n = input.read(buffer))) {
					output.write(buffer, 0, n);
				}
			} finally {
				output.close();
			}
		} finally {
			input.close();
		}
	},
	_getPathNames: function (basePath, filter) {
		var fileFilter, files;
		fileFilter = new java.io.FileFilter({ accept: filter });
		files = this._translateJavaArray(new java.io.File(basePath).listFiles(fileFilter));
		return jsmake.Utils.map(files, function (file) {
			return this._translateJavaString(file.getName());
		}, this);
	},
	_translateJavaArray: function (javaArray) {
		var ary = [], i;
		if (javaArray === null) {
			return null;
		}
		for (i = 0; i < javaArray.length; i += 1) {
			ary.push(javaArray[i]);
		}
		return ary;
	},
	_translateJavaString: function (javaString) {
		return String(javaString);
	}
};
jsmake.FsScanner = function (basePath, caseSensitive) {
	this._basePath = basePath;
	this._includeMatchers = [];
	this._excludeMatchers = [];
	this._caseSensitive = caseSensitive;
};
jsmake.FsScanner.prototype = {
	include: function (pattern) {
		this._includeMatchers.push(new jsmake.AntPathMatcher(pattern, this._caseSensitive));
		return this;
	},
	exclude: function (pattern) {
		this._excludeMatchers.push(new jsmake.AntPathMatcher(pattern, this._caseSensitive));
		return this;
	},
	scan: function () {
		var fileNames = [];
		if (this._includeMatchers.length === 0) {
			this.include('**/*');
		}
		this._scan('.', fileNames);
		return fileNames;
	},
	_scan: function (relativePath, fileNames) {
		var fullPath = jsmake.Fs.combinePaths(this._basePath, relativePath);
		jsmake.Utils.each(jsmake.Fs.getFileNames(fullPath), function (fileName) {
			fileName = jsmake.Fs.combinePaths(relativePath, fileName);
			if (this._evaluatePath(fileName, false)) {
				fileNames.push(jsmake.Fs.combinePaths(this._basePath, fileName));
			}
		}, this);
		jsmake.Utils.each(jsmake.Fs.getDirectoryNames(fullPath), function (dir) {
			dir = jsmake.Fs.combinePaths(relativePath, dir);
			if (this._evaluatePath(dir, true)) {
				this._scan(dir, fileNames);
			}
		}, this);
	},
	_evaluatePath: function (path, def) {
		if (this._runMatchers(this._excludeMatchers, path)) {
			return false;
		}
		if (this._runMatchers(this._includeMatchers, path)) {
			return true;
		}
		return def;
	},
	_runMatchers: function (matchers, value) {
		var match = false;
		jsmake.Utils.each(matchers, function (matcher) {
			match = match || matcher.match(value);
		}, this);
		return match;
	}
};

jsmake.CommandRunner = function (command) {
	this._command = command;
	this._arguments = [];
	this._logger = jsmake.Sys;
};
jsmake.CommandRunner.prototype = {
	args: function () {
		this._arguments = this._arguments.concat(jsmake.Utils.flatten(arguments));
		return this;
	},
	run: function () {
		this._logger.log(this._command + ' ' + this._arguments.join(' '));
		var exitStatus = jsmake.Sys.runCommand(this._command, { args: this._arguments });
		if (exitStatus !== 0) {
			throw 'Command failed with exit status ' + exitStatus;
		}
	}
};
jsmake.Main = function () {
	this._project = null;
	this._logger = jsmake.Sys;
};
jsmake.Main.prototype = {
	getProject: function () {
		if (!this._project) {
			throw 'No project defined';
		}
		return this._project;
	},
	initGlobalScope: function (global) {
		global.project = this._bind(this.project, this);
	},
	run: function (args) {
		this.getProject().run(args.shift(), args);
	},
	project: function (name, defaultTaskName, body) {
		if (this._project) {
			throw 'project already defined';
		}
		this._project = new jsmake.Project(name, defaultTaskName, body, this._logger);
	},
	_bind: function (fn, scope) {
		return function () {
			fn.apply(scope, arguments);
		};
	}
};
