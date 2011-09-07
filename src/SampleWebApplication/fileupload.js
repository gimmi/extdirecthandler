Ext.onReady(function () {
	Ext.direct.Manager.addProvider(Sample.server.REMOTING_API);

	var window = Ext.create('Ext.window.Window', {
		height: 150,
		width: 450,
		layout: {
			type: 'fit'
		},
		title: 'My Window',
		items: [{
			xtype: 'form',
			border: 0,
			bodyPadding: 10,
			api: {
				submit: Sample.server.DirectAction.submitFile
			},
			items: [{
				xtype: 'textfield',
				name: 'textValue',
				fieldLabel: 'Text value',
				anchor: '100%'
			}, {
				xtype: 'filefield',
				name: 'fileValue',
				fieldLabel: 'File value',
				anchor: '100%'
			}],
			buttons: [{
				text: 'Save',
				handler: function () {
					var form = this.up('form').getForm();
					form.submit();
				}
			}]
		}]
	});
	
	window.show();
});