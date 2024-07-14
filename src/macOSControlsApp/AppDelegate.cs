namespace macOSControlsApp;

[Register ("AppDelegate")]
public class AppDelegate : NSApplicationDelegate {
	public override void DidFinishLaunching (NSNotification notification)
	{
		var window = new NSWindow (new CoreGraphics.CGRect (200, 200, 400, 400), NSWindowStyle.Titled | NSWindowStyle.Closable | NSWindowStyle.Resizable | NSWindowStyle.Miniaturizable, NSBackingStore.Buffered, false);
		window.MakeKeyAndOrderFront(this);
	}

	public override void WillTerminate (NSNotification notification)
	{
		// Insert code here to tear down your application
	}
}
