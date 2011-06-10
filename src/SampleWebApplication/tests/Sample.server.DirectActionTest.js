/*global describe, beforeEach, expect, it */

describe("Sample.server.DirectAction", function () {
	var target;

	beforeEach(function () {
		target = Sample.server.DirectAction;
	});

	it('should echo value', function () {
		var actual;
		runs(function() {
			target.echo('hello world', function (ret) {
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
});