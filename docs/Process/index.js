import { Menu } from './menu.js';

$(document).ready(async () => {
  try {
    // Load sidebar menu and dashboard
    await Menu.initMenu();
    // Sidebar toggle
    $('#toggleSidebar').on('click', () => {
      $('#sidebarContainer').toggleClass('collapsed');
    });

    // Handle menu navigation
    $('#sideMenu').on('click', '.nav-link[data-page]', function (e) {
      e.preventDefault();
      const page = $(this).data('page');
      if (page) hms.invokeNavigation(page);
    });

    // Logout
    $('#btnLogout').on('click',async () => {
      sessionStorage.clear();
      let res = await hms.Modules.Server.Call('CLLogin/Logout', 'POST');

        if (!res.isError) {
            DevExpress.ui.notify(res.message, 'success', 2000);
            window.location.href = "login.html";
        }
        else {
            DevExpress.ui.notify(res.message, 'error', 2000);
        }
      hms.Server.Call
    });
  } catch (err) {
    console.error('Initialization Error:', err);
  }
});
