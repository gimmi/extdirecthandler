/*global describe, beforeEach, expect, it */

describe("Sample.server.DirectAction", function () {
	var target;

	beforeEach(function () {
		target = Sample.server.DirectAction;
	});

	it('should echo string value', function () {
		var actual;
		runs(function() {
			target.stringEcho('hello world', function (ret) {
				this.done = true;
				actual = ret;
			}, this);
		});
		waitsFor(function () {
			return this.done;
		});
		runs(function () {
			expect(actual).toEqual('hello world');
		});
	});

	it('should echo numeric value', function () {
		var actual;
		runs(function() {
			target.numberEcho(3.14, function (ret) {
				this.done = true;
				actual = ret;
			}, this);
		});
		waitsFor(function () {
			return this.done;
		});
		runs(function () {
			expect(actual).toEqual(3.14);
		});
	});

	it('should echo bool value', function () {
		var actual;
		runs(function() {
			target.boolEcho(true, function (ret) {
				this.done = true;
				actual = ret;
			}, this);
		});
		waitsFor(function () {
			return this.done;
		});
		runs(function () {
			expect(actual).toEqual(true);
		});
	});

	it('should echo array', function () {
		var actual;
		runs(function() {
			target.arrayEcho([ 1, 2, 3 ], function (ret) {
				this.done = true;
				actual = ret;
			}, this);
		});
		waitsFor(function () {
			return this.done;
		});
		runs(function () {
			expect(actual).toEqual([ 1, 2, 3 ]);
		});
	});

	it('should echo object', function () {
		var actual;
		var obj = { StringValue: 'hello', NumberValue: 3.14, BoolValue: true };
		runs(function() {
			target.objectEcho(obj, function (ret) {
				this.done = true;
				actual = ret;
			}, this);
		});
		waitsFor(function () {
			return this.done;
		});
		runs(function () {
			expect(actual).toEqual(obj);
		});
	});
});