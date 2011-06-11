/*global describe, beforeEach, expect, it */

describe("Sample.server.DirectAction", function () {
	var target;

	beforeEach(function () {
		target = Sample.server.DirectAction;
	});

	it('should echo string value', function () {
		var actual;
		runs(function () {
			target.stringEcho('hello world', function (ret) {
				this.done = true;
				actual = ret;
			}, this);
		});
		waitsFor(function () {
			return this.done;
		}, 'Server call', 1000);
		runs(function () {
			expect(actual).toEqual('hello world');
		});
	});

	it("should not corrupt strings", function () {
		runs(function () {
			target.stringEcho('тащий', function (result) {
				this.actual = result;
				this.completed = true;
			}, this);
		});

		waitsFor(function () {
			return this.completed;
		}, 'Server call', 1000);

		runs(function () {
			expect(this.actual).toEqual('тащий');
		});
	});

	it('should echo numeric value', function () {
		var actual;
		runs(function () {
			target.numberEcho(3.14, function (ret) {
				this.done = true;
				actual = ret;
			}, this);
		});
		waitsFor(function () {
			return this.done;
		}, 'Server call', 1000);
		runs(function () {
			expect(actual).toEqual(3.14);
		});
	});

	it('should echo bool value', function () {
		var actual;
		runs(function () {
			target.boolEcho(true, function (ret) {
				this.done = true;
				actual = ret;
			}, this);
		});
		waitsFor(function () {
			return this.done;
		}, 'Server call', 1000);
		runs(function () {
			expect(actual).toEqual(true);
		});
	});

	it('should echo array', function () {
		var actual;
		runs(function () {
			target.arrayEcho([1, 2, 3], function (ret) {
				this.done = true;
				actual = ret;
			}, this);
		});
		waitsFor(function () {
			return this.done;
		}, 'Server call', 1000);
		runs(function () {
			expect(actual).toEqual([1, 2, 3]);
		});
	});

	it('should echo object', function () {
		var actual;
		var obj = { stringValue: 'hello', numberValue: 3.14, boolValue: true };
		runs(function () {
			target.objectEcho(obj, function (ret) {
				this.done = true;
				actual = ret;
			}, this);
		});
		waitsFor(function () {
			return this.done;
		}, 'Server call', 1000);
		runs(function () {
			expect(actual).toEqual(obj);
		});
	});

	it('should echo raw JSON object', function () {
		var actual;
		var obj = { stringValue: 'hello', numberValue: 3.14, boolValue: true };
		runs(function () {
			target.jObjectEcho(obj, function (ret) {
				this.done = true;
				actual = ret;
			}, this);
		});
		waitsFor(function () {
			return this.done;
		}, 'Server call', 1000);
		runs(function () {
			expect(actual).toEqual(obj);
		});
	});

	it("should call methods without parameters", function () {
		runs(function () {
			target.noParams(function (result) {
				this.actual = result;
				this.done = true;
			}, this);
		});

		waitsFor(function () {
			return this.done;
		}, 'Server call', 1000);

		runs(function () {
			expect(this.actual).toEqual(true);
		});
	});

	describe('exception handling', function () {
		var exceptionCount;
		var exceptionHandler = function () {
			exceptionCount += 1;
		};

		beforeEach(function () {
			exceptionCount = 0;
			Ext.direct.Manager.on('exception', exceptionHandler);
		});

		afterEach(function () {
			Ext.direct.Manager.un('exception', exceptionHandler);
		});

		it("should return exception", function () {
			runs(function () {
				target.exception(function (result, event) {
					this.done = true;
					this.result = result;
					this.event = event;
				}, this);
			});

			waitsFor(function () {
				return this.done;
			}, 'Server call', 1000);

			runs(function () {
				expect(exceptionCount).toEqual(1);
				expect(this.result).toBeNull();
				expect(this.event.type).toEqual('exception');
			});
		});
	});

	it("should batch calls", function () {
		var requestCount, responseCount = 0;
		var request = function (i) {
			target.stringEcho('call ' + i, function (result) {
				expect(result).toEqual('call ' + i);
				responseCount += 1;
			});
		};
		for (requestCount = 1; requestCount <= 10; requestCount += 1) {
			request(requestCount);
		}

		waitsFor(function () {
			return responseCount === 10;
		}, 'Server call', 1000);
	});
});