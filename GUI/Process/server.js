export class Server {

  static Call(endPoint, method = 'GET', data = null, isAuthorized = true) {

    return new Promise((resolve, reject) => {

      let baseURL = 'http://localhost:5000/api/';

      $.ajax({
        url: baseURL + endPoint,
        method,
        data: data ? JSON.stringify(data) : undefined,
        contentType: 'application/json',
        headers: isAuthorized ? { 'Authorization': `Bearer ${hms.Modules.Session.get('token')}` } : {},
        success: function (response) {
          console.log(`Success: ${baseURL + endPoint}`, response);
          resolve(response); // resolve Promise
        },
        error: function (xhr) {
          console.error(`Error: ${baseURL + endPoint}`, xhr);
          reject(xhr); // reject Promise
        }
      });
      
    });
  }
}
