(async () => {
    let formInstance,
        objInv = {
            contact: '',
            email: '',
            roleId: 0,
            departmentId: 0,
        },

        // Load roles
        rolesRes = await hms.Modules.Server.Call('CLLogin/GetRoles'),
        rolesData = !rolesRes.isError ? rolesRes.data : [],

        deptData = [];

    // Form Items
    const formItems = [
        {
            dataField: 'email',
            label: { text: 'Email' },
            editorType: 'dxTextBox',
            editorOptions: { placeholder: 'Enter email' },
            validationRules: [
                { type: 'required', message: 'Email is required' },
                { type: 'email', message: 'Enter a valid email address' }
            ]
        },
        {
            dataField: 'contact',
            label: { text: 'Contact Number' },
            editorType: 'dxNumberBox',
            editorOptions: {
                placeholder: 'Enter contact number',
                showSpinButtons: false,
                format: '#',
                min: 0,
                max: 9999999999,
                onInput(e) {
                    let value = e.event.target.value;
                    if (value.length > 10) {
                        value = value.slice(0,10);
                        e.event.target.value = value;
                        e.component.option("value", value);
                    }
                }
            },
            validationRules: [
                { type: 'required', message: 'Contact number is required' },
                { type: 'pattern', pattern: /^\d{10}$/, message: 'Contact number must be 10 digits' }
            ]
        },
        {
            dataField: 'roleId',
            label: { text: 'Role' },
            editorType: 'dxSelectBox',
            editorOptions: {
                placeholder: 'Select role',
                dataSource: rolesData,
                valueExpr: 'roleId',
                displayExpr: 'roleName',
                onValueChanged: async (e) => await handleRoleChange(e.value)
            },
            validationRules: [
                { type: 'required', message: 'Please select a role' }
            ]
        },
        {
            dataField: 'departmentId',
            label: { text: 'Department' },
            editorType: 'dxSelectBox',
            editorOptions: {
                placeholder: 'Select department',
                disabled: true,
                dataSource: deptData,
                valueExpr: 'departmentId',
                displayExpr: 'departmentName',
                value: 0,
            }
        },
        {
            itemType: 'button',
            horizontalAlignment: 'center',
            buttonOptions: {
                text: 'Send Invitation',
                type: 'default',
                width: '100%',
                onClick: async (e) => await SendInvitation(e)
            }
        }
    ];

    // Create Form
    formInstance = $('#formContainer').dxForm({
        formData: objInv,
        labelLocation: 'top',
        validationGroup: 'inviteGroup',
        items: formItems
    }).dxForm('instance');

    async function handleRoleChange(selectedRoleId) {
        let depEditor = formInstance.getEditor('departmentId');

        if (!depEditor) return;

        if (selectedRoleId == 2) {
            if (deptData?.length <= 0) {
                let deptRes = await hms.Modules.Server.Call('CLLogin/GetDepartments');
                deptData = !deptRes.isError ? deptRes.data : [];
                depEditor.option('dataSource', deptData);
            }
            depEditor.option('disabled', false);
        }
        else {
            depEditor.option('value', 0);
            depEditor.option('disabled', true);
        }
    }

    async function SendInvitation(e) {
        let result = DevExpress.validationEngine.validateGroup('inviteGroup');
        if (!result.isValid) return;

        let data = formInstance.option('formData');

        e.component.option({ text: 'Sending...', disabled: true });

        try {
            let response = await hms.Modules.Server.Call('CLLogin/Invite', 'POST', data);

            console.log(response);

            if (!response.isError) {
                DevExpress.ui.notify(response.message, 'success', 2000);
                formInstance.resetValues();
            } else {
                DevExpress.ui.notify(response.message || 'Failed to send', 'error', 2000);
            }
        } catch (err) {
            DevExpress.ui.notify(`Error: ${err.message}`, 'error', 2000);
        } finally {
            e.component.option({ text: 'Send Invitation', disabled: false });
        }
    }

})();
