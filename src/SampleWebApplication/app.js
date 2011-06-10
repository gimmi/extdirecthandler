Ext.require('Ext.direct.*');

Ext.application({
	name: 'Sample',
	launch: function () {
		Ext.direct.Manager.addProvider(Sample.server.REMOTING_API);
		Ext.create('Ext.container.Viewport', {
			layout: 'fit',
			items: [ {
				title: 'Hello Ext',
				html: 'Hello! Welcome to Ext JS.'
			} ]
		});
	}
});
