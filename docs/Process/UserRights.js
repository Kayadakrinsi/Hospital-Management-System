(async () => {
    let selectedUser,
        gridUsers, gridActivities, btnSave,
        userdata = [], activitiesdata = [],
        selectedUserRights = [],
        dtoUserRights = {
            userId: 0,
            roleId: 0,
            addRights: '',
            removeRights: '',
        };

    async function GetUsers() {
        let res = await hms.Modules.Server.Call('CLUserRights/GetUsers');
        userdata = !res.isError ? res.data : [];
    }

    function prepareActivityData(raw) {

        // Build parent lookup
        const parentLookup = {};
        raw.forEach(a => {
            if (a.parentActivityId === 0) {
                parentLookup[a.activityId] = a.activityName;
            }
        });

        // Build child-only list
        const children = raw
            .filter(a => a.parentActivityId > 0)    // ONLY CHILD ACTIVITIES
            .map(a => ({
                activityId: a.activityId,
                activityName: a.activityName,
                isAllowed: a.isAllowed,
                parentName: parentLookup[a.parentActivityId]   // group header name
            }));

        return children;
    }

    async function GetUserActivities(userId, roleId) {
        let res = await hms.Modules.Server.Call(`CLUserRights/GetUserActivities?userId=${userId}&roleId=${roleId}`);

        if (!res.isError) {
            // Prepare structured datasource
            activitiesdata = prepareActivityData(res.data);

            // Mark rights as selected in grid
            activitiesdata.forEach(x => {
                x.isAllowed = selectedUserRights?.includes(x.activityId);
            });
        }
    }

    await GetUsers();

    gridUsers = $("#gridUsers").dxDataGrid({
        dataSource: userdata,
        keyExpr: "userId",
        showBorders: true,
        selection: {
            mode: "single"
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
        searchPanel: {
            visible: true,
            placeholder: "Search users..."
        },
        grouping: {
            autoExpandAll: false
        },
        columns: [
            {
                dataField: "role",
                caption: "Role",
                groupIndex: 0  // Group by Role
            },
            {
                dataField: "userName",
                caption: "User"
            }
        ],
        onSelectionChanged: async function (e) {

            let data = e.selectedRowsData[0];
            if (!data) return;

            selectedUser = data;
            selectedUserRights = data.rights;

            await GetUserActivities(data.userId, data.roleId);

            gridActivities.option("dataSource", activitiesdata);
            gridActivities.refresh();
            gridUsers.refresh();  // Apply highlight
        }

    }).dxDataGrid("instance");


    gridActivities = $('#gridUserActivities').dxDataGrid({
        dataSource: activitiesdata,
        showBorders: true,
        columnAutoWidth: true,
        searchPanel: {
            visible: true,
            placeholder: "Search users..."
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
        grouping: {
            autoExpandAll: false
        },
        columns: [
            {
                caption: 'Parent',
                dataField: 'parentName',
                groupIndex: 0,        // <= Groups rows by parent name
                visible: false
            },
            {
                caption: '',
                width: 60,
                allowSorting: false,
                allowFiltering: false,
                alignment: 'center',

                headerCellTemplate: function (container) {

                    $('<div>').dxCheckBox({
                        onValueChanged: function (e) {
                            let all = gridActivities.getDataSource().items();

                            all.forEach(row => {
                                if (row.items) {
                                    // This is a group row → update its children
                                    row.items.forEach(child => {
                                        child.isAllowed = e.value;
                                    });
                                } else {
                                    // Normal non-grouped row fallback
                                    row.isAllowed = e.value;
                                }
                            });

                            gridActivities.refresh();
                        }
                    }).appendTo(container);
                },

                cellTemplate: function (container, options) {
                    $('<div>').dxCheckBox({
                        value: options.data.isAllowed,
                        onValueChanged: function (e) {
                            // Correct way to update the row data
                            options.data.isAllowed = e.value;

                            // Force DevExtreme to refresh the grid row UI
                            options.component.refresh(true);
                        }
                    }).appendTo(container);
                }
            },
            {
                caption: 'activityId',
                visible: false,
            },
            {
                caption: 'activityKey',
                visible: false,
            },
            {
                caption: 'Activity',
                dataField: 'activityName',
                width: 300
            },
        ]
    }).dxDataGrid('instance');

    btnSave = $('#btnSave').dxButton({
        text: 'Save Rights',
        type: 'default',
        onClick: async function () {
            if (!selectedUser) {
                DevExpress.ui.notify('Please select a user.', 'warning', 2000);
                return;
            }

            let rows = gridActivities.getDataSource().items(),
                flatRows = [];

            rows.forEach(r => {
                if (r.items) {
                    flatRows.push(...r.items);
                } else {
                    flatRows.push(r);
                }
            });

            let newRights = flatRows
                .filter(x => x.isAllowed === true)
                .map(x => Number(x.activityId)),

                existingRights = selectedUserRights.map(Number), // your existing list

                // Compute added & removed using Set for reliability
                newSet = new Set(newRights),
                existingSet = new Set(existingRights),

                addedRights = newRights.filter(id => !existingSet.has(id)),

                // REMOVED = those previously allowed but now unchecked
                removedRights = existingRights.filter(id =>
                    flatRows.some(r => r.activityId == id && r.isAllowed === false)
                );

            dtoUserRights.userId = selectedUser.userId;
            dtoUserRights.roleId = selectedUser.roleId;
            dtoUserRights.addRights = addedRights.join(',');
            dtoUserRights.removeRights = removedRights.join(',');

            try {
                let response = await hms.Modules.Server.Call('CLUserRights/SaveUserRights', 'POST', dtoUserRights);

                DevExpress.ui.notify(response.message, (!response.isError) ? 'success' : 'error', 2000);

                await GetUsers();
            }
            catch (err) {
                DevExpress.ui.notify(`Error: ${err.message}`, 'error', 2000);
            }
        }
    });

})();
