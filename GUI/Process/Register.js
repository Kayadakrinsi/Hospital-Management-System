(async () => {

    let formInstance, formItems, invCode,

        // Registration Model
        objReg = {
            invCode: 0,
            username: '',
            password: '',
        },

        url = new URL(window.location.href);

    invCode = url.searchParams.get('invCode');

    if (invCode && invCode > 0) {
        objReg.invCode = parseInt(invCode);
    }

    // Form Items
    formItems = [
        {
            dataField: 'username',
            label: { text: 'Username' },
            editorType: 'dxTextBox',
            editorOptions: { placeholder: 'Enter username' },
            validationRules: [
                {
                    type: 'required',
                    message: 'Username is required'
                }
            ]
        },
        {
            dataField: 'password',
            label: { text: 'Password' },
            editorType: 'dxTextBox',
            editorOptions: {
                mode: 'password',
                placeholder: 'Enter password',
                buttons: [{
                    name: 'togglePassword',
                    location: 'after',
                    options: {
                        icon: 'eyeclose',
                        type: 'default',
                        stylingMode: 'text',
                        onClick: function (e) {
                            TogglePswdVisibility(e, formInstance.getEditor('password'));
                        }
                    }
                }]
            },
            validationRules: [
                {
                    type: 'required',
                    message: 'Password is required'
                },
                {
                    type: 'stringLength',
                    min: 6,
                    message: 'Password must be at least 6 characters'
                }
            ]
        },
        {
            dataField: 'confirmPassword',
            label: { text: 'Confirm Password' },
            editorType: 'dxTextBox',
            editorOptions: {
                mode: 'password',
                placeholder: 'Confirm password',
                buttons: [{
                    name: 'togglePassword',
                    location: 'after',
                    options: {
                        icon: 'eyeclose',
                        type: 'default',
                        stylingMode: 'text',
                        onClick: function (e) {
                            TogglePswdVisibility(e, formInstance.getEditor('confirmPassword'));
                        }
                    }
                }]
            },
            validationRules: [
                {
                    type: 'required',
                    message: 'Confirm Password is required'
                },
                {
                    type: 'compare',
                    comparisonTarget: function () {
                        return formInstance.option('formData').password;
                    },
                    message: 'Passwords do not match'
                }
            ]
        },
        {
            itemType: 'button',
            horizontalAlignment: 'center',
            buttonOptions: {
                text: 'Register',
                type: 'default',
                width: '100%',
                onClick: async (e) => await RegisterUser(e)
            }
        }
    ];

    // Create Form
    formInstance = $('#formContainer').dxForm({
        formData: objReg,
        labelLocation: 'top',
        validationGroup: 'registerGroup',
        items: formItems
    }).dxForm('instance');

    function TogglePswdVisibility(e, component) {
        let newMode = component.option('mode') === 'password' ? 'text' : 'password',
            newIcon = component.option('icon') === 'eyeopen' ? 'eyeclose' : 'eyeopen';

        component.option('mode', newMode);
        component.option('icon', newIcon);
    }

    // Submit Registration
    async function RegisterUser(e) {
        let result = DevExpress.validationEngine.validateGroup('registerGroup');

        if (!result.isValid) return;

        let data = formInstance.option('formData');

        // Remove confirmPassword before sending
        let sendObj = {
            invCode: objReg.invCode,
            username: data.username,
            password: data.password
        };

        e.component.option({ text: 'Registering...', disabled: true });

        try {
            let response = await hms.Modules.Server.Call('CLLogin/Register', 'POST', sendObj);

            if (!response.isError) {
                DevExpress.ui.notify(response.message || 'Registered Successfully!', 'success', 2000);
                formInstance.resetValues();
            } else {
                DevExpress.ui.notify(response.message || 'Registration failed', 'error', 2000);
            }
        }
        catch (err) {
            DevExpress.ui.notify(`Error: ${err.message}`, 'error', 2000);
        }
        finally {
            e.component.option({ text: 'Register', disabled: false });
        }
    }

})();
