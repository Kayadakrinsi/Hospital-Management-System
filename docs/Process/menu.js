export class Menu {
  static async initMenu() {
    try {
      let menuData, userRights, sidebar,
        userId = hms.Modules.Session.get('userId'),
        roleId = hms.Modules.Session.get('roleId'),
        res = await hms.Modules.Server.Call('CLUserRights/GetUserMenus', 'GET');

      if (res.isError) {
        console.error('Menu load failed:', res.message);
        return;
      };

      menuData = res.data || [];
      menuData.sort((a, b) => a.displayOrder - b.displayOrder);

      res = await hms.Modules.Server.Call(`CLUserRights/GetUserActivities?userId=${userId}&roleId=${roleId}`, 'GET');

      if (!res.isError) {
        userRights = (res?.data?.length > 0) ? res.data.map(a => a.activityId) : [];
        hms.Modules.Session.set('userRights', userRights)
      }

      // Clear sidebar
      sidebar = $('#sidebarContainer');

      sidebar.empty();

      // Create tree container
      $('<div id="treeMenu"></div>').appendTo(sidebar);

      // Initialize TreeView
      $('#treeMenu').dxTreeView({
        items: menuData,
        dataStructure: 'plain',
        keyExpr: 'menuId',
        parentIdExpr: 'parentId',
        displayExpr: 'text',
        expandNodesRecursive: false,
        animationEnabled: true,
        hoverStateEnabled: true,
        selectionMode: 'single',
        expandEvent: 'click',
        width: '100%',
        scrollDirection: 'vertical',
        submenuDirection: 'rightOrBottom',
        itemTemplate: function (itemData) {
          let iconHtml = itemData.iconName ? `<i class='${itemData.iconName} me-2'></i>` : '';
          return `<span class='d-flex align-items-center'>${iconHtml}${itemData.menuName}</span>`;
        },
        onItemClick: function (e) {
          let file = e.itemData.menuUrl;
          if (file && file !== '#') {
            hms.Modules.Navigation.invoke(file);
          }
        },
        onContentReady: function (e) {
          // Collapse all nodes by default
          e.component.collapseAll();
        }
      });
    } catch (ex) {
      console.error('Menu init failed:', ex);
    }
  }
}
