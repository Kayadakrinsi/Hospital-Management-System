# Hospital-Management-System

The Hospital Management System (HMS) is a single-page web application built using .NET API, DevExtreme, HTML, CSS, JavaScript, and MySQL.
It provides a complete workflow for managing hospital operations including user management, appointments, and role-based dashboards.

Key Features
🔐 User Management
User invitation and registration
Role-based access management
User-rights assignment by Admin

📅 Appointment Management
Book, view, cancel, and manage appointments
Role-specific appointment visibility and permissions
End-to-end appointment lifecycle handling

📊 Dashboards
Every user has a customized dashboard showing appointment analytics and statistics.

User Roles & Permissions
1. 👑 Admin
Can create and manage all other users
Assign rights to users which were created by admin
Full control over all appointments

2. 🩺 Doctor
Can view and manage only the appointments assigned to them
Full control over their own appointment schedules

3. 🧑‍⚕️ Receptionist
Can create patient user account
Can book, view, and cancel appointments for all patients
Overall appointment management rights except admin-level access

4. 🧑 Patien
Can view their own appointments
Can cancel their appointments
Personalized dashboard showing upcoming and past appointments
