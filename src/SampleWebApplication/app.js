Ext.application({
	name: 'HelloExt',
	launch: function () {
		Ext.direct.Manager.addProvider(Ext.app.REMOTING_API);
		Ext.create('Ext.container.Viewport', {
			layout: 'fit',
			items: [ {
				title: 'Hello Ext',
				html: 'Hello! Welcome to Ext JS.'
			} ]
		});
	}
});
