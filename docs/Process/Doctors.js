(async () => {
    let grid, doctorsData;

    async function LoadData() {
        res = await hms.Modules.Server.Call('CLAppointments/GetDoctors');
        doctorsData = !res.isError ? res.data : [];
    }


    await LoadData();

    grid = $('#gridDoctors').dxDataGrid({
        dataSource: doctorsData,
        keyExpr: 'userId',
        searchPanel: {
            visible: true,
            placeholder: "Search...",
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
        columns: [
            {
                dataField: 'userName',
                caption: 'Patient Name',
            },
            {
                dataField: 'email',
                caption: 'Email',
            },
            {
                dataField: 'contact',
                caption: 'Contact No.',
            },
            {
                dataField: 'gender',
                caption: 'Gender',
                lookup: {
                    dataSource: [
                        { id: 0, name: 'Other' },
                        { id: 1, name: 'Male' },
                        { id: 2, name: 'Female' },
                    ],
                    valueExpr: 'id',
                    displayExpr: 'name'
                }
            },
            {
                dataField: 'department',
                caption: 'Departments',
            },
            {
                dataField: 'createdOn',
                caption: 'Registeration Date',
                dataType: 'date',
                format: 'dd-MM-yyyy'
            },
        ],

    }).dxDataGrid('instance');
})();
