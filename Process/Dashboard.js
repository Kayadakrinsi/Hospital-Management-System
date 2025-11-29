(async () => {
    let chart, appoitmentsData;

    async function LoadData() {
        let roleId = hms.Modules.Session.get('roleId'),
            userId = hms.Modules.Session.get('userId'),
            url = 'CLAppointments/GetAppointments',
            statusLookup = [
                { id: 1, name: 'Scheduled' },
                { id: 2, name: 'Completed' },
                { id: 3, name: 'Cancelled' },
            ], grouped = {};

        if (roleId == 2) {
            url += `?doctorId=${userId}`;
        }
        else if (roleId == 3) {
            url += `?patientId=${userId}`;
        }

        res = await hms.Modules.Server.Call(url);
        appoitmentsData = !res.isError ? res.data : [];

        appoitmentsData.forEach(a => {
            let statusName = statusLookup.find(x => x.id === a.status)?.name || "Unknown";

            if (!grouped[statusName]) grouped[statusName] = 0;
            grouped[statusName]++;
        });

        appoitmentsData = Object.keys(grouped).map(k => ({
            status: k,
            count: grouped[k]
        }));
    }

    await LoadData();

    chart = $("#chartDashboard").dxPieChart({
        dataSource: appoitmentsData,
        type: "doughnut",
        title: "Appointment Status",
        palette: "Soft Pastel",
        legend: {
            visible: true,
            horizontalAlignment: "center",
            verticalAlignment: "bottom"
        },
        series: [{
            argumentField: "status",
            valueField: "count",
            label: {
                visible: true,
                connector: { visible: true },
                customizeText: function (e) {
                    return `${e.argument} : ${e.value}`;
                }
            }
        }]
    }).dxPieChart('instance');

})();
