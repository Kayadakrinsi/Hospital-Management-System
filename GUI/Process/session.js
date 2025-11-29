export class Session {
  static get(key) {
    try { return JSON.parse(sessionStorage.getItem(key)); }
    catch { return sessionStorage.getItem(key); }
  }

  static set(key, value) {
    sessionStorage.setItem(key, JSON.stringify(value));
  }

  static clear() {
    sessionStorage.clear();
  }

  static setLoginSession(data,token) {
    Session.set('token', token);
    Session.set('userId', data.userId);
    Session.set('userName', data.userName);
    Session.set('roleId', data.roleId);
    Session.set('departmentId', data.departmentId);
    Session.set('role', data.role);
    Session.set('department', data.department);
  }

}
