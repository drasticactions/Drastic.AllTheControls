using macOSControlsApp;

// This is the main entry point of the application.
NSApplication.Init ();
NSApplication.SharedApplication.Delegate = new AppDelegate ();
NSApplication.Main (args);
