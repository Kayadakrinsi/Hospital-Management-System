$(function () {

  let showPassword = false;
  
  let model = {
    Email: '',
    Password: '',
  };

  // --- Email TextBox ---
  let txtEmail = $('#txtUser').dxTextBox({
    placeholder: 'Enter email',
    validationGroup: 'loginValidationGroup',
    onValueChanged: function (e) {
      if (e.value) {
        model.Email = e.value;
      }
    }
  }).dxValidator({
    validationRules: [
      {
        type: 'required',
        message: 'Email is required'
      },
      {
        type: 'pattern',
        pattern: /^[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,}$/,
        message: 'Invalid Email format'
      },
    ]
  }).dxTextBox('instance');

  // --- Password TextBox ---
  let txtPassword = $('#txtPass').dxTextBox({
    mode: 'password',
    placeholder: 'Enter password',
    validationGroup: 'loginValidationGroup',
    onValueChanged: function (e) {
      if (e.value) {
        model.Password = e.value;
      }
    },
    buttons: [{
      name: 'togglePassword',
      location: 'after',
      options: {
        icon: 'fa fa-eye-slash',  // Font Awesome eye-slash initially
        stylingMode: 'text',
        onClick: function (e) {
          showPassword = !showPassword;

          // Toggle input mode
          txtPassword.option('mode', showPassword ? 'text' : 'password');

          // Toggle icon
          e.component.option('icon', showPassword ? 'fa fa-eye' : 'fa fa-eye-slash');
        }
      }
    }],
  }).dxValidator({
    validationRules: [
      {
        type: 'required',
        message: 'Password is required'
      },
      // {
      //   type: 'pattern',
      //   pattern: /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^A-Za-z0-9]).{6,50}$/,
      //   message: 'Password must be 6–50 chars long and contain at least one uppercase, one lowercase, one number, and one special character.'
      // }
    ]
  }).dxTextBox('instance');

  // --- Login Button ---
  let btn = $('#btnSubmit').dxButton({
    text: 'Login',
    type: 'default',
    width: '100%',
    validationGroup: 'loginValidationGroup', // attach same group for automatic linking
    onClick: async function (e) {
      const result = DevExpress.validationEngine.validateGroup(btn.option('vlidationGroup'));

      if (result.isValid) {
        try {
          // Await the API call
          const res = await hms.Modules.Server.Call('CLLogin/Login', 'POST', model, false);

          // Handle successful response
          if (!res.isError) {
            hms.Modules.Session.setLoginSession(res.data, res.token);
            DevExpress.ui.notify('Login successful!', 'success', 2000);
            window.location.href = 'index.html';
          } else {
            DevExpress.ui.notify(res.message || 'Login failed', 'error', 2000);
          }
        } catch (err) {
          // Handle server/network errors
          DevExpress.ui.notify('Server error', 'error', 2000);
          console.error('Login error:', err);
        }
      }

      return;
    }
  }).dxButton('instance');

});
