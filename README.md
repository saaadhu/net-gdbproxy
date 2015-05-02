# Pseudo

Pseudo is a gdb stub written in C#.

The stub implements gdb's remote serial protocol and
translates requests into abstract method calls on a Target
class, and return values back into responses.

To use Pseudo, derive your own class from Pseudo.Target, and
implement the abstract methods. Then do

	var t = new TestTarget();
        var proxy = new Pseudo.Proxy(t);
        proxy.Listen(new System.Net.IPEndPoint(System.Net.IPAddress.Any, 5565));

And that's it.
