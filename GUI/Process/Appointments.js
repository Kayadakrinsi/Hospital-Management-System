(async () => {
    let grid, popup, form,
        doctorsData, patientsData, appoitmentsData, res,
        userRights = hms.Modules.Session.get('userRights');

    async function LoadData() {
        let roleId = hms.Modules.Session.get('roleId'),
            userId = hms.Modules.Session.get('userId'),
            url = 'CLAppointments/GetAppointments';

        if (roleId == 2) {
            url += `?doctorId=${userId}`;
        }
        else if (roleId == 3) {
            url += `?patientId=${userId}`;
        }

        res = await hms.Modules.Server.Call(url);
        appoitmentsData = !res.isError ? res.data : [];
    }

    res = await hms.Modules.Server.Call('CLAppointments/GetDoctors');
    doctorsData = !res.isError ? res.data : [];

    res = await hms.Modules.Server.Call('CLAppointments/GetPatients');
    patientsData = !res.isError ? res.data : [];

    await LoadData();

    grid = $('#gridAppointments').dxDataGrid({
        dataSource: appoitmentsData,
        keyExpr: 'appointmentId',
        searchPanel: {
            visible: true,
            placeholder: "Search appointments...",
            highlightCaseSensitive: false
        },
        headerFilter: {
            visible: true,
            allowSearch: true
        },
        paging: {
            enabled: true,
            pageSize: 10
        },
        pager: {
            visible: true,
            showPageSizeSelector: true,
            allowedPageSizes: [5, 10, 20, 50],
            showInfo: true,
            showNavigationButtons: true
        },
        onToolbarPreparing: function (e) {
            e.toolbarOptions.items.push({
                widget: 'dxButton',
                location: 'after',
                options: {
                    text: 'Book Appointment',
                    icon: 'add',
                    type: 'default',
                    onClick: BookAppointment,
                    visible: userRights.includes(11)
                }
            });
        },
        columns: [
            {
                dataField: 'patientId',
                caption: 'Patient',
                lookup: {
                    dataSource: patientsData,
                    valueExpr: 'userId',
                    displayExpr: 'userName'
                }
            },
            {
                dataField: 'doctorId',
                caption: 'Doctor',
                lookup: {
                    dataSource: doctorsData,
                    valueExpr: 'userId',
                    displayExpr: 'userName'
                }
            },
            {
                dataField: 'appointmentDate',
                caption: 'Date',
                dataType: 'date',
                format: 'dd-MM-yyyy'
            },
            {
                dataField: 'status',
                caption: 'Status',
                lookup: {
                    dataSource: [
                        { id: 1, name: 'Scheduled' },
                        { id: 2, name: 'Completed' },
                        { id: 3, name: 'Cancelled' },
                    ],
                    valueExpr: 'id',
                    displayExpr: 'name'
                }
            },
            {
                caption: 'Complete',
                alignment: 'center',
                width: 100,
                visible: userRights.includes(12),
                cellTemplate: function (container, options) {
                    let status = options.data.status;

                    // show only if allowed + not already completed/cancelled
                    if (status === 1) {
                        $('<a>')
                            .text('Complete')
                            .addClass('action-link-complete')
                            .on('click', function () {
                                CompleteAppointment(options.data.appointmentId);
                            })
                            .appendTo(container);
                    }
                }
            },
            {
                caption: 'Cancel',
                alignment: 'center',
                width: 100,
                visible: userRights.includes(13),
                cellTemplate: function (container, options) {
                    let status = options.data.status;

                    // visible unless completed (3) or cancelled (2)
                    if (status === 1) {
                        $('<a>')
                            .text('Cancel')
                            .addClass('action-link-cancel')
                            .on('click', function () {
                                CancelAppointment(options.data.appointmentId);
                            })
                            .appendTo(container);
                    }
                }
            },
        ],

    }).dxDataGrid('instance');

    // Open popup
    function BookAppointment() {
        popup = $('#popupBookAppointment').dxPopup({
            width: 400,
            height: 'auto',
            showTitle: true,
            title: 'Book Appointment',
            visible: true,
            dragEnabled: true,
            closeOnOutsideClick: false,
            contentTemplate: function (content) {
                const $formContainer = $('<div>');   // Create DIV first

                $formContainer.dxForm({
                    labelLocation: 'top',
                    items: [
                        {
                            dataField: 'PatientId',
                            editorType: 'dxSelectBox',
                            label: { text: 'Patient' },
                            editorOptions: {
                                dataSource: patientsData,
                                valueExpr: 'userId',
                                displayExpr: 'userName',
                                searchEnabled: true,
                                placeholder: 'Select patient'
                            },
                            validationRules: [{ type: 'required' }]
                        },
                        {
                            dataField: 'DoctorId',
                            editorType: 'dxSelectBox',
                            label: { text: 'Doctor' },
                            editorOptions: {
                                dataSource: doctorsData,
                                valueExpr: 'userId',
                                displayExpr: 'userName',
                                searchEnabled: true,
                                placeholder: 'Select doctor'
                            },
                            validationRules: [{ type: 'required' }]
                        },
                        {
                            dataField: 'AppointmentDate',
                            editorType: 'dxDateBox',
                            label: { text: 'Appointment Date & Time' },
                            editorOptions: {
                                type: 'datetime',
                                displayFormat: 'dd-MM-yyyy HH:mm:ss',
                                placeholder: 'Select date & time',
                                min: (function () {
                                    const d = new Date(Date.now() + 30 * 60000); // now + 30 min buffer
                                    d.setSeconds(0);
                                    d.setMilliseconds(0);
                                    return d;
                                })(),

                                // ✔ Change DevExtreme's default “value is out of range”
                                invalidDateMessage: "Appointment must be at least 30 minutes from now",
                                useMaskBehavior: true
                            },
                            validationRules: [{ type: 'required', message: 'Appointment date & time is required' }]
                        }
                    ]
                });

                content.append($formContainer);

                form = $formContainer.dxForm("instance");

                $('<div>').css('text-align', 'right').append(
                    $('<div>').dxButton({
                        text: 'Save',
                        type: 'default',
                        onClick: SaveAppointment
                    }).css('margin-right', '10px'),
                    $('<div>').dxButton({
                        text: 'Cancel',
                        onClick: function () {
                            popup.hide();
                        }
                    })
                ).appendTo(content);
            }
        }).dxPopup('instance');
    }

    async function SaveAppointment() {

        debugger

        let data = form.option('formData');

        let validation = form.validate();

        if (!validation.isValid) {
            DevExpress.ui.notify('Please fill all required fields', 'error', 2000);
            return;
        }

        let dto = {
            PatientId: data.PatientId,
            DoctorId: data.DoctorId,
            AppointmentDate: data.AppointmentDate
        };

        let res = await hms.Modules.Server.Call('CLAppointments/BookAppointment', 'POST', dto);

        if (!res.isError) {
            DevExpress.ui.notify(res.message, 'success', 2000);
            popup.hide();
            await LoadData();
            grid.option('dataSource', appoitmentsData);
            grid.refresh();
        }
        else {
            DevExpress.ui.notify(res.message, 'error', 2000);
        }
    }

    async function CompleteAppointment(id) {
        let res = await hms.Modules.Server.Call(`CLAppointments/CompleteAppointment?appointmentId=${id}`, 'PUT');

        if (!res.isError) {
            DevExpress.ui.notify(res.message, 'success', 2000);

            await LoadData();
            grid.option('dataSource', appoitmentsData);
            grid.refresh();
        }
        else {
            DevExpress.ui.notify(res.message, 'error', 2000);
        }
    }

    async function CancelAppointment(id) {
        let res = await hms.Modules.Server.Call(`CLAppointments/CancelAppointment?appointmentId=${id}`, 'PUT');

        if (!res.isError) {
            DevExpress.ui.notify(res.message, 'success', 2000);

            await LoadData();
            grid.option('dataSource', appoitmentsData);
            grid.refresh();
        }
        else {
            DevExpress.ui.notify(res.message, 'error', 2000);
        }
    }


})();
