import { Server } from './server.js';
import { Session } from './session.js';
import { Navigation } from './navigation.js';

window.hms = window.hms || {};
hms.Modules = hms.Modules || {};

// Register all static modules globally
hms.Modules.Server = Server;
hms.Modules.Session = Session;
hms.Modules.Navigation = Navigation;