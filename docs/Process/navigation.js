export class Navigation {
  /**
     * Load HTML content and its matching JS file (if it exists)
     * @param {string} file - HTML filename to load (e.g., 'Dashboard.html')
     */
  static invoke(file) {
    if (!file) return;

    let baseName = file.replace(/\.html$/, ''), // remove extension if present
      htmlPath = `./View/${baseName}.html`,
      jsPath = `./Process/${baseName}.js`;

    $('#contentContainer').load(htmlPath, function (response, status) {
      if (status === 'error') {
        console.error(`Failed to load ${file}:`, response);
      } else {
        console.log(`Loaded HTML: ${file}`);
        Navigation.loadJS(jsPath);
      }
    });
  }

  /**
   * Dynamically load a JS file without needing type='module'
   * @param {string} scriptPath - Path to the JS file
   */
  static loadJS(scriptPath) {
    $.ajax({
      url: scriptPath,
      dataType: 'script',
      cache: false, // ensure fresh load each time
      success: function () {
        console.log(`Loaded JS: ${scriptPath}`);
      },
      error: function () {
        console.warn(`No JS found for: ${scriptPath} (skipped)`);
      }
    });
  }
}
